using System;
using UnityEngine;

public class InputBroker
{
	static KeyCode forcedKeyCode;

	public static bool GetKeyDown(KeyCode aKey)
	{
		if (aKey == forcedKeyCode) {
			forcedKeyCode = KeyCode.None;
			return true;
		}
		else
			return Input.GetKeyDown(aKey);
	}

	public static void SetKeyDown(KeyCode aKey)
	{
		forcedKeyCode = aKey;
	}
}

