using VidhyalayaAPI.Models;

namespace VidhyalayaAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
             builder.Services.AddCors(options => options.AddDefaultPolicy(
                builder => builder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowAnyOrigin()));            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var services = builder.Services;
            var config = builder.Configuration;

            services.Configure<ConnectionConfigDTO>(builder.Configuration.GetSection("ConnectionStrings"));

            var app = builder.Build();

            





            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())

            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}