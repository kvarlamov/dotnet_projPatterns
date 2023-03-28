using System.Net.Mime;
using ConfigExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Prometheus;

namespace BaseApi.Module;

public static class BaseApiModule
{
    public static void AddBaseApiModule(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);
        services.AddMemoryCache();
        // services.AddDistributedMemoryCache();
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = config.GetRedisConnectionString();
        });
        services.AddSwaggerGen(config.GetSwaggerGenOptions);
        //services.AddSwaggerGen();
        services.AddHealthChecks();
        //todo add Automapper
    }

    public static void UseBaseApiModule(
        this IApplicationBuilder app,
        IConfiguration config,
        IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            //https://learn.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?source=recommendations&view=aspnetcore-7.0
            app.UseExceptionHandler(exceptionHandlerApp =>
            {
                exceptionHandlerApp.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                    // using static System.Net.Mime.MediaTypeNames;
                    context.Response.ContentType = MediaTypeNames.Text.Plain;
                    
                    await context.Response.WriteAsync("An exception was thrown.");

                    // var exceptionHandlerPathFeature =
                    //     context.Features.Get<IExceptionHandlerPathFeature>();
                    //
                    // var error = exceptionHandlerPathFeature?.Error;
                    //
                    // await context.Response.WriteAsJsonAsync(error?.Message);

                    // if (exceptionHandlerPathFeature?.Error is FileNotFoundException)
                    // {
                    //     await context.Response.WriteAsync(" The file was not found.");
                    // }
                    //
                    // if (exceptionHandlerPathFeature?.Path == "/")
                    // {
                    //     await context.Response.WriteAsync(" Page: Home.");
                    // }
                });
            });
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        //app.UseCookiePolicy();
        
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint(config.GetSwaggerEndpointUrl(), config.GetSwaggerEndpointName());
            //options.EnablePersistAuthorization();
            //options.DocExpansion(DocExpansion.None);
        });

        app.UseHealthChecks(
            $"/health",
            new HealthCheckOptions {
                ResponseWriter = WriteJsonResponse,
                ResultStatusCodes = {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status429TooManyRequests,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                }
            });

        app.UseRouting();
        // app.UseRequestLocalization();
        // app.UseCors();
        app.UseHttpMetrics();
        app.UseAuthentication();
        app.UseAuthorization();
        // app.UseSession();
        // app.UseResponseCompression();
        // app.UseResponseCaching();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
            endpoints.MapMetrics();
        });
    }

    private static Task WriteJsonResponse(HttpContext httpContext, HealthReport result)
    {
        httpContext.Response.ContentType = "application/json";

        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.Formatting = Formatting.Indented;
        settings.Converters.Add(new StringEnumConverter());

        string json = JsonConvert.SerializeObject(result, settings);

        return httpContext.Response.WriteAsync(json);
    }
}