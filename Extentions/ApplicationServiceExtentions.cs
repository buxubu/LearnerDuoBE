using DatingApp.Services;
using LearnerDuo.Data;
using LearnerDuo.Helper;
using LearnerDuo.Services;
using LearnerDuo.SignalIR;
using LearnerDuo.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata.Ecma335;

namespace LearnerDuo.Extentions
{
    public static class ApplicationServiceExtentions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<LearnerDuoContext>(options => options.UseSqlServer(config.GetConnectionString("LearnerDuoDb"),
                                                                      options => options.MigrationsAssembly(typeof(LearnerDuoContext).Assembly.FullName)));

            services.AddCors(o => o.AddPolicy(name: "LearnerDuo",
                policy =>
                {
                    // alowCredentials is for SignalIR
                    policy.WithOrigins("http://localhost:4200").AllowCredentials().AllowAnyMethod().AllowAnyHeader();
                }));

            //signnalR
            services.AddSingleton<PresenceTracker>();
            services.AddScoped(typeof(IReponsitoryService<>), typeof(ReponsitoryService<>));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IAdminService, AdminService>();

            services.AddScoped<ILikesService, LikesService>();

            services.AddScoped<LogUserActivity>();

            return services;
        }

        public static void RegisterDI(this IServiceCollection services)
        {
        }



    }
}
