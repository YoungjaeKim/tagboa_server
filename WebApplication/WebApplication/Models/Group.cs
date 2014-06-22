using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.Models
{
	public class Group
	{
		private DateTime _timestamp = DateTime.UtcNow;
		public int ID { get; set; }
		public List<RoleInGroup> Users { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime Timestamp
		{
			get { return _timestamp; }
			set { _timestamp = value; }
		}

		public class RoleInGroup
		{
			public int ID { get; set; }
			public ApplicationUser User { get; set; }
			public string Role { get; set; }
		}
	}
}