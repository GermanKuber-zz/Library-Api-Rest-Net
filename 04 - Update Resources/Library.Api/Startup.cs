﻿using Library.Api.Models;
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
            loggerFactory.AddDebug();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {

                //Cambio  "ASPNETCORE_ENVIRONMENT": "Development" por >> "ASPNETCORE_ENVIRONMENT": "Production"
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
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

                //TODO : 04 - Agrego la configuracion de los nuevos DTO


                //cfg.CreateMap<BookUpdateDto, Book>();

                //cfg.CreateMap<Book, BookUpdateDto>();


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
