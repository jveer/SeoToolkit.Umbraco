﻿using System;

namespace SeoToolkit.Umbraco.SiteAudit.Core.Models.Config
{
    public class SiteAuditConfigModel
    {
        public int MinimumDelayBetweenRequest { get; set; } = 1000;
        public bool AllowMinimumDelayBetweenRequestSetting { get; set; } = false;

        public SiteAuditCheckConfigModel[] Checks { get; set; } = Array.Empty<SiteAuditCheckConfigModel>();
    }
}
