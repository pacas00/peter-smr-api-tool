using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using peter_ficsit_api;
using peter_ficsit_api.GraphQL;

namespace peter_ficsit_api_implement
{
    internal class ClientBuilder
    {
        public static Ificsit_api GetClient(bool withAuth = true)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSerializer<UploadSerializer>();

            serviceCollection
                .Addficsit_api()
                .ConfigureHttpClient(client => {
                    client.BaseAddress = new Uri(StaticOptions.APIURL);
                    if (withAuth)
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue(StaticOptions.APIKEY);
                    }
                });


            IServiceProvider services = serviceCollection.BuildServiceProvider();

            Ificsit_api client = services.GetRequiredService<Ificsit_api>();
            return client;
        }
    }
}
