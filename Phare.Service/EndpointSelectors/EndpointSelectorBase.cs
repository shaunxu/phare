using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Discovery;
using System.Text;

namespace Phare.Service.EndpointSelectors
{
    public abstract class EndpointSelectorBase : IEndpointSelector
    {
        public EndpointDiscoveryMetadata Select(IEnumerable<EndpointDiscoveryMetadata> endpoints)
        {
            // when no endpoints or only one endpoint then no need to invoke the actual selector, just return null or the only one
            var endpointCount = endpoints.Count();
            if (endpointCount <= 0)
            {
                return null;
            }
            else
            {
                if (endpointCount > 1)
                {
                    return OnSelect(endpoints, endpointCount);
                }
                else
                {
                    return endpoints.First();
                }
            }
        }

        protected abstract EndpointDiscoveryMetadata OnSelect(IEnumerable<EndpointDiscoveryMetadata> endpoints, int endpointCount);
    }
}
