using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;

namespace Phare.Shared.DiscoveryServiceResolvers
{
    public interface IDiscoveryServiceResolver
    {
        string AnnouncementEndpoint { get; }

        string ProbeEndpoint { get; }

        Binding AnnouncementBinding { get; }

        Binding ProbeBinding { get; }
    }
}
