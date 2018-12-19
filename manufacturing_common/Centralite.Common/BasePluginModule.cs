using Centralite.Common.Interfaces;
using Prism.Regions;
using System;
using System.Linq;

namespace Centralite.Common
{
    public abstract class BasePluginModule : IPlugin
    {
        protected IRegionManager regionManager;

        public BasePluginModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public abstract void LoadPlugin();

        public virtual void UnloadPlugin()
        {
            if (regionManager.Regions.ContainsRegionWithName("PluginRegion"))
            {
                regionManager.Regions["PluginRegion"].RemoveAll();
            }

            if (regionManager.Regions.ContainsRegionWithName("ConfigurePluginRegion"))
            {
                regionManager.Regions["ConfigurePluginRegion"].RemoveAll();
            }
        }

        protected void ConfigureRegions(Type PluginType, Type PluginConfigurationType)
        {
            if (regionManager.Regions.ContainsRegionWithName("PluginRegion") && !regionManager.Regions["PluginRegion"].Views.Any(x => x.GetType() == PluginType))
            {
                regionManager.RegisterViewWithRegion("PluginRegion", PluginType);
            }

            if (regionManager.Regions.ContainsRegionWithName("ConfigurePluginRegion") && !regionManager.Regions["ConfigurePluginRegion"].Views.Any(x => x.GetType() == PluginConfigurationType))
            {
                regionManager.RegisterViewWithRegion("ConfigurePluginRegion", PluginConfigurationType);
            }
        }
    }
}
