using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace WebApplication.Models
{
	public class Tag
	{
		public int ID { get; set; }
		public string Title { get; set; }
		public int Count { get; set; }
		public string Locale { get; set; }
		public int Parent { get; set; }
		public int Group { get; set; }
		public bool IsCurricular { get; set; }
	}
}