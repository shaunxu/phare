using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Phare.Test.DiscoverableService
{
    [ServiceContract]
    public interface ICalculateService
    {
        [OperationContract]
        int Add(int x, int y);
    }
}
