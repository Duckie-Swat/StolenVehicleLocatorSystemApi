using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StolenVehicleLocatorSystem.Business;
using StolenVehicleLocatorSystem.DataAccessor;
using System.Text;

namespace StolenVehicleLocatorSystem.Api
{
    public class Program
    {
        public static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string AllOrigins = "AllowAllOrigins";
            var configuration = builder.Configuration;
            // Add Business layer
            builder.Services.AddBusinessLayer(configuration);

            if (args.Contains("/seed-data"))
            {
                SeedData.EnsureSeedData(builder.Services);
            }


            builder.Services.AddControllers().AddJsonOptions(opt =>
            {
                var serializerOptions = opt.JsonSerializerOptions;
                serializerOptions.IgnoreNullValues = true;
                serializerOptions.IgnoreReadOnlyProperties = false;
                serializerOptions.WriteIndented = true;
            });



            // Adding Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            // Adding Jwt Bearer
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
                };
            });



            // Cors
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(AllOrigins,
                                  builder =>
                                  {
                                      builder.AllowAnyOrigin()
                                      .AllowAnyHeader()
                                      .AllowAnyMethod();
                                  });
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
             {

                 c.SwaggerDoc(configuration["Swagger:SwaggerDoc:Version"], new OpenApiInfo
                 {
                     Title = configuration["Swagger:SwaggerDoc:Title"],
                     Version = configuration["Swagger:SwaggerDoc:Version"]
                 });

                 c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                 {
                     Description = configuration["Swagger:SecurityDefinition:Description"],
                     Name = configuration["Swagger:SecurityDefinition:Name"],
                     In = ParameterLocation.Header,
                     Type = SecuritySchemeType.ApiKey,
                     Scheme = configuration["Swagger:SecurityDefinition:Scheme"]
                 });
                 c.AddSecurityRequirement(new OpenApiSecurityRequirement
                   {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                            },
                            new List<string>{ "openid profile identity api" }
                        }
                    });
             });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(AllOrigins);
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
            return Task.CompletedTask;
        }
    }
}