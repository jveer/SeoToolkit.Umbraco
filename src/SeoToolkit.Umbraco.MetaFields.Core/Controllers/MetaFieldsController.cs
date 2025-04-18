﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using SeoToolkit.Umbraco.Common.Core.Services.SeoSettingsService;
using SeoToolkit.Umbraco.MetaFields.Core.Collections;
using SeoToolkit.Umbraco.MetaFields.Core.Interfaces;
using SeoToolkit.Umbraco.MetaFields.Core.Interfaces.Services;
using SeoToolkit.Umbraco.MetaFields.Core.Models.SeoField.ViewModels;
using SeoToolkit.Umbraco.MetaFields.Core.Models.SeoFieldPreviewers;
using SeoToolkit.Umbraco.MetaFields.Core.Models.SeoSettings.PostModels;
using SeoToolkit.Umbraco.MetaFields.Core.Models.SeoSettings.ViewModels;
using SeoToolkit.Umbraco.MetaFields.Core.Services.DocumentTypeSettings;

namespace SeoToolkit.Umbraco.MetaFields.Core.Controllers
{
    [PluginController("SeoToolkit")]
    public class MetaFieldsController : UmbracoAuthorizedApiController
    {
        private readonly IMetaFieldsService _seoService;
        private readonly IMetaFieldsSettingsService _documentTypeSettingsService;
        private readonly SeoFieldCollection _fieldCollection;
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IMetaFieldsValueService _seoValueService;
        private readonly ILogger<MetaFieldsController> _logger;
        private readonly IVariationContextAccessor _variationContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly ISeoSettingsService _seoSettingsService;

        public MetaFieldsController(IMetaFieldsService seoService,
            IMetaFieldsSettingsService documentTypeSettingsService,
            SeoFieldCollection fieldCollection,
            IUmbracoContextFactory umbracoContextFactory,
            IMetaFieldsValueService seoValueService,
            ILogger<MetaFieldsController> logger,
            IVariationContextAccessor variationContextAccessor,
            ILocalizationService localizationService,
            ISeoSettingsService seoSettingsService)
        {
            _seoService = seoService;
            _documentTypeSettingsService = documentTypeSettingsService;
            _fieldCollection = fieldCollection;
            _umbracoContextFactory = umbracoContextFactory;
            _seoValueService = seoValueService;
            _logger = logger;
            _variationContextAccessor = variationContextAccessor;
            _localizationService = localizationService;
            _seoSettingsService = seoSettingsService;
        }

        [HttpGet]
        public IActionResult Get(int nodeId, string culture)
        {
            EnsureLanguage(culture);

            using var ctx = _umbracoContextFactory.EnsureUmbracoContext();
            var content = ctx.UmbracoContext.Content.GetById(true, nodeId);
            if (content is null)
            {
                _logger.LogInformation("Could not find content by id {0}", nodeId);
                return NotFound();
            }

            var metaTags = _seoService.Get(content);
            var userValues = _seoValueService.GetUserValues(nodeId);

            return new JsonResult(new MetaFieldsSettingsViewModel
            {
                Fields = metaTags.Fields.Select(it =>
                {
                    var (key, value) = it;
                    var userValue = userValues.ContainsKey(key.Alias)
                        ? key.EditEditor.ValueConverter.ConvertObjectToEditorValue(key.EditEditor.ValueConverter.ConvertDatabaseToObject(userValues[key.Alias]))
                        : null;
                    return new SeoSettingsFieldViewModel
                    {
                        Alias = key.Alias,
                        Title = key.Title,
                        Description = key.Description,
                        Value = value?.ToString(),
                        UserValue = userValue,
                        EditView = key.EditEditor.View,
                        EditConfig = key.EditEditor.Config
                    };
                }).ToArray(),
                Previewers = new[] { new FieldPreviewerViewModel(new BaseTagsFieldPreviewer()) }
            });
        }

        [HttpPost]
        public IActionResult Save(MetaFieldsSettingsPostViewModel postModel)
        {
            var settings = _documentTypeSettingsService.Get(postModel.ContentTypeId);
            
            if (!_seoSettingsService.IsEnabled(postModel.ContentTypeId))
                return BadRequest("SEO settings are turned off for this node!");

            EnsureLanguage(postModel.Culture);
            var isDirty = false;
            var values = new Dictionary<string, object>();
            foreach (var (seoField, _) in settings.Fields)
            {
                if (!postModel.UserValues.ContainsKey(seoField.Alias))
                {
                    values.Add(seoField.Alias, null);
                    continue;
                }

                var userValue = postModel.UserValues[seoField.Alias];

                values.Add(seoField.Alias, seoField.EditEditor.ValueConverter.ConvertEditorToDatabaseValue(userValue));
                isDirty = true;
            }
            if (isDirty)
                _seoValueService.AddValues(postModel.NodeId, values);

            return Get(postModel.NodeId, postModel.Culture);
        }

        private void EnsureLanguage(string culture)
        {
            if (!string.IsNullOrWhiteSpace(culture))
                _variationContextAccessor.VariationContext = new VariationContext(culture);
            else
                _variationContextAccessor.VariationContext = new VariationContext(_localizationService.GetDefaultLanguageIsoCode());
        }
    }
}
