using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using RadiosMotorola.Models;

namespace RadiosMotorola
{
    public class Startup
    {
        //Using dependency injection I create an interface (a spec for a specific object) 
        public IConfiguration Configuration { get; }

        //constructor that will configure the startUp
        public Startup(IConfiguration configuration)
        {
            //allows to use the configuration interface at a later point
            Configuration = configuration;
        }

        //Registers the services I will use
        public void ConfigureServices(IServiceCollection services)
        {
            //here we will add the services that we will add



            //1. Using the configuration in order to create a DB connection
            //setting up a DBContext class using dependency injection
            services.AddDbContext<RadioContext>
                (opt => opt.UseSqlServer(Configuration["Data:RadioApiConnection:ConnectionString"])); //Data:RadioAPIConnection:ConnectionString is the place where I look in the appsetting.json file in orde to get the name and address of the SQL server

            //2. Firstly I add the mvc service to be compatible with the MVC version 2_2 (the version on this maschine)
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //here we create a request pipeline

            //we make use of the application builder app, and we tell the program that we will use MVC, for better structuring
            app.UseMvc();
        }
    }
}
