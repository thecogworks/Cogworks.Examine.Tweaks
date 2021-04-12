using System;
using System.Collections.Generic;
using System.Linq;
using Examine;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Membership;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using Umbraco.Core.Strings;
using Umbraco.Examine;

namespace Cogworks.Examine.Tweaks.ValueSetBuilders
{
    public class CustomContentValueSetBuilder : ContentValueSetBuilder//BaseValueSetBuilder<IContent>, IContentValueSetBuilder, IPublishedContentValueSetBuilder
    {
        private readonly UrlSegmentProviderCollection _urlSegmentProviders;
        private readonly IUserService _userService;
        private readonly IScopeProvider _scopeProvider;

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
            _userService = userService;
            _scopeProvider = scopeProvider;
        }

        public override IEnumerable<ValueSet> GetValueSets(params IContent[] content)
        {
            return GetValueSetsEnumerable(content, new Dictionary<int, IProfile>(), new Dictionary<int, IProfile>());
        }


        private IEnumerable<ValueSet> GetValueSetsEnumerable(IContent[] content, Dictionary<int, IProfile> creatorIds, Dictionary<int, IProfile> writerIds)
        {
            // TODO: There is a lot of boxing going on here and ultimately all values will be boxed by Lucene anyways
            // but I wonder if there's a way to reduce the boxing that we have to do or if it will matter in the end since
            // Lucene will do it no matter what? One idea was to create a `FieldValue` struct which would contain `object`, `object[]`, `ValueType` and `ValueType[]`
            // references and then each array is an array of `FieldValue[]` and values are assigned accordingly. Not sure if it will make a difference or not.

            foreach (var c in content)
            {
                var isVariant = c.ContentType.VariesByCulture();

                var urlValue = c.GetUrlSegment(_urlSegmentProviders); //Always add invariant urlName
                var values = new Dictionary<string, IEnumerable<object>>
                {
                    {"icon", c.ContentType.Icon?.Yield() ?? Enumerable.Empty<string>()},
                    {UmbracoExamineIndex.PublishedFieldName, new object[] {c.Published ? "y" : "n"}},   //Always add invariant published value
                    {"id", new object[] {c.Id}},
                    {UmbracoExamineIndex.NodeKeyFieldName, new object[] {c.Key}},
                    {"parentID", new object[] {c.Level > 1 ? c.ParentId : -1}},
                    {"level", new object[] {c.Level}},
                    {"sortOrder", new object[] {c.SortOrder}},
                    {"nodeName", (PublishedValuesOnly               //Always add invariant nodeName
                                     ? c.PublishName?.Yield()
                                     : c.Name?.Yield()) ?? Enumerable.Empty<string>()},
                    {"urlName", urlValue?.Yield() ?? Enumerable.Empty<string>()},                  //Always add invariant urlName
                    {"path", c.Path?.Yield() ?? Enumerable.Empty<string>()},
                    {"nodeType", c.ContentType.Id.ToString().Yield() ?? Enumerable.Empty<string>()},
                    {"templateID", new object[] {c.TemplateId ?? 0}},
                    {UmbracoContentIndex.VariesByCultureFieldName, new object[] {"n"}},
                };

                if (isVariant)
                {
                    values[UmbracoContentIndex.VariesByCultureFieldName] = new object[] { "y" };

                    foreach (var culture in c.AvailableCultures)
                    {
                        var variantUrl = c.GetUrlSegment(_urlSegmentProviders, culture);
                        var lowerCulture = culture.ToLowerInvariant();
                        values[$"urlName_{lowerCulture}"] = variantUrl?.Yield() ?? Enumerable.Empty<string>();
                        values[$"nodeName_{lowerCulture}"] = (PublishedValuesOnly
                                                                 ? c.GetPublishName(culture)?.Yield()
                                                                 : c.GetCultureName(culture)?.Yield()) ?? Enumerable.Empty<string>();
                        values[$"{UmbracoExamineIndex.PublishedFieldName}_{lowerCulture}"] = (c.IsCulturePublished(culture) ? "y" : "n").Yield<object>();
                    }
                }

                var vs = new ValueSet(c.Id.ToInvariantString(), IndexTypes.Content, c.ContentType.Alias, values);

                yield return vs;
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
        /// <param name="culture">The culture.</param>
        /// <param name="urlSegmentProviders"></param>
        /// <returns>The url segment.</returns>
        public static string GetUrlSegment(this IContentBase content, System.Collections.Generic.IEnumerable<IUrlSegmentProvider> urlSegmentProviders, string culture = null)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            if (urlSegmentProviders == null) throw new ArgumentNullException(nameof(urlSegmentProviders));

            var url = urlSegmentProviders.Select(p => p.GetUrlSegment(content, culture)).FirstOrDefault(u => u != null);
            url = url ?? new DefaultUrlSegmentProvider().GetUrlSegment(content, culture); // be safe
            return url;
        }
    }
}