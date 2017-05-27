using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Library.API.Entities;
using Library.API.Services;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using Library.Data.Entities;
using Library.API.Models;
using LIbrary.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Library
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration["connectionStrings:LibraryDB"];
            services.AddDbContext<LibraryContext>(o => o.UseSqlServer(connectionString));

            // register the repository
            services.AddScoped<ILibraryRepository, LibraryRepository>();
            // Add framework services.

            services.AddMvc(
            //TODO : 19 - Envio un request con application/xml, Configuro el error  HTTP Error 406 Not acceptable 

            //setupAction =>
            //{
            //    //setupAction.ReturnHttpNotAcceptable = true;
            //    //////Install-Package Microsoft.AspNetCore.Mvc.Formatters.Xml
            // ////setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
            //}
            );
            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Library Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, LibraryContext libraryContext)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //TODO : 13 - Manejo la exception a nivel global
                //Cambio  "ASPNETCORE_ENVIRONMENT": "Development" por >>  "ASPNETCORE_ENVIRONMENT": "Production"
                //app.UseExceptionHandler(appBuilder =>
                //{
                //    appBuilder.Run(async context =>
                //    {
                //        context.Response.StatusCode = 500;
                //        await context.Response.WriteAsync("Ocurrio un error. Intente nuevamente o ponganse en contacto con admin@library.com");

                //    });
                //});
            }


            //TODO : 08 - Instalo automapper y realizo las configuraciones de mapeo. Utilizamos proyección para el mapeo de la edad
            //Install-Package AutoMapper
            //AutoMapper.Mapper.Initialize(cfg =>
            //{
            //    cfg.CreateMap<Author, AuthorDto>()
            //        .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
            //        $"{src.FirstName} {src.LastName}"))
            //        .ForMember(dest => dest.Age, opt => opt.MapFrom(src =>
            //        src.DateOfBirth.GetCurrentAge()));

            //    //TODO : 17 - Configuro el automapper
            //    //cfg.CreateMap<book, BookDto>();
            //});
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            libraryContext.EnsureSeedDataForContext();

            app.UseMvc();
        }
    }
}
