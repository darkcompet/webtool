namespace App;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

/// For seeding data via api.
/// Admin+ role is required to run this service.
/// For 3rd-authentication provider (google, facebook,...)
/// https://developers.google.com/identity/one-tap/android/idtoken-auth
/// https://developers.google.com/api-client-library
/// AccessToken and RefreshToken: https://codepedia.info/aspnet-core-jwt-refresh-token-authentication
public class BootCommand : BaseService {
	private readonly ILogger<BootCommand> logger;

	public BootCommand(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<BootCommand> logger
	) : base(dbContext, snapshot) {
		this.logger = logger;
	}

	/// Should write all boot commands in this function with date suffix.
	/// For new setup, just comment out all commands here and run them onetime.
	public async Task<ApiResponse> BootProject() {
		return await this._BootProject20231001();
	}

	private async Task<ApiResponse> _BootProject20231001() {
		return ApiResponse.Success("Done _BootProject20231001");
	}
}
