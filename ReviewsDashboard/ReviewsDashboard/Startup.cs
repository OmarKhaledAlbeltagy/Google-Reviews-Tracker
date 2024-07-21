using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using ReviewsDashboard.Privilage;
using ReviewsDashboard.Context;
using ReviewsDashboard.Repos;
using Microsoft.Web.Administration;
using Microsoft.Data.SqlClient;
using ReviewsDashboard.Entities;
namespace ReviewsDashboard
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(a => a.EnableEndpointRouting = false);
            services.AddControllers();

    
   
            services.AddIdentity<ExtendIdentityUser, IdentityRole>(op =>
            {
                op.Password.RequiredLength = 7;
                op.Password.RequireDigit = false;
                op.Password.RequireLowercase = false;
                op.Password.RequireNonAlphanumeric = false;
                op.Password.RequireUppercase = false;
                op.User.RequireUniqueEmail = true;
                op.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            }).AddDefaultTokenProviders().AddEntityFrameworkStores<DbContainer>();

       
            services.AddPooledDbContextFactory<DbContainer>(opt =>
            opt.UseSqlServer(Configuration.GetConnectionString("con")));

            services.AddDbContextPool<DbContainer>(op =>
            {
                op.UseSqlServer(Configuration.GetConnectionString("con"));
                
            });

            services.AddHttpContextAccessor();



            services.AddCors(options =>
            {
                options.AddPolicy("allow",
                                    a => a.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            });

            services.AddAuthentication();
            services.AddTransient<DbContainer>();
            services.AddScoped<IHttpRep, HttpRep>();
            services.AddScoped<ITimeRep, TimeRep>();
            services.AddTransient<IReviewTrackingRep, ReviewTrackingRep>();
            services.AddTransient<ICheckRep, CheckRep>();
            services.AddTransient<IBusinessRep, BusinessRep>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment en)
        {
         
            if (en.EnvironmentName == "Development")
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseHsts();
            app.UseMvc();
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseStaticFiles();
            
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("allow");
            app.UseEndpoints(a =>
            {
                a.MapDefaultControllerRoute();
            });

        }

    }
}
