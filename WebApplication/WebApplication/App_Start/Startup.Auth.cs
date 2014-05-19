using System.Collections.Generic;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Google;
using Owin;

namespace WebApplication
{
	public partial class Startup
	{
		// For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
		public void ConfigureAuth(IAppBuilder app)
		{
			// Enable the application to use a cookie to store information for the signed in user
			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
				LoginPath = new PathString("/Account/Login")
			});
			// Use a cookie to temporarily store information about a user logging in with a third party login provider
			app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

			// Uncomment the following lines to enable logging in with third party login providers
			//app.UseMicrosoftAccountAuthentication(
			//    clientId: "",
			//    clientSecret: "");

			//app.UseTwitterAuthentication(
			//   consumerKey: "",
			//   consumerSecret: "");

			// 참고.
			// http://blogs.msdn.com/b/webdev/archive/2013/10/16/get-more-information-from-social-providers-used-in-the-vs-2013-project-templates.aspx
			var x = new FacebookAuthenticationOptions();
			x.Scope.Add("email");
			x.Scope.Add("user_about_me");
			x.Scope.Add("user_friends");
			x.Scope.Add("friends_about_me");
			x.Scope.Add("read_friendlists");
			x.AppId = "1453066991603071";
			x.AppSecret = "04102e7b9a5810abe46babf7e33b3be4";
			x.Provider = new FacebookAuthenticationProvider()
			{
				OnAuthenticated = async context => context.Identity.AddClaim(
					new System.Security.Claims.Claim("FacebookAccessToken",
						context.AccessToken))
			};
			x.SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie;

			app.UseFacebookAuthentication(x);

			app.UseGoogleAuthentication();
		}
	}
}