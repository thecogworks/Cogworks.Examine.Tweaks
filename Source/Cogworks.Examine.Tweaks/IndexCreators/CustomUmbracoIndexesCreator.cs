using System;
using System.Collections.Generic;
using Examine;
using Examine.LuceneEngine;
using Lucene.Net.Analysis.Standard;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Services;
using Umbraco.Examine;
using Umbraco.Web.Search;

namespace Cogworks.Examine.Tweaks.IndexCreators
{
    internal class CustomUmbracoIndexesCreator : LuceneIndexCreator, IUmbracoIndexesCreator
    {
        private readonly IProfilingLogger _profilingLogger;
        private readonly ILocalizationService _languageService;
        private readonly IUmbracoIndexConfig _umbracoIndexConfig;

        public CustomUmbracoIndexesCreator(IProfilingLogger profilingLogger,
            ILocalizationService languageService, IUmbracoIndexConfig umbracoIndexConfig)
        {
            _profilingLogger = profilingLogger
                ?? throw new ArgumentNullException(nameof(profilingLogger));

            _languageService = languageService
                ?? throw new ArgumentNullException(nameof(languageService));

            _umbracoIndexConfig = umbracoIndexConfig
                ?? throw new ArgumentNullException(nameof(umbracoIndexConfig));
        }

        /// <summary>
        /// Creates the Umbraco indexes
        /// </summary>
        /// <returns>List of index definition.</returns>
        public override IEnumerable<IIndex> Create()
            => new[]
            {
                CreateInternalIndex(),
                CreateExternalIndex(),
                CreateMemberIndex()
            };

        private IIndex CreateInternalIndex()
            => new CustomUmbracoContentIndex(
                Constants.UmbracoIndexes.InternalIndexName,
                CreateFileSystemLuceneDirectory(Constants.UmbracoIndexes.InternalIndexPath),
                new UmbracoFieldDefinitionCollection(),
                new CultureInvariantWhitespaceAnalyzer(),
                _profilingLogger,
                _languageService,
                _umbracoIndexConfig.GetPublishedContentValueSetValidator());

        private IIndex CreateExternalIndex()
            => new CustomUmbracoContentIndex(
                Constants.UmbracoIndexes.ExternalIndexName,
                CreateFileSystemLuceneDirectory(Constants.UmbracoIndexes.ExternalIndexPath),
                new UmbracoFieldDefinitionCollection(),
                new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30),
                _profilingLogger,
                _languageService,
                _umbracoIndexConfig.GetPublishedContentValueSetValidator());

        private IIndex CreateMemberIndex()
            => new UmbracoMemberIndex(
                Constants.UmbracoIndexes.MembersIndexName,
                new UmbracoFieldDefinitionCollection(),
                CreateFileSystemLuceneDirectory(Constants.UmbracoIndexes.MembersIndexPath),
                new CultureInvariantWhitespaceAnalyzer(),
                _profilingLogger,
                _umbracoIndexConfig.GetMemberValueSetValidator());
    }
}
