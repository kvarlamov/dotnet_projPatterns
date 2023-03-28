using System.Dynamic;
using BaseTypes.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ConfigExtensions;

public static class ConfigurationExtensions
{
    public static void GetSwaggerGenOptions(this IConfiguration configuration, SwaggerGenOptions options)
    {
        OpenApiInfo swaggerDocInfoObject = new OpenApiInfo();
        configuration.GetSection(ConfigurationExtensionsConstants.SwaggerDocInfo).Bind(swaggerDocInfoObject);
        options.SwaggerDoc(configuration.GetValue<string>(ConfigurationExtensionsConstants.SwaggerDocName), swaggerDocInfoObject);
    }

    public static string GetSwaggerEndpointUrl(this IConfiguration configuration) => 
        configuration.GetValue<string>(ConfigurationExtensionsConstants.SwaggerEndpointUrl);

    public static string GetSwaggerEndpointName(this IConfiguration configuration) => 
        configuration.GetValue<string>(ConfigurationExtensionsConstants.SwaggerEndpointName);

    public static string GetRedisConnectionString(this IConfiguration configuration) =>
        configuration.GetValue<string>(ConfigurationExtensionsConstants.RedisConnectionString);
}