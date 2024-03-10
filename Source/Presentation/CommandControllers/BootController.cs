namespace App;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

/// Example to run BootProject api at local:
/// httprepl https://localhost:8000
/// cd api/project/boot
/// post
[ApiController, Route("api")]
public class BootController : ControllerBase {
	private readonly AppSetting appSetting;
	private readonly ILogger logger;
	private readonly BootCommand command;

	public BootController(
		IOptionsSnapshot<AppSetting> option,
		ILogger<BootController> logger,
		BootCommand command
	) {
		this.appSetting = option.Value;
		this.logger = logger;
		this.command = command;
	}

	/// <summary>
	/// Boot project. Run at initial time after configured servers.
	/// </summary>
	/// <response code="200"></response>
	[HttpPost, Route("cmd/project/Boot")]
	public async Task<ApiResponse> BootProject() {
		if (!this.appSetting.taskMode.enableCommand) {
			return new ApiBadRequestResponse("Bad mode");
		}
		return await this.command.BootProject();
	}
}
