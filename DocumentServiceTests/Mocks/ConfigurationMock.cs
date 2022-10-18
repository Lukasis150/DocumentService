using Microsoft.Extensions.Configuration;
using System.Text;

namespace DocumentServiceTests.Mocks
{
    internal class ConfigurationMock
    {
        public IConfiguration GetConfig()
        {

            var appSettings = @"{
            ""FileStorage"":{
                ""HDD"" : {
                    ""BasePath"": ""files""
            }}}";

            var builder = new ConfigurationBuilder();

            object value = builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(appSettings)));

            var configuration = builder.Build();

            return configuration;
        }

        
    }
}
