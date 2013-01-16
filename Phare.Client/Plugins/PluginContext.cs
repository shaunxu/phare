using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Discovery;
using System.ServiceModel.Description;
using System.Xml.Linq;
using System.Collections.ObjectModel;

namespace Phare.Client.Plugins
{
    public class PluginContext
    {
        public bool Cancel { get; set; }
        public AnnouncementEndpoint AnnouncementEndpoint { get; set; }
        public IEnumerable<DiscoverableServiceEndpoint> DiscoverableServiceEndpoints { get; private set; }

        public PluginContext(IEnumerable<DiscoverableServiceEndpoint> discoverableServiceEndpoints, AnnouncementEndpoint announcementEndpoint)
        {
            Cancel = false;
            AnnouncementEndpoint = announcementEndpoint;
            DiscoverableServiceEndpoints = discoverableServiceEndpoints ?? new DiscoverableServiceEndpoint[] { };
        }

        public class DiscoverableServiceEndpoint
        {
            public ServiceEndpoint Endpoint { get; private set; }

            public Collection<XElement> Extensions { get; private set; }

            public DiscoverableServiceEndpoint(ServiceEndpoint endpoint, Collection<XElement> extensions)
            {
                Endpoint = endpoint;
                Extensions = extensions;
            }
        }
    }
}
