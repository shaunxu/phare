using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using Phare.Shared;
using Newtonsoft.Json;

namespace Phare.Client
{
    public static class Extensions
    {
        public static void SetExtensions<T>(this ServiceEndpoint endpoint, T source, Func<T, Collection<XElement>> extensionsSelector, IEnumerable<string> ignoredContractTypeNames, IEnumerable<string> ignoredEndpoints)
        {
            var extensions = extensionsSelector.Invoke(source);
            // check if the contract or endpoint should be ignored
            var contractTypeName = endpoint.Contract.Name;
            var endpointAddress = endpoint.Address.Uri;
            if (ignoredContractTypeNames.Any(ctn => string.Compare(ctn, contractTypeName, true) == 0) ||
                ignoredEndpoints.Any(ep => string.Compare(ep, endpointAddress.Segments.Last(), true) == 0))
            {
                extensions.Add(new XElement(Constants.CST_XELEMNAME_IGNORED, true));
            }
            else
            {
                extensions.Add(new XElement(Constants.CST_XELEMNAME_IGNORED, false));
                // add the binding infomation
                var binding = endpoint.Binding;
                var bindingType = binding.GetType().AssemblyQualifiedName;
                var bindingJson = JsonConvert.SerializeObject(binding);
                extensions.Add(new XElement(Constants.CST_XELEMNAME_BINDINGTYPENAME, bindingType));
                extensions.Add(new XElement(Constants.CST_XELEMNAME_BINDING, bindingJson));
            }
        }
    }
}
