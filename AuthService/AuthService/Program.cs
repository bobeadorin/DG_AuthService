
using AuthService.DbConnection;
using AuthService.Services.Interfaces;
using AuthService.Services;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.IdentityModel.Tokens;

using AuthService.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using AuthService.Helpers.HelpersInterfaces;

namespace AuthService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("LocalDev", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            //Add token auth
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                    };
                });



            //Add logging 
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            //Registerd Services in DI
            builder.Services.AddScoped<HttpClient>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
            builder.Services.AddScoped<IAccountValidationService, AccountValidationService>();
            builder.Services.AddScoped<IRequestsService,RequestsService>();


            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection"));
            });


            //get current IPv4
            var myIp = LocalInfo.GetLocalIP();
            var url = $"http://{myIp}:5204";

            builder.WebHost.UseUrls(url);


            var app = builder.Build();

            var logger = app.Logger;

            logger.LogInformation($"Application will run on: {url}");
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseCors("LocalDev");

            app.UseAuthorization();

            app.MapControllers();

            logger.LogInformation("Application starting up.");
            app.Run();
        }
    }
}
