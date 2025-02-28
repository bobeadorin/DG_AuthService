
using AuthService.DbConnection;
using AuthService.Services.Interfaces;
using AuthService.Services;
using Microsoft.EntityFrameworkCore;
using System;

namespace AuthService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Add logging 
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            
            //Registerd Services in DI
            builder.Services.AddScoped<IUserService, UserService>();


            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection"));
            });



            var app = builder.Build();

            var logger = app.Logger;
            logger.LogInformation("Application starting up.");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            logger.LogInformation("Application starting up.");
            app.Run();
        }
    }
}
