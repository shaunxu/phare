using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Discovery;
using System.Text;
using Newtonsoft.Json;
using Phare.Shared;
using Phare.Shared.DiscoveryServiceResolvers;

namespace Phare.Client
{
    public class Discovery
    {
        #region Singleton

        private static Discovery _instance;

        public static Discovery Current
        {
            get
            {
                return _instance;
            }
        }

        static Discovery()
        {
            _instance = new Discovery();
        }

        #endregion

        private Discovery()
        {
        }

        public DiscoveryServiceMetadata FindService<TContract>(IDiscoveryServiceResolver discoveryServiceResolver)
        {
            return FindService(typeof(TContract), discoveryServiceResolver);
        }

        public DiscoveryServiceMetadata FindService(Type contractType, IDiscoveryServiceResolver discoveryServiceResolver)
        {
            // establish the connection to the discovery service
            var probeEndpointAddress = new EndpointAddress(discoveryServiceResolver.ProbeEndpoint);
            var discoveryEndpoint = new DiscoveryEndpoint(discoveryServiceResolver.ProbeBinding, probeEndpointAddress);
            using (var discoveryClient = new DiscoveryClient(discoveryEndpoint))
            {
                // probe the service metadata
                var criteria = new FindCriteria(contractType);
                var result = discoveryClient.Find(criteria);
                if (result != null && result.Endpoints.Any())
                {
                    // retrieve the address and binding information
                    var endpointMetadata = result.Endpoints.First();
                    var address = endpointMetadata.Address;
                    var bindingTypeName = endpointMetadata.Extensions
                        .Where(x => x.Name == Constants.CST_XELEMNAME_BINDINGTYPENAME)
                        .Select(x => x.Value)
                        .FirstOrDefault();
                    var bindingJson = endpointMetadata.Extensions
                        .Where(x => x.Name == Constants.CST_XELEMNAME_BINDING)
                        .Select(x => x.Value)
                        .FirstOrDefault();
                    var bindingType = Type.GetType(bindingTypeName, true, true);
                    var binding = JsonConvert.DeserializeObject(bindingJson, bindingType) as Binding;
                    return new DiscoveryServiceMetadata(address, binding);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
