using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVCTutorial.Models;

namespace MVCTutorial
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddTransient<IIpReflection,IpReflection>();
            services.AddDirectoryBrowser();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IIpReflection ipInfo)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            
            //Add .apk file support
            var ExtProvider = new FileExtensionContentTypeProvider();
            ExtProvider.Mappings[".apk"] = "application/vnd.android.package-archive";

            string locationPath = env.ContentRootPath + @"/store";
            var fileLocation = new PhysicalFileProvider(locationPath);
            app.UseStaticFiles(new StaticFileOptions() { RequestPath =(PathString)"/ftp", FileProvider=fileLocation,ContentTypeProvider=ExtProvider});
            app.UseDirectoryBrowser(options:(new DirectoryBrowserOptions() { RequestPath="/ftp",FileProvider=fileLocation}));

            //Middleware Test
            app.Use(reqDelegate => context=> {
                return reqDelegate(context); });

            app.UseRouting();

            app.UseAuthentication();//For User identity
            app.UseAuthorization();//For resource identity

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/ip", async context =>
                {
                    await context.Response.WriteAsync(ipInfo.GetIp());
                });
                endpoints.MapControllerRoute(
                    name: "default", pattern: "{controller:alpha}/{action:alpha}/{id?}",
                    constraints:new {controller="[a-zA-Z0-9]*"},
                    defaults:new { controller="Home",action="Index"}
                    );
            });
        }
    }
}
