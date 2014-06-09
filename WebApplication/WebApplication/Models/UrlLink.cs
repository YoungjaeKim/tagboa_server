using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
	public class UrlLink
	{
		public int ID { get; set; }
		/// <summary>
		/// URL 주소값
		/// </summary>
		[Required]
		public string Address { get; set; }
		/// <summary>
		/// 링크 상태
		/// </summary>
		public UrlLinkStatus Status { get; set; }
		/// <summary>
		/// 최근 링크 상태 확인 날짜
		/// </summary>
		public DateTime? RecentStatusChangedTime { get; set; }
		/// <summary>
		/// 비공개 여부
		/// </summary>
		public bool IsHidden { get; set; }
		/// <summary>
		/// 간단 소개말
		/// </summary>
		public string Note { get; set; }

		public UrlAdapter Adapter { get; set; }
	}

	public enum UrlLinkStatus
	{
		Normal,
		NotFound,
		Malicious,
		Reported
	}

	public enum UrlAdapter
	{
		Web,
		Mp4,
		EbsVideo,
		DaumPotPlayer,
	}
}