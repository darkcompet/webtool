namespace App;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class BingoStatisticController : BingoBaseController {
	private readonly AppDbContext dbContext;
	private readonly ILogger<BingoStatisticController> logger;

	public BingoStatisticController(
		AppDbContext dbContext,
		ILogger<BingoStatisticController> logger
	) {
		this.dbContext = dbContext;
		this.logger = logger;
	}

	public string[] targetBingoCodes { get; private set; }

	public async Task<IActionResult> Statistic([FromQuery] string? os = null) {
		this.targetBingoCodes = await this.dbContext.bingoClients
			.Where(m => m.bingo_hit_at == null && m.name != null)
			.Select(m => $"{m.name},{m.bingo_code}")
			.ToArrayAsync()
		;

		// Export txt file (client will download it at browser)
		var separator = os == "windows" ? "\r\n" : "\n";
		var bytes = System.Text.Encoding.UTF8.GetBytes(string.Join(separator, targetBingoCodes));

		// var tmpBingoDirPath = "./Assets/Tmp";
		// var bingoCodesFilePath = $"{tmpBingoDirPath}/{Guid.NewGuid().ToString()}.txt";
		// System.IO.Directory.CreateDirectory(tmpBingoDirPath);
		// await System.IO.File.WriteAllLinesAsync(bingoCodesFilePath, targetBingoCodes);
		// var bytes = await System.IO.File.ReadAllBytesAsync(bingoCodesFilePath);

		return File(bytes, "application/octet-stream", "userBingoCodes.txt");
	}
}
