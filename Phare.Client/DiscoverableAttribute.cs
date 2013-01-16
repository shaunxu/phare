using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using Phare.Shared;
using Phare.Shared.DiscoveryServiceResolvers;
using Phare.Client.Plugins;

namespace Phare.Client
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DiscoverableAttribute : Attribute, IServiceBehavior
    {
        private IServiceBehavior _serviceDiscoveryBehavior;
        private IEnumerable<string> _ignoredContractTypeNames;
        private IEnumerable<string> _ignoredEndpoints;

        private IEnumerable<PluginBase> _plugins;
        private AnnouncementEndpoint _announcementEndpoint;

        public DiscoverableAttribute(Type resolverType, string[] ignoredContractTypeNames, string[] ignoredEndpoints, params Type[] pluginTypes)
        {
            _ignoredContractTypeNames = ignoredContractTypeNames ?? new string[] { };
            _ignoredEndpoints = ignoredEndpoints ?? new string[] { };

            var resolver = Activator.CreateInstance(resolverType) as IDiscoveryServiceResolver;
            var binding = resolver.AnnouncementBinding;
            var endpointUrl = resolver.AnnouncementEndpoint;
            _announcementEndpoint = new AnnouncementEndpoint(binding, new EndpointAddress(endpointUrl));

            var serviceDiscoveryBehavior = new ServiceDiscoveryBehavior();
            serviceDiscoveryBehavior.AnnouncementEndpoints.Add(_announcementEndpoint);
            _serviceDiscoveryBehavior = serviceDiscoveryBehavior;

            // initialize the plugin instances (ensure the parameter is not null)
            var plugins = new List<PluginBase>();
            foreach (var plugInType in pluginTypes ?? new Type[] { })
            {
                var plugin = Activator.CreateInstance(plugInType) as PluginBase;
                if (plugin != null)
                {
                    plugins.Add(plugin);
                }
            }
            _plugins = plugins;
        }

        public DiscoverableAttribute(string[] ignoredContractTypeNames, string[] ignoredEndpoints, params Type[] pluginTypes)
            : this(typeof(AppSettingsDiscoveryServiceResolver),
                   ignoredContractTypeNames, ignoredEndpoints, pluginTypes)
        {
        }

        public DiscoverableAttribute(Type resolverType, params Type[] pluginTypes)
            : this(resolverType, new string[] { }, new string[] { }, pluginTypes)
        {
        }

        public DiscoverableAttribute(params Type[] pluginTypes)
            : this(typeof(AppSettingsDiscoveryServiceResolver), pluginTypes)
        {
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
            // apply the service discovery behavior opertaion
            _serviceDiscoveryBehavior.AddBindingParameters(serviceDescription, serviceHostBase, endpoints, bindingParameters);
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            var disServiceEndpoints = new List<PluginContext.DiscoverableServiceEndpoint>();
            // apply the service discovery behavior opertaion
            _serviceDiscoveryBehavior.ApplyDispatchBehavior(serviceDescription, serviceHostBase);
            // add the additional information into the endpoint metadata extension through the endpoint discovery binding for each endpoints
            foreach (var endpoint in serviceDescription.Endpoints)
            {
                var endpointDiscoveryBehavior = new EndpointDiscoveryBehavior();
                // check if the contract or endpoint should be ignored
                // set the extensions
                endpoint.SetExtensions(endpointDiscoveryBehavior, behavior => behavior.Extensions, _ignoredContractTypeNames, _ignoredEndpoints);
                endpoint.Behaviors.Add(endpointDiscoveryBehavior);
                // save the plugin context
                disServiceEndpoints.Add(new PluginContext.DiscoverableServiceEndpoint(endpoint, endpointDiscoveryBehavior.Extensions));
            }
            // invoke the plugins
            var context = new PluginContext(disServiceEndpoints, _announcementEndpoint);
            foreach (var plugin in _plugins)
            {
                plugin.Apply(context);
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            // apply the service discovery behavior opertaion
            _serviceDiscoveryBehavior.Validate(serviceDescription, serviceHostBase);
        }
    }
}
