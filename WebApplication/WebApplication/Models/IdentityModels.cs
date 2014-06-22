using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebApplication.Models
{
	// You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
	public class ApplicationUser : IdentityUser
	{
		public string Nickname { get; set; }

		[DefaultValue(0)]
		public int TotalItems { get; set; }

		[DefaultValue(0)]
		public int Level { get; set; }
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime? GenerateTime { get; set; }

		public List<Item> FavoriteItems { get; set; }
		public List<Curricular> FollowingCurriculum { get; set; }
		public List<Group> Groups { get; set; } 
	}


	// http://www.codeproject.com/Articles/682113/Extending-Identity-Accounts-and-Implementing-Rol
	/// <summary>
	/// 롤 관리 추가.
	/// </summary>
	public class IdentityManager
	{
		public bool RoleExists(string name)
		{
			var rm = new RoleManager<IdentityRole>(
				new RoleStore<IdentityRole>(new ApplicationDbContext()));
			return rm.RoleExists(name);
		}

		public string RoleName(string roleId)
		{
			var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
			var role = rm.FindById(roleId);
			return role.Name;
		}

		public bool CreateRole(string name)
		{
			var rm = new RoleManager<IdentityRole>(
				new RoleStore<IdentityRole>(new ApplicationDbContext()));
			var idResult = rm.Create(new IdentityRole(name));
			return idResult.Succeeded;
		}


		public bool CreateUser(ApplicationUser user, string password)
		{
			var um = new UserManager<ApplicationUser>(
				new UserStore<ApplicationUser>(new ApplicationDbContext()));
			var idResult = um.Create(user, password);
			return idResult.Succeeded;
		}


		public bool AddUserToRole(string userId, string roleName)
		{
			var um = new UserManager<ApplicationUser>(
				new UserStore<ApplicationUser>(new ApplicationDbContext()));
			var idResult = um.AddToRole(userId, roleName);
			return idResult.Succeeded;
		}


		public void ClearUserRoles(string userId)
		{
			var um = new UserManager<ApplicationUser>(
				new UserStore<ApplicationUser>(new ApplicationDbContext()));
			var user = um.FindById(userId);
			var currentRoles = new List<IdentityUserRole>();
			currentRoles.AddRange(user.Roles);
			foreach (var role in currentRoles)
			{
				um.RemoveFromRole(userId, RoleName(role.RoleId));
			}
		}
	}

}