using HotChocolate.Language;
using HotChocolate.Types;

namespace Graph
{
    public class HotChocolateQuery
    {
        public string Hot() => "chocolate";
    }

    public class QueryType : ObjectType<HotChocolateQuery>
    {
        protected override void Configure(IObjectTypeDescriptor<HotChocolateQuery> descriptor)
        {
            descriptor.Field("getCustomScalar").Type<CustomScalarType>().Resolve("hot chocolate custom scalar");
        }
    }

    public class CustomScalarType : ScalarType<string, StringValueNode>
    {
        public CustomScalarType() : base("customScalar")
        {
        }

        public override IValueNode ParseResult(object resultValue)
        {
            return ParseValue(resultValue);
        }

        protected override string ParseLiteral(StringValueNode valueSyntax)
        {
            return valueSyntax.ToString();
        }

        protected override StringValueNode ParseValue(string runtimeValue)
        {
            return new StringValueNode(runtimeValue);
        }
    }
}