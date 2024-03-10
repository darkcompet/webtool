namespace App;

using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class BingoNextWinnerController : BingoBaseController {
	private readonly AppDbContext dbContext;
	private readonly ILogger<BingoNextWinnerController> logger;

	public BingoNextWinnerController(
		AppDbContext dbContext,
		ILogger<BingoNextWinnerController> logger
	) {
		this.dbContext = dbContext;
		this.logger = logger;
	}

	public List<string> winnerBingoCodes { get; private set; } = new();
	public bool ownerNotExist { get; private set; }
	public bool noWinnerToGenerate { get; private set; }

	public async Task<IActionResult> NextWinner() {
		return View();

		var owner_client_id = this.GetOrGenerateClientUid();
		var owner = await this.dbContext.bingoClients.FirstOrDefaultAsync(m => m.client_id == owner_client_id);
		if (owner is null) {
			this.ownerNotExist = true;

			return View();
		}

		var nextWinner = await this.dbContext.bingoClients
			.Where(m => m.bingo_hit_at == null)
			.OrderBy(m => Guid.NewGuid())
			.FirstOrDefaultAsync()
		;
		if (nextWinner is null) {
			this.winnerBingoCodes = await this.dbContext.bingoClients
				.Where(m => m.bingo_hit_at != null && m.winner_issued_by_user == owner.id)
				.Select(m => m.bingo_code.ToString())
				.ToListAsync();

			this.noWinnerToGenerate = true;

			return View();
		}

		// Choose this user as winner
		nextWinner.bingo_hit_at = DateTime.UtcNow;
		nextWinner.winner_issued_by_user = owner.id;

		await this.dbContext.SaveChangesAsync();

		// Update UI
		this.winnerBingoCodes = await this.dbContext.bingoClients
			.Where(m => m.bingo_hit_at != null && m.winner_issued_by_user == owner.id)
			.Select(m => m.bingo_code.ToString())
			.ToListAsync();

		return View();
	}
}
