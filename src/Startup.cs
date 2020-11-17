using System;
using System.Collections.Generic;
using System.Net.Http;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using VotRomania.Extensions;
using VotRomania.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VotRomania.Providers;
using VotRomania.Services;
using VotRomania.Services.Location;
using VotRomania.Services.Location.HereMaps;
using VotRomania.Stores;
using Newtonsoft.Json.Converters;

namespace VotRomania
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _environment = environment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            services.AddOptions();
            services.AddMemoryCache();

            services.AddHealthChecks();
            services.Configure<HereMapsOptions>(Configuration.GetSection("HereMaps"));

            var authenticationSection = Configuration.GetSection("Authentication");
            services.Configure<AuthSettingOptions>(authenticationSection);
            services.Configure<ApplicationUsersOptions>(Configuration.GetSection("UserSettings"));
            services.AddPostgreSqlDbContext(Configuration);

            services.AddScoped<IPollingStationsRepository, PollingStationsRepository>();
            services.AddScoped<IApplicationContentRepository, ApplicationContentRepository>();
            services.AddScoped<IImportJobsRepository, ImportJobsRepository>();
            services.AddScoped<IImportedPollingStationsRepository, ImportedPollingStationsRepository>();
            services.AddScoped<IAddressBankRepository, AddressBankRepository>();

            services.AddScoped<IAddressLocationSearchService, HereAddressLocationSearchService>();
            services.AddSingleton<IPollingStationSearchService, IneffectiveSearchService>();

            services.AddHostedService<AddressResolverService>();
            services.AddSingleton<IBackgroundJobsQueue, BackgroundJobsQueue>();

            services.AddScoped<IUserProvider, UserProvider>();
            services.AddScoped<IAuthenticationProvider, AuthenticationProvider>();
            services.AddScoped<IExcelParser, ExcelParser>();

            // Authentication
            var appSettings = authenticationSection.Get<AuthSettingOptions>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            
            services.AddCors(o => o.AddPolicy("Permissive", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            services.AddAuthorization(options =>
            {
                options.AddPolicy("content", policy => policy.RequireClaim(ClaimTypes.NameIdentifier)
                    .RequireClaim("scope", "content-edit"));
                options.AddPolicy("polling-stations", policy => policy.RequireClaim(ClaimTypes.NameIdentifier)
                    .RequireClaim("scope", "polling-stations"));
            });

            services.AddControllersWithViews().
                AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });

            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddCors(options => options.AddPolicy("Permissive", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.DocumentFilter<OrderDefinitionsAlphabeticallyDocumentFilter>();
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "[Code4Ro] VotRomania API",
                    Contact = new OpenApiContact
                    {
                        Name = "Code4Romania Vot Romania",
                        Url = new Uri("https://github.com/code4romania/vot-romania")
                    }
                });

                c.SwaggerGeneratorOptions.DescribeAllParametersInCamelCase = true;

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
            });
            services.AddProblemDetails(ConfigureProblemDetails);
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        private void ConfigureProblemDetails(ProblemDetailsOptions options)
        {
            // This is the default behavior; only include exception details in a development environment.
            options.IncludeExceptionDetails = (ctx, ex) => _environment.IsDevelopment();

            // This will map NotImplementedException to the 501 Not Implemented status code.
            options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);

            // This will map HttpRequestException to the 503 Service Unavailable status code.
            options.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);

            // Because exceptions are handled polymorphically, this will act as a "catch all" mapping, which is why it's added last.
            // If an exception other than NotImplementedException and HttpRequestException is thrown, this will handle it.
            options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, VotRomaniaContext dbContext)
        {
            dbContext.Database.Migrate();
            app.UseCors("Permissive");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {

                options.DocumentTitle = "[Code4Ro] VotRomania API";
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "[Code4Ro] VotRomania API v1");
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("Permissive");

            app.UseProblemDetails();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });


            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
