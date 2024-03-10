namespace App;

using System.Diagnostics;
using System.Net.Mime;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

public class McToolController : WebController {
	private readonly AppSetting appSetting;
	private readonly IWebHostEnvironment environment;
	private readonly ILogger<McToolController> logger;
	private readonly CodeConversionService codeConversionService;
	private readonly McToolService service;

	public McToolController(
		IOptionsSnapshot<AppSetting> snapshot,
		IWebHostEnvironment environment,
		ILogger<McToolController> logger,
		CodeConversionService conversionService,
		McToolService service
	) {
		this.appSetting = snapshot.Value;
		this.environment = environment;
		this.logger = logger;
		this.codeConversionService = conversionService;
		this.service = service;
	}


	/// View: Index
	public async Task<IActionResult> Index(IFormFile? codeFile = null) {
		// Console.WriteLine(this.codeConversionService.ConvertDescription("""RED,BW,CONC., 3", 1.1/2", A234-WPB.std"""));
		// Console.WriteLine(this.codeConversionService.ConvertDescription("""
		// 	Reducer Exc. ASME B16.9 Sch XSxSch XS BW 1.5'' 1"
		// """));

		if (this.Request.Method == HttpMethod.Get.Method) {
			return this.View();
		}
		if (codeFile is null) {
			this.ViewBag.message = "No file was uploaded !";
			return this.View();
		}

		var uploadFileName = codeFile.FileName;

		// Prepare upload file
		var inFilePath = Path.Combine(this.environment.ContentRootPath, "storage", uploadFileName);
		using (var fileStream = new FileStream(inFilePath, FileMode.Create)) {
			await codeFile.CopyToAsync(fileStream);
		}

		// Convert code
		var excelData = await Task.Run(() => this.codeConversionService.ConvertCodeFromFile(inFilePath));

		// Write converted code to excel
		var outFilePath = Path.Combine(this.environment.ContentRootPath, "storage", Guid.NewGuid().ToString() + "-" + uploadFileName);
		ExcelHelper.WriteToExcelFile(outFilePath, excelData);

		// Response file to client
		var bytes = await System.IO.File.ReadAllBytesAsync(outFilePath);

		return this.File(bytes, MediaTypeNames.Application.Octet, "output-" + uploadFileName);
	}

	/// View: SignIn
	public async Task<IActionResult> SignIn(string? username, string? password) {
		// Skip authentication for local env
		if (this.appSetting.environment != AppSetting.ENV_DEVELOPMENT) {
			if (this.IsAuthenticated) {
				this.ViewBag.message = "Need sign in first !";
				return this.View("Index");
			}
			if (username is null || password is null) {
				this.ViewBag.message = "Please enter username and password !";
				return this.View();
			}
			var verification = await this.service.VerifySignIn(username, password);
			if (verification.failed) {
				this.ViewBag.message = "Incorrect username or password !";
				return this.View();
			}
		}

		var claims = new List<Claim>() {
			new(ClaimTypes.Name, this.appSetting.authCookie.audience),
			new(ClaimTypes.Email, username)
		};
		var claimIdentity = new ClaimsIdentity(claims, this.appSetting.authCookie.issuer);

		await this.HttpContext.SignInAsync(
			scheme: CookieAuthenticationDefaults.AuthenticationScheme,
			principal: new ClaimsPrincipal(claimIdentity),
			properties: new AuthenticationProperties {
				IsPersistent = true, // For 'remember me' feature
				ExpiresUtc = DateTime.UtcNow.AddDays(30)
			}
		);

		this.ViewBag.message = "Signed in !";

		return this.View("Index");
	}

	public async Task<IActionResult> Logout() {
		await this.HttpContext.SignOutAsync();

		this.ViewBag.message = "Logged out !";

		return this.View("Index");
	}

	/// View: Setting
	// [Authorize]
	public async Task<IActionResult> Setting(IFormFile? settingFile, string? uploadSettingFile, string? downloadSettingFile) {
		if (this.appSetting.environment != AppSetting.ENV_DEVELOPMENT) {
			if (!this.IsAuthenticated) {
				return this.View("SignIn");
			}
		}

		// Case upload
		if (uploadSettingFile != null) {
			if (settingFile is null) {
				this.ViewBag.message = "No setting file was uploaded !";
				return this.View();
			}

			var uploadFileName = settingFile.FileName;

			// Copy uploaded file to our tmp storage
			var tmpSettingFileRelativePath = Path.Combine("storage", "tmp-setting.xlsx");
			var settingFilePath = Path.Combine(this.environment.ContentRootPath, tmpSettingFileRelativePath);
			using (var fileStream = new FileStream(settingFilePath, FileMode.Create)) {
				await settingFile.CopyToAsync(fileStream);
			}

			// Reload setting
			try {
				var result = await CodeConversionService.LoadSetting(settingFilePath);
				if (result.failed) {
					throw new Exception(result.message);
				}

				// If ok, move to setting file
				System.IO.File.Move(tmpSettingFileRelativePath, AppConst.MCTOOL_SETTING_FILE_RELATIVE_PATH, overwrite: true);

				this.ViewBag.message = "Reload setting successful !";
			}
			catch (Exception e) {
				this.ViewBag.message = e.Message;
			}

			return this.View();
		}

		// Case download
		if (downloadSettingFile != null) {
			// Response file to client
			var bytes = await System.IO.File.ReadAllBytesAsync(AppConst.MCTOOL_SETTING_FILE_RELATIVE_PATH);

			return this.File(bytes, MediaTypeNames.Application.Octet, "Setting.xlsx");
		}

		return this.View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error() {
		this.RedirectToAction("Index");

		return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
	}
}
