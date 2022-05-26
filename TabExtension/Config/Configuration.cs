using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MelonLoader;
using MelonLoader.TinyJSON;
using MelonLoader.Preferences;

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
        public static MelonPreferences_Entry<int> TabsPerRow;

        public static Alignment ParsedTabAlignment;

        public enum Alignment
        {
            Left,
            Center,
            Right
        }

        public static void Init()
        {
            TabSorting = Category.CreateEntry("TabSorting", false, "Tab Sorting (config in UserData)");
            TabBackground = Category.CreateEntry("TabBackground", true, "Tab Background");
            TabAlignment = Category.CreateEntry("TabAlignment", nameof(Alignment.Center), "Tab Alignment");
            TabsPerRow = Category.CreateEntry("TabsPerRow", 7, "Tabs Per Row", validator: new IntegerValidator(7, 1, 7));

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
                TabExtensionMod.Logger.Msg(FileName + " was saved.");
            }
            catch (Exception e)
            {
                TabExtensionMod.Logger.Error("Error while saving " + FileName + ": " + e.ToString());
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
                TabExtensionMod.Logger.Error("Error while loading " + FileName + ": " + e.ToString());
                return null;
            }
        }

        private class IntegerValidator : ValueValidator
        {
            public int DefaultValue;
            public int MinValue;
            public int MaxValue;

            public IntegerValidator(int defaultValue, int minValue, int maxValue)
            {
                DefaultValue = defaultValue;
                MinValue = minValue;
                MaxValue = maxValue;
            }

            public override object EnsureValid(object value)
            {
                if (IsValid(value))
                    return value;
                else
                    return DefaultValue;
            }

            public override bool IsValid(object value)
            {
                int v = Convert.ToInt32(value);
                return v >= MinValue && v <= MaxValue;
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
