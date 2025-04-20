using Web.Api.Extensions;

namespace Web.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddMappings(typeof(DependencyInjection).Assembly);
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}
