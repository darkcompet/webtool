namespace App;

using System.Net;
using Tool.Compet.Json;

/// <summary>
/// Api exception.
/// </summary>
[Serializable]
public class ApiException(int status, string? code = null, string? msg = null, object? data = null, Exception? innerException = null)
	: Exception(msg, innerException) {
	/// <summary>
	/// Http status code.
	/// </summary>
	public int status { get; set; } = status;

	/// <summary>
	/// Response body code.
	/// </summary>
	public string? code { get; set; } = code;

	/// <summary>
	/// Json data field in body.
	/// </summary>
	public object? data { get; set; } = data;

	public override string ToString() {
		var baseMessage = base.ToString();
		var dataMessage = this.data != null ? $"\nAdditional Data: {DkJsons.ToJson(this.data)}" : "";

		return baseMessage + dataMessage;
	}

	/// <summary>
	/// Use this when client should fix then request again.
	/// </summary>
	/// <param name="msg"></param>
	/// <param name="code"></param>
	/// <param name="data"></param>
	/// <returns></returns>
	public static ApiException BadRequest(string? msg = "Bad request", string? code = null, object? data = null, Exception? innerException = null) {
		return new ApiException((int)HttpStatusCode.BadRequest, code, msg, data, innerException);
	}

	/// <summary>
	/// Response with status code 401 indicates client has not access to the site.
	/// </summary>
	/// <param name="msg"></param>
	/// <param name="code"></param>
	/// <param name="data"></param>
	/// <returns></returns>
	public static ApiException Unauthorized(string? msg = "Unauthorized", string? code = null, object? data = null, Exception? innerException = null) {
		return new ApiException((int)HttpStatusCode.Unauthorized, code, msg, data, innerException);
	}

	/// <summary>
	/// Response with status code 403 indicates client has not permission to the resource.
	/// </summary>
	/// <param name="msg"></param>
	/// <param name="code"></param>
	/// <param name="data"></param>
	/// <returns></returns>
	public static ApiException Forbidden(string? msg = "Forbidden", string? code = null, object? data = null, Exception? innerException = null) {
		return new ApiException((int)HttpStatusCode.Forbidden, code, msg, data, innerException);
	}

	/// <summary>
	/// Use this when internal server got some error. So client cannot handle by itself.
	/// </summary>
	/// <param name="msg"></param>
	/// <param name="code"></param>
	/// <param name="data"></param>
	/// <returns></returns>
	public static ApiException ServerError(string? msg = "Server error", string? code = null, object? data = null, Exception? innerException = null) {
		return new ApiException((int)HttpStatusCode.InternalServerError, code, msg, data, innerException);
	}

	public ApiException WithCode(string code) {
		this.code = code;
		return this;
	}

	public ApiException WithData(object data) {
		this.data = data;
		return this;
	}
}
