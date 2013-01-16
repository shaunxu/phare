using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Discovery;
using System.Text;

namespace Phare.Service.EndpointMetadataProviders
{
    public interface IEndpointMetadataProvider
    {
        void AddEndpointMetadata(EndpointDiscoveryMetadata metadata);

        void RemoveEndpointMetadata(EndpointDiscoveryMetadata metadata);

        EndpointDiscoveryMetadata MatchEndpoint(FindCriteria criteria);
    }
}
