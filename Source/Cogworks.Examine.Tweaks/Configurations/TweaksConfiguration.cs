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
            defaultValue: false);

        public static bool IsMediaCustomValueSetBuilderEnabled => AppSettings.Get<bool>(
            key: "Cogworks.Examine.Tweaks.UseMediaCustomValueSetBuilder",
            defaultValue: false);
    }
}