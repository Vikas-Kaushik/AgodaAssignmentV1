using System;
using Hotels.API.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using RateLimitServices;

namespace Hotels.API
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // ToDo: Get configuration parameters/numbers from appsetting.json
            // Confgure the parameters
            uint maxNumberOfRequests = Convert.ToUInt32(Configuration["RateLimitService:GetHotelsByCity:maxNumberOfRequests"]);
            ushort slotSpan = Convert.ToUInt16(Configuration["RateLimitService:GetHotelsByCity:slotSpan"]);
            ushort timeToBlock = Convert.ToUInt16(Configuration["RateLimitService:GetHotelsByCity:timeToBlock"]);
            
            services.AddSingleton<IRateLimitServiceForGetProjectByCity>(s => 
                new RateLimitServiceForGetProjectByCity(maxNumberOfRequests, slotSpan, timeToBlock));

            maxNumberOfRequests = Convert.ToUInt32(Configuration["RateLimitService:GetHotelsByRoom:maxNumberOfRequests"]);
            slotSpan = Convert.ToUInt16(Configuration["RateLimitService:GetHotelsByRoom:slotSpan"]);
            timeToBlock = Convert.ToUInt16(Configuration["RateLimitService:GetHotelsByRoom:timeToBlock"]);

            services.AddSingleton<IRateLimitServiceForGetProjectByRoom>(s =>
                new RateLimitServiceForGetProjectByRoom(maxNumberOfRequests, slotSpan, timeToBlock));

            services.AddSingleton<IHotelRepository, HotelRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
            loggerFactory.AddNLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
