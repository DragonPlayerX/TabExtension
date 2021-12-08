using System;
using System.IO;
using System.Collections.Generic;
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

        public static void Init()
        {
            TabSorting = Category.CreateEntry("TabSorting", false, "TabSorting (config in UserData)");
            TabBackground = Category.CreateEntry("TabBackground", true, "TabBackground");

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
                MelonLogger.Error("Error while loading TabSorting.json: " + e.ToString());
                return null;
            }
        }
    }
}
