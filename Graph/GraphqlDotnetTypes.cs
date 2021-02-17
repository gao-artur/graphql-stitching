using System;
using GraphQL.Language.AST;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Graph
{
    public class GraphqlDotnetSchema : Schema
    {
        public GraphqlDotnetSchema(IServiceProvider provider) : base(provider)
        {
            Query = provider.GetRequiredService<GraphqlDotnetQuery>();
        }
    }

    public class GraphqlDotnetQuery : ObjectGraphType
    {
        public GraphqlDotnetQuery()
        {
            Name = "graphqlDotnet";
            Field<StringGraphType>().Name("graphql").Resolve(context => "dotnet");

            // #### adding this field breaks the stitching
            Field<CustomScalarGraphType>().Name("getCustomScalar").Resolve(context => "dotnet custom scalar");
        }
    }

    public class CustomScalarGraphType : ScalarGraphType
    {
        public CustomScalarGraphType()
        {
            Name = "CustomScalar";
        }

        public override object Serialize(object value)
        {
            return value;
        }

        public override object ParseValue(object value)
        {
            return value;
        }

        public override object ParseLiteral(IValue value)
        {
            return value?.Value;
        }
    }
}