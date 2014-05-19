using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
	public class UrlLink
	{
		public int ID { get; set; }
		[Required]
		public string Address { get; set; }
		public bool IsBroken { get; set; }
		public bool IsHidden { get; set; }
		public string Note { get; set; }
	}
}