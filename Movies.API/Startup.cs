using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Movies.API.Data;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Movies.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
                //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Movies.API", Version = "v1" });
            });

            services.AddDbContext<MoviesContext>(options =>
            options.UseInMemoryDatabase("Movies"));
            services.AddHttpContextAccessor();
            //services.AddAuthentication(defaultScheme: IdentityServerAuthenticationDefaults.AuthenticationScheme)
            //   .AddIdentityServerAuthentication(options =>
            //   {
            //       options.Authority = Configuration["IDP_EndPoint"];
            //       options.ApiName = "movieApiClient";
            //       //options.ApiSecret = "secret";
            //   });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                     options.Authority = Configuration["IDP_EndPoint"];
                     options.Audience = "movieAPI";
                     options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
                 });

            services.AddAuthorization(authorizationOptions =>
            {
                //ClaimType=client_id
                //ClaimValue=[movieApiClient,movies_mvc_client]
                //authorizationOptions.AddPolicy("ClientIdPolicy", policy => policy.RequireClaim("client_id", "movieApiClient", "movies_mvc_client"));

                authorizationOptions.AddPolicy(
                    name: "SubscriptionLevelPolicy",
                    configurePolicy: policyBuilder =>
                    {
                        policyBuilder.RequireAuthenticatedUser();
                        policyBuilder.AddRequirements(new MyOwnAuthorizationRequirement());
                    });

            });
            services.AddScoped<IAuthorizationHandler, MyOwnAuthorizationHandler>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Movies.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
