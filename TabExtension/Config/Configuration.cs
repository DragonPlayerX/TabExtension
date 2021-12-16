using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MelonLoader;
using MelonLoader.TinyJSON;

namespace TabExtension.Config
{
    public static class Configuration
    {

        private static readonly MelonPreferences_Category Category = MelonPreferences.CreateCategory("TabExtension", "Tab Extension");
        private static readonly string Path = "UserData\\TabExtension\\";
        private static readonly string FileName = "TabSorting.json";

        public static MelonPreferences_Entry<bool> TabSorting;
        public static MelonPreferences_Entry<bool> TabBackground;
        public static MelonPreferences_Entry<string> TabAlignment;

        public static Alignment ParsedTabAlignment;

        public enum Alignment
        {
            Left,
            Center,
            Right
        }

        public static void Init()
        {
            TabSorting = Category.CreateEntry("TabSorting", false, "TabSorting (config in UserData)");
            TabBackground = Category.CreateEntry("TabBackground", true, "TabBackground");
            TabAlignment = Category.CreateEntry("TabAlignment", nameof(Alignment.Center), "TabAlignment");

            Action<string> parseAlignmentAction = new Action<string>(value =>
            {
                if (Enum.TryParse(value, true, out Alignment alignment))
                    ParsedTabAlignment = alignment;
            });

            TabAlignment.OnValueChanged += new Action<string, string>((oldValue, newValue) => parseAlignmentAction.Invoke(newValue));
            parseAlignmentAction.Invoke(TabAlignment.Value);

            if (MelonHandler.Mods.Any(mod => mod.Info.Name.Equals("UI Expansion Kit")))
                UIXIntegration.InitUIX();

            if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);
        }

        public static void Save(Dictionary<string, int> tabSorting)
        {
            try
            {
                File.WriteAllText(Path + FileName, Encoder.Encode(tabSorting, EncodeOptions.PrettyPrint));
                MelonLogger.Msg(FileName + " was saved.");
            }
            catch (Exception e)
            {
                MelonLogger.Error("Error while saving " + FileName + ": " + e.ToString());
            }
        }

        public static Dictionary<string, int> Load()
        {
            if (!File.Exists(Path + FileName)) return null;

            try
            {
                return Decoder.Decode(File.ReadAllText("UserData/TabExtension/TabSorting.json")).Make<Dictionary<string, int>>();
            }
            catch (Exception e)
            {
                MelonLogger.Error("Error while loading " + FileName + ": " + e.ToString());
                return null;
            }
        }

        internal class UIXIntegration
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            public static void InitUIX()
            {
                UIExpansionKit.API.ExpansionKitApi.RegisterSettingAsStringEnum(Category.Identifier, TabAlignment.Identifier, new List<(string, string)>()
                {
                    (nameof(Alignment.Left), "Left"),
                    (nameof(Alignment.Center), "Center"),
                    (nameof(Alignment.Right), "Right")
                });
            }
        }
    }
}
