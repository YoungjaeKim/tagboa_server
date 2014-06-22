using System;
using System.Linq;
using System.Threading.Tasks;
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


			// This is a key step of the solution as we need to supply a meaningful and fully working
			// implementation of the OAuthBearerOptions object when we configure the OAuth Bearer authentication mechanism. 
			// The trick here is to reuse the previously defined OAuthOptions object that already
			// implements almost everything we need
			OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
			OAuthBearerOptions.AccessTokenFormat = OAuthOptions.AccessTokenFormat;
			OAuthBearerOptions.AccessTokenProvider = OAuthOptions.AccessTokenProvider;
			OAuthBearerOptions.AuthenticationMode = OAuthOptions.AuthenticationMode;
			OAuthBearerOptions.AuthenticationType = OAuthOptions.AuthenticationType;
			OAuthBearerOptions.Description = OAuthOptions.Description;
			// The provider is the only object we need to redefine. See below for the implementation
			OAuthBearerOptions.Provider = new CustomBearerAuthenticationProvider();
			OAuthBearerOptions.SystemClock = OAuthOptions.SystemClock;
		}

		public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }

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

			app.UseGoogleAuthentication(
				clientId: "504247835751-6pubqtuq6sg8kja8an5bd71vcj1lhq23.apps.googleusercontent.com",
				clientSecret: "OWwne3NdCJe52Z8upu_LI84-");

			app.UseOAuthBearerAuthentication(OAuthBearerOptions);
			// http://thewayofcode.wordpress.com/2014/03/01/asp-net-webapi-identity-system-how-to-login-with-facebook-access-token/
			//OAuthBearerAuthenticationExtensions.UseOAuthBearerAuthentication(app, OAuthBearerOptions);

		}


		public class CustomBearerAuthenticationProvider : OAuthBearerAuthenticationProvider
		{
			// This validates the identity based on the issuer of the claim.
			// The issuer is set in the API endpoint that logs the user in
			public override Task ValidateIdentity(OAuthValidateIdentityContext context)
			{
				var claims = context.Ticket.Identity.Claims;
				if (!claims.Any() || claims.Any(claim => claim.Type != "FacebookAccessToken"))
					context.Rejected();
				return Task.FromResult<object>(null);
			}
		}

	}
}