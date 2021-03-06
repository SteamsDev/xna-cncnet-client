﻿using ClientCore;
using Rampastring.Tools;
using System;
using System.Collections.Generic;

namespace DTAClient.Domain.Multiplayer
{
    /// <summary>
    /// A multiplayer game mode.
    /// </summary>
    public class GameMode
    {
        const string BASE_INI_PATH = "INI\\Map Code\\";
        const string SPAWN_INI_OPTIONS_SECTION = "ForcedSpawnIniOptions";

        /// <summary>
        /// The internal (INI) name of the game mode.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The user-interface name of the game mode.
        /// </summary>
        public string UIName { get; private set; }

        public bool MultiplayerOnly { get; private set; }

        private string mapCodeININame;

        private string forcedOptionsSection;

        public List<Map> Maps = new List<Map>();

        public List<KeyValuePair<string, bool>> ForcedCheckBoxValues = new List<KeyValuePair<string, bool>>();
        public List<KeyValuePair<string, int>> ForcedDropDownValues = new List<KeyValuePair<string, int>>();

        private List<KeyValuePair<string, string>> ForcedSpawnIniOptions = new List<KeyValuePair<string, string>>();

        public int CoopDifficultyLevel { get; set; }

        public void Initialize()
        {
            IniFile forcedOptionsIni = new IniFile(ProgramConstants.GamePath + ClientConfiguration.Instance.MPMapsIniPath);

            CoopDifficultyLevel = forcedOptionsIni.GetIntValue(Name, "CoopDifficultyLevel", 0);
            UIName = forcedOptionsIni.GetStringValue(Name, "UIName", Name);
            MultiplayerOnly = forcedOptionsIni.GetBooleanValue(Name, "MultiplayerOnly", false);
            forcedOptionsSection = forcedOptionsIni.GetStringValue(Name, "ForcedOptions", string.Empty);
            mapCodeININame = forcedOptionsIni.GetStringValue(Name, "MapCodeININame", Name + ".ini");

            ParseForcedOptions(forcedOptionsIni);

            ParseSpawnIniOptions(forcedOptionsIni);
        }

        private void ParseForcedOptions(IniFile forcedOptionsIni)
        {
            if (string.IsNullOrEmpty(forcedOptionsSection))
                return;

            List<string> keys = forcedOptionsIni.GetSectionKeys(forcedOptionsSection);

            if (keys == null)
                return;

            foreach (string key in keys)
            {
                string value = forcedOptionsIni.GetStringValue(forcedOptionsSection, key, String.Empty);

                int intValue = 0;
                if (Int32.TryParse(value, out intValue))
                {
                    ForcedDropDownValues.Add(new KeyValuePair<string, int>(key, intValue));
                }
                else
                {
                    ForcedCheckBoxValues.Add(new KeyValuePair<string, bool>(key, Conversions.BooleanFromString(value, false)));
                }
            }
        }

        private void ParseSpawnIniOptions(IniFile forcedOptionsIni)
        {
            string section = Name + SPAWN_INI_OPTIONS_SECTION;

            List<string> spawnIniKeys = forcedOptionsIni.GetSectionKeys(section);

            if (spawnIniKeys == null)
                return;

            foreach (string key in spawnIniKeys)
            {
                ForcedSpawnIniOptions.Add(new KeyValuePair<string, string>(key, 
                    forcedOptionsIni.GetStringValue(section, key, String.Empty)));
            }
        }

        public void ApplySpawnIniCode(IniFile spawnIni)
        {
            foreach (KeyValuePair<string, string> key in ForcedSpawnIniOptions)
                spawnIni.SetStringValue("Settings", key.Key, key.Value);
        }

        public IniFile GetMapRulesIniFile()
        {
            return new IniFile(ProgramConstants.GamePath + BASE_INI_PATH + mapCodeININame);
        }
    }
}
