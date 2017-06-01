using Library.Api.Models;
using Library.Data;
using Library.Data.Entities;
using Library.Data.Respositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Library.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using NLog.Extensions.Logging;
namespace Library.Api
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

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration["connectionStrings:LibraryDB"];
            services.AddDbContext<LibraryContext>(o => o.UseSqlite(connectionString));

            services.AddScoped<ILibraryRepository, LibraryRepository>();

            services.AddMvc(setupAction =>
           {
               setupAction.ReturnHttpNotAcceptable = true;

               setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());

               setupAction.InputFormatters.Add(new XmlDataContractSerializerInputFormatter());
           });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Library Api", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, LibraryContext libraryContext)
        {

            


            loggerFactory.AddConsole(Configuration.GetSection("Logging"));

            //TODO : 11 - Install-Package Microsoft.Extensions.Logging.Debug
            //Configuro mi logg en information
            loggerFactory.AddDebug(LogLevel.Information);

            //TODO : 16 - Install-Package NLog.Extensions.Logging -Pre
            //Configuro NLog
            loggerFactory.AddNLog();

            if (!env.IsDevelopment())
            {
                //TODO : 13 - Niego el modo Development para ejecutar mi manejador
                app.UseDeveloperExceptionPage();
            }
            else
            {

                //Cambio  "ASPNETCORE_ENVIRONMENT": "Development" por >> "ASPNETCORE_ENVIRONMENT": "Production"
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        //TODO : 12 - Logueo la exception
                        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (exceptionHandlerFeature != null)
                        {
                            var logger = loggerFactory.CreateLogger("Global exception logger");
                            logger.LogError(500,
                                exceptionHandlerFeature.Error,
                                exceptionHandlerFeature.Error.Message);
                        }


                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("Ocurrio un error. Intente nuevamente o ponganse en contacto con admin@library.com");

                    });
                });
            }


            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Author, AuthorDto>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                    $"{src.FirstName} {src.LastName}"))
                    .ForMember(dest => dest.Age, opt => opt.MapFrom(src =>

                    src.DateOfBirth.GetCurrentAge()));

                cfg.CreateMap<Book, BookDto>();


                cfg.CreateMap<Models.AuthorCreationDto, Author>();

                cfg.CreateMap<Models.BookCreationDto, Book>();


                cfg.CreateMap<BookUpdateDto, Book>();

                cfg.CreateMap<Book, BookUpdateDto>();


            });
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
