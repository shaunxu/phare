using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Claims;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Discovery;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using Phare.Service.EndpointSelectors;
using Phare.Shared;

namespace Phare.Service.EndpointMetadataProviders
{
    public class DbEndpointMetadataProvider : EndpointMetadataProviderBase
    {
        private string _connectionString;
        private Func<string, IDbConnection> _connectionCreator;

        public DbEndpointMetadataProvider(IEndpointSelector endpointSelector, Func<string, IDbConnection> connectionCreator, string connectionString)
            : base(endpointSelector)
        {
            _connectionCreator = connectionCreator;
            _connectionString = connectionString;
        }

        public override void AddEndpointMetadata(EndpointDiscoveryMetadata metadata)
        {
            var uri = metadata.Address.Uri.AbsoluteUri;
            var addressJson = JsonConvert.SerializeObject(metadata.Address);
            var contractTypeNames = metadata.GetContractTypeNames();
            var bindingTypeName = metadata.GetBindingTypeName();
            var bindingJson = metadata.GetBindingJson();
            using (var conn = _connectionCreator.Invoke(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "RegisterServiceEndpointMetadata";
                cmd.AddParameter("uri", uri)
                   .AddParameter("contractType", null)
                   .AddParameter("address", addressJson)
                   .AddParameter("bindingType", bindingTypeName)
                   .AddParameter("binding", bindingJson);

                conn.Open();
                cmd.Transaction = conn.BeginTransaction();
                foreach (var contractTypeName in contractTypeNames)
                {
                    cmd.GetParameter("contractType").Value = contractTypeName;
                    cmd.ExecuteNonQuery();
                }
                cmd.Transaction.Commit();
            }
        }

        public override void RemoveEndpointMetadata(EndpointDiscoveryMetadata metadata)
        {
            var uri = metadata.Address.Uri.AbsoluteUri;
            var addressJson = JsonConvert.SerializeObject(metadata.Address);
            var contractTypeNames = metadata.GetContractTypeNames();
            var bindingTypeName = metadata.GetBindingTypeName();
            using (var conn = _connectionCreator.Invoke(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UnRegisterServiceEndpointMetadata";
                cmd.AddParameter("uri", uri)
                    .AddParameter("contractType", null)
                    .AddParameter("address", addressJson)
                    .AddParameter("bindingType", bindingTypeName);

                conn.Open();
                cmd.Transaction = conn.BeginTransaction();
                foreach (var contractTypeName in contractTypeNames)
                {
                    cmd.GetParameter("contractType").Value = contractTypeName;
                    cmd.ExecuteNonQuery();
                }
                cmd.Transaction.Commit();
            }
        }

        protected override IEnumerable<MatchedEndpointDiscoveryMetadata> OnMatchEndpoints(FindCriteria criteria)
        {
            var endpoints = new List<MatchedEndpointDiscoveryMetadata>();
            var contractTypeNames = criteria.GetContractTypeNames();
            using (var conn = _connectionCreator.Invoke(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetServiceEndpointMetadata";
                cmd.AddParameter("contractType", null);

                conn.Open();
                foreach (var contractTypeName in contractTypeNames)
                {
                    cmd.GetParameter("contractType").Value = contractTypeName;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var addressJson = (string)reader["Address"];
                            var bindingTypeName = (string)reader["BindingType"];
                            var bindingJson = (string)reader["Binding"];
                            var updatedOn = (DateTime)reader["UpdatedOn"];
                            var matchedEndpoint = new MatchedEndpointDiscoveryMetadata(addressJson, bindingTypeName, bindingJson, updatedOn);
                            endpoints.Add(matchedEndpoint);
                        }
                    }
                }
            }
            return endpoints;
        }
    }

    #region Database Helper Class

    internal static class DatabaseHelpers
    {
        public static IDbCommand AddParameter(this IDbCommand cmd, string name, object value)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = name;
            param.Value = value;
            cmd.Parameters.Add(param);
            return cmd;
        }

        public static IDbDataParameter GetParameter(this IDbCommand cmd, string parameterName)
        {
            return cmd.Parameters[parameterName] as IDbDataParameter;
        }
    }

    #endregion
}
