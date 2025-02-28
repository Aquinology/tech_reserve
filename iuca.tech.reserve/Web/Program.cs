using Application;
using Infrastructure.Data;
using NLog;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Add services for database operations and authentication
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add custom application interfaces and services
builder.Services.AddApplicationServices();

// Configure NLog for logging
LogManager.LoadConfiguration("nlog.config"); // Load log configuration from the nlog.config file
builder.Logging.ClearProviders(); // Clear built-in log providers
builder.Logging.AddNLogWeb(); // Add NLog as a logging provider

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Equipments/Error");
    app.UseHsts();
}
else
{
    // Populate the database with initial data
    await app.InitialiseDatabaseAsync();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Configure authentication
app.UseAuthentication();

app.UseAuthorization();

// Redirect to the homepage on 404 error (update the URL in the address bar)
app.UseStatusCodePages(context =>
{
    if (context.HttpContext.Response.StatusCode == 404)
    {
        context.HttpContext.Response.Redirect("/");
    }
    return Task.CompletedTask;
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Equipments}/{action=Index}/{id?}");

app.Run();
