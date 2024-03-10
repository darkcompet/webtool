namespace App;

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller {
	private readonly IWebHostEnvironment environment;
	private readonly ILogger<HomeController> logger;

	public HomeController(
		IWebHostEnvironment environment,
		ILogger<HomeController> logger
	) {
		this.environment = environment;
		this.logger = logger;
	}

	public IActionResult Index() {
		return View();
	}

	public IActionResult McTool() {
		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error() {
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
