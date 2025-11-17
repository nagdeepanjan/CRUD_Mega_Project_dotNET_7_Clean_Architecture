using CRUD_Last.Middleware;
using Entities;
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
builder.Services.AddDbContext<ApplicationDbContext>((options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))));

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
app.MapControllers();

app.Run();
