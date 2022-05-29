using Identity.ClaimProvider;
using Identity.CustomValidation;
using Identity.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<AppIdentityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerExpress"));
});

// Facebook Login
builder.Services.AddAuthentication().AddFacebook(opt =>
{
    opt.AppId = builder.Configuration.GetValue<string>("Authentication:Facebook:AppId");
    opt.AppSecret = builder.Configuration.GetValue<string>("Authentication:Facebook:AppSecret");
});

// Google Login
builder.Services.AddAuthentication().AddGoogle(opt =>
{
    opt.ClientId = builder.Configuration.GetValue<string>("Authentication:Google:ClientId");
    opt.ClientSecret = builder.Configuration.GetValue<string>("Authentication:Google:ClientSecret");
});

// Microsoft Login
builder.Services.AddAuthentication().AddMicrosoftAccount(opt =>
{
    opt.ClientId = builder.Configuration.GetValue<string>("Authentication:Microsoft:ClientId");
    opt.ClientSecret = builder.Configuration.GetValue<string>("Authentication:Microsoft:ClientSecret");
});

// Add Identity
builder.Services.AddIdentity<AppUser, AppRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.User.AllowedUserNameCharacters = "abcçdefgğhıijklmnoçpqrsştuüvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
        options.Password.RequiredLength = 4;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireDigit = false;
    }).AddPasswordValidator<CustomPasswordValidator>().AddUserValidator<CustomUserValidator>()
    .AddErrorDescriber<CustomIdentityErrorDescriber>()
    .AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();

//Adding cookie
CookieBuilder cookieBuilder = new();
cookieBuilder.Name = "MySite";
cookieBuilder.HttpOnly = false;
cookieBuilder.SameSite = SameSiteMode.Lax;
cookieBuilder.SecurePolicy = CookieSecurePolicy.SameAsRequest;

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = new PathString("/Home/Login");
    options.LogoutPath = new PathString("/Member/Logout");
    options.Cookie = cookieBuilder;
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(60);
    options.AccessDeniedPath = new PathString("/Member/AccessDenied");
    //options.LogoutPath= new PathString("");
});

builder.Services.AddTransient<IAuthorizationHandler, ExpireDateExchangeHandler>();
builder.Services.AddScoped<IClaimsTransformation, ClaimProvider>();
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("KutahyaPolicy", policy => { policy.RequireClaim("city", "Kütahya"); });
    opt.AddPolicy("ViolencePolicy", policy => { policy.RequireClaim("violence"); });
    opt.AddPolicy("ExchangePolicy", policy => { policy.AddRequirements(new Requirements()); });
});

var app = builder.Build();

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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();