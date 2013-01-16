using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Discovery;
using System.Text;
using Newtonsoft.Json;
using Phare.Shared;
using Phare.Service.Plugins;
using Phare.Service.EndpointMetadataProviders;

namespace Phare.Service
{
    public static class Extensions
    {
        internal static void Initialize(this IEnumerable<PluginBase> plugins, ManagedProxyDiscoveryService discoveryService, IEndpointMetadataProvider endpointMetadataProvider)
        {
            if (plugins != null)
            {
                plugins.All(plugin =>
                {
                    plugin.Initialize(discoveryService, endpointMetadataProvider);
                    return true;
                });
            }
        }

        internal static T Invoke<T>(this IEnumerable<PluginBase> plugins, Func<T> argsCreator, Action<PluginBase, T> action, bool throwException) where T : PluginContextBase
        {
            var e = argsCreator.Invoke();
            if (plugins != null)
            {
                plugins.All(plugin =>
                {
                    try
                    {
                        action.Invoke(plugin, e);
                    }
                    catch
                    {
                        if (throwException)
                        {
                            throw;
                        }
                    }
                    return true;
                });
            }
            return e;
        }

        internal static DateTime GetUpdatedOn(this EndpointDiscoveryMetadata metadata)
        {
            var updatedOnJson = metadata.Extensions
                .Where(x => x.Name == Constants.CST_XELEMNAME_UPDATEDON)
                .Select(x => x.Value)
                .FirstOrDefault();
            var updatedOn = JsonConvert.DeserializeObject<DateTime>(updatedOnJson);
            return updatedOn;
        }

        internal static bool IsIgnored(this EndpointDiscoveryMetadata metadata)
        {
            var ignored = false;
            bool.TryParse(
                metadata.Extensions
                    .Where(x => x.Name == Constants.CST_XELEMNAME_IGNORED)
                    .Select(x => x.Value)
                    .FirstOrDefault(),
                out ignored);
            return ignored;
        }

        public static string GetBindingTypeName(this EndpointDiscoveryMetadata metadata)
        {
            return metadata.Extensions
                    .Where(x => x.Name == Constants.CST_XELEMNAME_BINDINGTYPENAME)
                    .Select(x => x.Value)
                    .FirstOrDefault();
        }

        public static string GetBindingJson(this EndpointDiscoveryMetadata metadata)
        {
            return metadata.Extensions
                    .Where(x => x.Name == Constants.CST_XELEMNAME_BINDING)
                    .Select(x => x.Value)
                    .FirstOrDefault();
        }

        public static string GetAddressJson(this EndpointDiscoveryMetadata metadata)
        {
            return JsonConvert.SerializeObject(metadata.Address);
        }

        public static IEnumerable<string> GetContractTypeNames(this EndpointDiscoveryMetadata metadata)
        {
            return metadata.ContractTypeNames.Select(nm => string.Format("{0}, {1}", nm.Name, nm.Namespace));
        }

        public static IEnumerable<string> GetContractTypeNames(this FindCriteria criteria)
        {
            return criteria.ContractTypeNames.Select(nm => string.Format("{0}, {1}", nm.Name, nm.Namespace));
        }
    }
}
