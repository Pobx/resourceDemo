using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using IdentityModel.AspNetCore.AccessTokenValidation;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace resourceDemo {
  public class Startup {
    public Startup (IConfiguration configuration) {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices (IServiceCollection services) {

      services.AddCustomSwagger (Configuration);
      services.AddControllers ();
      // services.AddSwaggerGen (c => {
      //   c.SwaggerDoc ("v1", new OpenApiInfo { Title = "resourceDemo", Version = "v1" });
      // });

      // var handler = new HttpClientHandler ();
      // handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

      // services.AddAuthentication (JwtBearerDefaults.AuthenticationScheme)
      //   .AddJwtBearer (JwtBearerDefaults.AuthenticationScheme, options => {
      //     Console.WriteLine ("=========================> Bearer");
      //     options.Authority = "https://localhost:5005";
      //     options.Audience = "client.kma.jwt";
      //     options.BackchannelHttpHandler = handler;
      //     options.TokenValidationParameters.ValidTypes = new [] { "at+jwt" };
      //     options.TokenValidationParameters.ValidateAudience = false;
      //     // if token does not contain a dot, it is a reference token
      //     options.ForwardDefaultSelector = Selector.ForwardReferenceToken ("introspection");
      //   })
      //   .AddOAuth2Introspection ("introspection", options => {
      //     Console.WriteLine ("=========================> introspection");
      //     options.Authority = "https://localhost:5005";
      //     options.ClientId = "client.kma.token";
      //     options.ClientSecret = "1e4f9ffe-7949-4993-a92b-f74a9bf6b995";
      //   });

      // services.AddAuthorization (options => {
      //   options.AddPolicy ("ReadOnly", policy => policy.RequireScope (new string[] { "guest", "bio" }));
      //   options.AddPolicy ("FullOperation", policy => policy.RequireScope (new string[] { "pin" }));
      // });

      // services.AddHttpClient (OAuth2IntrospectionDefaults.BackChannelHttpClientName).ConfigurePrimaryHttpMessageHandler (() => handler);

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure (IApplicationBuilder app, IWebHostEnvironment env) {
      // if (env.IsDevelopment ()) {
      //   app.UseDeveloperExceptionPage ();
      // }

    	app.UseSwagger(c =>
			{
				c.RouteTemplate = "swagger/{documentName}/swagger.json";
			}).UseSwaggerUI(c =>
			{
				c.RoutePrefix = "swagger";
				c.SwaggerEndpoint($"v1/swagger.json", "Resource API v1");
			});

      app.UseHttpsRedirection ();

      app.UseRouting ();

      // app.UseAuthentication ();
      // app.UseAuthorization ();

      app.UseEndpoints (endpoints => {
        endpoints.MapControllers ();
      });
    }

  }

  static class CustomExtensionsMethods {
    public static IServiceCollection AddCustomSwagger (this IServiceCollection services, IConfiguration configuration) {
      services.AddSwaggerGen (options => {
        options.CustomSchemaIds (type => $"{type.Name}_{Guid.NewGuid()}");
        options.SwaggerDoc ("v1", new OpenApiInfo {
          Title = "Resource API",
            Version = "v1",
            Description = "The Resource Service HTTP API"
        });
        options.DescribeAllParametersInCamelCase ();
        // options.ExampleFilters ();
        options.EnableAnnotations ();

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine (AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments (xmlPath);
      });

      return services;
    }
  }
}