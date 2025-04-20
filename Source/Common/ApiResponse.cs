namespace App;

using System.Net;
using System.Text.Json.Serialization;

/// <summary>
/// Resful api response for client.
/// </summary>
public class ApiResponse(int status, object? data = null, string? code = null, string? msg = null) {
	[JsonPropertyName("data")]
	public object? data { get; set; } = data;

	[JsonPropertyName("code")]
	public string? code { get; set; } = code;

	[JsonPropertyName("debug")]
	public string? msg { get; set; } = msg;

	[JsonIgnore] public int status { get; private set; } = status;
	[JsonIgnore] public bool succeed = status == (int)HttpStatusCode.OK;
	[JsonIgnore] public bool failed = status != (int)HttpStatusCode.OK;

	/// <summary>
	/// When just response default succeed response, just use this instance to reduce memory allocation.
	/// </summary>
	public static readonly ApiResponse Ok = new((int)HttpStatusCode.OK, msg: "Ok");

	/// <summary>
	/// Response with status code 200.
	/// </summary>
	/// <param name="data"></param>
	/// <param name="code"></param>
	/// <param name="msg"></param>
	/// <returns></returns>
	public static ApiResponse Success(object? data = null, string? code = null, string? msg = "Ok") {
		return new ApiResponse((int)HttpStatusCode.OK, data, code, msg);
	}

	/// <summary>
	/// Response with status code 400 indicate client should fix the problem before request again.
	/// </summary>
	/// <param name="data"></param>
	/// <param name="code"></param>
	/// <param name="msg"></param>
	/// <returns></returns>
	public static ApiResponse BadRequest(object? data = null, string? code = null, string? msg = "Bad request") {
		return new ApiResponse((int)HttpStatusCode.BadRequest, data, code, msg);
	}

	/// <summary>
	/// Response with status code 401 indicates client has not access to the site.
	/// </summary>
	/// <param name="data"></param>
	/// <param name="code"></param>
	/// <param name="msg"></param>
	/// <returns></returns>
	public static ApiResponse Unauthorized(object? data = null, string? code = null, string? msg = "Unauthorized") {
		return new ApiResponse((int)HttpStatusCode.Unauthorized, data, code, msg);
	}

	/// <summary>
	/// Response with status code 403 indicates client has not permission to the resource.
	/// </summary>
	/// <param name="data"></param>
	/// <param name="code"></param>
	/// <param name="msg"></param>
	/// <returns></returns>
	public static ApiResponse Forbidden(object? data = null, string? code = null, string? msg = "Forbidden") {
		return new ApiResponse((int)HttpStatusCode.Forbidden, data, code, msg);
	}

	/// <summary>
	/// Use this when internal server got some error. So client cannot handle by itself.
	/// </summary>
	/// <param name="data"></param>
	/// <param name="code"></param>
	/// <param name="msg"></param>
	/// <returns></returns>
	public static ApiResponse ServerError(object? data = null, string? code = null, string? msg = "Server error") {
		return new ApiResponse((int)HttpStatusCode.InternalServerError, data, code, msg);
	}

	public ApiResponse WithMessage(string msg) {
		this.msg = msg;
		return this;
	}

	public ApiResponse WithCode(string code) {
		this.code = code;
		return this;
	}

	public ApiResponse WithData(object data) {
		this.data = data;
		return this;
	}
}
