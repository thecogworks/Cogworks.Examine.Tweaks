using System;
using System.Collections.Generic;
using System.Linq;
using Examine;
using Newtonsoft.Json;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.PropertyEditors.ValueConverters;
using Umbraco.Core.Services;
using Umbraco.Core.Strings;
using Umbraco.Examine;

namespace Cogworks.Examine.Tweaks.ValueSetBuilders
{
    public class CustomMediaValueSetBuilder : MediaValueSetBuilder
    {
        private readonly UrlSegmentProviderCollection _urlSegmentProviders;
        private readonly IUserService _userService;
        private readonly ILogger _logger;

        public CustomMediaValueSetBuilder(PropertyEditorCollection propertyEditors,
            UrlSegmentProviderCollection urlSegmentProviders,
            IUserService userService, ILogger logger)
            : base(propertyEditors, urlSegmentProviders, userService, logger)
        {
            _urlSegmentProviders = urlSegmentProviders;
            _userService = userService;
            _logger = logger;
        }

        /// <inheritdoc />
        public override IEnumerable<ValueSet> GetValueSets(params IMedia[] media)
        {
            foreach (var m in media)
            {
                var urlValue = m.GetUrlSegment(_urlSegmentProviders);

                var umbracoFilePath = string.Empty;
                var umbracoFile = string.Empty;

                var umbracoFileSource = m.GetValue<string>(Constants.Conventions.Media.File);

                if (umbracoFileSource.DetectIsJson())
                {
                    ImageCropperValue cropper = null;
                    try
                    {
                        cropper = JsonConvert.DeserializeObject<ImageCropperValue>(
                            m.GetValue<string>(Constants.Conventions.Media.File));
                    }
                    catch (Exception ex)
                    {
                        _logger.Error<MediaValueSetBuilder>(ex, $"Could not Deserialize ImageCropperValue for item with key {m.Key} ");
                    }

                    if (cropper != null)
                    {
                        umbracoFilePath = cropper.Src;
                    }
                }
                else
                {
                    umbracoFilePath = umbracoFileSource;
                }

                if (!string.IsNullOrEmpty(umbracoFilePath))
                {
                    // intentional dummy Uri
                    var uri = new Uri("https://localhost/" + umbracoFilePath);
                    umbracoFile = uri.Segments.Last();
                }

                var values = new Dictionary<string, IEnumerable<object>>
                {
                    {"icon", m.ContentType.Icon?.Yield() ?? Enumerable.Empty<string>()},
                    {"id", new object[] {m.Id}},
                    {UmbracoExamineIndex.NodeKeyFieldName, new object[] {m.Key}},
                    {"parentID", new object[] {m.Level > 1 ? m.ParentId : -1}},
                    {"level", new object[] {m.Level}},
                    {"sortOrder", new object[] {m.SortOrder}},
                    {"nodeName", m.Name?.Yield() ?? Enumerable.Empty<string>()},
                    {"urlName", urlValue?.Yield() ?? Enumerable.Empty<string>()},
                    {"path", m.Path?.Yield() ?? Enumerable.Empty<string>()},
                    {"nodeType", m.ContentType.Id.ToString().Yield() },
                    {UmbracoExamineIndex.UmbracoFileFieldName, umbracoFile.Yield()}
                };

                var vs = new ValueSet(m.Id.ToInvariantString(), IndexTypes.Media, m.ContentType.Alias, values);

                yield return vs;
            }
        }
    }

}
