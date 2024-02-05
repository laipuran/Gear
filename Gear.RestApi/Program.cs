using Microsoft.OpenApi.Models;

namespace Gear.RestApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebApp(args).Run();
        }

        public static WebApplication CreateWebApp(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //builder.Environment.ApplicationName = "Gear.RestApi";
            builder.Services.AddControllers()
                .AddApplicationPart(System.Reflection.Assembly.GetExecutingAssembly());

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Gear.RestApi", Version = "v1" });
            });

            //builder.Services.AddSingleton<Base.Interface.INotifyQueueService, Base.Interface.INotifyQueueService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            return app;
        }
    }
}
