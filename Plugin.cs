using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using UnityEngine;
using HarmonyLib;
using System;
using System.Linq;

namespace UltraCooldownInfo;

//TO DO:
//icon adjustment
//force update doesnt work on counter
//null checking
//custom icon color
//overheat jumpstart charge issue
//show icons in menu toggle

[BepInPlugin("UltraCooldownInfo", "UltraCooldownInfo", "0.01")]
public class Plugin : BaseUnityPlugin
{    
    public static Color[] GetVariantColors(float opacity, float colorDeepness)
    {
        Color var0Color = new Color(MonoSingleton<ColorBlindSettings>.Instance.variationColors[0].r * colorDeepness, MonoSingleton<ColorBlindSettings>.Instance.variationColors[0].g * colorDeepness, MonoSingleton<ColorBlindSettings>.Instance.variationColors[0].b * colorDeepness);
        Color var1Color = new Color(MonoSingleton<ColorBlindSettings>.Instance.variationColors[1].r * colorDeepness, MonoSingleton<ColorBlindSettings>.Instance.variationColors[1].g * colorDeepness, MonoSingleton<ColorBlindSettings>.Instance.variationColors[1].b * colorDeepness);
        Color var2Color = new Color(MonoSingleton<ColorBlindSettings>.Instance.variationColors[2].r * colorDeepness, MonoSingleton<ColorBlindSettings>.Instance.variationColors[2].g * colorDeepness, MonoSingleton<ColorBlindSettings>.Instance.variationColors[2].b * colorDeepness);
        Color[] arr = {var0Color, var1Color, var2Color};
        return arr;
    }
    public static bool hideGUI = false;
    public static KeyCode showGUIKeyCode = KeyCode.None;
    public static bool showGUIKeyToggle;
    public static bool modEnabled = true;
    public static bool showIconsInMenu = false;
    public static string DefaultParentFolder = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}";
    string barImageFile = $"{Path.Combine(DefaultParentFolder!, "bar.png")}";
    public static string[] weaponNames = {  "Piercer Revolver Alt", "Marksman Revolver Alt", "Sharpshooter Revolver Alt",                       //0 1 2
                                            "Piercer Slab Fire", "Marksman Slab Fire", "Sharpshooter Slab Fire",                                //3 4 5
                                            "Sawed-On Shotgun Alt",                                                                             //6
                                            "Core Eject Jackhammer Fire", "Pump Charge Jackhammer Fire","Sawed-On Jackhammer Fire",             //7 8 9
                                            "Attractor Nailgun Alt", "Overheat Nailgun Alt", "Jumpstart Nailgun Alt",                           //10 11 12
                                            "Attractor Nailgun Fire",                                                                           //13 
                                            "Railcannon",                                                                                       //14
                                            "Freezeframe Rocket Launcher Alt", "S.R.S. Rocket Launcher Alt", "Firestarter Rocket Launcher Alt", //15 16 17
                                            "Rocket Launcher Fire",                                                                             //18
                                            "Fist", //19
                                            "Core Eject Shotgun Alt", "Pump Charge Shotgun Alt"}; //these exist for consistency only, pretty useless      //20 21                                 
    public static bool[] needsUseChargeColors = {   true, false, true,   //0 1 2
                                                    false, false, false, //3 4 5
                                                    true,                //6
                                                    true, true, true,    //7 8 9
                                                    true, true, true,    //10 11 12
                                                    false,               //13
                                                    false,               //14
                                                    false, true, false,  //15 16 17
                                                    false,               //18
                                                    false,               //19
                                                    true, true};         //20 21
    public static bool[] needsChargeColors =    {   true, true, true,   //0 1 2
                                                    true, true, true, //3 4 5
                                                    true,                //6
                                                    true, true, true,    //7 8 9
                                                    true, true, true,    //10 11 12
                                                    true,               //13
                                                    true,               //14
                                                    true, true, true,  //15 16 17
                                                    true,               //18
                                                    true,               //19
                                                    false, false};         //20 21
    public static int[] arrayVariant0 = {0 ,3 ,7 ,10,13,15,20};
    public static int[] arrayVariant1 = {1 ,4 ,8 ,11,16,21};
    public static int[] arrayVariant2 = {2 ,5 ,6 ,9 ,12,17};
    public static UltraCooldownInfoWeapon[] UltraCooldownInfoWeapons = new UltraCooldownInfoWeapon[weaponNames.Length];
    //float[] chargeAmounts = new float[UltraCooldownInfoWeapons.Length];
    //float[] usingChargeAmounts = new float[UltraCooldownInfoWeapons.Length];
    public static Color normalSharpshooterBackgroundChargingColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    Texture2D barTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);

    Texture2D weapon_piercerTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D weapon_piercerAltTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D weapon_marksmanTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D weapon_marksmanAltTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D weapon_sharpshooterTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D weapon_sharpshooterAltTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D weapon_coreejectTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D weapon_coreejectAltTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D weapon_pumpchargeTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D weapon_pumpchargeAltTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D weapon_sawedonTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D weapon_sawedonAltTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D weapon_attractorTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D weapon_attractorAltTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D weapon_overheatTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D weapon_overheatAltTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D weapon_jumpstartTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D weapon_jumpstartAltTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D weapon_railcannonTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D weapon_freezeframeTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D weapon_srsTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D weapon_firestarterTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D weapon_fistTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    public void Start()
    {
        barTexture.LoadImage(File.ReadAllBytes(barImageFile));

        weapon_piercerTexture.LoadImage(File.ReadAllBytes($"{Path.Combine(DefaultParentFolder!, "weapon_piercer.png")}"));
        weapon_piercerAltTexture.LoadImage(File.ReadAllBytes($"{Path.Combine(DefaultParentFolder!, "weapon_piercerAlt.png")}"));
        weapon_marksmanTexture.LoadImage(File.ReadAllBytes($"{Path.Combine(DefaultParentFolder!, "weapon_marksman.png")}"));
        weapon_marksmanAltTexture.LoadImage(File.ReadAllBytes($"{Path.Combine(DefaultParentFolder!, "weapon_marksmanAlt.png")}"));
        weapon_sharpshooterTexture.LoadImage(File.ReadAllBytes($"{Path.Combine(DefaultParentFolder!, "weapon_sharpshooter.png")}"));
        weapon_sharpshooterAltTexture.LoadImage(File.ReadAllBytes($"{Path.Combine(DefaultParentFolder!, "weapon_sharpshooterAlt.png")}"));
        weapon_coreejectTexture.LoadImage(File.ReadAllBytes($"{Path.Combine(DefaultParentFolder!, "weapon_coreeject.png")}"));
        weapon_coreejectAltTexture.LoadImage(File.ReadAllBytes($"{Path.Combine(DefaultParentFolder!, "weapon_coreejectAlt.png")}"));
        weapon_pumpchargeTexture.LoadImage(File.ReadAllBytes($"{Path.Combine(DefaultParentFolder!, "weapon_pumpcharge.png")}"));
        weapon_pumpchargeAltTexture.LoadImage(File.ReadAllBytes($"{Path.Combine(DefaultParentFolder!, "weapon_pumpchargeAlt.png")}"));
        weapon_sawedonTexture.LoadImage(File.ReadAllBytes($"{Path.Combine(DefaultParentFolder!, "weapon_sawedon.png")}"));
        weapon_sawedonAltTexture.LoadImage(File.ReadAllBytes($"{Path.Combine(DefaultParentFolder!, "weapon_sawedonAlt.png")}"));
        weapon_attractorTexture.LoadImage(File.ReadAllBytes($"{Path.Combine(DefaultParentFolder!, "weapon_attractor.png")}"));
        weapon_attractorAltTexture.LoadImage(File.ReadAllBytes($"{Path.Combine(DefaultParentFolder!, "weapon_attractorAlt.png")}"));
        weapon_overheatTexture.LoadImage(File.ReadAllBytes($"{Path.Combine(DefaultParentFolder!, "weapon_overheat.png")}"));
        weapon_overheatAltTexture.LoadImage(File.ReadAllBytes($"{Path.Combine(DefaultParentFolder!, "weapon_overheatAlt.png")}"));
        weapon_jumpstartTexture.LoadImage(File.ReadAllBytes($"{Path.Combine(DefaultParentFolder!, "weapon_jumpstart.png")}"));
        weapon_jumpstartAltTexture.LoadImage(File.ReadAllBytes($"{Path.Combine(DefaultParentFolder!, "weapon_jumpstartAlt.png")}"));
        weapon_railcannonTexture.LoadImage(File.ReadAllBytes($"{Path.Combine(DefaultParentFolder!, "weapon_railcannon.png")}"));
        weapon_freezeframeTexture.LoadImage(File.ReadAllBytes($"{Path.Combine(DefaultParentFolder!, "weapon_freezeframe.png")}"));
        weapon_srsTexture.LoadImage(File.ReadAllBytes($"{Path.Combine(DefaultParentFolder!, "weapon_srs.png")}"));
        weapon_firestarterTexture.LoadImage(File.ReadAllBytes($"{Path.Combine(DefaultParentFolder!, "weapon_firestarter.png")}"));
        weapon_fistTexture.LoadImage(File.ReadAllBytes($"{Path.Combine(DefaultParentFolder!, "weapon_fist.png")}"));
    }

    CustomSoundPlayer csp;

    float[] chargeAmounts = new float[UltraCooldownInfoWeapons.Length];
    float[] usingChargeAmounts = new float[UltraCooldownInfoWeapons.Length];
    private void Awake()
    {
        csp = gameObject.AddComponent<CustomSoundPlayer>();
        for (int i = 0; i < UltraCooldownInfoWeapons.Length; i++)
        {
            chargeAmounts[i] = 1; //useful later. Prevents sounds from being played upon first level start
            usingChargeAmounts[i] = 1;
        }
        for(int i = 0; i < UltraCooldownInfoWeapons.Length; i++)
        {
            UltraCooldownInfoWeapons[i] = new UltraCooldownInfoWeapon(needsUseChargeColors[i], needsChargeColors[i], weaponNames[i]);
            if(i == 12) {UltraCooldownInfoWeapons[i].usingChargeChargedColorEnabled = false;} //jumpstart nailgun - never fully using charges
        }
        for(int i = 0; i < UltraCooldownInfoWeapons.Length; i++)
        {
            UltraCooldownInfoWeapons[i].chargeAmount = 1.0f;
        }
        //config has to be created in update because jank
        //Harmony harmony = new Harmony("UltraCooldownInfo");
        //harmony.PatchAll();
        Logger.LogInfo("Plugin UltraCooldownInfo is loaded!");
    }

    public static bool IsGameplayScene() //copied from UltraTweaker
    {
        string[] NonGameplay = {"Intro","Bootstrap","Main Menu","Level 2-S","Intermission1","Intermission2"};
        if(SceneHelper.CurrentScene == null) {return false;}
        return !NonGameplay.Contains(SceneHelper.CurrentScene);
    }

    public static bool IsMenu()
    {
        if(MonoSingleton<OptionsManager>.Instance != null && !MonoSingleton<OptionsManager>.Instance.paused && !MonoSingleton<FistControl>.Instance.shopping && GameStateManager.Instance != null && !GameStateManager.Instance.PlayerInputLocked)
        {
            return false;
        }
        return true;
    }

    Revolver weapon_piercer, weapon_marksman, weapon_sharpshooter;
    GameObject weapon_coreEject, weapon_pumpCharge, weapon_sawedOn; //these cant be shotguns because ShotgunHammers arent Shotguns... fuck you hakita.
    Nailgun weapon_attractor, weapon_overheat, weapon_jumpstart;
    RocketLauncher weapon_freezeframe, weapon_srs, weapon_firestarter;
    GameObject weapon, prevWeapon;

    public Texture2D GetTextureByIndex(int i)
    {
        Texture2D texture = barTexture;
        if(i == 0)       {texture = weapon_piercer.altVersion ? weapon_piercerAltTexture : weapon_piercerTexture;}
        else if(i == 1)  {texture = weapon_marksman.altVersion ? weapon_marksmanAltTexture : weapon_marksmanTexture;}
        else if(i == 2)  {texture = weapon_sharpshooter.altVersion ? weapon_sharpshooterAltTexture : weapon_sharpshooterTexture;}
        else if(i == 3)  {texture = weapon_piercerAltTexture;}
        else if(i == 4)  {texture = weapon_marksmanAltTexture;}
        else if(i == 5)  {texture = weapon_sharpshooterAltTexture;}
        else if(i == 6)  {texture = weapon_sawedOn.GetComponent<ShotgunHammer>() == null ? weapon_sawedonTexture : weapon_sawedonAltTexture;} 
        else if(i == 7)  {texture = weapon_coreejectAltTexture;}
        else if(i == 8)  {texture = weapon_pumpchargeAltTexture;}
        else if(i == 9)  {texture = weapon_sawedonAltTexture;}
        else if(i == 10) {texture = weapon_attractor.altVersion ? weapon_attractorAltTexture : weapon_attractorTexture;}
        else if(i == 11) {texture = weapon_overheat.altVersion ? weapon_overheatAltTexture : weapon_overheatTexture;}
        else if(i == 12) {texture = weapon_jumpstart.altVersion ? weapon_jumpstartAltTexture : weapon_jumpstartTexture;}
        else if(i == 13) {texture = weapon_attractor.altVersion ? weapon_attractorAltTexture : weapon_attractorTexture;}
        else if(i == 14) {texture = weapon_railcannonTexture;}
        else if(i == 15) {texture = weapon_freezeframeTexture;}
        else if(i == 16) {texture = weapon_srsTexture;}
        else if(i == 17) {texture = weapon_firestarterTexture;}
        else if(i == 18) {texture = weapon_freezeframeTexture;}
        else if(i == 19) {texture = weapon_fistTexture;}
        else if(i == 20) {texture = weapon_coreEject.GetComponent<ShotgunHammer>() == null ? weapon_coreejectTexture : weapon_coreejectAltTexture;}
        else if(i == 21) {texture = weapon_pumpCharge.GetComponent<ShotgunHammer>() == null ? weapon_pumpchargeTexture : weapon_pumpchargeAltTexture;}
        return texture;
    }

    public void findWeapons()
    {
        if(weapon != MonoSingleton<GunControl>.Instance.currentWeapon) {prevWeapon = weapon;}
        weapon = MonoSingleton<GunControl>.Instance.currentWeapon;

        foreach (GameObject go in MonoSingleton<GunControl>.Instance.slot1)
        {
            if(go.GetComponent<Revolver>().gunVariation == 0) {weapon_piercer = go.GetComponent<Revolver>();}
            else if(go.GetComponent<Revolver>().gunVariation == 1) {weapon_marksman = go.GetComponent<Revolver>();}
            else if(go.GetComponent<Revolver>().gunVariation == 2) {weapon_sharpshooter = go.GetComponent<Revolver>();}
        }

        //ShotgunHammer isnt a Shotgun so we have to check for both types
        foreach (GameObject go in MonoSingleton<GunControl>.Instance.slot2)
        {
            if(go == null) {continue;}
            if(go.GetComponent<Shotgun>() == null) {continue;}

            if(go.GetComponent<Shotgun>().variation == 0) {weapon_coreEject = go;}
            else if(go.GetComponent<Shotgun>().variation == 1) {weapon_pumpCharge = go;}
            else if(go.GetComponent<Shotgun>().variation == 2) {weapon_sawedOn = go;}
        }

        foreach(GameObject go in MonoSingleton<GunControl>.Instance.slot2)
        {
            if(go == null) {continue;}
            if(go.GetComponent<ShotgunHammer>() == null) {continue;}

            if(go.GetComponent<ShotgunHammer>().variation == 0) {weapon_coreEject = go;}
            else if(go.GetComponent<ShotgunHammer>().variation == 1) {weapon_pumpCharge = go;}
            else if(go.GetComponent<ShotgunHammer>().variation == 2) {weapon_sawedOn = go;}
        }

        foreach (GameObject go in MonoSingleton<GunControl>.Instance.slot3)
        {   //is it not ordered properly??
            if(go.GetComponent<Nailgun>().variation == 1) {weapon_attractor = go.GetComponent<Nailgun>();}
            else if(go.GetComponent<Nailgun>().variation == 0) {weapon_overheat = go.GetComponent<Nailgun>();}
            else if(go.GetComponent<Nailgun>().variation == 2) {weapon_jumpstart = go.GetComponent<Nailgun>();}
        }

        foreach (GameObject go in MonoSingleton<GunControl>.Instance.slot5)
        {
            if(go.GetComponent<RocketLauncher>().variation == 0) {weapon_freezeframe = go.GetComponent<RocketLauncher>();}
            else if(go.GetComponent<RocketLauncher>().variation == 1) {weapon_srs = go.GetComponent<RocketLauncher>();}
            else if(go.GetComponent<RocketLauncher>().variation == 2) {weapon_firestarter = go.GetComponent<RocketLauncher>();}
        }
    }

    public void findRevolverValues()
    {
        //Piercer Revolver Alt -- WORKS
        //The charge value is not kept track of by the game when it is out of the player hand, so we have to estimate it ourselves. The formula is copied over.
        if(weapon.GetComponent<Revolver>() == weapon_piercer) 
        {
            UltraCooldownInfoWeapons[0].chargeAmount = weapon_piercer.pierceCharge / 100f; 
        }
        else
        {
            float num = 1f;
            if(weapon_piercer.altVersion) {num = 0.5f;}
            UltraCooldownInfoWeapons[0].chargeAmount += 40f * Time.deltaTime * num / 100f;
            if(UltraCooldownInfoWeapons[0].chargeAmount >= 1f) {UltraCooldownInfoWeapons[0].chargeAmount = 1f;}
        }
        UltraCooldownInfoWeapons[0].usingChargeAmount = weapon_piercer.pierceShotCharge / 100f;

        //Marksman Revolver Alt -- WORKS
        //the charge value is private in Revolver... this is a workaround
        if(weapon.GetComponent<Revolver>() == weapon_marksman) 
        {
            float sum = 0f;
            for(int i = 0; i < weapon_marksman.coinPanels.Length; i++)
            {
                sum += weapon_marksman.coinPanels[i].fillAmount; 
            }
            UltraCooldownInfoWeapons[1].chargeAmount = sum / weapon_marksman.coinPanels.Length; 
        }
        else
        {
            UltraCooldownInfoWeapons[1].chargeAmount += (Time.deltaTime * 0.25f) / 4.0f;
            if(UltraCooldownInfoWeapons[1].chargeAmount >= 1f) {UltraCooldownInfoWeapons[1].chargeAmount = 1f;}
        }
        UltraCooldownInfoWeapons[1].usingChargeAmount = -1f; 
        
        //Sharpshooter Revolver Alt -- NEEDS WORK
        //the code for the sharpshooter is a mess, this is approximate.
        if(weapon.GetComponent<Revolver>() == weapon_sharpshooter) 
        {
            float sum = 0f;
            for(int i = 0; i < weapon_sharpshooter.coinPanels.Length; i++)
            {
                sum += weapon_sharpshooter.coinPanels[i].fillAmount; 
            }
            UltraCooldownInfoWeapons[2].chargeAmount = sum / weapon_sharpshooter.coinPanels.Length; 
        }
        else
        {
            float num2 = 0.85f;
            if(weapon_sharpshooter.altVersion)
            {
                num2 = 1.7f;
            }
            UltraCooldownInfoWeapons[2].chargeAmount += ((Time.deltaTime / 5.0f) / 3) * num2;
            if(UltraCooldownInfoWeapons[2].chargeAmount > 1f) {UltraCooldownInfoWeapons[2].chargeAmount = 1f;}
        }
        UltraCooldownInfoWeapons[2].usingChargeAmount = weapon_sharpshooter.pierceShotCharge / 100f;

        //Piercer Slab Fire -- WORKS
        UltraCooldownInfoWeapons[3].chargeAmount = (2.0f - MonoSingleton<WeaponCharges>.Instance.revaltpickupcharges[0]) / 2.0f;
        UltraCooldownInfoWeapons[3].usingChargeAmount = -1f;
        //Marksman Slab Fire -- WORKS
        UltraCooldownInfoWeapons[4].chargeAmount = (2.0f - MonoSingleton<WeaponCharges>.Instance.revaltpickupcharges[1]) / 2.0f;
        UltraCooldownInfoWeapons[4].usingChargeAmount = -1f;
        //Sharpshooter Slab Fire -- WORKS
        UltraCooldownInfoWeapons[5].chargeAmount = (2.0f - MonoSingleton<WeaponCharges>.Instance.revaltpickupcharges[2]) / 2.0f;
        UltraCooldownInfoWeapons[5].usingChargeAmount = -1f;
    }

    public void findShotgunValues()
    {
        //Sawed-On Shotgun Alt -- WORKS
        UltraCooldownInfoWeapons[6].chargeAmount = MonoSingleton<WeaponCharges>.Instance.shoSawCharge;
        if(weapon_sawedOn != null && weapon_sawedOn.GetComponent<Shotgun>() != null)
        {
            UltraCooldownInfoWeapons[6].usingChargeAmount = weapon_sawedOn.GetComponent<Shotgun>().grenadeForce / 60f;
        }
        else if(weapon_sawedOn != null && weapon_sawedOn.GetComponent<ShotgunHammer>() != null)
        {
            UltraCooldownInfoWeapons[6].usingChargeAmount = weapon_sawedOn.GetComponent<ShotgunHammer>().chargeForce / 60f;
        }
        
        //Core Eject Jackhammer Fire -- WORKS
        UltraCooldownInfoWeapons[7].chargeAmount = (7.0f - MonoSingleton<WeaponCharges>.Instance.shoaltcooldowns[0]) / 7.0f; 
        if(weapon_coreEject != null && weapon_coreEject.GetComponent<ShotgunHammer>() != null)
        {
            UltraCooldownInfoWeapons[7].usingChargeAmount = weapon_coreEject.GetComponent<ShotgunHammer>().swingCharge;
        }
        else {UltraCooldownInfoWeapons[7].usingChargeAmount = -1f;}
        
        //Pump Charge Jackhammer Fire -- WORKS
        UltraCooldownInfoWeapons[8].chargeAmount = (7.0f - MonoSingleton<WeaponCharges>.Instance.shoaltcooldowns[1]) / 7.0f; 
        if(weapon_pumpCharge != null && weapon_pumpCharge.GetComponent<ShotgunHammer>() != null)
        {
            UltraCooldownInfoWeapons[8].usingChargeAmount = weapon_pumpCharge.GetComponent<ShotgunHammer>().swingCharge;
        }
        else {UltraCooldownInfoWeapons[8].usingChargeAmount = -1f;}
        //Sawed-On Jackhammer Fire -- WORKS
        UltraCooldownInfoWeapons[9].chargeAmount = (7.0f - MonoSingleton<WeaponCharges>.Instance.shoaltcooldowns[2]) / 7.0f; 
        if(weapon_sawedOn != null && weapon_sawedOn.GetComponent<ShotgunHammer>() != null)
        {
            UltraCooldownInfoWeapons[9].usingChargeAmount = weapon_sawedOn.GetComponent<ShotgunHammer>().swingCharge;
        }
        else {UltraCooldownInfoWeapons[9].usingChargeAmount = -1f;}

        //Core Eject Alt -- 
        if(weapon_coreEject != null && weapon_coreEject.GetComponent<Shotgun>() != null)
        {
            UltraCooldownInfoWeapons[20].usingChargeAmount = weapon_coreEject.GetComponent<Shotgun>().grenadeForce / 60f;
        }
        else
        {
            UltraCooldownInfoWeapons[20].usingChargeAmount = 0f;
        }
        if(weapon_coreEject != null && weapon_coreEject.GetComponent<ShotgunHammer>() != null)
        {
            UltraCooldownInfoWeapons[20].chargeAmount = MonoSingleton<WeaponCharges>.Instance.shoAltNadeCharge;
        }
        else
        {
            UltraCooldownInfoWeapons[20].chargeAmount = 1f;
        }
        //Pump Charge Alt -- 
        UltraCooldownInfoWeapons[21].chargeAmount = 1f;
        if(weapon_pumpCharge != null && weapon_pumpCharge.GetComponent<Shotgun>() != null)
        {
            UltraCooldownInfoWeapons[21].usingChargeAmount = weapon_pumpCharge.GetComponent<Shotgun>().primaryCharge / 3.0f;
        }
        else if(weapon_pumpCharge != null && weapon_pumpCharge.GetComponent<ShotgunHammer>() != null) 
        {
            UltraCooldownInfoWeapons[21].usingChargeAmount = weapon_pumpCharge.GetComponent<ShotgunHammer>().primaryCharge / 3.0f;
        }
        else {UltraCooldownInfoWeapons[21].usingChargeAmount = 0f;}
    }

    public void findNailgunValues()
    {
        //Attractor Nailgun Alt
        if(weapon_attractor.wc == null) {UltraCooldownInfoWeapons[10].chargeAmount = 1.0f;}
        else {UltraCooldownInfoWeapons[10].chargeAmount = weapon_attractor.wc.naiMagnetCharge / 3.0f;} //can be better
        UltraCooldownInfoWeapons[10].usingChargeAmount = -1f;
        //Overheat Nailgun Alt -- WORKS
        //this is pretty scuffed too, but I guess it works. You have to switch off (you can still switch back on again) the weapon for the meter to be read from the game... so...
        if(weapon_overheat.altVersion == true)
        {
            if(MonoSingleton<WeaponCharges>.Instance.naiSawHeatsinks - 1f < 0.001 && weapon.GetComponent<Nailgun>() == weapon_overheat)
            {
                if(!MonoSingleton<InputManager>.Instance.InputSource.Fire1.IsPressed) {UltraCooldownInfoWeapons[11].chargeAmount += Time.deltaTime / 8.03f;}
                if(UltraCooldownInfoWeapons[11].chargeAmount >= 1) 
                {   //if the weapon was fired, deplete the meter.
                    if(weapon_overheat.canShoot && !weapon_overheat.burnOut && MonoSingleton<InputManager>.Instance.InputSource.Fire2.WasPerformedThisFrame && (double) weapon_overheat.heatUp >= 0.1) {UltraCooldownInfoWeapons[11].chargeAmount = 0;}
                    else {UltraCooldownInfoWeapons[11].chargeAmount = 1;}
                }
            }
            else
            {
                UltraCooldownInfoWeapons[11].chargeAmount = MonoSingleton<WeaponCharges>.Instance.naiSawHeatsinks; //only works if you switch off?
            }
        }
        else
        {
            if(MonoSingleton<WeaponCharges>.Instance.naiHeatsinks - 2f < 0.001 && weapon.GetComponent<Nailgun>() == weapon_overheat)
            {
                if(!MonoSingleton<InputManager>.Instance.InputSource.Fire1.IsPressed) {UltraCooldownInfoWeapons[11].chargeAmount += Time.deltaTime / (2 * 8.03f);}
                if(UltraCooldownInfoWeapons[11].chargeAmount >= 0.5) 
                {   //if the weapon was fired, deplete the meter.
                    if(weapon_overheat.canShoot && !weapon_overheat.burnOut && MonoSingleton<InputManager>.Instance.InputSource.Fire2.WasPerformedThisFrame && weapon_overheat.heatUp >= 0.1) {UltraCooldownInfoWeapons[11].chargeAmount += -0.5f;}
                    else if(UltraCooldownInfoWeapons[11].chargeAmount >= 1.0f) {UltraCooldownInfoWeapons[11].chargeAmount = 1;}
                }
            }
            else
            {
                UltraCooldownInfoWeapons[11].chargeAmount = MonoSingleton<WeaponCharges>.Instance.naiHeatsinks / 2.0f; //only works if you switch off?
            }
        }

        if(weapon.GetComponent<Nailgun>() != null) {UltraCooldownInfoWeapons[11].usingChargeAmount = weapon.GetComponent<Nailgun>().heatUp;}
        else
        {
            UltraCooldownInfoWeapons[11].usingChargeAmount += -Time.deltaTime / 8.0f;
            if(UltraCooldownInfoWeapons[11].usingChargeAmount < 0) {UltraCooldownInfoWeapons[11].usingChargeAmount = 0;}
        }
        if(UltraCooldownInfoWeapons[11].chargeAmount < 1f)
        {
            UltraCooldownInfoWeapons[11].usingChargeAmount = 0f;
        }

        //Jumpstart Nailgun Alt
        UltraCooldownInfoWeapons[12].chargeAmount = MonoSingleton<WeaponCharges>.Instance.naiZapperRecharge / 5.0f; 
        
        if(weapon_jumpstart.currentZapper == null)
        {
            UltraCooldownInfoWeapons[12].usingChargeAmount = 0f;
        }
        else
        {
            UltraCooldownInfoWeapons[12].usingChargeAmount = weapon_jumpstart.currentZapper.charge / 5.0f;
        }
        //Attractor Nailgun Fire
        if(weapon_attractor.altVersion == true)
        {
            UltraCooldownInfoWeapons[13].chargeAmount = MonoSingleton<WeaponCharges>.Instance.naiSaws / 10.0f;
        }
        else
        {
            UltraCooldownInfoWeapons[13].chargeAmount = MonoSingleton<WeaponCharges>.Instance.naiAmmo / 100.0f;
        }

        UltraCooldownInfoWeapons[13].usingChargeAmount = -1f;
    }

    public void findRailcannonValues()
    {
        //Railcannon -- WORKS
        UltraCooldownInfoWeapons[14].chargeAmount = MonoSingleton<WeaponCharges>.Instance.raicharge / 4.0f; 
        UltraCooldownInfoWeapons[14].usingChargeAmount = -1f;
    }

    public void findRocket_LauncherValues()
    {
        //Freezeframe Rocket Launcher Alt -- WORKS
        UltraCooldownInfoWeapons[15].chargeAmount = MonoSingleton<WeaponCharges>.Instance.rocketFreezeTime / 5f; 
        UltraCooldownInfoWeapons[15].usingChargeAmount = -1f;
        //S.R.S. Rocket Launcher Alt
        UltraCooldownInfoWeapons[16].chargeAmount = MonoSingleton<WeaponCharges>.Instance.rocketCannonballCharge; 
        UltraCooldownInfoWeapons[16].usingChargeAmount = weapon_srs.cbCharge;
        //Firestarter Rocket Launcher Alt -- WORKS
        UltraCooldownInfoWeapons[17].chargeAmount = MonoSingleton<WeaponCharges>.Instance.rocketNapalmFuel;  
        UltraCooldownInfoWeapons[17].usingChargeAmount = -1f;
        //Rocket Launcher Fire
        if(weapon.GetComponent<RocketLauncher>() != null)
        {
            UltraCooldownInfoWeapons[18].chargeAmount = 1f - weapon.GetComponent<RocketLauncher>().cooldown; 
        }
        else
        {
            UltraCooldownInfoWeapons[18].chargeAmount += Time.deltaTime;
            if(UltraCooldownInfoWeapons[18].chargeAmount >= 1.0f) {UltraCooldownInfoWeapons[18].chargeAmount = 1.0f;}
        }
        
        UltraCooldownInfoWeapons[18].usingChargeAmount = -1f;
    }

    public void findMiscValues()
    {
        //Fist -- WORKS
        UltraCooldownInfoWeapons[19].chargeAmount = 1.0f - (MonoSingleton<FistControl>.Instance.fistCooldown / 2.30f); //2.30 seems to be the max... not quite sure
        UltraCooldownInfoWeapons[19].usingChargeAmount = -1f;
    }

    float[] flashingIconOpacities = new float[UltraCooldownInfoWeapons.Length];
    public void onNewFullCharges()
    {
        for(int i = 0; i < UltraCooldownInfoWeapons.Length; i++)
        {
            if(UltraCooldownInfoWeapons[i].soundEnabled == false) {goto NoSound;}

            if(usingChargeAmounts[i] != UltraCooldownInfoWeapons[i].usingChargeAmount && UltraCooldownInfoWeapons[i].usingChargeAmount >= 1f) //this runs before soundEnabledWhileHeld check because... this only matters with the weapon held.
            {
                if(i == 11 && UltraCooldownInfoWeapons[i].chargeAmount < 1f) {goto NoSound;} //usingChargeAmount can be 1 even if chargeAmount < 1, probably dont want the sound unless the weapon is useable hence this line.
                csp.PlaySound(UltraCooldownInfoWeapons[i].filePathUsingCharge, i);
            }

            if(UltraCooldownInfoWeapons[i].soundEnabledWhileHeld == false)
            {
                if     (i == 0 && weapon.GetComponent<Revolver>() == weapon_piercer) {goto NoSound;}
                else if(i == 1 && weapon.GetComponent<Revolver>() == weapon_marksman) {goto NoSound;}
                else if(i == 2 && weapon.GetComponent<Revolver>() == weapon_sharpshooter) {goto NoSound;}
                else if(i == 3 && weapon.GetComponent<Revolver>() == weapon_piercer) {goto NoSound;}
                else if(i == 4 && weapon.GetComponent<Revolver>() == weapon_marksman) {goto NoSound;}
                else if(i == 5 && weapon.GetComponent<Revolver>() == weapon_sharpshooter) {goto NoSound;}
                else if(i == 6 && weapon == weapon_sawedOn) {goto NoSound;}
                else if(i == 7 && weapon == weapon_coreEject) {goto NoSound;}
                else if(i == 8 && weapon == weapon_pumpCharge) {goto NoSound;}
                else if(i == 9 && weapon == weapon_sawedOn) {goto NoSound;}
                else if(i == 10 && weapon.GetComponent<Nailgun>() == weapon_attractor) {goto NoSound;}
                else if(i == 11 && weapon.GetComponent<Nailgun>() == weapon_overheat) {goto NoSound;}
                else if(i == 12 && weapon.GetComponent<Nailgun>() == weapon_jumpstart) {goto NoSound;}
                else if(i == 13 && weapon.GetComponent<Nailgun>() == weapon_attractor) {goto NoSound;}
                else if(i == 14 && weapon.GetComponent<Railcannon>() != null) {goto NoSound;}
                else if(i == 15 && weapon.GetComponent<RocketLauncher>() == weapon_freezeframe) {goto NoSound;}
                else if(i == 16 && weapon.GetComponent<RocketLauncher>() == weapon_srs) {goto NoSound;}
                else if(i == 17 && weapon.GetComponent<RocketLauncher>() == weapon_firestarter) {goto NoSound;}
                else if(i == 18 && weapon.GetComponent<RocketLauncher>() != null) {goto NoSound;}
                else if(i == 19) {} //dont do anything for fist
                else if(i == 20 && weapon == weapon_coreEject) {goto NoSound;}
                else if(i == 21 && weapon == weapon_pumpCharge) {goto NoSound;}
            }
            //plays intermedieate sounds
            float value1 = UltraCooldownInfoWeapons[i].chargeAmount * UltraCooldownInfoWeapons[i].soundDivisions - (int)(UltraCooldownInfoWeapons[i].chargeAmount * UltraCooldownInfoWeapons[i].soundDivisions - 0.001f);
            float value2 = chargeAmounts[i] * UltraCooldownInfoWeapons[i].soundDivisions - (int)(chargeAmounts[i] * UltraCooldownInfoWeapons[i].soundDivisions - 0.001f); //should be less
            if(value1 > 1 && value2 < 1)
            {
                csp.PlaySound(UltraCooldownInfoWeapons[i].filePathFractionCharge, i);
            }

            //plays completely charged sound
            if(chargeAmounts[i] != UltraCooldownInfoWeapons[i].chargeAmount && UltraCooldownInfoWeapons[i].chargeAmount >= 1f)
            {
                csp.PlaySound(UltraCooldownInfoWeapons[i].filePathCharge, i);
            }
            NoSound:;

            //intermedieate
            float value3 = UltraCooldownInfoWeapons[i].chargeAmount * UltraCooldownInfoWeapons[i].numberFlashes - (int)(UltraCooldownInfoWeapons[i].chargeAmount * UltraCooldownInfoWeapons[i].numberFlashes - 0.001f);
            float value4 = chargeAmounts[i] * UltraCooldownInfoWeapons[i].numberFlashes - (int)(chargeAmounts[i] * UltraCooldownInfoWeapons[i].numberFlashes - 0.001f); //should be less
            if(value3 > 1 && value4 < 1)
            {
                flashingIconOpacities[i] = 1;
            }
            //completely charged
            if(chargeAmounts[i] != UltraCooldownInfoWeapons[i].chargeAmount && UltraCooldownInfoWeapons[i].chargeAmount >= 1f)
            {
                flashingIconOpacities[i] = 1;
            }

            usingChargeAmounts[i] = UltraCooldownInfoWeapons[i].usingChargeAmount;
            chargeAmounts[i] = UltraCooldownInfoWeapons[i].chargeAmount;
        }
    }

    bool tempDisable = false;
    bool configCreated = false;
    public void Update() 
    {
        if(configCreated == false && MonoSingleton<ColorBlindSettings>.Instance != null)
        {
            PluginConfig.CreateConfig();
            configCreated = true;
        }
        if(!modEnabled){return;}
        if(!IsGameplayScene()){return;}
        if(IsMenu()){return;}

        if(showGUIKeyToggle == false)
        {
            if(showGUIKeyCode != KeyCode.None && Input.GetKey(showGUIKeyCode) == false) {return;}
        }
        else
        {
            if(showGUIKeyCode != KeyCode.None && Input.GetKeyDown(showGUIKeyCode) == true) {tempDisable = !tempDisable;}
        }

        findWeapons();

        findRevolverValues();
        findShotgunValues();
        findNailgunValues();
        findRailcannonValues();
        findRocket_LauncherValues();
        findMiscValues();

        onNewFullCharges();

        //displaying stuff is handled in OnGUI
    }

    public void DrawClassicHUDElement(int i)
    {
        if(tempDisable) {return;}
        UltraCooldownInfoWeapon UCIWepConf = UltraCooldownInfoWeapons[i];
        
        if(UCIWepConf.flipped == true)
        {
            if(weapon.GetComponent<Revolver>() == weapon_sharpshooter && weapon_sharpshooter.altVersion == false && UltraCooldownInfoWeapons[i].chargeAmount >= 0.3333 && UltraCooldownInfoWeapons[i].usingChargeAmount > 0f) 
            { //special case; only weapon that both charges up and has multiple charges.
                if(UltraCooldownInfoWeapons[i].usingChargeAmount >= 0.995f)
                {
                    UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos, UCIWepConf.yPos, UCIWepConf.width, UCIWepConf.height), barTexture, ScaleMode.StretchToFill, true, 0, UCIWepConf.usingChargeChargedColor, 0, 0);
                }
                else
                {
                    UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos + UCIWepConf.width - UCIWepConf.width * UltraCooldownInfoWeapons[i].usingChargeAmount, UCIWepConf.yPos, -UCIWepConf.width * (1 - UltraCooldownInfoWeapons[i].usingChargeAmount), UCIWepConf.height), barTexture, ScaleMode.StretchToFill, true, 0, normalSharpshooterBackgroundChargingColor, 0, 0);
                    UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos + UCIWepConf.width, UCIWepConf.yPos, -UCIWepConf.width * UltraCooldownInfoWeapons[i].usingChargeAmount, UCIWepConf.height), barTexture, ScaleMode.StretchToFill, true, 0, UCIWepConf.usingChargeColor, 0, 0);
                }
                goto beforeDrawSegments;
            }
            if(UltraCooldownInfoWeapons[i].chargeAmount >= 1)
            {
                UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos, UCIWepConf.yPos, UCIWepConf.width, UCIWepConf.height), barTexture, ScaleMode.StretchToFill, true, 0, UCIWepConf.chargedColor, 0, 0);
                if(UltraCooldownInfoWeapons[i].usingChargeAmount > 0f) //not everything "charges". Things that dont will have negative values.
                {
                    if(UltraCooldownInfoWeapons[i].usingChargeAmount >= 0.99f)
                    {
                        UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos, UCIWepConf.yPos, UCIWepConf.width, UCIWepConf.height), barTexture, ScaleMode.StretchToFill, true, 0, UCIWepConf.usingChargeChargedColor, 0, 0);
                    }
                    else
                    {
                        UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos + UCIWepConf.width, UCIWepConf.yPos, -UCIWepConf.width * UltraCooldownInfoWeapons[i].usingChargeAmount, UCIWepConf.height), barTexture, ScaleMode.StretchToFill, true, 0, UCIWepConf.usingChargeColor, 0, 0);
                    }
                }
            }
            else
            {
                UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos + UCIWepConf.width, UCIWepConf.yPos, -UCIWepConf.width * UltraCooldownInfoWeapons[i].chargeAmount, UCIWepConf.height), barTexture, ScaleMode.StretchToFill, true, 0, UCIWepConf.chargingColor, 0, 0);
                UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos + UCIWepConf.width - UCIWepConf.width * UltraCooldownInfoWeapons[i].chargeAmount, UCIWepConf.yPos, -UCIWepConf.width * (1 - UltraCooldownInfoWeapons[i].chargeAmount), UCIWepConf.height), barTexture, ScaleMode.StretchToFill, true, 0, UCIWepConf.remainingColor, 0, 0);
            }
            beforeDrawSegments:;
        }
        else
        {   
            if(weapon.GetComponent<Revolver>() == weapon_sharpshooter && weapon_sharpshooter.altVersion == false && UltraCooldownInfoWeapons[i].chargeAmount >= 0.3333 && UltraCooldownInfoWeapons[i].usingChargeAmount > 0f) 
            { //special case; only weapon that both charges up and has multiple charges.
                if(UltraCooldownInfoWeapons[i].usingChargeAmount >= 0.995f)
                {
                    UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos, UCIWepConf.yPos, UCIWepConf.width, UCIWepConf.height), barTexture, ScaleMode.StretchToFill, true, 0, UCIWepConf.usingChargeChargedColor, 0, 0);
                }
                else
                {
                    UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos + UCIWepConf.width * UltraCooldownInfoWeapons[i].usingChargeAmount, UCIWepConf.yPos, UCIWepConf.width * (1 - UltraCooldownInfoWeapons[i].usingChargeAmount), UCIWepConf.height), barTexture, ScaleMode.StretchToFill, true, 0, normalSharpshooterBackgroundChargingColor, 0, 0);
                    UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos, UCIWepConf.yPos, UCIWepConf.width * UltraCooldownInfoWeapons[i].usingChargeAmount, UCIWepConf.height), barTexture, ScaleMode.StretchToFill, true, 0, UCIWepConf.usingChargeColor, 0, 0);
                }
                goto beforeDrawSegments;
            }
            if(UltraCooldownInfoWeapons[i].chargeAmount >= 1)
            {
                UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos, UCIWepConf.yPos, UCIWepConf.width, UCIWepConf.height), barTexture, ScaleMode.StretchToFill, true, 0, UCIWepConf.chargedColor, 0, 0);
                if(UltraCooldownInfoWeapons[i].usingChargeAmount > 0f) //not everything "charges". Things that dont will have negative values.
                {
                    if(UltraCooldownInfoWeapons[i].usingChargeAmount >= 0.99f)
                    {
                        UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos, UCIWepConf.yPos, UCIWepConf.width, UCIWepConf.height), barTexture, ScaleMode.StretchToFill, true, 0, UCIWepConf.usingChargeChargedColor, 0, 0);
                    }
                    else
                    {
                        UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos, UCIWepConf.yPos, UCIWepConf.width * UltraCooldownInfoWeapons[i].usingChargeAmount, UCIWepConf.height), barTexture, ScaleMode.StretchToFill, true, 0, UCIWepConf.usingChargeColor, 0, 0);
                    }
                }
            }
            else
            {
                UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos, UCIWepConf.yPos, UCIWepConf.width * UltraCooldownInfoWeapons[i].chargeAmount, UCIWepConf.height), barTexture, ScaleMode.StretchToFill, true, 0, UCIWepConf.chargingColor, 0, 0);
                UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos + UCIWepConf.width * UltraCooldownInfoWeapons[i].chargeAmount, UCIWepConf.yPos, UCIWepConf.width * (1 - UltraCooldownInfoWeapons[i].chargeAmount), UCIWepConf.height), barTexture, ScaleMode.StretchToFill, true, 0, UCIWepConf.remainingColor, 0, 0);
            }
            beforeDrawSegments:;
        }

        UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos - UCIWepConf.borderThickness, UCIWepConf.yPos - UCIWepConf.borderThickness, UCIWepConf.width + 2 * UCIWepConf.borderThickness , UCIWepConf.borderThickness), barTexture, ScaleMode.StretchToFill, true, 0, UCIWepConf.backgroundColor, 0, 0); //top
        UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos - UCIWepConf.borderThickness, UCIWepConf.height + UCIWepConf.yPos, UCIWepConf.width + 2 * UCIWepConf.borderThickness , UCIWepConf.borderThickness), barTexture, ScaleMode.StretchToFill, true, 0, UCIWepConf.backgroundColor, 0, 0); //bottom
        UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos - UCIWepConf.borderThickness, UCIWepConf.yPos - UCIWepConf.borderThickness, UCIWepConf.borderThickness, UCIWepConf.height + 2 * UCIWepConf.borderThickness), barTexture, ScaleMode.StretchToFill, true, 0, UCIWepConf.backgroundColor, 0, 0); //left
        UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos + UCIWepConf.width, UCIWepConf.yPos - UCIWepConf.borderThickness, UCIWepConf.borderThickness, UCIWepConf.height + 2 * UCIWepConf.borderThickness), barTexture, ScaleMode.StretchToFill, true, 0, UCIWepConf.backgroundColor, 0, 0); //right

        for(int j = 0; j < UCIWepConf.numberDivisions - 1; j++)
        {
            UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos + UCIWepConf.width * ((1.0f + j) / UCIWepConf.numberDivisions) - UCIWepConf.divisionWidth / 2, UCIWepConf.yPos, UCIWepConf.divisionWidth, UCIWepConf.height), barTexture, ScaleMode.StretchToFill, true, 0, UCIWepConf.backgroundColor, 0, 0);
        }
        if(UCIWepConf.iconEnabled == true)
        {
            Color color = UCIWepConf.chargeBarIconColor;

            Texture2D texture = barTexture;
            texture = GetTextureByIndex(i);

            float barWidth = UCIWepConf.width + UCIWepConf.borderThickness * 2;
            float barHeight = UCIWepConf.height + UCIWepConf.borderThickness * 2;

            float iconWidth = 0f; float iconHeight = 0f;

            float flipXBonux = 0f;
            if(UCIWepConf.flipped == true) {flipXBonux = 1f;}
            if(UCIWepConf.iconSide == SideEnum.Left)
            {
                iconHeight = barHeight;
                iconWidth = ((float)texture.width / texture.height) * barHeight;
                UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos - iconWidth - UCIWepConf.iconDistance - UCIWepConf.borderThickness, UCIWepConf.yPos - UCIWepConf.borderThickness, iconWidth, barHeight), texture, ScaleMode.StretchToFill, true, 0, color, 0, 0);
            }
            else if(UCIWepConf.iconSide == SideEnum.Right)
            {
                iconHeight = barHeight;
                iconWidth = ((float)texture.width / texture.height) * barHeight;
                UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos + UCIWepConf.borderThickness + barWidth * 2 + UCIWepConf.iconDistance, UCIWepConf.yPos - UCIWepConf.borderThickness, -iconWidth, iconHeight), texture, ScaleMode.StretchToFill, true, 0, color, 0, 0);
            }
            else if(UCIWepConf.iconSide == SideEnum.Top)
            {
                iconHeight = ((float)texture.height / texture.width) * barWidth;
                iconWidth = barWidth;
                UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos - UCIWepConf.borderThickness + barWidth * flipXBonux, UCIWepConf.yPos - iconHeight - UCIWepConf.iconDistance - UCIWepConf.borderThickness, iconWidth * (1.0f - 2.0f * flipXBonux), iconHeight), texture, ScaleMode.StretchToFill, true, 0, color, 0, 0);
            }
            else if(UCIWepConf.iconSide == SideEnum.Bottom)
            {
                iconHeight = ((float)texture.height / texture.width) * barWidth;
                iconWidth = barWidth;
                UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPos - UCIWepConf.borderThickness + barWidth * flipXBonux, UCIWepConf.yPos + barHeight + UCIWepConf.iconDistance - UCIWepConf.borderThickness, iconWidth * (1.0f - 2.0f * flipXBonux), iconHeight), texture, ScaleMode.StretchToFill, true, 0, color, 0, 0);
            }
        }
    }

    public void DrawWeaponImageSplitHUDElement(int i)
    {
        if(tempDisable) {return;}
        UltraCooldownInfoWeapon UCIWepConf = UltraCooldownInfoWeapons[i];
        Color color1 = new Color(1.0f, 1.0f, 1.0f, 1.0f); //charging
        Color color2 = new Color(1.0f, 1.0f, 1.0f, 1.0f); //remaining
        Color color3 = new Color(1.0f, 1.0f, 1.0f, 1.0f); //charged

        float fillAmount = UCIWepConf.chargeAmount;
        if(UCIWepConf.chargeAmount >= 1 && UCIWepConf.usingChargeAmount >= 0f) {fillAmount = UCIWepConf.usingChargeAmount;}

        Texture2D texture = GetTextureByIndex(i);

        if(UCIWepConf.chargeAmount < 1)
        {
            color1 = UCIWepConf.chargingColor;
            color2 = UCIWepConf.remainingColor;
            color3 = UCIWepConf.chargedColor;
        }
        if(UCIWepConf.chargeAmount >= 1)
        {
            color1 = UCIWepConf.chargedColor;
            color2 = UCIWepConf.chargedColor;
            color3 = UCIWepConf.chargedColor;
        }
        if(UCIWepConf.usingChargeAmount > 0)
        {
            color1 = UCIWepConf.usingChargeColor;
            color2 = UCIWepConf.chargedColor;
            if(i == 2 && weapon_sharpshooter.altVersion == false) {color2 = normalSharpshooterBackgroundChargingColor; fillAmount = UCIWepConf.usingChargeAmount;}
            color3 = UCIWepConf.usingChargeColor; //I think it looks bad with usingChargeChargedColor
        }
        if(UCIWepConf.usingChargeAmount >= 0.995f)
        {
            color1 = UCIWepConf.usingChargeChargedColor;
            color2 = UCIWepConf.usingChargeChargedColor;
            color3 = UCIWepConf.usingChargeChargedColor;
        }

        if(UCIWepConf.flipped == false)
        {
            for(int j = 0; j < UCIWepConf.numberDivisions; j++)
            {
                float fraction = (j + 0.0f) / UCIWepConf.numberDivisions;
                if(fillAmount * UCIWepConf.numberDivisions >= j + 1)
                {
                    GUI.BeginGroup(new Rect(UCIWepConf.xPos + UCIWepConf.width * fraction, UCIWepConf.yPos, UCIWepConf.width * (1.0f / UCIWepConf.numberDivisions), UCIWepConf.height * 1.0f));
                    GUI.DrawTexture(new Rect(-UCIWepConf.width * fraction, 0, UCIWepConf.width, UCIWepConf.height), texture, ScaleMode.StretchToFill, true, 0, color3, 0, 0);
                    GUI.EndGroup();
                }
                else
                {
                    GUI.BeginGroup(new Rect(UCIWepConf.xPos + UCIWepConf.width * fraction, UCIWepConf.yPos, UCIWepConf.width * (fillAmount - fraction), UCIWepConf.height * 1.0f));
                    GUI.DrawTexture(new Rect(-UCIWepConf.width * fraction, 0, UCIWepConf.width, UCIWepConf.height), texture, ScaleMode.StretchToFill, true, 0, color1, 0, 0);
                    GUI.EndGroup();
                    break;
                }
            }
            GUI.BeginGroup(new Rect(UCIWepConf.xPos + UCIWepConf.width * fillAmount, UCIWepConf.yPos, UCIWepConf.width * 1.0f, UCIWepConf.height * 1.0f));
            GUI.DrawTexture(new Rect(-UCIWepConf.width * fillAmount, 0, UCIWepConf.width, UCIWepConf.height), texture, ScaleMode.StretchToFill, true, 0, color2, 0, 0);
            GUI.EndGroup();
        }
        else //flipped 
        {
            for(int j = 0; j < UCIWepConf.numberDivisions; j++)
            {
                float fraction = (j + 0.0f) / UCIWepConf.numberDivisions;
                if(fillAmount * UCIWepConf.numberDivisions >= j + 1)
                {
                    GUI.BeginGroup(new Rect(UCIWepConf.xPos + UCIWepConf.width * fraction, UCIWepConf.yPos, UCIWepConf.width * (1.0f / UCIWepConf.numberDivisions), UCIWepConf.height * 1.0f));
                    GUI.DrawTexture(new Rect(UCIWepConf.width * (1.0f - fraction), 0, -UCIWepConf.width, UCIWepConf.height), texture, ScaleMode.StretchToFill, true, 0, color3, 0, 0);
                    GUI.EndGroup();
                }
                else
                {
                    GUI.BeginGroup(new Rect(UCIWepConf.xPos + UCIWepConf.width * fraction, UCIWepConf.yPos, UCIWepConf.width * (fillAmount - fraction), UCIWepConf.height * 1.0f));
                    GUI.DrawTexture(new Rect(UCIWepConf.width * (1.0f - fraction), 0, -UCIWepConf.width, UCIWepConf.height), texture, ScaleMode.StretchToFill, true, 0, color1, 0, 0);
                    GUI.EndGroup();
                    break;
                }
            }
            GUI.BeginGroup(new Rect(UCIWepConf.xPos + UCIWepConf.width * fillAmount, UCIWepConf.yPos, UCIWepConf.width * 1.0f, UCIWepConf.height * 1.0f));
            GUI.DrawTexture(new Rect(UCIWepConf.width * (1.0f - fillAmount), 0, -UCIWepConf.width, UCIWepConf.height), texture, ScaleMode.StretchToFill, true, 0, color2, 0, 0);
            GUI.EndGroup();
        }

        if(UCIWepConf.divisionWidth == 0) {return;}
        for(int j = 0; j < UCIWepConf.numberDivisions - 1; j++) //draw lines
        {   //looks like crap but its here anyways
            float fraction = (j + 1.0f) / UCIWepConf.numberDivisions;
            GUI.BeginGroup(new Rect(UCIWepConf.xPos + UCIWepConf.width * fraction, UCIWepConf.yPos, UCIWepConf.divisionWidth, UCIWepConf.height * 1.0f));
            GUI.DrawTexture(new Rect(-UCIWepConf.width * fraction, 0, UCIWepConf.width, UCIWepConf.height), texture, ScaleMode.StretchToFill, true, 0, UCIWepConf.backgroundColor, 0, 0);
            GUI.EndGroup();
        }
    }

    public void DrawWeaponImageGradientHUDElement(int i)
    {
        if(tempDisable) {return;}
        UltraCooldownInfoWeapon UCIWepConf = UltraCooldownInfoWeapons[i];
        Color color1 = new Color(1.0f, 1.0f, 1.0f, 1.0f); //charged
        Color color2 = new Color(1.0f, 1.0f, 1.0f, 1.0f); //remaining
        Color color3 = new Color(1.0f, 1.0f, 1.0f, 1.0f); //in between

        float fillAmount = UCIWepConf.chargeAmount;
        if(UCIWepConf.chargeAmount >= 1) {fillAmount = UCIWepConf.usingChargeAmount;}
        float newR = 0f; float newG = 0f; float newB = 0f;
        if(UCIWepConf.chargeAmount < 1)
        {
            color1 = UCIWepConf.chargedColor; 
            color2 = UCIWepConf.remainingColor;
            float fraction = (fillAmount * UCIWepConf.numberDivisions) - (int)(fillAmount * UCIWepConf.numberDivisions); //just the decimal
            newR = UCIWepConf.chargingColor.r * fraction + UCIWepConf.remainingColor.r * (1.0f - fraction);
            newG = UCIWepConf.chargingColor.g * fraction + UCIWepConf.remainingColor.g * (1.0f - fraction);
            newB = UCIWepConf.chargingColor.b * fraction + UCIWepConf.remainingColor.b * (1.0f - fraction);
            color3 = new Color(newR, newG, newB, UCIWepConf.opacity);
        }
        else {color1 = UCIWepConf.chargedColor; color2 = UCIWepConf.chargedColor; color3 = UCIWepConf.chargedColor;}

        if(UCIWepConf.usingChargeAmount > 0)
        {
            color1 = UCIWepConf.usingChargeChargedColor; 
            color2 = UCIWepConf.chargedColor;
            float fraction = (fillAmount * UCIWepConf.numberDivisions) - (int)(fillAmount * UCIWepConf.numberDivisions); //just the decimal
            newR = UCIWepConf.usingChargeColor.r * fraction + UCIWepConf.chargedColor.r * (1.0f - fraction);
            newG = UCIWepConf.usingChargeColor.g * fraction + UCIWepConf.chargedColor.g * (1.0f - fraction);
            newB = UCIWepConf.usingChargeColor.b * fraction + UCIWepConf.chargedColor.b * (1.0f - fraction);
            if(i == 2 && weapon_sharpshooter.altVersion == false) 
            {
                color2 = normalSharpshooterBackgroundChargingColor;
                fillAmount = UCIWepConf.usingChargeAmount;
                fraction = (fillAmount * UCIWepConf.numberDivisions) - (int)(fillAmount * UCIWepConf.numberDivisions);
                newR = UCIWepConf.usingChargeColor.r * fraction + normalSharpshooterBackgroundChargingColor.r * (1.0f - fraction);
                newG = UCIWepConf.usingChargeColor.g * fraction + normalSharpshooterBackgroundChargingColor.g * (1.0f - fraction);
                newB = UCIWepConf.usingChargeColor.b * fraction + normalSharpshooterBackgroundChargingColor.b * (1.0f - fraction);
            }
            color3 = new Color(newR, newG, newB, UCIWepConf.opacity);
        }
        if(UCIWepConf.usingChargeAmount >= 1f)
        {
            color1 = UCIWepConf.usingChargeChargedColor; color2 = UCIWepConf.usingChargeChargedColor; color3 = UCIWepConf.usingChargeChargedColor;
        }

        Texture2D texture = GetTextureByIndex(i);;

        bool drawnGradient = false;
        if(UCIWepConf.flipped == false)
        {
            for(int j = 0; j < UCIWepConf.numberDivisions; j++)
            {
                float fraction = (j + 0.0f) / UCIWepConf.numberDivisions;
                if(fillAmount * UCIWepConf.numberDivisions >= j + 1)
                {
                    GUI.BeginGroup(new Rect(UCIWepConf.xPos + UCIWepConf.width * fraction, UCIWepConf.yPos, UCIWepConf.width * (1.0f / UCIWepConf.numberDivisions), UCIWepConf.height * 1.0f));
                    GUI.DrawTexture(new Rect(-UCIWepConf.width * fraction, 0, UCIWepConf.width, UCIWepConf.height), texture, ScaleMode.StretchToFill, true, 0, color1, 0, 0);
                    GUI.EndGroup();
                }
                else
                {
                    if(drawnGradient)
                    {
                        GUI.BeginGroup(new Rect(UCIWepConf.xPos + UCIWepConf.width * fraction, UCIWepConf.yPos, UCIWepConf.width * (1.0f / UCIWepConf.numberDivisions), UCIWepConf.height * 1.0f));
                        GUI.DrawTexture(new Rect(-UCIWepConf.width * fraction, 0, UCIWepConf.width, UCIWepConf.height), texture, ScaleMode.StretchToFill, true, 0, color2, 0, 0);
                        GUI.EndGroup();
                    }
                    else
                    {
                        drawnGradient = true;
                        GUI.BeginGroup(new Rect(UCIWepConf.xPos + UCIWepConf.width * fraction, UCIWepConf.yPos, UCIWepConf.width * (1.0f / UCIWepConf.numberDivisions), UCIWepConf.height * 1.0f));
                        GUI.DrawTexture(new Rect(-UCIWepConf.width * fraction, 0, UCIWepConf.width, UCIWepConf.height), texture, ScaleMode.StretchToFill, true, 0, color3, 0, 0);
                        GUI.EndGroup();
                    }
                }
            }
        }
        else //flipped
        {
            for(int j = 0; j < UCIWepConf.numberDivisions; j++)
            {
                float fraction = (j + 0.0f) / UCIWepConf.numberDivisions;
                if(fillAmount * UCIWepConf.numberDivisions >= j + 1)
                {
                    GUI.BeginGroup(new Rect(UCIWepConf.xPos + UCIWepConf.width * fraction, UCIWepConf.yPos, UCIWepConf.width * (1.0f / UCIWepConf.numberDivisions), UCIWepConf.height * 1.0f));
                    GUI.DrawTexture(new Rect(UCIWepConf.width * (1.0f - fraction), 0, -UCIWepConf.width, UCIWepConf.height), texture, ScaleMode.StretchToFill, true, 0, color1, 0, 0);
                    GUI.EndGroup();
                }
                else
                {
                    if(drawnGradient)
                    {
                        GUI.BeginGroup(new Rect(UCIWepConf.xPos + UCIWepConf.width * fraction, UCIWepConf.yPos, UCIWepConf.width * (1.0f / UCIWepConf.numberDivisions), UCIWepConf.height * 1.0f));
                        GUI.DrawTexture(new Rect(UCIWepConf.width * (1.0f - fraction), 0, -UCIWepConf.width, UCIWepConf.height), texture, ScaleMode.StretchToFill, true, 0, color2, 0, 0);
                        GUI.EndGroup();
                    }
                    else
                    {
                        drawnGradient = true;
                        GUI.BeginGroup(new Rect(UCIWepConf.xPos + UCIWepConf.width * fraction, UCIWepConf.yPos, UCIWepConf.width * (1.0f / UCIWepConf.numberDivisions), UCIWepConf.height * 1.0f));
                        GUI.DrawTexture(new Rect(UCIWepConf.width * (1.0f - fraction), 0, -UCIWepConf.width, UCIWepConf.height), texture, ScaleMode.StretchToFill, true, 0, color3, 0, 0);
                        GUI.EndGroup();
                    }
                }
            }
        }

        if(UCIWepConf.divisionWidth == 0) {return;}
        for(int j = 0; j < UCIWepConf.numberDivisions - 1; j++) //draw lines
        {   //looks like crap but its here anyways
            float fraction = (j + 1.0f) / UCIWepConf.numberDivisions;
            GUI.BeginGroup(new Rect(UCIWepConf.xPos + UCIWepConf.width * fraction, UCIWepConf.yPos, UCIWepConf.divisionWidth, UCIWepConf.height * 1.0f));
            GUI.DrawTexture(new Rect(-UCIWepConf.width * fraction, 0, UCIWepConf.width, UCIWepConf.height), texture, ScaleMode.StretchToFill, true, 0, UCIWepConf.backgroundColor, 0, 0);
            GUI.EndGroup();
        }
    }
    public void tryDrawFlashingIcon(int i)
    {
        UltraCooldownInfoWeapon UCIWepConf = UltraCooldownInfoWeapons[i];
        Color color = UCIWepConf.colorFlash;
        Texture2D texture = GetTextureByIndex(i);

        //not entirely accurate but whaddya gonna do
        bool wasJustCharged = (UCIWepConf.chargeAmount >= 1f) || UCIWepConf.chargeAmount < 1.0f / UCIWepConf.numberFlashes; 

        if((wasJustCharged && UCIWepConf.lengthFlash > 0) || UCIWepConf.numberFlashes == 1) {flashingIconOpacities[i] += -Time.deltaTime / UCIWepConf.lengthFlash;}
        else if(UCIWepConf.lengthMiniFlash > 0) {flashingIconOpacities[i] += -Time.deltaTime / UCIWepConf.lengthMiniFlash;}
        if(flashingIconOpacities[i] < 0) {flashingIconOpacities[i] = 0;}

        float flipX = 1.0f;
        if(UCIWepConf.iconFlipped) {flipX = -1.0f;}

        if(UCIWepConf.flashType == FlashTypeEnum.LinearFade && flashingIconOpacities[i] > 0)
        {
            color = new Color(color.r, color.g, color.b, flashingIconOpacities[i]);
            UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPosFlash + UCIWepConf.widthFlash * (1.0f - flipX) / 2f, UCIWepConf.yPosFlash, UCIWepConf.widthFlash * flipX, UCIWepConf.heightFlash), texture, ScaleMode.StretchToFill, true, 0, color, 0, 0);
        }
        else if(UCIWepConf.flashType == FlashTypeEnum.NoFade && flashingIconOpacities[i] > 0)
        {
            UnityEngine.GUI.DrawTexture(new Rect(UCIWepConf.xPosFlash + UCIWepConf.widthFlash * (1.0f - flipX) / 2f, UCIWepConf.yPosFlash, UCIWepConf.widthFlash * flipX, UCIWepConf.heightFlash), texture, ScaleMode.StretchToFill, true, 0, color, 0, 0);
        }
    }
    void OnGUI() //called every frame
    {
        if(!modEnabled || !IsGameplayScene() || (IsMenu() && showIconsInMenu == false)){return;}
        if(MonoSingleton<NewMovement>.Instance.hp <= 0) {return;} //dont display this when dead cause it draws over everything if enabled

        for(int i = 0; i < UltraCooldownInfoWeapons.Length; i++)
        {
            UltraCooldownInfoWeapon UCIWepConf = UltraCooldownInfoWeapons[i];
            if(UCIWepConf.enabled == true)
            {
                if(UCIWepConf.chargeBarType == ChargeBarTypeEnum.Classic)
                {
                    DrawClassicHUDElement(i);
                }
                else if(UCIWepConf.chargeBarType == ChargeBarTypeEnum.WeaponImageSplit)
                {
                    DrawWeaponImageSplitHUDElement(i);
                }
                else if(UCIWepConf.chargeBarType == ChargeBarTypeEnum.WeaponImageGradient)
                {
                    DrawWeaponImageGradientHUDElement(i);
                }
                if(flashingIconOpacities[i] > 0)
                {
                    tryDrawFlashingIcon(i);
                }
            }
        }
    }
}
