using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

namespace WebApplication.Models
{
	public class Item
	{
		private DateTime _timestamp = DateTime.UtcNow;
		private List<Tag> _tags = new List<Tag>();
		private List<UrlLink> _links = new List<UrlLink>();

		public int ID { get; set; }
		/// <summary>
		/// 제목
		/// </summary>
		[Required]
		public string Title { get; set; }
		/// <summary>
		/// 장르
		/// </summary>
		public string Genre { get; set; }
		/// <summary>
		/// 작성자ID
		/// </summary>
		public string Author { get; set; }
		/// <summary>
		/// 등급
		/// </summary>
		public double Rating { get; set; }
		/// <summary>
		/// 부가 설명
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// 접속 횟수
		/// </summary>
		public int ReadCount { get; set; }

		/// <summary>
		/// 기록 시간
		/// </summary>
		[Display(Name = "Release Date")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime Timestamp
		{
			get { return _timestamp; }
			set { _timestamp = value; }
		}

		/// <summary>
		/// 태그 목록
		/// </summary>
		public virtual List<Tag> Tags
		{
			get { return _tags; }
			set { _tags = value; }
		}

		/// <summary>
		/// 링크 목록
		/// </summary>
		public virtual List<UrlLink> Links
		{
			get { return _links; }
			set { _links = value; }
		}

		public void AddTag(string tagName)
		{
			// TODO: 태그 검색
			Tag tag = null;
			AddTag(tag);
		}

		/// <summary>
		/// 태그 추가
		/// </summary>
		/// <param name="tag">태그</param>
		public void AddTag(Tag tag)
		{
			if(tag == null)
				throw new ArgumentNullException("tag");
			if(tag.Title == null)
				throw new ArgumentNullException("tag.Title");

			if(Tags == null)
				Tags = new List<Tag>();

			// 중복 체크.
			if (Tags.Any(t => t.ID.Equals(tag.ID)))
				return;

			Tags.Add(tag);
		}

	}

}