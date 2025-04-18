﻿using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.PublishedContent;
using SeoToolkit.Umbraco.MetaFields.Core.Common.Converters.EditorConverters;
using SeoToolkit.Umbraco.MetaFields.Core.Interfaces.Converters;
using SeoToolkit.Umbraco.MetaFields.Core.Interfaces.SeoField;

namespace SeoToolkit.Umbraco.MetaFields.Core.Models.SeoFieldEditors
{
    public class SeoFieldPropertyEditor : ISeoFieldEditor
    {
        private readonly string _propertyView;
        public string View => "/App_Plugins/SeoToolkit/MetaFields/Interface/SeoFieldEditors/PropertyEditor/propertyEditor.html";

        private Dictionary<string, object> _config;

        public Dictionary<string, object> Config => _config;

        public IEditorValueConverter ValueConverter { get; }

        public SeoFieldPropertyEditor(string propertyView) : this(propertyView, new TextValueConverter())
        {
        }

        public SeoFieldPropertyEditor(string propertyView, IEditorValueConverter valueConverter)
        {
            _propertyView = propertyView;
            ValueConverter = valueConverter;

            _config = new Dictionary<string, object>
            {
                {"view", _propertyView}
            };
        }

        public void SetExtraInformation(string information)
        {
            _config["extraInformation"] = information;
        }
    }
}
