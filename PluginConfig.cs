using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PluginConfig.API;
using PluginConfig.API.Decorators;
using PluginConfig.API.Fields;
using PluginConfig.API.Functionals;
using UnityEngine;

namespace UltraCooldownInfo;

public enum SideEnum{Right, Top, Left, Bottom}
public enum ChargeBarTypeEnum {Classic, WeaponImageSplit, WeaponImageGradient}
public enum FlashTypeEnum {LinearFade, NoFade}

public class UltraCooldownInfoWeapon
{
    //charge bar
    public ChargeBarTypeEnum chargeBarType;
    public float xPos = 0;
    public float yPos = 0;
    public float width = 0;
    public float height = 0;
    public bool enabled = false;
    public bool iconEnabled = false;
    public int iconDistance = 0;
    public bool flipped = false;
    public Color chargeBarIconColor = new Color(1.0f,1.0f,1.0f);
    public Color backgroundColor = new Color(1.0f, 1.0f, 1.0f);
    public float backgroundOpacity = 1.0f;
    public float opacity = 1.0f;
    public Color remainingColor = new Color(1.0f, 1.0f, 1.0f);
    public Color chargingColor = new Color(1.0f, 1.0f, 1.0f);
    public Color chargedColor = new Color(1.0f, 1.0f, 1.0f);
    public bool usingChargeColors = true;
    public bool usingChargeChargedColorEnabled = true;
    public bool chargeColors = true;
    public Color usingChargeColor = new Color(1.0f, 1.0f, 1.0f);
    public Color usingChargeChargedColor = new Color(1.0f, 1.0f, 1.0f);
    public int borderThickness = 0;
    public int numberDivisions = 1;
    public int divisionWidth = 1;

    public SideEnum iconSide;

    //sound
    public float volumeMult = 1.0f;
    public string filePathCharge;
    public string filePathFractionCharge;
    public string filePathUsingCharge;
    public bool soundEnabled = false;
    public bool soundEnabledWhileHeld = false;
    public int soundDivisions = 1;

    //icon flash
    public float xPosFlash = 0;
    public float yPosFlash = 0;
    public float widthFlash = 0;
    public float heightFlash = 0;
    public float lengthFlash = 0;
    public bool iconFlashEnabled = false;
    public bool iconFlipped = false;
    public Color colorFlash = Color.white;
    public FlashTypeEnum flashType = FlashTypeEnum.LinearFade;
    public int numberFlashes;
    public float lengthMiniFlash = 0;

    
    //data
    public GameObject weapon;
    public float chargeAmount = 1f;
    public float usingChargeAmount = -1f;
    public string name = "UltraCooldownInfo - YOU SHOULD NEVER SEE THIS";

    public UltraCooldownInfoWeapon(bool usingChargeColors, bool chargeColors, string name)
    {
        this.usingChargeColors = usingChargeColors;
        this.chargeColors = chargeColors;
        this.name = name;
    }
}

public class PluginConfig
{
    public enum KeyEnum
    {
        None, Backspace, Tab, Escape, Space, UpArrow, DownArrow, RightArrow, LeftArrow, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, 
        Alpha1, Alpha2, Alpha3, Alpha4, Alpha5, Alpha6, Alpha7, Alpha8, Alpha9, Alpha0, CapsLock,
        RightShift, LeftShift, RightControl, LeftControl, RightAlt, LeftAlt, Mouse1, Mouse2, Mouse3, Mouse4, Mouse5, Mouse6, Mouse7, //following is courtesy of tetriscat
        BackQuote, EqualsSign, Minus, LeftBracket, RightBracket, Semicolon, Quote, Comma, Period, Slash, Backslash, 
		Numlock, KeypadDivide, KeypadMultiply, KeypadMinus, KeypadPlus, KeypadEnter, KeypadPeriod, 
		Keypad0, Keypad1, Keypad2, Keypad3, Keypad4, Keypad5, Keypad6, Keypad7, Keypad8, Keypad9, 
		Home, End, PageUp, PageDown, Enter, 
		F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12, F13, F14, F15
    }
    public static KeyCode convertKeyEnumToKeyCode(KeyEnum value)
    {
        KeyCode code = KeyCode.None;
        if (value.Equals(KeyEnum.Backspace)) {code = KeyCode.Backspace;}
        else if (value.Equals(KeyEnum.Tab)) {code = KeyCode.Tab;}
        else if (value.Equals(KeyEnum.Escape)) {code = KeyCode.Escape;}
        else if (value.Equals(KeyEnum.Space)) {code = KeyCode.Space;}
        else if (value.Equals(KeyEnum.UpArrow)) {code = KeyCode.UpArrow;}
        else if (value.Equals(KeyEnum.DownArrow)) {code = KeyCode.DownArrow;}
        else if (value.Equals(KeyEnum.RightArrow)) {code = KeyCode.RightArrow;}
        else if (value.Equals(KeyEnum.LeftArrow)) {code = KeyCode.LeftArrow;}
        else if (value.Equals(KeyEnum.A)) {code = KeyCode.A;}
        else if (value.Equals(KeyEnum.B)) {code = KeyCode.B;}
        else if (value.Equals(KeyEnum.C)) {code = KeyCode.C;}
        else if (value.Equals(KeyEnum.D)) {code = KeyCode.D;}
        else if (value.Equals(KeyEnum.E)) {code = KeyCode.E;}
        else if (value.Equals(KeyEnum.F)) {code = KeyCode.F;}
        else if (value.Equals(KeyEnum.G)) {code = KeyCode.G;}
        else if (value.Equals(KeyEnum.H)) {code = KeyCode.H;}
        else if (value.Equals(KeyEnum.I)) {code = KeyCode.I;}
        else if (value.Equals(KeyEnum.J)) {code = KeyCode.J;}
        else if (value.Equals(KeyEnum.K)) {code = KeyCode.K;}
        else if (value.Equals(KeyEnum.L)) {code = KeyCode.L;}
        else if (value.Equals(KeyEnum.M)) {code = KeyCode.M;}
        else if (value.Equals(KeyEnum.N)) {code = KeyCode.N;}
        else if (value.Equals(KeyEnum.O)) {code = KeyCode.O;}
        else if (value.Equals(KeyEnum.P)) {code = KeyCode.P;}
        else if (value.Equals(KeyEnum.Q)) {code = KeyCode.Q;}
        else if (value.Equals(KeyEnum.R)) {code = KeyCode.R;}
        else if (value.Equals(KeyEnum.S)) {code = KeyCode.S;}
        else if (value.Equals(KeyEnum.T)) {code = KeyCode.T;}
        else if (value.Equals(KeyEnum.U)) {code = KeyCode.U;}
        else if (value.Equals(KeyEnum.V)) {code = KeyCode.V;}
        else if (value.Equals(KeyEnum.W)) {code = KeyCode.W;}
        else if (value.Equals(KeyEnum.X)) {code = KeyCode.X;}
        else if (value.Equals(KeyEnum.Y)) {code = KeyCode.Y;}
        else if (value.Equals(KeyEnum.Z)) {code = KeyCode.Z;}
        else if (value.Equals(KeyEnum.Alpha1)) {code = KeyCode.Alpha1;}
        else if (value.Equals(KeyEnum.Alpha2)) {code = KeyCode.Alpha2;}
        else if (value.Equals(KeyEnum.Alpha3)) {code = KeyCode.Alpha3;}
        else if (value.Equals(KeyEnum.Alpha4)) {code = KeyCode.Alpha4;}
        else if (value.Equals(KeyEnum.Alpha5)) {code = KeyCode.Alpha5;}
        else if (value.Equals(KeyEnum.Alpha6)) {code = KeyCode.Alpha6;}
        else if (value.Equals(KeyEnum.Alpha7)) {code = KeyCode.Alpha7;}
        else if (value.Equals(KeyEnum.Alpha8)) {code = KeyCode.Alpha8;}
        else if (value.Equals(KeyEnum.Alpha9)) {code = KeyCode.Alpha9;}
        else if (value.Equals(KeyEnum.Alpha0)) {code = KeyCode.Alpha0;}
        else if (value.Equals(KeyEnum.CapsLock)) {code = KeyCode.CapsLock;}
        else if (value.Equals(KeyEnum.RightShift)) {code = KeyCode.RightShift;}
        else if (value.Equals(KeyEnum.LeftShift)) {code = KeyCode.LeftShift;}
        else if (value.Equals(KeyEnum.RightControl)) {code = KeyCode.RightControl;}
        else if (value.Equals(KeyEnum.LeftControl)) {code = KeyCode.LeftControl;}
        else if (value.Equals(KeyEnum.RightAlt)) {code = KeyCode.RightAlt;}
        else if (value.Equals(KeyEnum.LeftAlt)) {code = KeyCode.LeftAlt;}
        else if (value.Equals(KeyEnum.Mouse1)) {code = KeyCode.Mouse0;} //these dont line up
        else if (value.Equals(KeyEnum.Mouse2)) {code = KeyCode.Mouse1;}
        else if (value.Equals(KeyEnum.Mouse3)) {code = KeyCode.Mouse2;}
        else if (value.Equals(KeyEnum.Mouse4)) {code = KeyCode.Mouse3;}
        else if (value.Equals(KeyEnum.Mouse5)) {code = KeyCode.Mouse4;}
        else if (value.Equals(KeyEnum.Mouse6)) {code = KeyCode.Mouse5;}
        else if (value.Equals(KeyEnum.Mouse7)) {code = KeyCode.Mouse6;}
        
        else if(value.Equals(KeyEnum.BackQuote)) {return KeyCode.BackQuote;} 
        else if(value.Equals(KeyEnum.EqualsSign)) {return KeyCode.Equals;} 
        else if(value.Equals(KeyEnum.Minus)) {return KeyCode.Minus;} 
        else if(value.Equals(KeyEnum.LeftBracket)) {return KeyCode.LeftBracket;} 
        else if(value.Equals(KeyEnum.RightBracket)) {return KeyCode.RightBracket;} 
        else if(value.Equals(KeyEnum.Semicolon)) {return KeyCode.Semicolon;} 
        else if(value.Equals(KeyEnum.Quote)) {return KeyCode.Quote;} 
        else if(value.Equals(KeyEnum.Comma)) {return KeyCode.Comma;} 
        else if(value.Equals(KeyEnum.Period)) {return KeyCode.Period;} 
        else if(value.Equals(KeyEnum.Slash)) {return KeyCode.Slash;} 
        else if(value.Equals(KeyEnum.Backslash)) {return KeyCode.Backslash;} 
        else if(value.Equals(KeyEnum.Numlock)) {return KeyCode.Numlock;} 
        else if(value.Equals(KeyEnum.KeypadDivide)) {return KeyCode.KeypadDivide;} 
        else if(value.Equals(KeyEnum.KeypadMultiply)) {return KeyCode.KeypadMultiply;} 
        else if(value.Equals(KeyEnum.KeypadMinus)) {return KeyCode.KeypadMinus;} 
        else if(value.Equals(KeyEnum.KeypadPlus)) {return KeyCode.KeypadPlus;} 
        else if(value.Equals(KeyEnum.KeypadEnter)) {return KeyCode.KeypadEnter;} 
        else if(value.Equals(KeyEnum.KeypadPeriod)) {return KeyCode.KeypadPeriod;} 
        else if(value.Equals(KeyEnum.Keypad0)) {return KeyCode.Keypad0;} 
        else if(value.Equals(KeyEnum.Keypad1)) {return KeyCode.Keypad1;} 
        else if(value.Equals(KeyEnum.Keypad2)) {return KeyCode.Keypad2;} 
        else if(value.Equals(KeyEnum.Keypad3)) {return KeyCode.Keypad3;} 
        else if(value.Equals(KeyEnum.Keypad4)) {return KeyCode.Keypad4;} 
        else if(value.Equals(KeyEnum.Keypad5)) {return KeyCode.Keypad5;} 
        else if(value.Equals(KeyEnum.Keypad6)) {return KeyCode.Keypad6;} 
        else if(value.Equals(KeyEnum.Keypad7)) {return KeyCode.Keypad7;} 
        else if(value.Equals(KeyEnum.Keypad8)) {return KeyCode.Keypad8;} 
        else if(value.Equals(KeyEnum.Keypad9)) {return KeyCode.Keypad9;} 
        else if(value.Equals(KeyEnum.Home)) {return KeyCode.Home;} 
        else if(value.Equals(KeyEnum.End)) {return KeyCode.End;} 
        else if(value.Equals(KeyEnum.PageUp)) {return KeyCode.PageUp;} 
        else if(value.Equals(KeyEnum.PageDown)) {return KeyCode.PageDown;} 
        else if(value.Equals(KeyEnum.Enter)) {return KeyCode.Return;} 
        else if(value.Equals(KeyEnum.F1)) {return KeyCode.F1;} 
        else if(value.Equals(KeyEnum.F2)) {return KeyCode.F2;}
        else if(value.Equals(KeyEnum.F3)) {return KeyCode.F3;} 
        else if(value.Equals(KeyEnum.F4)) {return KeyCode.F4;} 
        else if(value.Equals(KeyEnum.F5)) {return KeyCode.F5;} 
        else if(value.Equals(KeyEnum.F6)) {return KeyCode.F6;} 
        else if(value.Equals(KeyEnum.F7)) {return KeyCode.F7;} 
        else if(value.Equals(KeyEnum.F8)) {return KeyCode.F8;} 
        else if(value.Equals(KeyEnum.F9)) {return KeyCode.F9;} 
        else if(value.Equals(KeyEnum.F10)) {return KeyCode.F10;} 
        else if(value.Equals(KeyEnum.F11)) {return KeyCode.F11;} 
        else if(value.Equals(KeyEnum.F12)) {return KeyCode.F12;} 
        else if(value.Equals(KeyEnum.F13)) {return KeyCode.F13;} 
        else if(value.Equals(KeyEnum.F14)) {return KeyCode.F14;}
        else if(value.Equals(KeyEnum.F15)) {return KeyCode.F15;}
        return code;
    }

    //call for each EnumField (formats display names for some enums) 
    //courtesy of tetriscat
	private static void SetDisplayNames(EnumField<KeyEnum> field) 
    {
		field.SetEnumDisplayName(KeyEnum.Minus, "-");
		field.SetEnumDisplayName(KeyEnum.EqualsSign, "=");
		field.SetEnumDisplayName(KeyEnum.LeftBracket, "[");
		field.SetEnumDisplayName(KeyEnum.RightBracket, "]");
		field.SetEnumDisplayName(KeyEnum.CapsLock, "Caps Lock");
		field.SetEnumDisplayName(KeyEnum.LeftShift, "Left Shift");
		field.SetEnumDisplayName(KeyEnum.RightShift, "Right Shift");
		field.SetEnumDisplayName(KeyEnum.LeftControl, "Left Control");
		field.SetEnumDisplayName(KeyEnum.RightControl, "Right Control");
		field.SetEnumDisplayName(KeyEnum.LeftAlt, "Left Alt");
		field.SetEnumDisplayName(KeyEnum.RightAlt, "Right Alt");
		field.SetEnumDisplayName(KeyEnum.BackQuote, "`");
		field.SetEnumDisplayName(KeyEnum.Quote, "'");
		field.SetEnumDisplayName(KeyEnum.Semicolon, ";");
		field.SetEnumDisplayName(KeyEnum.Slash, "/");
		field.SetEnumDisplayName(KeyEnum.Backslash, "\\");
		field.SetEnumDisplayName(KeyEnum.Keypad0, "Keypad 0");
		field.SetEnumDisplayName(KeyEnum.Keypad1, "Keypad 1");
		field.SetEnumDisplayName(KeyEnum.Keypad2, "Keypad 2");
		field.SetEnumDisplayName(KeyEnum.Keypad3, "Keypad 3");
		field.SetEnumDisplayName(KeyEnum.Keypad4, "Keypad 4");
		field.SetEnumDisplayName(KeyEnum.Keypad5, "Keypad 5");
		field.SetEnumDisplayName(KeyEnum.Keypad6, "Keypad 6");
		field.SetEnumDisplayName(KeyEnum.Keypad7, "Keypad 7");
		field.SetEnumDisplayName(KeyEnum.Keypad8, "Keypad 8");
		field.SetEnumDisplayName(KeyEnum.Keypad9, "Keypad 9");
		field.SetEnumDisplayName(KeyEnum.KeypadDivide, "Keypad /");
		field.SetEnumDisplayName(KeyEnum.KeypadMultiply, "Keypad *");
		field.SetEnumDisplayName(KeyEnum.KeypadPlus, "Keypad Plus");
		field.SetEnumDisplayName(KeyEnum.KeypadMinus, "Keypad Minus");
		field.SetEnumDisplayName(KeyEnum.KeypadEnter, "Keypad Enter");
		field.SetEnumDisplayName(KeyEnum.KeypadPeriod, "Keypad .");
		field.SetEnumDisplayName(KeyEnum.Period, ".");
		field.SetEnumDisplayName(KeyEnum.Comma, ",");
		field.SetEnumDisplayName(KeyEnum.PageUp, "Page Up");
		field.SetEnumDisplayName(KeyEnum.PageDown, "Page Down");
		field.SetEnumDisplayName(KeyEnum.UpArrow, "Up Arrow");
		field.SetEnumDisplayName(KeyEnum.DownArrow, "Down Arrow");
		field.SetEnumDisplayName(KeyEnum.LeftArrow, "Left Arrow");
		field.SetEnumDisplayName(KeyEnum.RightArrow, "Right Arrow");
		field.SetEnumDisplayName(KeyEnum.Alpha0, "0");
		field.SetEnumDisplayName(KeyEnum.Alpha1, "1");
		field.SetEnumDisplayName(KeyEnum.Alpha2, "2");
		field.SetEnumDisplayName(KeyEnum.Alpha3, "3");
		field.SetEnumDisplayName(KeyEnum.Alpha4, "4");
		field.SetEnumDisplayName(KeyEnum.Alpha5, "5");
		field.SetEnumDisplayName(KeyEnum.Alpha6, "6");
		field.SetEnumDisplayName(KeyEnum.Alpha7, "7");
		field.SetEnumDisplayName(KeyEnum.Alpha8, "8");
		field.SetEnumDisplayName(KeyEnum.Alpha9, "9");
		field.SetEnumDisplayName(KeyEnum.Mouse1, "Mouse 1");
		field.SetEnumDisplayName(KeyEnum.Mouse2, "Mouse 2");
		field.SetEnumDisplayName(KeyEnum.Mouse3, "Mouse 3");
		field.SetEnumDisplayName(KeyEnum.Mouse4, "Mouse 4");
		field.SetEnumDisplayName(KeyEnum.Mouse5, "Mouse 5");
		field.SetEnumDisplayName(KeyEnum.Mouse6, "Mouse 6");
		field.SetEnumDisplayName(KeyEnum.Mouse7, "Mouse 7");
	}
    public static string DefaultParentFolder = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}";                                                                   //19

    public enum SoundEnum{None, CustomSound1, CustomSound2, CustomSound3, CustomSound4, CustomSound5, CustomSound6, CustomSound7, CustomSound8}
    public static string convertSoundEnumToFile(SoundEnum value)
    {
        string sound = "";
        if(value == SoundEnum.CustomSound1) {sound = $"{Path.Combine(DefaultParentFolder!, "customSound1.wav")}";}
        else if(value == SoundEnum.CustomSound2) {sound = $"{Path.Combine(DefaultParentFolder!, "customSound2.wav")}";}
        else if(value == SoundEnum.CustomSound3) {sound = $"{Path.Combine(DefaultParentFolder!, "customSound3.wav")}";}
        else if(value == SoundEnum.CustomSound4) {sound = $"{Path.Combine(DefaultParentFolder!, "customSound4.wav")}";}
        else if(value == SoundEnum.CustomSound5) {sound = $"{Path.Combine(DefaultParentFolder!, "customSound5.wav")}";}
        else if(value == SoundEnum.CustomSound6) {sound = $"{Path.Combine(DefaultParentFolder!, "customSound6.wav")}";}
        else if(value == SoundEnum.CustomSound7) {sound = $"{Path.Combine(DefaultParentFolder!, "customSound7.wav")}";}
        else if(value == SoundEnum.CustomSound8) {sound = $"{Path.Combine(DefaultParentFolder!, "customSound8.wav")}";}
        
        return sound;
    }
    public static void OpenSoundFolder() {Application.OpenURL(DefaultParentFolder);}

    public static bool settingsColorsActivated = true;

    public static void CreateConfig()
    {
        var config = PluginConfigurator.Create("UltraCooldownInfo", "UltraCooldownInfo");
        config.SetIconWithURL($"{Path.Combine(DefaultParentFolder!, "icon.png")}");

        ConfigHeader infoHeader = new ConfigHeader(config.rootPanel, "Everything in this mod must be configured by the user, there are no default presets. Consult the readme.md if you need help configuring.");
        infoHeader.textSize = 12;

        ConfigPanel globalSettingsPanel = new ConfigPanel(config.rootPanel, "Global Settings", "globalSettings");

        BoolField modEnabledField = new BoolField(globalSettingsPanel, "Mod Enabled", "modEnabled", true);

        EnumField<KeyEnum> showKeyField = new EnumField<KeyEnum>(globalSettingsPanel, "Key to show charge meters", "showKey", KeyEnum.None);
        showKeyField.onValueChange += (EnumField<KeyEnum>.EnumValueChangeEvent e) => {Plugin.showGUIKeyCode = convertKeyEnumToKeyCode(e.value);}; 
        SetDisplayNames(showKeyField);
        Plugin.showGUIKeyCode = convertKeyEnumToKeyCode(showKeyField.value);

        BoolField showKeyToggleField = new BoolField(globalSettingsPanel, "Key is toggle instead of hold", "showKeyToggle", false);
        showKeyToggleField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.showGUIKeyToggle = e.value;};
        Plugin.showGUIKeyToggle = showKeyToggleField.value;

        BoolField settingsColorsActivatedField = new BoolField(globalSettingsPanel, "Setng. Panel Colors (Must restart)", "settingsColorsActivated", true);
        settingsColorsActivatedField.onValueChange += (BoolField.BoolValueChangeEvent e) => {settingsColorsActivated = e.value;};
        settingsColorsActivated = settingsColorsActivatedField.value;

        BoolField showIconsInMenuField = new BoolField(globalSettingsPanel, "Show UI in menu?", "ShowIconsInMenu", false);
        showIconsInMenuField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.showIconsInMenu = e.value;};
        Plugin.showIconsInMenu = showIconsInMenuField.value;

        ConfigDivision division = new ConfigDivision(config.rootPanel, "division");

        modEnabledField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.modEnabled = e.value; division.interactable = e.value;};
        Plugin.modEnabled = modEnabledField.value; division.interactable = modEnabledField.value; 

        //lets us change the order in which we display things.
        //int[] configOrder = {0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21};
        int[] configOrder = {3,4,5,0,1,2,7,8,9,20,21,6,13,10,11,12,14,18,15,16,17,19};

        foreach(int i in configOrder)
        {
            UltraCooldownInfoWeapon UCIWepConf = Plugin.UltraCooldownInfoWeapons[i];

            Color redColor = new Color(1,0.7f,0.7f);
            Color greenColor = new Color(0.7f,1,0.7f);
            Color blueColor = new Color(0.7f,0.7f,1);

            if(settingsColorsActivated == false)
            {
                redColor = Color.white;
                greenColor = Color.white;
                blueColor = Color.white;
            }

            if(i == 0)       {new ConfigHeader(division, "Revolver Alt Fire").textColor = blueColor;}
            else if(i == 3)  {new ConfigHeader(division, "Slab Revolver Primary Fire").textColor = redColor;}
            else if(i == 20) {new ConfigHeader(division, "Shotgun Alt Fire").textColor = blueColor;}
            else if(i == 7)  {new ConfigHeader(division, "Jackhammer Primary Fire").textColor = redColor;}
            else if(i == 10) {new ConfigHeader(division, "Nailgun/Saw Alt Fire").textColor = blueColor;}
            else if(i == 13) {new ConfigHeader(division, "Nailgun/Saw Primary Fire").textColor = redColor;}
            else if(i == 14) {new ConfigHeader(division, "Railcannon Fire").textColor = greenColor;}
            else if(i == 15) {new ConfigHeader(division, "Rocket Launcher Alt Fire").textColor = blueColor;}
            else if(i == 18) {new ConfigHeader(division, "Rocket Launcher Primary Fire").textColor = redColor;}
            else if(i == 19) {new ConfigHeader(division, "Misc.").textColor = greenColor;}

            Color[] variantColors = Plugin.GetVariantColors(1f, 0.3f);
            Color panelColor = new Color(0.1f,0.1f,0.1f); //default dark grey
            if(settingsColorsActivated)
            {
                if(Plugin.arrayVariant0.Contains(i)) {panelColor = variantColors[0];}
                else if(Plugin.arrayVariant1.Contains(i)) {panelColor = variantColors[1];}
                else if(Plugin.arrayVariant2.Contains(i)) {panelColor = variantColors[2];}
            }
            else
            {
                panelColor = new Color(0.0f,0.0f,0.0f); //black if disabled
            }

            ConfigPanel newPanel = new ConfigPanel(division, UCIWepConf.name, UCIWepConf.name.Replace(" ", string.Empty) + "Panel");
            newPanel.fieldColor = panelColor;

            ConfigPanel newChargeBarPanel = new ConfigPanel(newPanel, "Charge Bar", UCIWepConf.name.Replace(" ", string.Empty) + "ChargeBarPanel");

            ConfigHeader warningHeader = new ConfigHeader(newChargeBarPanel, "This weapon must be equipped for proper functionality.");
            warningHeader.textSize = 16;

            //////////////
            //CHARGE BAR//
            //////////////

            BoolField enabledField = new BoolField(newChargeBarPanel, "Enabled", UCIWepConf.name.Replace(" ", string.Empty) + "Enabled", false);
            enabledField.onValueChange += (BoolField.BoolValueChangeEvent e) => {UCIWepConf.enabled = e.value;};
            UCIWepConf.enabled = enabledField.value;

            IntField xPosField = new IntField(newChargeBarPanel, "x Position (px)", UCIWepConf.name.Replace(" ", string.Empty) + "xPos", 0, 0, Screen.width);
            xPosField.onValueChange += (IntField.IntValueChangeEvent e) => {UCIWepConf.xPos = e.value;};
            UCIWepConf.xPos = xPosField.value;

            IntField yPosField = new IntField(newChargeBarPanel, "y Position (px)", UCIWepConf.name.Replace(" ", string.Empty) + "yPos", 0, 0, Screen.height);
            yPosField.onValueChange += (IntField.IntValueChangeEvent e) => {UCIWepConf.yPos = e.value;};
            UCIWepConf.yPos = yPosField.value;

            IntField widthField = new IntField(newChargeBarPanel, "Width (px)", UCIWepConf.name.Replace(" ", string.Empty) + "Width", 0, 0, Screen.width);
            widthField.onValueChange += (IntField.IntValueChangeEvent e) => {UCIWepConf.width = e.value;};
            UCIWepConf.width = widthField.value;

            IntField heightField = new IntField(newChargeBarPanel, "Height (px)", UCIWepConf.name.Replace(" ", string.Empty) + "Height", 0, 0, Screen.height);
            heightField.onValueChange += (IntField.IntValueChangeEvent e) => {UCIWepConf.height = e.value;};
            UCIWepConf.height = heightField.value;

            BoolField flippedField = new BoolField(newChargeBarPanel, "Flip Visual", UCIWepConf.name.Replace(" ", string.Empty) + "FlipEnabled", false);
            flippedField.onValueChange += (BoolField.BoolValueChangeEvent e) => {UCIWepConf.flipped = e.value;};
            UCIWepConf.flipped = flippedField.value;

            FloatField opacityField = new FloatField(newChargeBarPanel, "Charge Opacity (0.0 - 1.0)", UCIWepConf.name.Replace(" ", string.Empty) + "Opacity", 1.0f, 0f, 1.0f);
            UCIWepConf.opacity = opacityField.value;

            ColorField remainingColorField = null;
            ColorField chargingColorField = null;
            if(UCIWepConf.chargeColors == true)
            {
                remainingColorField = new ColorField(newChargeBarPanel, "Remaining Cooldown Charge Color", UCIWepConf.name.Replace(" ", string.Empty) + "RemainingColor", new Color(1.0f, 0f, 0f));
                remainingColorField.onValueChange += (ColorField.ColorValueChangeEvent e) => {UCIWepConf.remainingColor = new Color(e.value.r, e.value.g, e.value.b, UCIWepConf.opacity);};
                UCIWepConf.remainingColor = new Color(remainingColorField.value.r, remainingColorField.value.g, remainingColorField.value.b, UCIWepConf.opacity);

                chargingColorField = new ColorField(newChargeBarPanel, "Charging Cooldown Color", UCIWepConf.name.Replace(" ", string.Empty) + "ChargingColor", new Color(0.0f, 1.0f, 0f));
                chargingColorField.onValueChange += (ColorField.ColorValueChangeEvent e) => {UCIWepConf.chargingColor = new Color(e.value.r, e.value.g, e.value.b, UCIWepConf.opacity);};
                UCIWepConf.chargingColor = new Color(chargingColorField.value.r, chargingColorField.value.g, chargingColorField.value.b, UCIWepConf.opacity);
            }
            ColorField chargedColorField = new ColorField(newChargeBarPanel, "Cooldown Charged Color", UCIWepConf.name.Replace(" ", string.Empty) + "ChargedColor", new Color(0.0f, 0.7f, 0.0f));
            chargedColorField.onValueChange += (ColorField.ColorValueChangeEvent e) => {UCIWepConf.chargedColor = new Color(e.value.r, e.value.g, e.value.b, UCIWepConf.opacity);};
            UCIWepConf.chargedColor = new Color(chargedColorField.value.r, chargedColorField.value.g, chargedColorField.value.b, UCIWepConf.opacity);

            ColorField usingChargeColorField = null;
            ColorField usingChargeChargedColorField = null;
            //these weapons dont need these fields
            if(UCIWepConf.usingChargeColors == true)
            {
                usingChargeColorField = new ColorField(newChargeBarPanel, "Using Charge Charging Color", UCIWepConf.name.Replace(" ", string.Empty) + "UsingChargeColor", new Color(1.0f, 1.0f, 0.0f));
                usingChargeColorField.onValueChange += (ColorField.ColorValueChangeEvent e) => {UCIWepConf.usingChargeColor = new Color(e.value.r, e.value.g, e.value.b, UCIWepConf.opacity);};
                UCIWepConf.usingChargeColor = new Color(usingChargeColorField.value.r, usingChargeColorField.value.g, usingChargeColorField.value.b, UCIWepConf.opacity);
                if(UCIWepConf.usingChargeChargedColorEnabled) //false only for jumpstart because it automatically uses it when fully using charge charged.
                {
                    usingChargeChargedColorField = new ColorField(newChargeBarPanel, "Using Charge Charged Color", UCIWepConf.name.Replace(" ", string.Empty) + "UsingChargeChargedColor", new Color(0.0f, 1.0f, 1.0f));
                    usingChargeChargedColorField.onValueChange += (ColorField.ColorValueChangeEvent e) => {UCIWepConf.usingChargeChargedColor = new Color(e.value.r, e.value.g, e.value.b, UCIWepConf.opacity);};
                    UCIWepConf.usingChargeChargedColor = new Color(usingChargeChargedColorField.value.r, usingChargeChargedColorField.value.g, usingChargeChargedColorField.value.b, UCIWepConf.opacity);
                }
            }
            opacityField.onValueChange += (FloatField.FloatValueChangeEvent e) => 
            {
                UCIWepConf.opacity = e.value;
                UCIWepConf.remainingColor = new Color(remainingColorField.value.r, remainingColorField.value.g, remainingColorField.value.b, UCIWepConf.opacity);
                UCIWepConf.chargingColor = new Color(chargingColorField.value.r, chargingColorField.value.g, chargingColorField.value.b, UCIWepConf.opacity);
                UCIWepConf.chargedColor = new Color(chargedColorField.value.r, chargedColorField.value.g, chargedColorField.value.b, UCIWepConf.opacity);
                if(UCIWepConf.usingChargeColors == true)
                {
                    UCIWepConf.usingChargeColor = new Color(usingChargeColorField.value.r, usingChargeColorField.value.g, usingChargeColorField.value.b, UCIWepConf.opacity); 
                    UCIWepConf.usingChargeChargedColor = new Color(usingChargeChargedColorField.value.r, usingChargeChargedColorField.value.g, usingChargeChargedColorField.value.b, UCIWepConf.opacity);
                }
                Plugin.normalSharpshooterBackgroundChargingColor = new Color(Plugin.normalSharpshooterBackgroundChargingColor.r,Plugin.normalSharpshooterBackgroundChargingColor.g,Plugin.normalSharpshooterBackgroundChargingColor.b, UCIWepConf.opacity);
            };

            if(i == 2) //special case
            {
                ColorField normalMarksmanBackgroundChargingColorField = new ColorField(newChargeBarPanel, "Backgrnd. Charging Color (normal var.)", UCIWepConf.name.Replace(" ", string.Empty) + "SharpshooterBackgroundChargingColor", new Color(0.5f,0.5f,0.5f, UCIWepConf.opacity));
                normalMarksmanBackgroundChargingColorField.onValueChange += (ColorField.ColorValueChangeEvent e) => {Plugin.normalSharpshooterBackgroundChargingColor = new Color(e.value.r,e.value.g,e.value.b, UCIWepConf.opacity);};
                Plugin.normalSharpshooterBackgroundChargingColor = new Color(normalMarksmanBackgroundChargingColorField.value.r,normalMarksmanBackgroundChargingColorField.value.g,normalMarksmanBackgroundChargingColorField.value.b, UCIWepConf.opacity);
            }

            EnumField<ChargeBarTypeEnum> ChargeBarTypeField = new  EnumField<ChargeBarTypeEnum>(newChargeBarPanel, "Type of Charge Bar", UCIWepConf.name.Replace(" ", string.Empty) + "ChargeBarType", ChargeBarTypeEnum.WeaponImageSplit);
            ChargeBarTypeField.SetEnumDisplayName(ChargeBarTypeEnum.WeaponImageSplit, "Weapon Image - Split");
            ChargeBarTypeField.SetEnumDisplayName(ChargeBarTypeEnum.WeaponImageGradient, "Weapon Image - Gradient");
            ConfigDivision ChargeExtraSettingsDivision = new ConfigDivision(newChargeBarPanel, "chargeDivision");
            ChargeBarTypeField.onValueChange += (EnumField<ChargeBarTypeEnum>.EnumValueChangeEvent e) => 
            {
                UCIWepConf.chargeBarType = e.value;
                if(e.value == ChargeBarTypeEnum.Classic) {ChargeExtraSettingsDivision.interactable = true;}
                else if(e.value == ChargeBarTypeEnum.WeaponImageGradient) {ChargeExtraSettingsDivision.interactable = false;}
                else if(e.value == ChargeBarTypeEnum.WeaponImageSplit) {ChargeExtraSettingsDivision.interactable = false; }
            };
            UCIWepConf.chargeBarType = ChargeBarTypeField.value;
            if(ChargeBarTypeField.value == ChargeBarTypeEnum.Classic) {ChargeExtraSettingsDivision.interactable = true;}
            else if(ChargeBarTypeField.value == ChargeBarTypeEnum.WeaponImageGradient) {ChargeExtraSettingsDivision.interactable = false;}
            else if(ChargeBarTypeField.value == ChargeBarTypeEnum.WeaponImageSplit) {ChargeExtraSettingsDivision.interactable = false;}

            IntField numberDivisionsField = new IntField(newChargeBarPanel, "Number of Divisions TOTAL", UCIWepConf.name.Replace(" ", string.Empty) + "NumberDivisions", 1, 1, 10);
            numberDivisionsField.onValueChange += (IntField.IntValueChangeEvent e) => {UCIWepConf.numberDivisions = e.value;};
            UCIWepConf.numberDivisions = numberDivisionsField.value;
            
            IntField divisionWidthField = new IntField(newChargeBarPanel, "Division Width (px)", UCIWepConf.name.Replace(" ", string.Empty) + "DivisionWidth", 0, 0, 1000);
            divisionWidthField.onValueChange += (IntField.IntValueChangeEvent e) => {UCIWepConf.divisionWidth = e.value;};
            UCIWepConf.divisionWidth = divisionWidthField.value;

            BoolField iconEnabledField = new BoolField(ChargeExtraSettingsDivision, "Icon Enabled", UCIWepConf.name.Replace(" ", string.Empty) + "IconEnabled", false);
            iconEnabledField.onValueChange += (BoolField.BoolValueChangeEvent e) => {UCIWepConf.iconEnabled = e.value;};
            UCIWepConf.iconEnabled = iconEnabledField.value;

            EnumField<SideEnum> iconSideField = new EnumField<SideEnum>(ChargeExtraSettingsDivision,"Icon Side", UCIWepConf.name.Replace(" ", string.Empty) + "IconSide", SideEnum.Left);
            iconSideField.onValueChange += (EnumField<SideEnum>.EnumValueChangeEvent e) => {UCIWepConf.iconSide = e.value;};
            UCIWepConf.iconSide = iconSideField.value;

            IntField iconDistanceField = new IntField(ChargeExtraSettingsDivision, "Icon Distance (px)", UCIWepConf.name.Replace(" ", string.Empty) + "IconDistance", 0, -1000000, 1000000);
            iconDistanceField.onValueChange += (IntField.IntValueChangeEvent e) => {UCIWepConf.iconDistance = e.value;};
            UCIWepConf.iconDistance = iconDistanceField.value;

            Color color = new Color(0f,0f,0f);
            Color[] variationColors = Plugin.GetVariantColors(UCIWepConf.opacity, 1f);
            if(Plugin.arrayVariant0.Contains(i)) {color = variationColors[0];}
            else if(Plugin.arrayVariant1.Contains(i)) {color = variationColors[1];}
            else if(Plugin.arrayVariant2.Contains(i)) {color = variationColors[2];}

            ColorField chargeBarIconColorField = new ColorField(ChargeExtraSettingsDivision, "Icon Color", UCIWepConf.name.Replace(" ", string.Empty) + "ChargeBarIconColor", color);
            chargeBarIconColorField.onValueChange += (ColorField.ColorValueChangeEvent e) => {UCIWepConf.chargeBarIconColor = e.value;};
            UCIWepConf.chargeBarIconColor = chargeBarIconColorField.value;

            IntField borderThicknessField = new IntField(ChargeExtraSettingsDivision, "Border Thickness (px)", UCIWepConf.name.Replace(" ", string.Empty) + "BorderThickness", 0, 0, 1000000);
            borderThicknessField.onValueChange += (IntField.IntValueChangeEvent e) => {UCIWepConf.borderThickness = e.value;};
            UCIWepConf.borderThickness = borderThicknessField.value;

            FloatField backgroundOpacityField = new FloatField(newChargeBarPanel, "Background Opacity (0.0 - 1.0)", UCIWepConf.name.Replace(" ", string.Empty) + "BackgroundOpacity", 1.0f, 0f, 1.0f);
            ColorField backgroundColorField = new ColorField(newChargeBarPanel, "Background / Divisions Color", UCIWepConf.name.Replace(" ", string.Empty) + "BackgroundChargingColor", new Color(0.3f,0.3f,0.3f, UCIWepConf.backgroundOpacity));
            backgroundOpacityField.onValueChange += (FloatField.FloatValueChangeEvent e) => {UCIWepConf.backgroundOpacity = e.value; UCIWepConf.backgroundColor = new Color(backgroundColorField.value.r,backgroundColorField.value.g,backgroundColorField.value.b, UCIWepConf.backgroundOpacity);};
            UCIWepConf.backgroundOpacity = backgroundOpacityField.value;
            backgroundColorField.onValueChange += (ColorField.ColorValueChangeEvent e) => {UCIWepConf.backgroundColor = new Color(e.value.r,e.value.g,e.value.b, UCIWepConf.backgroundOpacity);};
            UCIWepConf.backgroundColor = new Color(backgroundColorField.value.r,backgroundColorField.value.g,backgroundColorField.value.b, UCIWepConf.backgroundOpacity);

            /////////
            //SOUND//
            /////////

            ConfigPanel newSoundPanel = new ConfigPanel(newPanel, "Sound Alert", UCIWepConf.name.Replace(" ", string.Empty) + "SoundPanel");

            ConfigHeader warningHeader2 = new ConfigHeader(newSoundPanel, "This weapon must be equipped for proper functionality.");
            warningHeader2.textSize = 16;

            BoolField enableSoundField = new BoolField(newSoundPanel, "Sound Enabled", UCIWepConf.name.Replace(" ", string.Empty) + "SoundEnabled", false);
            enableSoundField.onValueChange += (BoolField.BoolValueChangeEvent e) => {UCIWepConf.soundEnabled = e.value;};
            UCIWepConf.soundEnabled = enableSoundField.value;

            BoolField enableSoundWhileHeldField = new BoolField(newSoundPanel, "Still Play Sound When Held", UCIWepConf.name.Replace(" ", string.Empty) + "SoundEnabledWhileHeld", false);
            enableSoundWhileHeldField.onValueChange += (BoolField.BoolValueChangeEvent e) => {UCIWepConf.soundEnabledWhileHeld = e.value;};
            UCIWepConf.soundEnabledWhileHeld = enableSoundWhileHeldField.value;

            FloatField volumeMultField = new FloatField(newSoundPanel, "Volume Multiplier", UCIWepConf.name.Replace(" ", string.Empty) + "VolumeMult", 1f, 0f, 1f);
            volumeMultField.onValueChange += (FloatField.FloatValueChangeEvent e) => {UCIWepConf.volumeMult = e.value;};
            UCIWepConf.volumeMult = volumeMultField.value;

            IntField soundDivisionsField = new IntField(newSoundPanel, "Num. Sound Charge Divisions TOTAL", UCIWepConf.name.Replace(" ", string.Empty) + "SoundDivisions", 1, 1, 1000);
            soundDivisionsField.onValueChange += (IntField.IntValueChangeEvent e) => {UCIWepConf.soundDivisions = e.value;};
            UCIWepConf.soundDivisions = soundDivisionsField.value;

            EnumField<SoundEnum> FractionChargeSoundField = new EnumField<SoundEnum>(newSoundPanel, "Charge Division Finished Sound", UCIWepConf.name.Replace(" ", string.Empty) + "FractionChargeSound", SoundEnum.None);
            FractionChargeSoundField.onValueChange += (EnumField<SoundEnum>.EnumValueChangeEvent e) => {UCIWepConf.filePathFractionCharge = convertSoundEnumToFile(e.value);};
            UCIWepConf.filePathFractionCharge = convertSoundEnumToFile(FractionChargeSoundField.value);

            EnumField<SoundEnum> ChargeSoundField = new EnumField<SoundEnum>(newSoundPanel, "Charge Finished Sound", UCIWepConf.name.Replace(" ", string.Empty) + "ChargeSound", SoundEnum.None);
            ChargeSoundField.onValueChange += (EnumField<SoundEnum>.EnumValueChangeEvent e) => {UCIWepConf.filePathCharge = convertSoundEnumToFile(e.value);};
            UCIWepConf.filePathCharge = convertSoundEnumToFile(ChargeSoundField.value);

            if(UCIWepConf.usingChargeColors == true)
            {
                EnumField<SoundEnum> UsingChargeSoundField = new EnumField<SoundEnum>(newSoundPanel, "Using Charge Finished Sound", UCIWepConf.name.Replace(" ", string.Empty) + "UsingChargeSound", SoundEnum.None);
                UsingChargeSoundField.onValueChange += (EnumField<SoundEnum>.EnumValueChangeEvent e) => {UCIWepConf.filePathUsingCharge = convertSoundEnumToFile(e.value);};
                UCIWepConf.filePathUsingCharge = convertSoundEnumToFile(UsingChargeSoundField.value);
            }

            ConfigHeader warningHeaderFiles = new ConfigHeader(newSoundPanel, "Keep backups of your sounds, they will be overwritten when the mod is updated.");
            warningHeaderFiles.textSize = 16;

            ButtonField openSoundsFolderField = new ButtonField(newSoundPanel, "Open Sounds Folder", "button.openfolder");
            openSoundsFolderField.onClick += new ButtonField.OnClick(OpenSoundFolder);

            //////////////
            //ICON FLASH//
            //////////////

            ConfigPanel flashingIconPanel = new ConfigPanel(newPanel, "Flashing Icon", UCIWepConf.name.Replace(" ", string.Empty) + "FlashingIconPanel");

            ConfigHeader warningHeader3 = new ConfigHeader(flashingIconPanel, "This weapon must be equipped for proper functionality.");
            warningHeader3.textSize = 16;

            BoolField flashingIconEnabledField = new BoolField(flashingIconPanel, "Enabled", UCIWepConf.name.Replace(" ", string.Empty) + "flashingIconEnabled", false);
            flashingIconEnabledField.onValueChange += (BoolField.BoolValueChangeEvent e) => {UCIWepConf.iconFlashEnabled = e.value;};
            UCIWepConf.iconFlashEnabled = flashingIconEnabledField.value;

            IntField xPosFlashField = new IntField(flashingIconPanel, "x Position (px)", UCIWepConf.name.Replace(" ", string.Empty) + "xPosFlash", 0, 0, Screen.width);
            xPosFlashField.onValueChange += (IntField.IntValueChangeEvent e) => {UCIWepConf.xPosFlash = e.value;};
            UCIWepConf.xPosFlash = xPosFlashField.value;

            IntField yPosFlashField = new IntField(flashingIconPanel, "y Position (px)", UCIWepConf.name.Replace(" ", string.Empty) + "yPosFlash", 0, 0, Screen.width);
            yPosFlashField.onValueChange += (IntField.IntValueChangeEvent e) => {UCIWepConf.yPosFlash = e.value;};
            UCIWepConf.yPosFlash = yPosFlashField.value;

            IntField widthFlashField = new IntField(flashingIconPanel, "width (px)", UCIWepConf.name.Replace(" ", string.Empty) + "widthFlash", 0, 0, Screen.width);
            widthFlashField.onValueChange += (IntField.IntValueChangeEvent e) => {UCIWepConf.widthFlash = e.value;};
            UCIWepConf.widthFlash = widthFlashField.value;

            IntField heightFlashField = new IntField(flashingIconPanel, "height (px)", UCIWepConf.name.Replace(" ", string.Empty) + "heightFlash", 0, 0, Screen.width);
            heightFlashField.onValueChange += (IntField.IntValueChangeEvent e) => {UCIWepConf.heightFlash = e.value;};
            UCIWepConf.heightFlash = heightFlashField.value;

            BoolField flipFlashField = new BoolField(flashingIconPanel, "Flip Flash Icon", UCIWepConf.name.Replace(" ", string.Empty) + "FlipFlashIcon", false);
            flipFlashField.onValueChange += (BoolField.BoolValueChangeEvent e) => {UCIWepConf.iconFlipped = e.value;};
            UCIWepConf.iconFlipped = flipFlashField.value;

            EnumField<FlashTypeEnum> flashTypeField = new EnumField<FlashTypeEnum>(flashingIconPanel, "Type of Flash", UCIWepConf.name.Replace(" ", string.Empty) + "typeFlash", FlashTypeEnum.LinearFade);
            flashTypeField.onValueChange += (EnumField<FlashTypeEnum>.EnumValueChangeEvent e) => {UCIWepConf.flashType = e.value;};
            UCIWepConf.flashType = flashTypeField.value;
            flashTypeField.SetEnumDisplayName(FlashTypeEnum.LinearFade, "Linear Fade");
            flashTypeField.SetEnumDisplayName(FlashTypeEnum.NoFade, "No Fade");

            FloatField lengthFlashField = new FloatField(flashingIconPanel, "Length Flash (s)", UCIWepConf.name.Replace(" ", string.Empty) + "LengthFlash", 0f, 0f, 1000f);
            lengthFlashField.onValueChange += (FloatField.FloatValueChangeEvent e) => {UCIWepConf.lengthFlash = e.value;};
            UCIWepConf.lengthFlash = lengthFlashField.value;

            ColorField colorFlashField = new ColorField(flashingIconPanel, "Flash Color", UCIWepConf.name.Replace(" ", string.Empty) + "ColorFlash", Color.white);
            colorFlashField.onValueChange += (ColorField.ColorValueChangeEvent e) => {UCIWepConf.colorFlash = e.value;};
            UCIWepConf.colorFlash = colorFlashField.value;

            IntField numberMiniFlashesField = new IntField(flashingIconPanel, "Number of divisions", UCIWepConf.name.Replace(" ", string.Empty) + "NumberFlashes", 1, 1, 1000);
            numberMiniFlashesField.onValueChange += (IntField.IntValueChangeEvent e) => {UCIWepConf.numberFlashes = e.value;};
            UCIWepConf.numberFlashes = numberMiniFlashesField.value;

            FloatField lengthMiniFlashField = new FloatField(flashingIconPanel, "Length of Subdivision Flash", UCIWepConf.name.Replace(" ", string.Empty) + "LengthMiniFlash", 0f, 0f, 1000f);
            lengthMiniFlashField.onValueChange += (FloatField.FloatValueChangeEvent e) => {UCIWepConf.lengthMiniFlash = e.value;};
            UCIWepConf.lengthMiniFlash = lengthMiniFlashField.value;
        }
    }
}