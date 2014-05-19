using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
	public class FacebookViewModel
	{
		[Required]
		[Display(Name = "Friend's name")]
		public string Name { get; set; }

		public string ImageUrl { get; set; }
	}
}