# Tab Extension

## Requirements

- [MelonLoader 0.5.x](https://melonwiki.xyz/)

## Features

This mod basically just replaces the layout of the QuickMenu tabs and make it capable for multiple rows to prevent overflowing.

### **None of the existing mods have to change any code. TabExtension will not break the default way to add tab buttons.**

## Tab Sorting

You can customize the position of all tabs (includes vanilla and mod tabs) with the configuration file found in "UserData/TabExtension/TabSorting.json". You just have to change the numbers of the entries (index 1 is first tab) to change their positions. To reload the config just disable and enable the "TabSorting" with UIExpansionKit (It's disabled by default).

**Example of "UserData/TabExtension/TabSorting.json"**
```json
{
	"QuickMenuDashboard": 1,
	"QuickMenuNotifications": 2,
	"QuickMenuHere": 3,
	"QuickMenuCamera": 4,
	"QuickMenuAudioSettings": 5,
	"QuickMenuSettings": 6,
	"QuickMenuDevTools": 7,
	"QuickMenuReModReModCE": 8,
	"QuickMenuScreenshotManager": 9,
	"emmVRC_MainMenu": 10,
	"QuickMenuFaceTracking": 11
}
```

---

### Example Screenshot

![Screenshot](https://i.imgur.com/6VQ9WyW.png)