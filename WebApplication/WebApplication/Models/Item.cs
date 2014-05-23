﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace WebApplication.Models
{
	public class Item
	{
		private DateTime _timestamp = DateTime.UtcNow;
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
		public virtual List<Tag> Tags { get; set; }
		/// <summary>
		/// 링크 목록
		/// </summary>
		public virtual List<UrlLink> Links { get; set; }

	}

}