using System;
using UnityEngine;

public class InputBroker
{
	static string forcedKeyCode;

	public static bool GetButtonDown(string aKey)
	{
		if (aKey == forcedKeyCode) {
			forcedKeyCode = string.Empty;
			return true;
		}
		else
			return Input.GetButtonDown(aKey);
	}

	public static void SetButtonDown(string aKey)
	{
		forcedKeyCode = aKey;
	}
}

