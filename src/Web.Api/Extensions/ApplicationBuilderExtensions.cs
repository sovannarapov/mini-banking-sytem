using Web.Api.Constants;

namespace Web.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSwaggerWithUi(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint(SwaggerConstants.EndpointUrl, SwaggerConstants.EndpointName);
            options.RoutePrefix = SwaggerConstants.RoutePrefix;
            options.DocumentTitle = "Mini Banking API Documentation";
        });

        return app;
    }
}
