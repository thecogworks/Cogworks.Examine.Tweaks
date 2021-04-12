using Cogworks.Examine.Tweaks.Configurations;
using Cogworks.Examine.Tweaks.IndexCreators;
using Cogworks.Examine.Tweaks.ValueSetBuilders;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;
using Umbraco.Core.Strings;
using Umbraco.Examine;
using Umbraco.Web.Search;

namespace Cogworks.Examine.Tweaks.Composers
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    [ComposeAfter(typeof(ExamineComposer))]
    internal class CustomExamineComposer : ICoreComposer
    {
        public void Compose(Composition composition)
        {
            if (!TweaksConfiguration.Enabled)
            {
                return;
            }

            if (TweaksConfiguration.IsCustomIndexCreatorEnabled)
            {
                composition.RegisterUnique<IUmbracoIndexesCreator, CustomUmbracoIndexesCreator>();
            }

            if (TweaksConfiguration.IsPublishedContentCustomValueSetBuilderEnabled)
            {
                composition.RegisterUnique<IPublishedContentValueSetBuilder>(factory =>
                    new CustomContentValueSetBuilder(
                        factory.GetInstance<PropertyEditorCollection>(),
                        factory.GetInstance<UrlSegmentProviderCollection>(),
                        factory.GetInstance<IUserService>(),
                        factory.GetInstance<IScopeProvider>(),
                        true));

                composition.RegisterUnique<IContentValueSetBuilder>(factory =>
                    new CustomContentValueSetBuilder(
                        factory.GetInstance<PropertyEditorCollection>(),
                        factory.GetInstance<UrlSegmentProviderCollection>(),
                        factory.GetInstance<IUserService>(),
                        factory.GetInstance<IScopeProvider>(),
                        false));
            }

            if (TweaksConfiguration.IsMediaCustomValueSetBuilderEnabled)
            {
                composition.RegisterUnique<IValueSetBuilder<IMedia>, CustomMediaValueSetBuilder>();
            }
        }
    }
}
