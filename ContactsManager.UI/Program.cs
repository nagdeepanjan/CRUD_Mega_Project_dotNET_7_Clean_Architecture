using ContactsManager.Core.Domain.IdentityEntities;
using CRUD_Last.Middleware;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using Services;
using RepositoryContracts;
using Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();

builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonsGetterService, PersonsGetterService>();
builder.Services.AddScoped<IPersonsAdderService, PersonsAdderService>();
builder.Services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();
builder.Services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
builder.Services.AddScoped<IPersonsSorterService, PersonsSorterService>();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
});

//DB 
builder.Services.AddDbContext<ApplicationDbContext>((options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))));

//Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>((options) =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = true;
        options.Password.RequiredUniqueChars = 2;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
    .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();


var app = builder.Build();

if (builder.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
//else
{
    //app.UseExceptionHandler("/Error");              //Built-in middleware to show error-page
    //app.UseExceptionHandlingMiddleware();
}


app.Logger.LogError("Deepz log!");

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();                //Reading identity cookie from browser
app.UseAuthorization();

app.MapControllers();

app.Run();
