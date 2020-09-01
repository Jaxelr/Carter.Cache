using Carter.OpenApi;

namespace Sample.Carter.Cache.Application.Modules
{
    public class GetHello : RouteMetaData
    {
        private const string TagInfo = "Hello";
        private const string DescriptionInfo = "Returns a message with hello world";

        public override RouteMetaDataResponse[] Responses { get; } =
        {
            new RouteMetaDataResponse
            {
                Code = 200,
                Description = "A message with hello world",
                Response = typeof(string)
            }
        };

        public override string Description => DescriptionInfo;

        public override string Tag => TagInfo;

        public override string OperationId => nameof(GetHello);
    }
}
