using FileManager.View;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Module
{
    public class SearchModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var region = containerProvider.Resolve<IRegionManager>();
            //region.RegisterViewWithRegion("SearchRegion", typeof(Search));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}
