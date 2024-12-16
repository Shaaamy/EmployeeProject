using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Demo.DAL.Contexts;
using Demo.DAL.Models;
using Demo.PL.MappingProfiles;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext < MvcAppG01DbContext>(Options =>
{
    Options.UseSqlServer(connectionString);
});//allow dependancy injection
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddAutoMapper(M => M.AddProfiles(new List<Profile>
{
    new EmployeeProfile(),
    new UserProfile(),
    new RoleProfile()
}));

//builder.Services.AddAutoMapper(M=>M.AddProfile(new UserProfile()));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(Options =>
{
    Options.Password.RequireNonAlphanumeric = true;
    Options.Password.RequireDigit = true;
    Options.Password.RequireLowercase = true;
    Options.Password.RequireUppercase = true;

})
                .AddEntityFrameworkStores<MvcAppG01DbContext>()
                .AddDefaultTokenProviders();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(Options =>
{
    Options.LoginPath = "Account/Login";
    Options.AccessDeniedPath = "Home/Error";
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
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
