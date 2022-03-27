﻿using System.Linq;
using Microsoft.Extensions.Options;
using uSeoToolkit.Umbraco.Common.Core.Services.SettingsService;
using uSeoToolkit.Umbraco.RobotsTxt.Core.Config.Models;

namespace uSeoToolkit.Umbraco.RobotsTxt.Core.Config
{
    public class RobotsTxtConfigurationService : DefaultAppSettingsService<RobotsTxtConfigModel>
    {
        private readonly IOptionsMonitor<RobotsTxtAppSettingsModel> _config;

        public RobotsTxtConfigurationService(IOptionsMonitor<RobotsTxtAppSettingsModel> config)
        {
            _config = config;
        }

        public override RobotsTxtConfigModel GetSettings()
        {
            var settings = _config.CurrentValue;
            return new RobotsTxtConfigModel
            {
                DisabledModules = settings.DisabledModules
            };
        }
    }
}
