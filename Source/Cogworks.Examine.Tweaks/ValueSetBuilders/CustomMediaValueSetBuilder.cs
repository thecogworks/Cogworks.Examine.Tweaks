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
    internal class CustomMediaValueSetBuilder : MediaValueSetBuilder
    {
        private readonly UrlSegmentProviderCollection _urlSegmentProviders;
        private readonly ILogger _logger;

        public CustomMediaValueSetBuilder(PropertyEditorCollection propertyEditors,
            UrlSegmentProviderCollection urlSegmentProviders,
            IUserService userService, ILogger logger)
            : base(propertyEditors, urlSegmentProviders, userService, logger)
        {
            _urlSegmentProviders = urlSegmentProviders;
            _logger = logger;
        }

        /// <inheritdoc />
        public override IEnumerable<ValueSet> GetValueSets(params IMedia[] medias)
        {
            foreach (var media in medias)
            {
                var urlValue = media.GetUrlSegment(_urlSegmentProviders);

                var umbracoFilePath = string.Empty;
                var umbracoFile = string.Empty;

                var umbracoFileSource = media.GetValue<string>(Constants.Conventions.Media.File);

                if (umbracoFileSource.DetectIsJson())
                {
                    ImageCropperValue cropper = null;

                    try
                    {
                        cropper = JsonConvert.DeserializeObject<ImageCropperValue>(
                            media.GetValue<string>(Constants.Conventions.Media.File));
                    }
                    catch (Exception ex)
                    {
                        _logger.Error<MediaValueSetBuilder>(ex, $"Could not Deserialize ImageCropperValue for item with key {media.Key} ");
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
                    {"icon", media.ContentType.Icon?.Yield() ?? Enumerable.Empty<string>()},
                    {"id", new object[] {media.Id}},
                    {UmbracoExamineIndex.NodeKeyFieldName, new object[] {media.Key}},
                    {"parentID", new object[] {media.Level > 1 ? media.ParentId : -1}},
                    {"level", new object[] {media.Level}},
                    {"sortOrder", new object[] {media.SortOrder}},
                    {"nodeName", media.Name?.Yield() ?? Enumerable.Empty<string>()},
                    {"urlName", urlValue?.Yield() ?? Enumerable.Empty<string>()},
                    {"path", media.Path?.Yield() ?? Enumerable.Empty<string>()},
                    {"nodeType", media.ContentType.Id.ToString().Yield() },
                    {UmbracoExamineIndex.UmbracoFileFieldName, umbracoFile.Yield()}
                };

                yield return new ValueSet(
                    media.Id.ToInvariantString(),
                    IndexTypes.Media,
                    media.ContentType.Alias,
                    values);
            }
        }
    }

}
