using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Discovery;
using System.Threading;

namespace Phare.Client.Plugins
{
    public class HeartBeatPlugin : PluginBase
    {
        private int _interval;
        private Task _heartbeatTask;

        public HeartBeatPlugin()
            : this(15)
        {
        }

        public HeartBeatPlugin(int intervalInSeconds)
        {
            _interval = intervalInSeconds * 1000;
        }

        protected override void OnApply(PluginContext context)
        {
            // convert to the discovery service metadata for future sending announcement message
            var metadatas = new List<EndpointDiscoveryMetadata>();
            foreach (var discoverable in context.DiscoverableServiceEndpoints)
            {
                var endpoint = discoverable.Endpoint;
                var extensions = discoverable.Extensions;
                var metadata = EndpointDiscoveryMetadata.FromServiceEndpoint(endpoint);
                foreach (var extension in extensions)
                {
                    metadata.Extensions.Add(extension);
                }
                metadatas.Add(metadata);
            }
            // start the task send the heart beat periodly
            _heartbeatTask = Task.Factory.StartNew(() => HeartBeat(metadatas, context.AnnouncementEndpoint));
            base.OnApply(context);
        }

        private void HeartBeat(IEnumerable<EndpointDiscoveryMetadata> metadatas, AnnouncementEndpoint announcementEndpoint)
        {
            while (true)
            {
                Thread.Sleep(_interval);

                using (var announcementClient = new AnnouncementClient(announcementEndpoint))
                {
                    foreach (var metadata in metadatas)
                    {
                        announcementClient.AnnounceOnline(metadata);
                    }
                }
            }
        }
    }
}
