﻿using System;

namespace uSeoToolkit.Umbraco.ScriptManager.Core.Config.Models
{
    public class ScriptManagerConfigModel
    {
        public ScriptManagerDefinitionConfigModel[] Definitions { get; set; } =
            Array.Empty<ScriptManagerDefinitionConfigModel>();

        public string[] DisabledModules { get; set; } = Array.Empty<string>();
    }
}
