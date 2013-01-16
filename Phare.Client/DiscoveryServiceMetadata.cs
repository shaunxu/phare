using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace Phare.Client
{
    public class DiscoveryServiceMetadata
    {
        public EndpointAddress EndpointAddress { get; private set; }

        public Binding Binding { get; private set; }

        internal DiscoveryServiceMetadata(EndpointAddress endpointAddress, Binding binding)
        {
            EndpointAddress = endpointAddress;
            Binding = binding;
        }
    }
}
