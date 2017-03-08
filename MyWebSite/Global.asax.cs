using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IKriv.Sample.CallWinService.WebSite
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var formatters = GlobalConfiguration.Configuration.Formatters;

            // By default, Web API does not handle [Serializable] classes properly
            // See http://stackoverflow.com/questions/29962044/using-serializable-attribute-on-model-in-webapi

            formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    IgnoreSerializableAttribute = true,
                }
            };

            formatters.XmlFormatter.UseXmlSerializer = true;
        }
    }
}
