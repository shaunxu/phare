using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Discovery;
using Phare.Service;
using Phare.Service.EndpointMetadataProviders;
using Phare.Service.EndpointSelectors;

namespace Phare.Hosting.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            IEndpointSelector selector = new RandomEndpointSelector();

            var connectionString = ConfigurationManager.ConnectionStrings["localhost"].ConnectionString;
            IEndpointMetadataProvider metadataProvider = new DbEndpointMetadataProvider(selector, connString => new SqlConnection(connString), connectionString);

            using (var host = new ManagedProxyDiscoveryServiceHost(metadataProvider))
            {
                host.Opened += (sender, e) =>
                {
                    host.Description.Endpoints.All((ep) =>
                    {
                        Console.WriteLine(ep.ListenUri);
                        return true;
                    });
                };

                try
                {
                    host.Open();

                    Console.WriteLine("Press any key to exit.");
                    Console.ReadKey();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            Console.WriteLine("Done.");
            Console.ReadKey();
        }
    }
}
