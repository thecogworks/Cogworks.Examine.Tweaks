using System;
using System.Collections.Generic;
using System.Linq;
using Examine;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using Umbraco.Core.Strings;
using Umbraco.Examine;

namespace Cogworks.Examine.Tweaks.ValueSetBuilders
{
    public class CustomContentValueSetBuilder : ContentValueSetBuilder
    {
        private readonly UrlSegmentProviderCollection _urlSegmentProviders;

        [Obsolete("Use the other ctor instead")]
        public CustomContentValueSetBuilder(PropertyEditorCollection propertyEditors,
            UrlSegmentProviderCollection urlSegmentProviders,
            IUserService userService,
            bool publishedValuesOnly)
            : this(propertyEditors, urlSegmentProviders, userService, Current.ScopeProvider, publishedValuesOnly)
        {
        }

        public CustomContentValueSetBuilder(PropertyEditorCollection propertyEditors,
            UrlSegmentProviderCollection urlSegmentProviders,
            IUserService userService,
            IScopeProvider scopeProvider,
            bool publishedValuesOnly)
            : base(propertyEditors, urlSegmentProviders, userService, scopeProvider, publishedValuesOnly)
        {
            _urlSegmentProviders = urlSegmentProviders;
        }

        public override IEnumerable<ValueSet> GetValueSets(params IContent[] contents)
            => GetValueSetsEnumerable(contents);

        private IEnumerable<ValueSet> GetValueSetsEnumerable(IEnumerable<IContent> contents)
        {
            foreach (var content in contents)
            {
                var isVariant = content.ContentType.VariesByCulture();

                var urlValue = content.GetUrlSegment(_urlSegmentProviders);
                var values = new Dictionary<string, IEnumerable<object>>
                {
                    {"icon", content.ContentType.Icon?.Yield() ?? Enumerable.Empty<string>()},
                    {UmbracoExamineIndex.PublishedFieldName, new object[] {content.Published ? "y" : "n"}},
                    {"id", new object[] {content.Id}},
                    {UmbracoExamineIndex.NodeKeyFieldName, new object[] {content.Key}},
                    {"parentID", new object[] {content.Level > 1 ? content.ParentId : -1}},
                    {"level", new object[] {content.Level}},
                    {"sortOrder", new object[] {content.SortOrder}},
                    {"nodeName", (PublishedValuesOnly
                                     ? content.PublishName?.Yield()
                                     : content.Name?.Yield()) ?? Enumerable.Empty<string>()},
                    {"urlName", urlValue?.Yield() ?? Enumerable.Empty<string>()},
                    {"path", content.Path?.Yield() ?? Enumerable.Empty<string>()},
                    {"nodeType", content.ContentType.Id.ToString().Yield() ?? Enumerable.Empty<string>()},
                    {"templateID", new object[] {content.TemplateId ?? 0}},
                    {UmbracoContentIndex.VariesByCultureFieldName, new object[] {"n"}},
                };

                if (isVariant)
                {
                    values[UmbracoContentIndex.VariesByCultureFieldName] = new object[] { "y" };

                    foreach (var culture in content.AvailableCultures)
                    {
                        var variantUrl = content.GetUrlSegment(_urlSegmentProviders, culture);

                        var lowerCulture = culture.ToLowerInvariant();
                        values[$"urlName_{lowerCulture}"] = variantUrl?.Yield()
                                                            ?? Enumerable.Empty<string>();

                        values[$"nodeName_{lowerCulture}"] =
                            (PublishedValuesOnly
                                ? content.GetPublishName(culture)?.Yield()
                                : content.GetCultureName(culture)?.Yield())
                            ?? Enumerable.Empty<string>();

                        values[$"{UmbracoExamineIndex.PublishedFieldName}_{lowerCulture}"] = (content.IsCulturePublished(culture) ? "y" : "n").Yield<object>();
                    }
                }

                yield return new ValueSet(
                    content.Id.ToInvariantString(),
                    IndexTypes.Content,
                    content.ContentType.Alias,
                    values);
            }
        }
    }

    /// <summary>
    /// Provides extension methods to IContentBase to get url segments.
    /// </summary>
    internal static class ContentBaseExtensions
    {
        /// <summary>
        /// Gets the url segment for a specified content and culture.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="urlSegmentProviders"></param>
        /// <param name="culture">The culture.</param>
        /// <returns>The url segment.</returns>
        public static string GetUrlSegment(this IContentBase content,
            IEnumerable<IUrlSegmentProvider> urlSegmentProviders, string culture = null)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (urlSegmentProviders == null)
            {
                throw new ArgumentNullException(nameof(urlSegmentProviders));
            }

            var url = urlSegmentProviders
                .Select(p => p.GetUrlSegment(content, culture))
                .FirstOrDefault(u => u != null);

            url ??= new DefaultUrlSegmentProvider().GetUrlSegment(content, culture);

            return url;
        }
    }
}