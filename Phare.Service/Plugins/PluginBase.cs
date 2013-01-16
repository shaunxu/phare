using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phare.Service.EndpointMetadataProviders;

namespace Phare.Service.Plugins
{
    public abstract class PluginBase
    {
        private ManagedProxyDiscoveryService _discoveryService;
        private IEndpointMetadataProvider _endpointMetadataProvider;

        protected ManagedProxyDiscoveryService DiscoveryService
        {
            get
            {
                return _discoveryService;
            }
        }

        protected IEndpointMetadataProvider EndpointMetadataProvider
        {
            get
            {
                return _endpointMetadataProvider;
            }
        }

        protected PluginBase()
        {
        }

        internal void Initialize(ManagedProxyDiscoveryService discoveryService, IEndpointMetadataProvider endpointMetadataProvider)
        {
            _discoveryService = discoveryService;
            _endpointMetadataProvider = endpointMetadataProvider;
        }

        private void InvokeIfNotPassThrough<T>(T e, Action<T> action) where T : PluginContextBase
        {
            if (!e.PassThrough)
            {
                action.Invoke(e);
            }
        }

        public void Start(StartPluginContext e)
        {
            InvokeIfNotPassThrough(e, (x) => OnStart(x));
        }

        public void Stop(StopPluginContext e)
        {
            InvokeIfNotPassThrough(e, (x) => OnStop(x));
        }

        public void BeforeBeginOnlineAnnouncement(AnnouncementPluginContext e)
        {
            InvokeIfNotPassThrough(e, (x) => OnBeforeBeginOnlineAnnouncement(x));
        }

        public void AfterBeginOnlineAnnouncement(AnnouncementPluginContext e)
        {
            InvokeIfNotPassThrough(e, (x) => OnAfterBeginOnlineAnnouncement(x));
        }

        public void BeforeBeginOfflineAnnouncement(AnnouncementPluginContext e)
        {
            InvokeIfNotPassThrough(e, (x) => OnBeforeBeginOfflineAnnouncement(x));
        }

        public void AfterBeginOfflineAnnouncement(AnnouncementPluginContext e)
        {
            InvokeIfNotPassThrough(e, (x) => OnAfterBeginOfflineAnnouncement(x));
        }

        public void BeforeBeginFind(FindPluginContext e)
        {
            InvokeIfNotPassThrough(e, (x) => OnBeforeBeginFind(x));
        }

        public void AfterBeginFind(FindPluginContext e)
        {
            InvokeIfNotPassThrough(e, (x) => OnAfterBeginFind(x));
        }

        protected virtual void OnStart(StartPluginContext e)
        {
        }

        protected virtual void OnStop(StopPluginContext e)
        {
        }

        protected virtual void OnBeforeBeginOnlineAnnouncement(AnnouncementPluginContext e)
        {
        }

        protected virtual void OnAfterBeginOnlineAnnouncement(AnnouncementPluginContext e)
        {
        }

        protected virtual void OnBeforeBeginOfflineAnnouncement(AnnouncementPluginContext e)
        {
        }

        protected virtual void OnAfterBeginOfflineAnnouncement(AnnouncementPluginContext e)
        {
        }

        protected virtual void OnBeforeBeginFind(FindPluginContext e)
        {
        }

        protected virtual void OnAfterBeginFind(FindPluginContext e)
        {
        }
    }
}
