using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Web.Api.Endpoints;

namespace Web.Api.Extensions;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        ServiceDescriptor[] serviceDescriptors = assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    public static IApplicationBuilder MapEndpoints(
        this WebApplication app,
        RouteGroupBuilder routeGroupBuilder)
    {
        IEnumerable<IEndpoint> endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        foreach (IEndpoint endpoint in endpoints)
        {
            endpoint.MapEndpoint(routeGroupBuilder);
        }

        return app;
    }

    public static RouteHandlerBuilder HasPermission(this RouteHandlerBuilder app, string permission)
    {
        return app.RequireAuthorization(permission);
    }

    public static RouteGroupBuilder WithVersioning(this WebApplication app, string prefix = "api")
    {
        IVersionedEndpointRouteBuilder version = app.NewVersionedApi();
        RouteGroupBuilder group = version.MapGroup($$"""/{{prefix}}/v{version:apiVersion}""")
            .WithOpenApi()
            .HasApiVersion(new ApiVersion(1, 0));

        return group;
    }
}
