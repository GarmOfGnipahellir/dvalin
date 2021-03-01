using UnityEngine;
using UnityEditor.SettingsManagement;

namespace Dvalin.Editor
{
    public class ModInfo
    {
        [UserSettingAttribute("Project Settings", "Icon")]
        public static UserSetting<Texture2D> icon = DvalinSettings.ProjectSetting("ModIcon", (Texture2D)null);

        [UserSettingAttribute("Project Settings", "Author")]
        public static UserSetting<string> author = DvalinSettings.ProjectSetting("ModAuthor", Application.companyName);

        [UserSettingAttribute("Project Settings", "Name")]
        public static UserSetting<string> name = DvalinSettings.ProjectSetting("ModName", Application.productName);

        [UserSettingAttribute("Project Settings", "Version")]
        public static UserSetting<string> version = DvalinSettings.ProjectSetting("ModVersion", Application.version);

        [UserSettingAttribute("Project Settings", "Licence")]
        public static UserSetting<string> licence = DvalinSettings.ProjectSetting("ModLicence", "");

        [UserSettingAttribute("Project Settings", "Website URL")]
        public static UserSetting<string> websiteUrl = DvalinSettings.ProjectSetting("ModWebsiteURL", "");

        [UserSettingAttribute("Project Settings", "Description")]
        public static UserSetting<string> description = DvalinSettings.ProjectSetting("ModDescription", "");

        [UserSettingAttribute("Project Settings", "Readme")]
        public static UserSetting<TextAsset> readme = DvalinSettings.ProjectSetting("ModReadme", (TextAsset)null);
    }
}