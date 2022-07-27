# Disclaimer

Since 27/07/2022 VRChat implemented [Easy Anti-Cheat](https://easy.ac) into their game.
This decision is horrible and will totally harm VRChat more than it will help.

They basically removed all access to Quality of Life mods including serval protections mods against common types of crashing and also all the mods that added features to the game which the community very much wanted but VRChat takes a shit ton of time to implement barely anything of it.

If you think EAC will prevent malicious people from harming you, you're wrong. The Anti-Cheat can be bypassed and they will continue doing their stuff.

The community tried to stop this with as much effort as possible, including the most upvoted [Feedback Post](https://feedback.vrchat.com/open-beta/p/eac-in-a-social-vr-game-creates-more-problems-than-it-solves) with 22k votes, a [Petition](https://www.change.org/p/vrchat-delete-anticheat-system) with almost 14.000 signatures and serval YouTube videos, posts and general social media activity. But they haven't listened to us.

**If you're currently subscribed to VRC+ then please consider of cancelling the subscription and leaving the game entirely.**

Also check out [ChilloutVR](https://discord.gg/abi) and [NeosVR](https://discord.gg/NeosVR).

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
