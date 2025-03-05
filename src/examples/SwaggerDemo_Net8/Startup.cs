using System.Reflection;
using Swashbuckle.AspNetCore.Extensions;

namespace SwaggerDemo_Net8
{
    public class Startup
    {
        private static readonly string NamespaceName = Assembly.GetEntryAssembly()?.FullName;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerConfig(NamespaceName, Configuration, signKeyDict: SignDict);
            //services.AddSwaggerConfig(NamespaceName, Configuration);
        }


        Dictionary<string, string> SignDict = new Dictionary<string, string>
        {
            {"Authorization","格式 Bearer xx"},
            {"AppId","执行的AppId"},
            {"AppCode","执行的AppCode"},
            {"AppToken","当前请求的Token"},
            {"H1","H1"},
            {"Ts","当前时间戳"},
        };

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();
            app.UseSwaggerConfig(NamespaceName);

            app.UseAuthorization();

            //  app.UseMiddleware<SwaggerSignValidMiddlewareBase>(SignDict);
            app.UseMiddleware<SelfSwaggerSignValidMiddleware>(SignDict);
            //app.UseMiddleware<SelfSwaggerSignValidMiddleware>();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        }


    }
}
