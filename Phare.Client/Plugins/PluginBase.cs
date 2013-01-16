using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phare.Client.Plugins
{
    public abstract class PluginBase
    {
        public void Apply(PluginContext context)
        {
            if (!context.Cancel)
            {
                OnApply(context);
            }
        }

        protected virtual void OnApply(PluginContext context)
        {
        }
    }
}
