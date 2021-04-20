using Cogworks.Essentials.Extensions;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using Umbraco.Examine;

namespace Cogworks.Examine.Tweaks.Configurations
{
    public class CustomIndexConfig : UmbracoIndexConfig, IUmbracoIndexConfig
    {
        private readonly IPublicAccessService _publicAccessService;
        private readonly IScopeProvider _scopeProvider;

        public CustomIndexConfig(IPublicAccessService publicAccessService,
            IScopeProvider scopeProvider) : base(publicAccessService)
        {
            _publicAccessService = publicAccessService;
            _scopeProvider = scopeProvider;
        }

        IContentValueSetValidator IUmbracoIndexConfig.GetContentValueSetValidator()
            => !(!TweaksConfiguration.InternalIncludedItemTypes.HasAny()
                    && !TweaksConfiguration.InternalExcludedItemTypes.HasAny())
                ? new ContentValueSetValidator(
                    publishedValuesOnly: false,
                    supportProtectedContent: true,
                    publicAccessService: _publicAccessService,
                    scopeProvider: _scopeProvider,
                    includeItemTypes: TweaksConfiguration.InternalIncludedItemTypes,
                    excludeItemTypes: TweaksConfiguration.InternalExcludedItemTypes)
                : base.GetContentValueSetValidator();

        IContentValueSetValidator IUmbracoIndexConfig.GetPublishedContentValueSetValidator()
            => !(!TweaksConfiguration.ExternalIncludedItemTypes.HasAny()
                    && !TweaksConfiguration.ExternalExcludedItemTypes.HasAny())
                ? new ContentValueSetValidator(
                    publishedValuesOnly: true,
                    supportProtectedContent: true,
                    publicAccessService: _publicAccessService,
                    scopeProvider: _scopeProvider,
                    includeItemTypes: TweaksConfiguration.ExternalIncludedItemTypes,
                    excludeItemTypes: TweaksConfiguration.ExternalExcludedItemTypes)
                : base.GetPublishedContentValueSetValidator();
    }
}
