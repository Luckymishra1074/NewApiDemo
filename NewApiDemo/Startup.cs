using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NewApiDemo.DbContext;
using NewApiDemo.Helper;
using NewApiDemo.Identity;
using NewApiDemo.Interfaces;
using NewApiDemo.Services;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewApiDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration,IWebHostEnvironment environment)
        {
            var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
            builder.SetBasePath(environment.ContentRootPath)
                   //add configuration.json  
                   .AddJsonFile("Ocelot.json", optional: false, reloadOnChange: true)
                   .AddEnvironmentVariables();

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DemoApiConnection")));

            // services.Configure<AppSetting>(Configuration.GetSection("AppSettings"));

            services.AddControllersWithViews();


            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICustomerService, CustomerService>();

            // For Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Adding Authentication
            services.AddAuthentication(options =>
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
         ValidAudience = Configuration.GetSection("Audience").Value,
         ValidIssuer = Configuration.GetSection("Issuer").Value,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("Key").Value))
     };
     options.Events = new JwtBearerEvents
     {
         OnAuthenticationFailed = context => {
             if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
             {
                 context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
             }
             return Task.CompletedTask;
         }
     };
 });
           

            //        services.AddSwaggerGen(setup =>
            //        {
            //            // Include 'SecurityScheme' to use JWT Authentication
            //            var jwtSecurityScheme = new OpenApiSecurityScheme
            //            {
            //                BearerFormat = "JWT",
            //                Name = "JWT Authentication",
            //                In = ParameterLocation.Header,
            //                Type = SecuritySchemeType.Http,
            //                Scheme = JwtBearerDefaults.AuthenticationScheme,
            //                Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

            //                Reference = new OpenApiReference
            //                {
            //                    Id = JwtBearerDefaults.AuthenticationScheme,
            //                    Type = ReferenceType.SecurityScheme
            //                }
            //            };

            //            setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

            //            setup.AddSecurityRequirement(new OpenApiSecurityRequirement
            //{
            //    { jwtSecurityScheme, Array.Empty<string>() }
            //});

            //        });


            services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ASP.NET 5 Web API",
                    Description = "Authentication and Authorization in ASP.NET 5 with JWT and Swagger"
                });
                // To Enable authorization using Swagger (JWT)
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter ‘Bearer’ [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                    {
                    new OpenApiSecurityScheme
                    {
                    Reference = new OpenApiReference
                    {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                    }
                    },
                    new string[] {}
                    
                    }
                    });
            });

              services.AddHealthChecks();

            // Here is the GUI setup and history storage for health checks
            services.AddHealthChecksUI(options =>
            {
                options.SetEvaluationTimeInSeconds(5); //Sets the time interval in which HealthCheck will be triggered
                options.MaximumHistoryEntriesPerEndpoint(10); //Sets the maximum number of records displayed in history
                options.AddHealthCheckEndpoint("Health Checks API", "/Admin"); //Sets the Health Check endpoint
            }).AddInMemoryStorage(); //Here is the memory bank configuration


           // services.AddOcelot(Configuration);


    }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
          //  app.UseOcelot();
            app.UseAuthentication();
            app.UseAuthorization();

           // app.UseMiddleware<JWTmidddlewaree>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllers();

                endpoints.MapHealthChecks("/Admin");
            });

            //Sets Health Check dashboard options
            app.UseHealthChecks("/Admin", new HealthCheckOptions
            {
                Predicate = p => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            //Sets the Health Check dashboard configuration
            app.UseHealthChecksUI(options => { options.UIPath = "/dashboard"; });

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Authorization", "Satinder singh");
             
                await next();
            });

        }
    }
}
