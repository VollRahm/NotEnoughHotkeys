// This is just a higher level keyboard hook that needs to be in a different Assembly
//

#include "pch.h"
#include "framework.h"
#include "NEH_KbdHook.h"

HWND callback_reciever = NULL;
HHOOK kbHook = NULL;

BOOL alreadyHooked = false;

static LRESULT CALLBACK HookCallback(int nCode, WPARAM wParam, LPARAM lParam)
{
	if (nCode >= 0)
	{
		LRESULT blockKey = SendMessage(callback_reciever, WM_HOOK, wParam, lParam);
		if (blockKey)
		{
			return 1;
		}
	}
	return CallNextHookEx(kbHook, nCode, wParam, lParam);
}


NEHKBDHOOK_API BOOL StartHook(HWND _callback_reciever)
{
	if (alreadyHooked)
	{
		return FALSE;
	}
	if (hookInstance == NULL)
	{
		MessageBox(NULL, L"FAil", L"Dbg", MB_OK);
	}
	kbHook = SetWindowsHookEx(WH_KEYBOARD, (HOOKPROC)HookCallback, hookInstance, 0);
	
	if (kbHook == NULL)
	{
		return FALSE;
	}

	callback_reciever = _callback_reciever;
	return TRUE;
}

NEHKBDHOOK_API BOOL StopHook()
{
	if (kbHook == NULL) return TRUE;

	BOOL success = UnhookWindowsHookEx(kbHook);

	if (!success)
	{
		return FALSE;
	}

	callback_reciever = FALSE;
	kbHook = NULL;
	return TRUE;
}



