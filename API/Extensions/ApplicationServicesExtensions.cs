using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config){
       
       services.AddCors(builder=>{
            builder.AddPolicy("CorsPolicy",option=>{
                option.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
                
            });
        });

        services.AddDbContext<DataContext>(option=>{
            option.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });
       
        return services;
    }
}
