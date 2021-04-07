using System;
using System.Collections.Generic;
using GraphQL.Server;
using HotChocolate.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ISchema = GraphQL.Types.ISchema;

namespace Graph
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // graphql-dotnet
            services
                .AddSingleton<CustomScalarGraphType>()
                .AddSingleton<ISchema, GraphqlDotnetSchema>()
                .AddSingleton<GraphqlDotnetQuery>()
                .AddGraphQL(options => options.EnableMetrics = false)
                .AddSystemTextJson()
                .AddUserContextBuilder(httpContext => new Dictionary<string, object>());

            // hot-chocolate
            services
                .AddGraphQL("hotChocolate")
                .AddQueryType<QueryType>()
                .AddType<CustomScalarType>();

            // stitching
            services.AddHttpClient("graphqlDotnet", (sp, client) =>
            {
                client.BaseAddress = new Uri($"{Configuration["URLS"]}/graphql-dotnet");
            });

            services.AddGraphQLServer()
                .AddRemoteSchema("graphqlDotnet")
                .AddLocalSchema("hotChocolate")
                .AddType<CustomScalarType>();

            services
                .AddGraphQLServer("graphqlDotnet")
                .AddType(new AnyType("CustomScalar"));
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseGraphQLPlayground();

            app.UseGraphQL<ISchema>("/graphql-dotnet");

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL(path: "/hot-chocolate", schemaName: "hotChocolate");
                endpoints.MapGraphQL(path: "/graphql");
            });
        }
    }
}
