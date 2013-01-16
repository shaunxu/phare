using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Discovery;

namespace Phare.Service.Plugins
{
    public abstract class PluginContextBase
    {
        public bool Cancel { get; set; }
        public bool PassThrough { get; set; }

        public PluginContextBase()
        {
            Cancel = false;
            PassThrough = false;
        }
    }

    public class StartPluginContext : PluginContextBase
    {
        public StartPluginContext()
            : base()
        {
        }
    }

    public class StopPluginContext : PluginContextBase
    {
        public StopPluginContext()
            : base()
        {
        }
    }

    public class AnnouncementPluginContext : PluginContextBase
    {
        public EndpointDiscoveryMetadata Metadata { get; set; }

        public AnnouncementPluginContext(EndpointDiscoveryMetadata metadata)
            : base()
        {
            Metadata = metadata;
        }
    }

    public class FindPluginContext : PluginContextBase
    {
        public FindCriteria Criteria { get; set; }
        public EndpointDiscoveryMetadata CandidateEndpoint { get; set; }

        public FindPluginContext(FindCriteria criteria, EndpointDiscoveryMetadata candidateEndpoint)
            : base()
        {
            Criteria = criteria;
            CandidateEndpoint = candidateEndpoint;
        }

        public FindPluginContext(FindPluginContext other)
        {
            Criteria = other.Criteria;
            CandidateEndpoint = other.CandidateEndpoint;
        }
    }
}
