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
using Phare.Shared.DiscoveryServiceResolvers;

namespace Phare.Test.Client
{
    class Program
    {
        //static Tuple<EndpointAddress, Binding> FindServiceEndpoint()
        //{
        //    var probeEndpointAddress = new EndpointAddress(ConfigurationManager.AppSettings[Constants.CST_CONFIGKEY_ANNOUNCEMENTENDPOINT]);
        //    var probeBinding = Activator.CreateInstance(Type.GetType(ConfigurationManager.AppSettings[Constants.CST_CONFIGKEY_ANNOUNCEMENTBINDING], true, true)) as Binding;
        //    var discoveryEndpoint = new DiscoveryEndpoint(probeBinding, probeEndpointAddress);

        //    EndpointAddress address = null;
        //    Binding binding = null;
        //    FindResponse result = null;
        //    using (var discoveryClient = new DiscoveryClient(discoveryEndpoint))
        //    {
        //        result = discoveryClient.Find(new FindCriteria(typeof(IStringService)));
        //    }

        //    if (result != null && result.Endpoints.Any())
        //    {
        //        var endpointMetadata = result.Endpoints.First();
        //        address = endpointMetadata.Address;
        //        var bindingTypeName = endpointMetadata.Extensions
        //            .Where(x => x.Name == Constants.CST_XELEMNAME_BINDINGTYPENAME)
        //            .Select(x => x.Value)
        //            .FirstOrDefault();
        //        var bindingJson = endpointMetadata.Extensions
        //            .Where(x => x.Name == Constants.CST_XELEMNAME_BINDING)
        //            .Select(x => x.Value)
        //            .FirstOrDefault();
        //        var bindingType = Type.GetType(bindingTypeName, true, true);
        //        binding = JsonConvert.DeserializeObject(bindingJson, bindingType) as Binding;
        //    }
        //    return new Tuple<EndpointAddress, Binding>(address, binding);
        //}

        static void Main(string[] args)
        {
            Console.WriteLine("Say something...");
            var content = Console.ReadLine();
            while (!string.IsNullOrWhiteSpace(content))
            {
                Console.WriteLine("Finding the service endpoint...");
                var serviceMetadata = Discovery.Current.FindService<IStringService>(new AppSettingsDiscoveryServiceResolver());
                if (serviceMetadata == null)
                {
                    Console.WriteLine("There is no endpoint matches the criteria.");
                }
                else
                {
                    var address = serviceMetadata.EndpointAddress;
                    var binding = serviceMetadata.Binding;
                    Console.WriteLine("Found the endpoint {0} use binding {1}", address.Uri, binding.Name);

                    var factory = new ChannelFactory<IStringService>(binding, address);
                    factory.Opened += (sender, e) =>
                    {
                        Console.WriteLine("Connecting to {0}.", factory.Endpoint.ListenUri);
                    };
                    var proxy = factory.CreateChannel();
                    using (proxy as IDisposable)
                    {
                        Console.WriteLine("ToUpper: {0} => {1}", content, proxy.ToUpper(content));
                    }
                }

                Console.WriteLine("Say something...");
                content = Console.ReadLine();
            }
        }
    }
}
