using DTech.DAO;
using DTech.Library;
using DTech.Library.Service;
using DTech.Models.EF;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.Facebook;


var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
var envFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
DotEnv.Load(options: new DotEnvOptions(envFilePaths: new[] { envFilePath }));

builder.Configuration
       .AddJsonFile("appsettings.json")
       .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<EcommerceWebContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DTech"));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<EcommerceWebContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie(options =>
    {
        options.LoginPath = "/Authentication/Login";
        options.LogoutPath = "/Authentication/Logout";
        options.AccessDeniedPath = "/Authentication/AccessDenied";
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToAccessDenied = context =>
            {
                
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    context.Response.StatusCode = 403;
                    context.Response.ContentType = "application/json";
                    var result = System.Text.Json.JsonSerializer.Serialize(new { error = "You do not have permission to access this resource" });
                    return context.Response.WriteAsync(result);
                }

                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            }
        };
    })
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.ClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") ?? string.Empty;
        options.ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET") ?? string.Empty;
        options.CallbackPath = "/signin-google";
    })
    .AddFacebook(options =>
    {
        options.ClientId = Environment.GetEnvironmentVariable("FACEBOOK_CLIENT_ID") ?? string.Empty;
        options.ClientSecret = Environment.GetEnvironmentVariable("FACEBOOK_CLIENT_SECRET") ?? string.Empty;
        options.CallbackPath = "/signin-facebook";
    });

builder.Services.AddRazorPages();


builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    //options.Password.RequiredUniqueChars = 1;
    options.Password.RequiredLength = 6;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});

//Register DAO
var daoAssembly = typeof(AdminDAO).Assembly;

foreach (var type in daoAssembly.GetTypes())
{
    if (type.IsClass && !type.IsAbstract && type.Name.EndsWith("DAO"))
    {
        builder.Services.AddScoped(type);
    }
}

// Register the service
builder.Services.AddHostedService<CodeStatusCheckerService>();
builder.Services.AddSingleton<CloudinaryService>();
builder.Services.AddTransient<IEmailService, EmailService>();

// Register the setting helper
foreach (var type in daoAssembly.GetTypes())
{
    if (type.IsClass && !type.IsAbstract && type.Name.EndsWith("Helper"))
    {
        builder.Services.AddScoped(type);
    }
}

// Resgister the ViewBag
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<SetViewBagAttributesAttribute>();
});

//Time out
builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
    options.LoginPath = "/Authentication/Login";
    options.AccessDeniedPath = "/Authentication/AccessDenied";
});

var app = builder.Build();

//Not Found
app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
