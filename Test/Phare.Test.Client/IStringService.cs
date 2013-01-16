using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Phare.Test.Client
{
    [ServiceContract]
    public interface IStringService
    {
        [OperationContract]
        string ToUpper(string content);
    }
}
