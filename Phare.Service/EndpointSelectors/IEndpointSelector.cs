using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Discovery;
using System.Text;

namespace Phare.Service.EndpointSelectors
{
    public interface IEndpointSelector
    {
        EndpointDiscoveryMetadata Select(IEnumerable<EndpointDiscoveryMetadata> endpoints);
    }
}
