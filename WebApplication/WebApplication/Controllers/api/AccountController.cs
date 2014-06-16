using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebApplication.Models;
using WebApplication.Providers;
using WebApplication.Results;

namespace WebApplication.Controllers.api
{
	[Authorize]
	[RoutePrefix("api/Account")]
	public class AccountController : ApiController
	{
		private const string LocalLoginProvider = "Local";

		public AccountController()
			: this(Startup.UserManagerFactory(), Startup.OAuthOptions.AccessTokenFormat)
		{
		}

		public AccountController(UserManager<ApplicationUser> userManager,
			ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
		{
			UserManager = userManager;
			AccessTokenFormat = accessTokenFormat;
		}

		public UserManager<ApplicationUser> UserManager { get; private set; }
		public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

		// GET api/Account/UserInfo
		[HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
		[Route("UserInfo")]
		public UserInfoViewModel GetUserInfo()
		{
			ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

			return new UserInfoViewModel
			{
				UserName = User.Identity.GetUserName(),
				HasRegistered = externalLogin == null,
				LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
			};
		}

		// POST api/Account/Logout
		[Route("Logout")]
		public IHttpActionResult Logout()
		{
			Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
			return Ok();
		}

		// GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
		[Route("ManageInfo")]
		public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
		{
			ApplicationUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

			if (user == null)
			{
				return null;
			}

			List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

			foreach (IdentityUserLogin linkedAccount in user.Logins)
			{
				logins.Add(new UserLoginInfoViewModel
				{
					LoginProvider = linkedAccount.LoginProvider,
					ProviderKey = linkedAccount.ProviderKey
				});
			}

			if (user.PasswordHash != null)
			{
				logins.Add(new UserLoginInfoViewModel
				{
					LoginProvider = LocalLoginProvider,
					ProviderKey = user.UserName,
				});
			}

			return new ManageInfoViewModel
			{
				LocalLoginProvider = LocalLoginProvider,
				UserName = user.UserName,
				Logins = logins,
				ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
			};
		}

		// POST api/Account/ChangePassword
		[Route("ChangePassword")]
		public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
				model.NewPassword);
			IHttpActionResult errorResult = GetErrorResult(result);

			if (errorResult != null)
			{
				return errorResult;
			}

			return Ok();
		}

		// POST api/Account/SetPassword
		[Route("SetPassword")]
		public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
			IHttpActionResult errorResult = GetErrorResult(result);

			if (errorResult != null)
			{
				return errorResult;
			}

			return Ok();
		}

		// POST api/Account/AddExternalLogin
		[Route("AddExternalLogin")]
		public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

			AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

			if (ticket == null || ticket.Identity == null || (ticket.Properties != null
				&& ticket.Properties.ExpiresUtc.HasValue
				&& ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
			{
				return BadRequest("External login failure.");
			}

			ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

			if (externalData == null)
			{
				return BadRequest("The external login is already associated with an account.");
			}

			IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
				new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

			IHttpActionResult errorResult = GetErrorResult(result);

			if (errorResult != null)
			{
				return errorResult;
			}

			return Ok();
		}

		// POST api/Account/RemoveLogin
		[Route("RemoveLogin")]
		public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			IdentityResult result;

			if (model.LoginProvider == LocalLoginProvider)
			{
				result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
			}
			else
			{
				result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
					new UserLoginInfo(model.LoginProvider, model.ProviderKey));
			}

			IHttpActionResult errorResult = GetErrorResult(result);

			if (errorResult != null)
			{
				return errorResult;
			}

			return Ok();
		}

		// http://stackoverflow.com/a/21358918/361100 참고.
		// GET api/Account/ExternalLogin
		[OverrideAuthentication]
		[HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
		[AllowAnonymous]
		[Route("ExternalLogin", Name = "ExternalLogin")]
		public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
		{
			if (error != null)
			{
				return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
			}

			if (!User.Identity.IsAuthenticated)
			{
				return new ChallengeResult(provider, this);
			}

			ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

			if (externalLogin == null)
			{
				return InternalServerError();
			}

			if (externalLogin.LoginProvider != provider)
			{
				Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
				return new ChallengeResult(provider, this);
			}

			ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
				externalLogin.ProviderKey));

			bool hasRegistered = user != null;

			if (hasRegistered)
			{
				Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
				ClaimsIdentity oAuthIdentity = await UserManager.CreateIdentityAsync(user,
					OAuthDefaults.AuthenticationType);
				ClaimsIdentity cookieIdentity = await UserManager.CreateIdentityAsync(user,
					CookieAuthenticationDefaults.AuthenticationType);
				AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
				Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
			}
			else
			{
				IEnumerable<Claim> claims = externalLogin.GetClaims();
				ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
				Authentication.SignIn(identity);
			}

			return Ok();
		}

		// GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
		[AllowAnonymous]
		[Route("ExternalLogins")]
		public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
		{
			IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
			List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

			string state;

			if (generateState)
			{
				const int strengthInBits = 256;
				state = RandomOAuthStateGenerator.Generate(strengthInBits);
			}
			else
			{
				state = null;
			}

			foreach (AuthenticationDescription description in descriptions)
			{
				ExternalLoginViewModel login = new ExternalLoginViewModel
				{
					Name = description.Caption,
					Url = Url.Route("ExternalLogin", new
					{
						provider = description.AuthenticationType,
						response_type = "token",
						client_id = Startup.PublicClientId,
						redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
						state = state
					}),
					State = state
				};
				logins.Add(login);
			}

			return logins;
		}

		// POST api/Account/Register
		[AllowAnonymous]
		[Route("Register")]
		public async Task<IHttpActionResult> Register(RegisterBindingModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			ApplicationUser user = new ApplicationUser
			{
				UserName = model.UserName
			};

			IdentityResult result = await UserManager.CreateAsync(user, model.Password);
			IHttpActionResult errorResult = GetErrorResult(result);

			if (errorResult != null)
			{
				return errorResult;
			}

			return Ok();
		}

		// POST api/Account/RegisterExternal
		[OverrideAuthentication]
		[HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
		[Route("RegisterExternal")]
		public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

			if (externalLogin == null)
			{
				return InternalServerError();
			}

			ApplicationUser user = new ApplicationUser
			{
				UserName = model.UserName
			};
			user.Logins.Add(new IdentityUserLogin
			{
				LoginProvider = externalLogin.LoginProvider,
				ProviderKey = externalLogin.ProviderKey
			});
			IdentityResult result = await UserManager.CreateAsync(user);
			IHttpActionResult errorResult = GetErrorResult(result);

			if (errorResult != null)
			{
				return errorResult;
			}

			return Ok();
		}

		// POST api/Account/FacebookLogin
		[HttpPost]
		[AllowAnonymous]
		[Route("FacebookLogin")]
		public async Task<IHttpActionResult> FacebookLogin([FromBody] FacebookLoginModel model)
		{
			string log = String.Empty;
			// http://thewayofcode.wordpress.com/2014/03/01/asp-net-webapi-identity-system-how-to-login-with-facebook-access-token/

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (string.IsNullOrEmpty(model.token))
			{
				return BadRequest("No access token");
			}

			var tokenExpirationTimeSpan = TimeSpan.FromDays(300);
			ApplicationUser user = null;
			string username;
			// Get the fb access token and make a graph call to the /me endpoint
			var fbUser = await VerifyFacebookAccessToken(model.token);
			if (fbUser == null)
			{
				return BadRequest("Invalid OAuth access token");
			}

			UserLoginInfo loginInfo = new UserLoginInfo("Facebook", model.userid);
			user = await UserManager.FindAsync(loginInfo);

			//var loginInfo = await HttpContext.Current.GetOwinContext().Authentication.GetExternalLoginInfoAsync("XsrfId", userid);
			// If not, register it
			if (user == null)
			{
				if (String.IsNullOrEmpty(model.username))
					return BadRequest("unregistered user");

				log += "  1";
				user = new ApplicationUser { UserName = model.username };

				var result = await UserManager.CreateAsync(user);
				if (result.Succeeded)
				{
					result = await UserManager.AddLoginAsync(user.Id, loginInfo);
					username = model.username;
					if (!result.Succeeded)
						return BadRequest("cannot add facebook login");
					log += "  2";
				}
				else
				{
					return BadRequest("cannot create user");
				}

				//var randomPassword = System.Web.Security.Membership.GeneratePassword(10, 5);
				//user = await RegisterUserAsync(fbUser.Username, randomPassword, fbUser.ID);
				//var customer = await RegisterCustomerAsync(fbUser.FirstName, fbUser.LastName, fbUser.Email, user);
			}
			else
			{
				// 이미 있는 유저.
				username = user.UserName;
				log += "  3";
			}

			// 공통 프로세스: 페이스북 클레임 업데이트, 로그인 토큰 생성

			// Check if the user is already registered
			user = await UserManager.FindByNameAsync(username);

			// 자동 이메일 인증.
			user.Email = fbUser.email;
			user.EmailConfirmed = true;
			await UserManager.UpdateAsync(user);

			// Sign-in the user using the OWIN flow
			var identity = new ClaimsIdentity(Startup.OAuthBearerOptions.AuthenticationType);

			//identity.AddClaim(new Claim("FacebookAccessToken", model.token));
			//identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName, null, "Facebook"));
			//// This is very important as it will be used to populate the current user id 
			//// that is retrieved with the User.Identity.GetUserId() method inside an API Controller
			//identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id, null, "LOCAL_AUTHORITY"));

			var claims = await UserManager.GetClaimsAsync(user.Id);
			var newClaim = new Claim("FacebookAccessToken", model.token);
			var facebookClaim = claims.FirstOrDefault(c => c.Type.Equals("FacebookAccessToken"));
			if (facebookClaim == null)
			{
				var claimResult = await UserManager.AddClaimAsync(user.Id, newClaim);
				if (!claimResult.Succeeded)
					return BadRequest("cannot add claims");
				log += "  4";

			}
			else
			{
				await UserManager.RemoveClaimAsync(user.Id, facebookClaim);
				await UserManager.AddClaimAsync(user.Id, newClaim);
				log += "  5";
			}

			AuthenticationTicket ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
			var currentUtc = new Microsoft.Owin.Infrastructure.SystemClock().UtcNow;
			ticket.Properties.IssuedUtc = currentUtc;
			ticket.Properties.ExpiresUtc = currentUtc.Add(tokenExpirationTimeSpan);
			var accesstoken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
			Request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accesstoken);
			Authentication.SignIn(identity);

			// Create the response building a JSON object that mimics exactly the one issued by the default /Token endpoint
			JObject blob = new JObject(
				new JProperty("userName", user.UserName),
				new JProperty("access_token", accesstoken),
				new JProperty("token_type", "bearer"),
				new JProperty("expires_in", tokenExpirationTimeSpan.TotalSeconds.ToString()),
				new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
				new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString()),
				new JProperty("model.token", model.token),
				new JProperty("log", log)
			);
			// Return OK
			return Ok(blob);
		}

		//private async Task<ClaimsIdentity> StoreFacebookAuthToken(ApplicationUser user)
		//{
		//	var claimsIdentity = await AuthenticationManager.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);
		//	if (claimsIdentity != null)
		//	{
		//		// Retrieve the existing claims for the user and add the FacebookAccessTokenClaim
		//		var currentClaims = await UserManager.GetClaimsAsync(user.Id);
		//		var hasFacebook = claimsIdentity.FindAll("FacebookAccessToken");
		//		if (hasFacebook != null && hasFacebook.Any())
		//		{
		//			var facebookAccessToken = claimsIdentity.FindAll("FacebookAccessToken").First();
		//			if (!currentClaims.Any())
		//			{
		//				await UserManager.AddClaimAsync(user.Id, facebookAccessToken);
		//			}
		//		}
		//	}
		//}

		private async Task<FacebookUserViewModel> VerifyFacebookAccessToken(string accessToken)
		{
			FacebookUserViewModel fbUser = null;
			var path = "https://graph.facebook.com/me?access_token=" + accessToken;
			var client = new HttpClient();
			var uri = new Uri(path);
			var response = await client.GetAsync(uri);
			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				fbUser = Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookUserViewModel>(content);
			}
			return fbUser;
		}


		public class FacebookUserViewModel
		{
			public string id { get; set; }
			public string first_name { get; set; }
			public string last_name { get; set; }
			public string username { get; set; }
			public string email { get; set; }
		}
		public class FacebookLoginModel
		{
			//[JsonProperty("id")]
			public string token { get; set; }
			//[JsonProperty("first_name")]
			public string username { get; set; }
			//[JsonProperty("last_name")]
			public string userid { get; set; }
		}



		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				UserManager.Dispose();
			}

			base.Dispose(disposing);
		}

		#region Helpers

		private IAuthenticationManager Authentication
		{
			get { return Request.GetOwinContext().Authentication; }
		}

		private IHttpActionResult GetErrorResult(IdentityResult result)
		{
			if (result == null)
			{
				return InternalServerError();
			}

			if (!result.Succeeded)
			{
				if (result.Errors != null)
				{
					foreach (string error in result.Errors)
					{
						ModelState.AddModelError("", error);
					}
				}

				if (ModelState.IsValid)
				{
					// No ModelState errors are available to send, so just return an empty BadRequest.
					return BadRequest();
				}

				return BadRequest(ModelState);
			}

			return null;
		}

		private class ExternalLoginData
		{
			public string LoginProvider { get; set; }
			public string ProviderKey { get; set; }
			public string UserName { get; set; }

			public IList<Claim> GetClaims()
			{
				IList<Claim> claims = new List<Claim>();
				claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

				if (UserName != null)
				{
					claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
				}

				return claims;
			}

			public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
			{
				if (identity == null)
				{
					return null;
				}

				Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

				if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
					|| String.IsNullOrEmpty(providerKeyClaim.Value))
				{
					return null;
				}

				if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
				{
					return null;
				}

				return new ExternalLoginData
				{
					LoginProvider = providerKeyClaim.Issuer,
					ProviderKey = providerKeyClaim.Value,
					UserName = identity.FindFirstValue(ClaimTypes.Name)
				};
			}
		}

		private static class RandomOAuthStateGenerator
		{
			private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

			public static string Generate(int strengthInBits)
			{
				const int bitsPerByte = 8;

				if (strengthInBits % bitsPerByte != 0)
				{
					throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
				}

				int strengthInBytes = strengthInBits / bitsPerByte;

				byte[] data = new byte[strengthInBytes];
				_random.GetBytes(data);
				return HttpServerUtility.UrlTokenEncode(data);
			}
		}

		#endregion
	}
}

