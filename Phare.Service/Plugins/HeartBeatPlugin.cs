using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phare.Service.Plugins
{
    public class HeartBeatPlugin : PluginBase
    {
        private TimeSpan _expiration;

        public HeartBeatPlugin(TimeSpan expiration)
            : base()
        {
            _expiration = expiration;
        }

        protected override void OnAfterBeginFind(FindPluginContext e)
        {
            var candidate = e.CandidateEndpoint;
            var updatedOn = candidate.GetUpdatedOn();
            var now = DateTime.UtcNow;
            var expiredOn = updatedOn + _expiration;
            if (now > expiredOn)
            {
                e.Cancel = true;
            }

            base.OnAfterBeginFind(e);
        }
    }
}
