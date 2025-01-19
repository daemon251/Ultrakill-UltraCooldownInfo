# WARNING

This mod is in development. Expect issues. Contact me at discord: daemon8363 if you encounter problems.

Mod updates will reset any of the custom sounds that you use for the mod. Keep a seperate copy.

# OVERVIEW

This mod provides a way to view the cooldowns of all of your weapons even when they are not in hand. Indicators can be placed on the screen anywhere you want, in whatever size you want, in a couple of styles. 
This mod also lets you play a sound when a specific weapon's cooldown is over. 

# EXAMPLES

*Revolver Example HUD*

![UltraCooldownInfo0](https://raw.githubusercontent.com/daemon251/Ultrakill-UltraCooldownInfo/refs/heads/main/releaseContent/revolverHud.gif)

# CONFIGURATION DETAILS

Everything in the player arsenal with a charge bar has an entry in the main configuration panel. Every entry has three options: Charge Bar, Sound, Flashing Icon.

## GENERAL

Images are shown starting at the top left, and extend a distance (width) rightward and a distance (height) downward. 

The cooldowns can be split into 'divisions', which lets the charge bar/sound/icon do custom behavior throughout the charge. This is useful for weapons like 
the marksman, which has 4 charges. Note that throughout the mod, it is divisions TOTAL not divisions ADDED (so one division corresponds to doing the behavior upon completely fully charged).

## Charge Bar 

There are three types of styles that the charge bar can be. Classic simply shows two rectangles, representing the amount charged and the amount of charge remaining. A background can also be applied to classic bars.

Weapon Image - Split works a similar way, except instead of being two rectangles, it is two segments of the weapon image in question, each correspondingly colored and sized.

Weapon Image - Gradient also uses the weapon image, but colors the entirety of it between the amount charged and the amount of charge remaining. 

Due to the way that the mod works, charge bars will render over each other, and not nessecarily in the way that they are ordered in the menu. 

Weapon image charge bars may have artifacts if you have multiple divisions. These artifacts can be removed by adjusting the width by 1 either way until they disappear. 

## Sound 

Sounds can be played when the weapon is fully charged or if a division is charged. You are able to make up to eight custom sounds (they must be in WAV and be named customSound1.wav or customSound2.wav ... etc.).
Mod updates will overwrite your custom sounds, so you should save them in a seperate location in case I update the mod. 

## Flashing Icon 

Shows a flashing icon of the weapon when it is fully charged (or partially charged if you have multiple divisions). 
Flashing icons are rendered over the charge bars, so placing a flashing icon over a Weapon Image charge bar works and can look nice.


