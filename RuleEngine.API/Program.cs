using RuleEngine.Core.Interfaces;
using RuleEngine.Core.Models;
using RuleEngine.Core.Services;
using RuleEngine.Definitions.Parser;

namespace RuleEngine.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            builder.Services.Configure<RuleEngineSettings>(builder.Configuration.GetSection("RuleEngineSettings"));
            builder.Services.AddControllers();
            builder.Services.AddScoped<IRuleParser, XmlRuleParser>();
            builder.Services.AddScoped<XmlRuleEngine>();           
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

           
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
