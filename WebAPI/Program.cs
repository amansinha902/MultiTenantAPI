
using Application;
using Infrastructure;
using Microsoft.AspNetCore.Components.Web;

namespace WebAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.AddJwtAuthentication(builder.Services.GetJwtSettings(builder.Configuration));

            builder.Services.AddApplicationServices();
            var app = builder.Build();
            await app.Services.AddDatabaseInitializer();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();

            //app.UseAuthorization();
            app.UseInfrastructure();
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.MapControllers();

            app.Run();
        }
    }
}
