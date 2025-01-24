using DatingApp.Services;
using LearnerDuo.Data;
using LearnerDuo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LearnerDuo.Extentions;
using LearnerDuo.Middleware;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration);
//config token
builder.Services.AddIdentityServices(builder.Configuration);
//seedData
builder.Services.AddTransient<Seed>();
//registerDI
builder.Services.RegisterDI();
//automapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);
//http get httpContext.User Claims
builder.Services.AddHttpContextAccessor();



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

//if (args.Length == 1 && args[0].ToLower() == "seedData")
//    SeedData(app);

//static async void SeedData(IHost app)
//{
//    var scoopedFactory = app.Services.GetService<IServiceScopeFactory>();

//    using (var scope = scoopedFactory.CreateScope())
//    {
//        var service = scope.ServiceProvider;
//        try
//        {
//            var context = service.GetRequiredService<LearnerDuoContext>();
//            await context.SaveChangesAsync();
//            await Seed.SeedUsers(context);
//        }
//        catch (Exception ex)
//        {
//            var logger = service.GetRequiredService<ILogger<Program>>();
//            logger.LogError(ex, "An error occured during migration. ");
//        }
//    }
//}


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

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<LearnerDuoContext>();
    var logger = services.GetService<ILogger<Program>>();

    try
    {
        await context.Database.MigrateAsync();
        //await context.SaveChangesAsync();
        await Seed.SeedUsers(context);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occured during migration. ");
    }
}

await app.RunAsync();
