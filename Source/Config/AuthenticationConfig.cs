namespace App;

using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

public static class AuthenticationConfig {
	public static void ConfigureJwtAuthenticationDk(this IServiceCollection services, AppSetting appSetting) {
		services
			// Use Bearer token for authentication challenge by `Authorize` attribute.
			.AddAuthentication(options => {
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options => {
				options.RequireHttpsMetadata = true;
				options.SaveToken = true;
				options.TokenValidationParameters = new TokenValidationParameters {
					// Always check lifetime of the token (this is used at `Authorize` attribute).
					// Note: Set clockSkew to zero (default value is 5 minutes) so we can start expiration-timeout for a token.
					ClockSkew = TimeSpan.Zero,
					ValidateLifetime = true,

					ValidateIssuer = true,
					ValidIssuer = appSetting.jwt.issuer,

					ValidateAudience = true,
					ValidAudience = appSetting.jwt.audience,

					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSetting.jwt.key))
				};
			});
	}

	public static void ConfigureCookieBasedAuthenticationDk(this IServiceCollection services, AppSetting appSetting) {
		// Ref: https://referbruv.com/blog/implementing-cookie-authentication-in-aspnet-core-without-identity/
		services
			.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
			.AddCookie(options => {
				options.LoginPath = "/Home/Index";
				options.LogoutPath = "/Home/Index";
				options.Events = new CookieAuthenticationEvents {
					OnSignedIn = context => {
						Console.WriteLine("---> 1. {0} - {1}: {2}", DateTime.Now, "OnSignedIn", context.Principal.Identity.Name);
						return Task.CompletedTask;
					},
					OnSigningOut = context => {
						Console.WriteLine("---> 2. {0} - {1}: {2}", DateTime.Now, "OnSigningOut", context.HttpContext.User.Identity.Name);
						return Task.CompletedTask;
					},
					OnValidatePrincipal = context => {
						Console.WriteLine("---> 3. {0} - {1}: {2}", DateTime.Now, "OnValidatePrincipal", context.Principal.Identity.Name);
						return Task.CompletedTask;
					}
				};
			})
		;
	}
}
