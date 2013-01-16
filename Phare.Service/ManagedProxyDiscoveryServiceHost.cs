using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Phare.Service.EndpointMetadataProviders;
using System.ServiceModel.Channels;
using System.ServiceModel.Discovery;
using System.Configuration;
using Phare.Shared.DiscoveryServiceResolvers;
using Phare.Service.Plugins;

namespace Phare.Service
{
    public class ManagedProxyDiscoveryServiceHost : ServiceHost
    {
        public ManagedProxyDiscoveryServiceHost(IEndpointMetadataProvider endpointMetadataProvider, params PluginBase[] plugins)
            : this(endpointMetadataProvider, new AppSettingsDiscoveryServiceResolver(), plugins)
        {
        }

        public ManagedProxyDiscoveryServiceHost(IEndpointMetadataProvider endpointMetadataProvider, IDiscoveryServiceResolver discoveryServiceResolver, params PluginBase[] plugins)
            : base(new ManagedProxyDiscoveryService(endpointMetadataProvider, plugins))
        {
            var announcementEndpoint = new AnnouncementEndpoint(discoveryServiceResolver.AnnouncementBinding, new EndpointAddress(discoveryServiceResolver.AnnouncementEndpoint));
            var probeEndpoint = new DiscoveryEndpoint(discoveryServiceResolver.ProbeBinding, new EndpointAddress(discoveryServiceResolver.ProbeEndpoint));
            probeEndpoint.IsSystemEndpoint = false;
            AddServiceEndpoint(announcementEndpoint);
            AddServiceEndpoint(probeEndpoint);
        }
    }
}
