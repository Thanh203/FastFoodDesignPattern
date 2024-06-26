﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FastFoodSystem.WebApp.Models.Data;
using FastFoodSystem.WebApp.Models;
using DinkToPdf.Contracts;
using DinkToPdf;
using FastFoodSystem.WebApp.Models.ViewModel;

using FluentAssertions.Common;

var builder = WebApplication.CreateBuilder(args);
//var connectionString = builder.Configuration.GetConnectionString("AgentManagerDbContextConnection") ?? throw new InvalidOperationException("Connection string 'AgentManagerDbContextConnection' not found.");

//builder.Services.AddDbContext<AgentManagerDbContext>(options =>
//    options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<FastFoodSystemDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("AgentManagerDbContext")));
builder.Services.AddRazorPages();

builder.Services.AddScoped<IRepository<FFSVoucher>, VoucherRepository>();
builder.Services.AddScoped<IRepository<FFSProduct>, ProductRepository>();
builder.Services.AddScoped<IRepository<FFSProductCategory>, CategoryRepository>();



builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
#region Session and cookies settings

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
});

#endregion

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddIdentity<Staff, IdentityRole>()
                .AddEntityFrameworkStores<FastFoodSystemDbContext>()
                .AddDefaultTokenProviders();
builder.Services.AddScoped<DBHelper>();
builder.Services.AddTransient<ICartItemFactory, CartItemFactory>();

builder.Services.Configure<IdentityOptions>(options => {
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = false;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại

});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = $"/Identity/Pages/Account/AccessDenied.cshtml";
});

builder.Services.AddOptions();



var app = builder.Build();

//Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.MapRazorPages();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "pdfRoute",
        pattern: "pdf/export",
        defaults: new { controller = "YourController", action = "ExportToPdf" }
    );
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
    );
});
