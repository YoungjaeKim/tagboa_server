using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace WebApplication.Models
{
	public class Tag
	{
		/// <summary>
		/// ID
		/// </summary>
		public int ID { get; set; }
		/// <summary>
		/// 제목
		/// </summary>
		public string Title { get; set; }
		/// <summary>
		/// 동일한 태그 숫자
		/// </summary>
		public int? Count { get; set; }
		/// <summary>
		/// 언어
		/// </summary>
		public string Locale { get; set; }
		/// <summary>
		/// 부모 태그
		/// </summary>
		public Tag Parent { get; set; }
		/// <summary>
		/// 그룹 ID. 아직 안쓰임. 폴더를 만들 때 쓸 수 있다.
		/// </summary>
		public int? Group { get; set; }
		/// <summary>
		/// 커리큘럼 관련 태그 여부.
		/// </summary>
		public bool IsCurricular { get; set; }
	}
}