using System.Collections.Generic;
using Cogworks.Essentials.Extensions;
using Cogworks.Umbraco.Essentials.Constants;
using Cogworks.Umbraco.Essentials.Helpers;

namespace Cogworks.Examine.Tweaks.Configurations
{
    public static class TweaksConfiguration
    {
        public static bool Enabled => AppSettings.Get<bool>(
            key: "Cogworks.Examine.Tweaks.Enabled",
            defaultValue: false);

        public static bool IsPublishedContentCustomValueSetBuilderEnabled => AppSettings.Get<bool>(
            key: "Cogworks.Examine.Tweaks.UsePublishedContentCustomValueSetBuilder",
            defaultValue: true);

        public static bool IsContentCustomValueSetBuilderEnabled => AppSettings.Get<bool>(
            key: "Cogworks.Examine.Tweaks.UseContentCustomValueSetBuilder",
            defaultValue: true);

        public static bool IsMediaCustomValueSetBuilderEnabled => AppSettings.Get<bool>(
            key: "Cogworks.Examine.Tweaks.UseMediaCustomValueSetBuilder",
            defaultValue: true);

        public static bool IsInternalIndexDisabled => AppSettings.Get<bool>(
            key: "Cogworks.Examine.Tweaks.InternalIndexDisabled",
            defaultValue: false);

        public static bool IsExternalIndexDisabled => AppSettings.Get<bool>(
            key: "Cogworks.Examine.Tweaks.ExternalIndexDisabled",
            defaultValue: false);

        public static IEnumerable<string> InternalIncludedItemTypes => AppSettings
            .Get<string>("Cogworks.Examine.Tweaks.InternalIncludedItemTypes")
            ?.SplitToList(StringConstants.Separators.Comma);

        public static IEnumerable<string> InternalExcludedItemTypes => AppSettings
            .Get<string>("Cogworks.Examine.Tweaks.InternalExcludedItemTypes")
            ?.SplitToList(StringConstants.Separators.Comma);

        public static IEnumerable<string> ExternalIncludedItemTypes => AppSettings
            .Get<string>("Cogworks.Examine.Tweaks.ExternalIncludedItemTypes")
            ?.SplitToList(StringConstants.Separators.Comma);

        public static IEnumerable<string> ExternalExcludedItemTypes => AppSettings
            .Get<string>("Cogworks.Examine.Tweaks.ExternalExcludedItemTypes")
            ?.SplitToList(StringConstants.Separators.Comma);
    }
}