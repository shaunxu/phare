using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phare.Client;

namespace Phare.Test.DiscoverableService
{
    [Discoverable(new string[] { "ICalculateService" }, new string[] { "mex" })]
    public class MixedService : IStringService, ICalculateService
    {
        public string ToUpper(string content)
        {
            return content.ToUpper();
        }

        public int Add(int x, int y)
        {
            return x + y;
        }
    }
}
