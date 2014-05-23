using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.Owin.Security;

namespace WebApplication.Models
{
	/// <summary>
	/// 학습 단원 정보
	/// </summary>
	public class Curricular
	{
		private DateTime _timestamp = DateTime.UtcNow;

		public int ID { get; set; }

		/// <summary>
		/// 단원 제목
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// 학년
		/// </summary>
		public double Grade { get; set; }
		/// <summary>
		/// 등록 아이템
		/// </summary>
		public virtual List<Item> Items { get; set; }

		/// <summary>
		/// 과목
		/// </summary>
		public string Subject { get; set; }
		/// <summary>
		/// 언어
		/// </summary>
		public string Locale { get; set; }
		/// <summary>
		/// 출판사
		/// </summary>
		public string Publisher { get; set; }

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
	}
}