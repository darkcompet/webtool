namespace App;

using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class BingoSeedCodesController : BingoBaseController {
	private readonly AppDbContext dbContext;
	private readonly ILogger<BingoSeedCodesController> logger;

	public BingoSeedCodesController(
		AppDbContext dbContext,
		ILogger<BingoSeedCodesController> logger
	) {
		this.dbContext = dbContext;
		this.logger = logger;
	}

	public int generatedBingoCodeCount { get; private set; }

	public async Task<IActionResult> OnGet([FromQuery] int fromCode, [FromQuery] int toCode) {
		var bingoCode = toCode;
		while (bingoCode >= fromCode) {
			this.dbContext.bingos.Attach(new BingoModel() {
				code = bingoCode.ToString()
			});
			--bingoCode;
			++generatedBingoCodeCount;
		}
		await this.dbContext.SaveChangesAsync();

		return View();
	}
}
