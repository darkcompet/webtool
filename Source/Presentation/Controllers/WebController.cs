namespace App;

using Microsoft.AspNetCore.Mvc;

/// View supported base mvc-controller.
public class WebController : Controller {
	public bool IsAuthenticated => this.User.Identity?.IsAuthenticated ?? false;
}
