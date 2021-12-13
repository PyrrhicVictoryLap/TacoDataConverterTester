using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Taco.DataConverter.WebApi.Options;

namespace Taco.DataConverter.WebApi.Extensions
{
    /// <summary>
    /// Extension methods for service collection.
    /// </summary>
    public static class ServiceCollectionExtension
    {


        /// <summary>
        /// Sets up swagger for Taco Orders API.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="environment">The hosting environment.</param>
        /// <returns>Service collection.</returns>
        public static IServiceCollection AddSwaggerForTacoDataCoverter(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            //Get identity server information.
            var authOptions = configuration.GetSection("ApiAuthenticationOptions").Get<ServiceAuthenticationBearerOptions>();
            var client = new System.Net.Http.HttpClient();
            //var discoveryResult = client.GetDiscoveryDocumentAsync(authOptions.Authority).Result;

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Taco.DataConverter Api", Version = "v1" });

                //TODO: After the identity server authorization is setup, uncomment the following code and update the list of available scopes.
                //Don't blow up the application if a failure occurs with the identity server - simply exclude it.
                //if (!discoveryResult.IsError)
                //{
                //    var identityScheme = new OpenApiSecurityScheme()
                //    {
                //        Name = "Bearer",
                //        OpenIdConnectUrl = new Uri(new Uri(authOptions.Authority), ".well-known/openid-configuration"),
                //        Scheme = "oauth2",
                //        Type = SecuritySchemeType.OAuth2,
                //        In = ParameterLocation.Header,
                //        Flows = new OpenApiOAuthFlows()
                //        {
                //            ClientCredentials = new OpenApiOAuthFlow()
                //            {
                //                TokenUrl = new Uri(discoveryResult.TokenEndpoint),
                //                Scopes = new Dictionary<string, string>()
                //            {
                //                { Constants.CustomIdentityScopes.TransformationStationApi_FullAccess, "" }
                //            }
                //            }
                //        },
                //        Reference = new OpenApiReference()
                //        {
                //            Type = ReferenceType.SecurityScheme,
                //            Id = "oauth2"
                //        }
                //    };

                //    c.AddSecurityDefinition("oauth2", identityScheme);

                //    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                //{
                //    { identityScheme, new string[] { } }
                //});
                //}

                var xmlFiles = new DirectoryInfo(Path.Combine(environment.ContentRootPath)).GetFiles("*.xml", SearchOption.AllDirectories);

                foreach (var file in xmlFiles)
                {
                    c.IncludeXmlComments(file.FullName);
                }
            });

            return services;
        }
    }
}
