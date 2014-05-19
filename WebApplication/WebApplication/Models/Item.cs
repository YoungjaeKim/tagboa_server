using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace WebApplication.Models
{
	public class Item
	{
		private DateTime _timestamp = DateTime.UtcNow;
		public int ID { get; set; }
		[Required]
		public string Title { get; set; }
		public string Genre { get; set; }
		public string Author { get; set; }
		public double Rating { get; set; }
		public string Description { get; set; }
		public int ReadCount { get; set; }

		[Display(Name = "Release Date")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime Timestamp
		{
			get { return _timestamp; }
			set { _timestamp = value; }
		}

		public virtual List<Tag> Tags { get; set; }
		public virtual List<UrlLink> Links { get; set; }

	}

}