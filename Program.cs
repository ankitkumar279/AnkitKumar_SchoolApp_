using AnkitKumar_SchoolApp.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

//builder.Services.AddDbContext<SchoolDbContext>(options =>
//    options.UseMySql(
//        builder.Configuration.GetConnectionString("SchoolDbConnection"),
//        new MySqlServerVersion(new Version(5, 7, 24))
//    )
//);

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.Run();
