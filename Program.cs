using DatingApp.Services;
using LearnerDuo.Data;
using LearnerDuo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LearnerDuo.Extentions;
using LearnerDuo.Middleware;
using Microsoft.AspNetCore.Identity;
using LearnerDuo.Models;
using Microsoft.Extensions.Logging;
using LearnerDuo.SignalIR;
using LearnerDuo.SignalR;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration);

//config token
builder.Services.AddIdentityServices(builder.Configuration);

//seedData
builder.Services.AddTransient<Seed>(); // when u want to use uncomment it

//registerDI
builder.Services.RegisterDI();

//automapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

//http get httpContext.User Claims
builder.Services.AddHttpContextAccessor();

//signalIR
builder.Services.AddSignalR();



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Config Middleware 
app.UseMiddleware<ExceptionMiddleware>();


app.UseHttpsRedirection();

app.UseCors("LearnerDuo");

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");

app.MapFallbackToController("Index", "Fallback");


// this functionn when use start console it auto run and seed data 

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var context = services.GetRequiredService<LearnerDuoContext>();
//    var logger = services.GetService<ILogger<Program>>();
//    var userManager = services.GetRequiredService<UserManager<User>>();
//    var roleManager = services.GetRequiredService<RoleManager<Role>>();

//    try
//    {

//        //await context.SaveChangesAsync();
//        logger.LogInformation("Starting migration...");
//        await context.Database.MigrateAsync(); // create database if not exist and update database if exist

//        await Seed.SeedUsers(userManager, roleManager);

//        logger.LogInformation("Users seeded successfully.");
//    }
//    catch (Exception ex)
//    {
//        logger.LogError(ex, "An error occured during migration. ");
//    }
//}

await app.RunAsync();
