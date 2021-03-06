using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Movies.Client.ApiServices;
using Movies.Client.HttpHandlers;

namespace Movies.Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddScoped<IMovieApiService, MovieApiService>();

            services.AddTransient<AuthenticationDelegatingHandler>();

            services.AddHttpClient("MovieAPIClient", client =>
            {
                client.BaseAddress = new Uri(Configuration["API_EndPoint"]);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            }).AddHttpMessageHandler<AuthenticationDelegatingHandler>();


            services.AddHttpContextAccessor();



            // http operations

            // 1 create an HttpClient used for accessing the Movies.API



            //// 2 create an HttpClient used for accessing the IDP
            //services.AddHttpClient("IDPClient", client =>
            //{
            //    client.BaseAddress = new Uri("https://localhost:5005/");
            //    client.DefaultRequestHeaders.Clear();
            //    client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            //});


            //services.AddSingleton(new ClientCredentialsTokenRequest
            //{
            //    Address = "https://localhost:5005/connect/token",
            //    ClientId = "movieApiClient",
            //    ClientSecret = "secret",
            //    Scope = "movieAPI"
            //});

            // http operations


            services.AddAuthentication(options =>
            {
                //options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;

                options.DefaultScheme = "cookie";
                options.DefaultChallengeScheme = "oidc";

            })
                .AddCookie("cookie", options =>
                //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.AccessDeniedPath = "/Authorization/AccessDenied";
                })
                  .AddOpenIdConnect("oidc", options =>

                  //.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>

                  {
                      options.Authority = Configuration["IDP_EndPoint"];
                      options.ClientId = Configuration["ClientId"];
                      options.ClientSecret = Configuration["ClientSecret"];
                      options.ResponseType = "code";
                      options.ResponseMode = "query";

                      options.Scope.Add("openid");
                      options.Scope.Add("profile");
                      options.Scope.Add("address");
                      options.Scope.Add("email");
                      options.Scope.Add("movieAPI.read");
                      options.Scope.Add("offline_access");
                      options.Scope.Add("roles");
                      options.Scope.Add("subscriptionlevel");

                      //options.ClaimActions.DeleteClaim("address");
                      options.ClaimActions.DeleteClaim("sid");
                      options.ClaimActions.DeleteClaim("idp");
                      options.ClaimActions.DeleteClaim("s_hash");
                      options.ClaimActions.DeleteClaim("auth_time");


                      // ?????? ?????? ???????? ?????? ???????????? ???????? ?????????????? ???????? ???? ???? ?????????? ????????

                      options.ClaimActions.MapUniqueJsonKey(claimType: "role", jsonKey: "role");
                      options.ClaimActions.MapUniqueJsonKey(claimType: "subscriptionlevel", jsonKey: "subscriptionlevel");

                      // for having 2 or more roles
                      //options.ClaimActions.MapJsonKey(claimType: "role", jsonKey: "role"); 




                      options.UsePkce = true;
                      options.SaveTokens = true;
                      options.GetClaimsFromUserInfoEndpoint = true;


                  // ???? ?????????? ???????????? ?????????? ???????? ???? ?????? ???? ???? ???????? ???????? ????????????
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      NameClaimType = JwtClaimTypes.GivenName,
                      RoleClaimType = JwtClaimTypes.Role,



                      };
                  });


            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                   name: "Director",
                   configurePolicy: policyBuilder =>
                   {
                       policyBuilder.RequireAuthenticatedUser();
                       policyBuilder.RequireClaim(claimType: "subscriptionlevel", "a1", "b1");
                   });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}