using App;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

/// [Services]
var services = builder.Services;

// Change path to Views
// Views: https://learn.microsoft.com/en-us/aspnet/core/mvc/views/overview?view=aspnetcore-7.0
services.Configure<RazorViewEngineOptions>(options => {
	// Clear default paths and add our view paths.
	options.ViewLocationFormats.Clear();
	options.ViewLocationFormats.Add("/Source/Presentation/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
	options.ViewLocationFormats.Add("/Source/Presentation/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
});

// Add services to the container.
services.AddControllersWithViews();

// Register appsettings.json which be bind with TOptions.
// Ref: https://stackoverflow.com/questions/40470556/actually-read-appsettings-in-configureservices-phase-in-asp-net-core
_ = services.Configure<AppSetting>(config.GetSection(AppSetting.SECTION_APP));

// DI (dependency injection)
services
	.AddScoped<McToolService>()
	.AddScoped<CodeConversionService>()
;

// Our app setting
var appSetting = config.GetSection(AppSetting.SECTION_APP).Get<AppSetting>()!;
var isDevelopment = appSetting.environment == AppSetting.ENV_DEVELOPMENT;
var isStaging = appSetting.environment == AppSetting.ENV_STAGING;
var isProduction = appSetting.environment == AppSetting.ENV_PRODUCTION;

Console.WriteLine($"----> Enable cronjob: {appSetting.taskMode.enableCronJob}");
Console.WriteLine($"----> Enable command: {appSetting.taskMode.enableCommand}");

// Config database connections
if (appSetting.environment != AppSetting.ENV_DEVELOPMENT) {
	services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(appSetting.database.appdb));
}

// Config Authentication (JWT or Cookie)
// services.ConfigureJwtAuthenticationDk(appSetting);
services.ConfigureCookieBasedAuthenticationDk(appSetting);

// Load custom code
await CodeConversionService.LoadSetting();


/// [App]
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add authentication middleware (authenticate with JWT)
// Requires authenticated access via [Authenticate] annotation
app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
