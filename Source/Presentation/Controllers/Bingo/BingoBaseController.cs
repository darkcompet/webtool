namespace App;

using Microsoft.AspNetCore.Mvc;

public class BingoBaseController : Controller {
	public string GetOrGenerateClientUid() {
		var uid = this.Request.Cookies.FirstOrDefault(m => m.Key == AppConst.CLIENT_UID_KEY).Value;

		if (uid is null || uid.Trim().Length == 0) {
			uid = Guid.NewGuid().ToString("N");
			this.Response.Cookies.Append(AppConst.CLIENT_UID_KEY, uid);

			Console.WriteLine($"Generated new client uid: {uid}");
		}

		return uid;
	}
}
