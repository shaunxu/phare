using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Discovery;
using System.Text;

namespace Phare.Service.EndpointSelectors
{
    public class AlwaysLatestEndpointSelector : EndpointSelectorBase
    {
        protected override EndpointDiscoveryMetadata OnSelect(IEnumerable<EndpointDiscoveryMetadata> endpoints, int endpointCount)
        {
            return endpoints.OrderByDescending(ep => ep.GetUpdatedOn()).First();
        }
    }
}
