using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using DatabaseREST.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace DatabaseREST
{
    public class Startup
    {

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configure)
        {
            Configuration = configure;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //Sæt den dbContext der benyttes af DatabaseAPI'en + hent dens connectionstring fra appsettings.json
            services.AddDbContext<intrusiveContext>(option => option.UseMySql(Configuration["Data:DatabaseAPIConnection:ConnectionString"]));

            services.AddDbContext<intrusiveContextReadOnly>(option => option.UseMySql(Configuration["Data:DatabaseAPIConnection:ConnectionStringReadOnly"]));


            services.AddMvc(option => option.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            //services.AddMvc().AddNewtonsoftJson();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMvc();
            Console.WriteLine("INCLUDING PLAYERSZZZ");


            //string t = "{\"RefreshToken\":\"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJBYmJvdHQ0NDM5U2NvdCIsImF1ZCI6InJlZnJlc2giLCJuYmYiOjE1ODg2NjUyNjUsImV4cCI6MTU4OTg3NDg2NSwiaWF0IjoxNTg4NjY1MjY1fQ.wu-U8nCNn1x39JWxMzlTVYmr2JmYJ7JygNCrf137klI\",\"AccessToken\":\"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJBYmJvdHQ0NDM5U2NvdCIsImF1ZCI6ImFjY2VzcyIsIm5iZiI6MTU4ODY2NTI2NSwiZXhwIjoxNTg4NjY4ODY1LCJpYXQiOjE1ODg2NjUyNjV9.Fvh1pESNil6ihQwRrKfUkt3O9UfX2xbTgd8Nx3wE6vQ\"}";

            //TokenModel tsd = JsonConvert.DeserializeObject<TokenModel>(t);

            //var kfdkf = Token.GenerateToken(null, 1, 0, 0);


            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            //app.UseRouting();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync("Hello World!");
            //    });
            //});
        }
    }
}
