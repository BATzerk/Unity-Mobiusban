﻿using UnityEngine;
using System.Collections;

public static class PlatformManager {
	#if UNITY_IOS || UNITY_ANDROID
	public static bool IsMobile = true;
	#else
	public static bool IsMobile = false;
	#endif
    
	public static bool IsiOS () {
        return Application.platform == RuntimePlatform.IPhonePlayer;
    }
	public static bool IsAndroid () {
        return Application.platform == RuntimePlatform.Android;
    }
	public static bool IsLinux () {
		return Application.platform == RuntimePlatform.LinuxPlayer;
	}
	public static bool IsMac () {
		return Application.platform == RuntimePlatform.OSXEditor
			|| Application.platform == RuntimePlatform.OSXPlayer;
	}
	public static bool IsWindows () {
		return Application.platform == RuntimePlatform.WindowsEditor
			|| Application.platform == RuntimePlatform.WindowsPlayer;
	}
	public static bool IsPlayStation () {
		return Application.platform == RuntimePlatform.PS4;
	}
	public static bool IsXbox () {
		return Application.platform == RuntimePlatform.XboxOne;
	}


	public static bool IsConsole () {
		return Application.isConsolePlatform;
	}
	public static bool IsPC () {
		return IsLinux () || IsMac () || IsWindows ();
	}


	public static string Debug_GetPlatformName () {
		if (IsAndroid()) { return "Android"; }
		if (IsiOS()) { return "iOS"; }
		if (IsLinux()) { return "Linux"; }
		if (IsMac()) { return "Mac"; }
		if (IsWindows()) { return "Windows"; }
		if (IsPlayStation()) { return "PlayStation"; }
		if (IsXbox()) { return "Xbox"; }
		return "Unknown";
	}

}
