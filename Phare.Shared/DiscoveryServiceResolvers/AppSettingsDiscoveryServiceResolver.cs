using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ServiceModel.Channels;

namespace Phare.Shared.DiscoveryServiceResolvers
{
    public class AppSettingsDiscoveryServiceResolver : IDiscoveryServiceResolver
    {
        public string AnnouncementEndpoint
        {
            get 
            {
                return ConfigurationManager.AppSettings[Constants.CST_CONFIGKEY_ANNOUNCEMENTENDPOINT];
            }
        }

        public string ProbeEndpoint
        {
            get 
            {
                return ConfigurationManager.AppSettings[Constants.CST_CONFIGKEY_PROBEENDPOINT];
            }
        }

        public Binding AnnouncementBinding
        {
            get 
            {
                var bindingTypeName = ConfigurationManager.AppSettings[Constants.CST_CONFIGKEY_DISCOVERYBINDING];
                var binding = Activator.CreateInstance(Type.GetType(bindingTypeName, true, true)) as Binding;
                return binding;
            }
        }

        public Binding ProbeBinding
        {
            get 
            {
                return AnnouncementBinding;
            }
        }
    }
}
