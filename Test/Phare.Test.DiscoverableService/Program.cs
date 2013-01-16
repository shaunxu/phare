using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Text;
using Phare.Client;

namespace Phare.Test.DiscoverableService
{
    class Program
    {
        static void Main(string[] args)
        {
            var baseAddress = new Uri(string.Format("http://localhost:11001/mixedservice/{0}/", Guid.NewGuid().ToString()));

            using (var host = new ServiceHost(typeof(MixedService), baseAddress))
            {
                host.Opened += (sender, e) =>
                {
                    host.Description.Endpoints.All((ep) =>
                    {
                        Console.WriteLine(ep.Contract.Name + ": " + ep.ListenUri);
                        return true;
                    });
                };

                var serviceMetadataBehavior = new ServiceMetadataBehavior();
                serviceMetadataBehavior.HttpGetEnabled = true;
                serviceMetadataBehavior.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                host.Description.Behaviors.Add(serviceMetadataBehavior);

                host.AddServiceEndpoint(typeof(IStringService), new BasicHttpBinding(), "string");
                host.AddServiceEndpoint(typeof(ICalculateService), new BasicHttpBinding(), "calculate");
                host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexHttpBinding(), "mex");

                host.Open();

                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }
    }
}
