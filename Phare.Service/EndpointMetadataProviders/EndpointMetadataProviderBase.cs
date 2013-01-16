using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Discovery;
using System.Text;
using Phare.Service.EndpointSelectors;

namespace Phare.Service.EndpointMetadataProviders
{
    public abstract class EndpointMetadataProviderBase : IEndpointMetadataProvider
    {
        private IEndpointSelector _endpointSelector;

        protected EndpointMetadataProviderBase(IEndpointSelector endpointSelector)
        {
            _endpointSelector = endpointSelector;
        }

        public abstract void AddEndpointMetadata(System.ServiceModel.Discovery.EndpointDiscoveryMetadata metadata);

        public abstract void RemoveEndpointMetadata(System.ServiceModel.Discovery.EndpointDiscoveryMetadata metadata);

        public EndpointDiscoveryMetadata MatchEndpoint(FindCriteria criteria)
        {
            EndpointDiscoveryMetadata endpoint = null;
            var endpoints = OnMatchEndpoints(criteria);
            var metadatas = new List<EndpointDiscoveryMetadata>();
            foreach (var ep in endpoints)
            {
                var metadata = ep.ToEndpointDiscoveryMetadata();
                metadatas.Add(metadata);
            }
            endpoint = _endpointSelector.Select(metadatas);
            return endpoint;
        }

        protected abstract IEnumerable<MatchedEndpointDiscoveryMetadata> OnMatchEndpoints(FindCriteria criteria);
    }
}
