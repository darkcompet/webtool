namespace App;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public class McToolService : BaseService {
	private readonly ILogger<McToolService> logger;

	public McToolService(
		AppDbContext dbContext,
		IOptionsSnapshot<AppSetting> snapshot,
		ILogger<McToolService> logger
	) : base(dbContext: dbContext, snapshot: snapshot) {
		this.logger = logger;
	}

	public async Task<ApiResponse> VerifySignIn(string username, string password) {
		var passwordHasher = new PasswordHasher<UserModel>();
		var user = await this.dbContext.users.FirstOrDefaultAsync(m => m.code == username || m.email == username);
		if (user is null || user.password is null) {
			return new ApiBadRequestResponse("No user");
		}
		if (passwordHasher.VerifyHashedPassword(user, user.password, password) == PasswordVerificationResult.Failed) {
			return new ApiUnauthorizedResponse("Invalid");
		}

		return new ApiSuccessResponse();
	}
}
