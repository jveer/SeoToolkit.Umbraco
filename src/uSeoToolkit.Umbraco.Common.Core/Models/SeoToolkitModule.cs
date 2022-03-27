﻿using uSeoToolkit.Umbraco.Common.Core.Enums;

namespace uSeoToolkit.Umbraco.Common.Core.Models
{
    public class SeoToolkitModule
    {
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Icon { get; set; }
        public string Link { get; set; }
        public SeoToolkitModuleStatus Status { get; set; }
    }
}
