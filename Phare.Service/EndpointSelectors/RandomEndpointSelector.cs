using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Discovery;
using System.Text;

namespace Phare.Service.EndpointSelectors
{
    public class RandomEndpointSelector : EndpointSelectorBase
    {
        private Random _rnd;

        public RandomEndpointSelector()
        {
            _rnd = new Random();
        }

        protected override EndpointDiscoveryMetadata OnSelect(IEnumerable<EndpointDiscoveryMetadata> endpoints, int endpointCount)
        {
            var index = _rnd.Next(0, endpointCount - 1);
            return endpoints.ElementAt(index);
        }
    }
}
