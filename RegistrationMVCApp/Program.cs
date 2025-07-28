using Microsoft.AspNetCore.Authentication.Cookies;
using RegistrationApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient(); // for LoginController's constructor

builder.Services.AddHttpClient<RegistrationApiClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7028/"); // API base URL
});

builder.Services.AddHttpContextAccessor();

// ? Configure Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Registration/Login"; // redirects to login if unauthorized
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Optional: Session expiration
        options.SlidingExpiration = true;
    });

// ? Configure Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Registration/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ? Correct Middleware Order:
app.UseSession();          // Session FIRST
app.UseAuthentication();   // THEN Authentication
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Registration}/{action=Index}/{id?}");

app.Run();
