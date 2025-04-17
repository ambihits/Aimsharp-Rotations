using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using AimsharpWow.API;

public class AmbiHits : Rotation
{
    private string manifestUrl = "https://raw.githubusercontent.com/ambihits/Aimsharp-Rotations/main/manifest.json";
    private string rotationUrl = "https://raw.githubusercontent.com/ambihits/Aimsharp-Rotations/main/AmbiHits.cs";
    private string exeUrl = "https://raw.githubusercontent.com/ambihits/Aimsharp-Rotations/main/bin/Aimsharpwow.exe";
    private string dllUrl = "https://raw.githubusercontent.com/ambihits/Aimsharp-Rotations/main/bin/Aimsharpwow.dll";

    private string selectedClass = "";
    private string selectedSpec = "";
    private bool rotationStarted = false;

    public override void LoadSettings()
    {
        Settings.Add(new Setting("Class", new List<string>(new string[] {
            "Death Knight", "Demon Hunter", "Druid", "Hunter", "Mage", "Monk",
            "Paladin", "Priest", "Rogue", "Shaman", "Warlock", "Warrior"
        }), "Shaman"));

        Settings.Add(new Setting("Spec", new List<string>(new string[] {
            "Enhancement", "Elemental", "Restoration"
        }), "Enhancement"));

        Settings.Add(new Setting("Rotation Profile", new List<string>(new string[] {
            "Default", "PvP Aggressive", "Dungeon Cleave", "Raid Sustain"
        }), "Default"));

        Settings.Add(new Setting("Start Rotation", false));
        Settings.Add(new Setting("Stop Rotation", false));

        Settings.Add(new Setting("Delete Logs on Stop", true));

        Settings.Add(new Setting("WoW Path", "C:\\Program Files (x86)\\World of Warcraft\\_retail_"));
        Settings.Add(new Setting("Addon Name", "AmbiHits"));

        Settings.Add(new Setting("Language", new List<string> { "EN", "ES", "FR", "DE", "ZH" }, "EN"));

        // Ability Toggles Example
        Settings.Add(new Setting("Use Stormstrike", true));
        Settings.Add(new Setting("Use Flame Shock", true));
        Settings.Add(new Setting("Use Feral Spirit", true));
    }

    public override void Initialize()
    {
        if (IsBattleNetRunning())
        {
            Aimsharp.PrintMessage("[SECURITY] Battle.net is open. AmbiHits will not load.", Color.Red);
            Environment.Exit(0);
            return;
        }

        selectedClass = GetDropDown("Class");
        selectedSpec = GetDropDown("Spec");
        string profile = GetDropDown("Rotation Profile");
        bool start = GetCheckBox("Start Rotation");
        bool stop = GetCheckBox("Stop Rotation");

        if (stop)
        {
            rotationStarted = false;
            Aimsharp.PrintMessage("⛔ Rotation stopped. You may now load another.", Color.Orange);

            if (GetCheckBox("Delete Logs on Stop"))
            {
                Aimsharp.PrintMessage("🧹 Do you want to clear the logs? (Y/N)", Color.Yellow);
                // Can't block for user input in real-time; logs will be cleared automatically
                File.WriteAllText("logs.txt", "");
                Aimsharp.PrintMessage("✅ Logs cleared.", Color.Green);
            }

            return;
        }

        if (start)
        {
            Aimsharp.PrintMessage($"✅ Loading {selectedClass} - {selectedSpec} ({profile})", Color.Cyan);
            rotationStarted = true;

            // In a real system, you'd load logic here based on class/spec
            Spellbook.Add("Stormstrike");
            Spellbook.Add("Flame Shock");
            Spellbook.Add("Feral Spirit");
        }
    }

    public override bool CombatTick()
    {
        if (!rotationStarted)
            return false;

        if (!GetCheckBox("Use Stormstrike") && !GetCheckBox("Use Flame Shock"))
            return false;

        if (GetCheckBox("Use Stormstrike") && CanCast("Stormstrike"))
        {
            Cast("Stormstrike");
            return true;
        }

        if (GetCheckBox("Use Flame Shock") && CanCast("Flame Shock") && !HasDebuff("Flame Shock", "target"))
        {
            Cast("Flame Shock");
            return true;
        }

        return false;
    }

    public override bool OutOfCombatTick()
    {
        return false;
    }

    public override bool MountedTick()
    {
        return false;
    }

    public override void CleanUp()
    {
        return;
    }

    private bool IsBattleNetRunning()
    {
        try
        {
            return Process.GetProcessesByName("Battle.net").Length > 0;
        }
        catch
        {
            return false;
        }
    }
}
