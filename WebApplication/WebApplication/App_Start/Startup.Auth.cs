using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.OAuth;
using Owin;
using WebApplication.Models;
using WebApplication.Providers;

namespace WebApplication
{
	public partial class Startup
	{
		/* == WebApi 추가 부분. ==*/
		static Startup()
		{
			PublicClientId = "self";

			UserManagerFactory = () => new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

			OAuthOptions = new OAuthAuthorizationServerOptions
			{
				TokenEndpointPath = new PathString("/Token"),
				Provider = new ApplicationOAuthProvider(PublicClientId, UserManagerFactory),
				AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
				AccessTokenExpireTimeSpan = TimeSpan.FromDays(300),
				AllowInsecureHttp = true
			};
		}

		public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

		public static Func<UserManager<ApplicationUser>> UserManagerFactory { get; set; }

		public static string PublicClientId { get; private set; }

		// For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
		public void ConfigureAuth(IAppBuilder app)
		{
			//app.UseOAuthAuthorizationServer(OAuthOptions);

			// Enable the application to use a cookie to store information for the signed in user
			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
				LoginPath = new PathString("/Account/Login")
			});

			// Use a cookie to temporarily store information about a user logging in with a third party login provider
			app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

			// Enable the application to use bearer tokens to authenticate users
			app.UseOAuthBearerTokens(OAuthOptions);

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
			x.Scope.Add("user_likes");
			x.Scope.Add("user_friends");
			x.Scope.Add("friends_about_me");
			x.Scope.Add("read_friendlists");
			x.Scope.Add("publish_actions");
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