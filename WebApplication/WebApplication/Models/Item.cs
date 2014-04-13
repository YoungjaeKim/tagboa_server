using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace WebApplication.Models
{
	public class Item
	{
		public int ID { get; set; }
		public string Title { get; set; }
		public string Genre { get; set; }
		public string Author { get; set; }
		public double Rating { get; set; }
		public string Description { get; set; }
		public int ReadCount { get; set; }

		[Display(Name = "Release Date")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime Timestamp { get; set; }

		public List<Tag> Tags { get; set; }
		public List<UrlLink> Links { get; set; }

	}

}