using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Text;
using Phare.Service.EndpointMetadataProviders;
using Phare.Shared;
using Phare.Service.Plugins;

namespace Phare.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ManagedProxyDiscoveryService : DiscoveryProxy, IDisposable
    {
        private IEnumerable<PluginBase> _plugins;
        private IEndpointMetadataProvider _metadataProvider;

        public ManagedProxyDiscoveryService(IEndpointMetadataProvider metadataProvider, IEnumerable<PluginBase> plugins)
        {
            _plugins = plugins ?? new PluginBase[] { };
            _metadataProvider = metadataProvider;

            _plugins.Initialize(this, _metadataProvider);
            _plugins.Invoke(() => new StartPluginContext(), (plugin, e) => plugin.Start(e), true);
        }

        protected override IAsyncResult OnBeginOnlineAnnouncement(DiscoveryMessageSequence messageSequence, EndpointDiscoveryMetadata endpointDiscoveryMetadata, AsyncCallback callback, object state)
        {
            var context = _plugins.Invoke(() => new AnnouncementPluginContext(endpointDiscoveryMetadata), (plugin, e) => plugin.BeforeBeginOnlineAnnouncement(e), true);

            if (!context.Cancel)
            {
                if (!endpointDiscoveryMetadata.IsIgnored())
                {
                    _metadataProvider.AddEndpointMetadata(endpointDiscoveryMetadata);
                }
            }

            _plugins.Invoke(() => new AnnouncementPluginContext(context.Metadata), (plugin, e) => plugin.AfterBeginOnlineAnnouncement(e), true);
            return new OnOnlineAnnouncementAsyncResult(callback, state);
        }

        protected override void OnEndOnlineAnnouncement(IAsyncResult result)
        {
            OnOnlineAnnouncementAsyncResult.End(result);
        }

        protected override IAsyncResult OnBeginOfflineAnnouncement(DiscoveryMessageSequence messageSequence, EndpointDiscoveryMetadata endpointDiscoveryMetadata, AsyncCallback callback, object state)
        {
            var context = _plugins.Invoke(() => new AnnouncementPluginContext(endpointDiscoveryMetadata), (plugin, e) => plugin.BeforeBeginOfflineAnnouncement(e), true);

            if (!context.Cancel)
            {
                if (!endpointDiscoveryMetadata.IsIgnored())
                {
                    _metadataProvider.RemoveEndpointMetadata(endpointDiscoveryMetadata);
                }
            }

            _plugins.Invoke(() => new AnnouncementPluginContext(context.Metadata), (plugin, e) => plugin.AfterBeginOfflineAnnouncement(e), true);
            return new OnOfflineAnnouncementAsyncResult(callback, state);
        }

        protected override void OnEndOfflineAnnouncement(IAsyncResult result)
        {
            OnOfflineAnnouncementAsyncResult.End(result);
        }

        protected override IAsyncResult OnBeginFind(FindRequestContext findRequestContext, AsyncCallback callback, object state)
        {
            var context = _plugins.Invoke(() => new FindPluginContext(findRequestContext.Criteria, null), (plugin, e) => plugin.BeforeBeginFind(e), true);
            if (!context.Cancel)
            {
                context.CandidateEndpoint = _metadataProvider.MatchEndpoint(findRequestContext.Criteria);
            }

            _plugins.Invoke(() => new FindPluginContext(context), (plugin, e) => plugin.AfterBeginFind(e), true);
            if (!context.Cancel && context.CandidateEndpoint != null)
            {
                findRequestContext.AddMatchingEndpoint(context.CandidateEndpoint);
            }

            return new OnFindAsyncResult(callback, state);
        }

        protected override void OnEndFind(IAsyncResult result)
        {
            OnFindAsyncResult.End(result);
        }

        protected override IAsyncResult OnBeginResolve(ResolveCriteria resolveCriteria, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        protected override EndpointDiscoveryMetadata OnEndResolve(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            // stop all plugins and hide all exceptions occurred
            _plugins.Invoke(() => new StopPluginContext(), (plugin, e) => plugin.Stop(e), false);
        }
    }
}
