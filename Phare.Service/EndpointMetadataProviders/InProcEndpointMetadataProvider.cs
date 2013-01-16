using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Text;
using Phare.Service.EndpointSelectors;

namespace Phare.Service.EndpointMetadataProviders
{
    public class InProcEndpointMetadataProvider : EndpointMetadataProviderBase
    {
        private ConcurrentDictionary<EndpointAddress, EndpointDiscoveryMetadata> _endpoints;

        public InProcEndpointMetadataProvider(IEndpointSelector endpointSelector)
            : base(endpointSelector)
        {
            _endpoints = new ConcurrentDictionary<EndpointAddress, EndpointDiscoveryMetadata>();
        }

        public override void AddEndpointMetadata(EndpointDiscoveryMetadata metadata)
        {
            _endpoints.AddOrUpdate(metadata.Address, metadata, (key, value) => metadata);
        }

        public override void RemoveEndpointMetadata(EndpointDiscoveryMetadata metadata)
        {
            EndpointDiscoveryMetadata value = null;
            _endpoints.TryRemove(metadata.Address, out value);
        }

        protected override IEnumerable<MatchedEndpointDiscoveryMetadata> OnMatchEndpoints(FindCriteria criteria)
        {
            return _endpoints
                .Where(meta => criteria.IsMatch(meta.Value))
                .Select(meta => new MatchedEndpointDiscoveryMetadata(meta.Value))
                .ToList();
        }
    }
}
