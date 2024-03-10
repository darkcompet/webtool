namespace App;

using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class BingoIndexController : BingoBaseController {
	private readonly AppDbContext dbContext;
	private readonly ILogger<BingoIndexController> logger;

	public BingoIndexController(
		AppDbContext dbContext,
		ILogger<BingoIndexController> logger
	) {
		this.dbContext = dbContext;
		this.logger = logger;
	}

	public bool couldNotCreateNewUser { get; private set; }
	public BingoClientModel? user { get; private set; }

	public async Task<IActionResult> Index() {
		// Get or Create new client id
		var uid = GetOrGenerateClientUid();
		var user = await this.dbContext.bingoClients.FirstOrDefaultAsync(m => m.client_id == uid);

		// Update view data
		this.user = user;

		return View();
	}

	public async Task<IActionResult> OnPost() {
		var uid = GetOrGenerateClientUid();
		var user = await this.dbContext.bingoClients.FirstOrDefaultAsync(m => m.client_id == uid);

		if (user is null) {
			var raw_user_name = this.Request.Form["user_name"];
			var user_name = $"{raw_user_name}".Replace(",", "").Trim();

			if (user_name.Length > 0) {
				user = await this.CreateNewUserWithBingoCodeRecursively(uid, user_name);

				if (user is null) {
					this.couldNotCreateNewUser = true;
					return View();
				}
			}
		}

		// Update view data
		this.user = user;

		return View();
	}

	private async Task<BingoClientModel?> CreateNewUserWithBingoCodeRecursively(string uid, string user_name, int tryCount = 0) {
		Console.WriteLine($"---> CreateNewUserWithBingoCodeRecursively, tryCount: {tryCount}");

		if (tryCount > 10) {
			return null;
		}

		using (var txScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) {
			try {
				// Lock on a bingo
				var rnd_bingo_id = await this.dbContext.bingos
					.Where(m => m.consumed_at == null)
					.Select(m => m.id)
					.OrderBy(m => Guid.NewGuid())
					.FirstOrDefaultAsync()
				;
				var bingo = await this.dbContext.bingos
					.FromSqlRaw($"SELECT * FROM [{DbConst.table_bingo}] WITH (UPDLOCK) WHERE [id] = {{0}}", rnd_bingo_id)
					.FirstOrDefaultAsync()
				;

				if (bingo is null || bingo.consumed_at != null) {
					return await CreateNewUserWithBingoCodeRecursively(uid, user_name, tryCount + 1);
				}

				// Consume the bingo
				bingo.consumed_at = DateTime.UtcNow;

				var newUser = new BingoClientModel() {
					client_id = uid,
					name = user_name,
					bingo_code = bingo.code
				};
				this.dbContext.bingoClients.Attach(newUser);

				await this.dbContext.SaveChangesAsync();
				txScope.Complete();

				return newUser;
			}
			catch (Exception e) {
				Console.WriteLine($"---> Could not create new user, tryCount: {tryCount}, error: {e.Message}");

				// Try again
				return await CreateNewUserWithBingoCodeRecursively(uid, user_name, tryCount + 1);
			}
		}
	}
}
