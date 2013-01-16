using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Xml;
using System.ServiceModel.Channels;
using System.ServiceModel.Discovery;
using Phare.Shared;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.IdentityModel.Claims;

namespace Phare.Service
{
    public class MatchedEndpointDiscoveryMetadata
    {
        private EndpointDiscoveryMetadata _metadata;

        private string _addressJson;
        private string _bindingTypeName;
        private string _bindingJson;
        private DateTime _updatedOn;

        public MatchedEndpointDiscoveryMetadata(EndpointDiscoveryMetadata metadata)
        {
            _metadata = metadata;
        }

        public MatchedEndpointDiscoveryMetadata(string addressJson, string bindingTypeName, string bindingJson, DateTime updatedOn)
        {
            _metadata = null;

            _addressJson = addressJson;
            _bindingTypeName = bindingTypeName;
            _bindingJson = bindingJson;
            _updatedOn = updatedOn;
        }

        public EndpointDiscoveryMetadata ToEndpointDiscoveryMetadata()
        {
            if (_metadata == null)
            {
                _metadata = new EndpointDiscoveryMetadata();
                var address = JsonConvert.DeserializeObject<EndpointAddressWrapper>(_addressJson).EndpointAddress;
                _metadata.Address = address;
                _metadata.Extensions.Add(new XElement(Constants.CST_XELEMNAME_BINDINGTYPENAME, _bindingTypeName));
                _metadata.Extensions.Add(new XElement(Constants.CST_XELEMNAME_BINDING, _bindingJson));
                _metadata.Extensions.Add(new XElement(Constants.CST_XELEMNAME_UPDATEDON, JsonConvert.SerializeObject(_updatedOn)));
            }
            return _metadata;
        }
    }

    #region JSON Format Wrapper Class

    internal class AddressHeaderWrapper
    {
        public string Name { get; set; }

        public string Namespace { get; set; }

        public AddressHeader ToAddressHeader()
        {
            return AddressHeader.CreateAddressHeader(Name, Namespace, null);
        }
    }

    internal class EndpointIdentityWrapper
    {
        public Claim IdentityClaim { get; set; }

        public EndpointIdentity EndpointIdentity
        {
            get
            {
                return EndpointIdentity.CreateIdentity(IdentityClaim);
            }
        }
    }

    internal class EndpointAddressWrapper
    {
        public AddressHeaderWrapper[] Headers { get; set; }

        public EndpointIdentityWrapper Identity { get; set; }

        public bool IsAnonymous { get; set; }

        public bool IsNone { get; set; }

        public Uri Uri { get; set; }

        public EndpointAddress EndpointAddress
        {
            get
            {
                var headerWrappers = Headers ?? new AddressHeaderWrapper[] { };
                var headers = headerWrappers.Select(h => h.ToAddressHeader()).ToArray();
                if (Identity == null)
                {
                    return new EndpointAddress(Uri, headers);
                }
                else
                {
                    return new EndpointAddress(Uri, Identity.EndpointIdentity, headers);
                }
            }
        }
    }

    #endregion
}
