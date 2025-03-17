
using Infrastructure;
using System.Globalization;

namespace WebAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.GetJwtSettings(builder.Configuration);
            builder.Services.AddJwtAuthentication(builder.Services.GetJwtSettings(builder.Configuration));
            var app = builder.Build();
            await app.Services.AddDatabaseInitializer();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseInfrastructure();

            app.MapControllers();

            app.Run();
        }
    }
}
