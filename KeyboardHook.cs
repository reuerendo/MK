using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Linq;

namespace MiniKeyboard
{
    public class KeyboardHook : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        private const uint KEYEVENTF_KEYDOWN = 0x0000;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;

        private Dictionary<string, Keys> _keyMappings;

        public KeyboardHook()
        {
            InitializeKeyMappings();
        }

        private void InitializeKeyMappings()
        {
            _keyMappings = new Dictionary<string, Keys>
            {
                // Modifier keys
                { "LControlKey", Keys.LControlKey },
                { "RControlKey", Keys.RControlKey },
                { "LShiftKey", Keys.LShiftKey },
                { "RShiftKey", Keys.RShiftKey },
                { "LAltKey", Keys.LMenu },
                { "RAltKey", Keys.RMenu },
                { "LWinKey", Keys.LWin },
                { "RWinKey", Keys.RWin },
                
                // Letters
                { "A", Keys.A }, { "B", Keys.B }, { "C", Keys.C }, { "D", Keys.D },
                { "E", Keys.E }, { "F", Keys.F }, { "G", Keys.G }, { "H", Keys.H },
                { "I", Keys.I }, { "J", Keys.J }, { "K", Keys.K }, { "L", Keys.L },
                { "M", Keys.M }, { "N", Keys.N }, { "O", Keys.O }, { "P", Keys.P },
                { "Q", Keys.Q }, { "R", Keys.R }, { "S", Keys.S }, { "T", Keys.T },
                { "U", Keys.U }, { "V", Keys.V }, { "W", Keys.W }, { "X", Keys.X },
                { "Y", Keys.Y }, { "Z", Keys.Z },
                
                // Numbers
                { "0", Keys.D0 }, { "1", Keys.D1 }, { "2", Keys.D2 }, { "3", Keys.D3 },
                { "4", Keys.D4 }, { "5", Keys.D5 }, { "6", Keys.D6 }, { "7", Keys.D7 },
                { "8", Keys.D8 }, { "9", Keys.D9 },
                
                // Function keys
                { "F1", Keys.F1 }, { "F2", Keys.F2 }, { "F3", Keys.F3 }, { "F4", Keys.F4 },
                { "F5", Keys.F5 }, { "F6", Keys.F6 }, { "F7", Keys.F7 }, { "F8", Keys.F8 },
                { "F9", Keys.F9 }, { "F10", Keys.F10 }, { "F11", Keys.F11 }, { "F12", Keys.F12 },
                
                // Special keys
                { "Enter", Keys.Enter },
                { "Space", Keys.Space },
                { "Tab", Keys.Tab },
                { "Backspace", Keys.Back },
                { "Delete", Keys.Delete },
                { "Insert", Keys.Insert },
                { "Home", Keys.Home },
                { "End", Keys.End },
                { "PageUp", Keys.PageUp },
                { "PageDown", Keys.PageDown },
                { "Escape", Keys.Escape },
                
                // Arrow keys
                { "Up", Keys.Up },
                { "Down", Keys.Down },
                { "Left", Keys.Left },
                { "Right", Keys.Right }
            };
        }

        public void SendKeyDown(string keyCombination)
        {
            var keys = ParseKeyCombination(keyCombination);
            
            // Press all keys down
            foreach (var key in keys)
            {
                SendKeyDownInternal(key);
            }
            
            // Release all keys up (in reverse order)
            foreach (var key in keys.Reverse())
            {
                SendKeyUpInternal(key);
            }
        }

        public void SendKeyDownOnly(string keyCombination)
        {
            var keys = ParseKeyCombination(keyCombination);
            foreach (var key in keys)
            {
                SendKeyDownInternal(key);
            }
        }

        public void SendKeyUpOnly(string keyCombination)
        {
            var keys = ParseKeyCombination(keyCombination);
            foreach (var key in keys.Reverse())
            {
                SendKeyUpInternal(key);
            }
        }

        private List<Keys> ParseKeyCombination(string combination)
        {
            var result = new List<Keys>();
            var parts = combination.Split('+');
            
            foreach (var part in parts)
            {
                var trimmedPart = part.Trim();
                if (_keyMappings.ContainsKey(trimmedPart))
                {
                    result.Add(_keyMappings[trimmedPart]);
                }
            }
            
            return result;
        }

        private void SendKeyDownInternal(Keys key)
        {
            byte vkCode = (byte)key;
            byte scanCode = (byte)MapVirtualKey(vkCode, 0);
            uint flags = KEYEVENTF_KEYDOWN;
            
            // Check if it's an extended key
            if (IsExtendedKey(key))
            {
                flags |= KEYEVENTF_EXTENDEDKEY;
            }
            
            keybd_event(vkCode, scanCode, flags, UIntPtr.Zero);
        }

        private void SendKeyUpInternal(Keys key)
        {
            byte vkCode = (byte)key;
            byte scanCode = (byte)MapVirtualKey(vkCode, 0);
            uint flags = KEYEVENTF_KEYUP;
            
            // Check if it's an extended key
            if (IsExtendedKey(key))
            {
                flags |= KEYEVENTF_EXTENDEDKEY;
            }
            
            keybd_event(vkCode, scanCode, flags, UIntPtr.Zero);
        }

        private bool IsExtendedKey(Keys key)
        {
            return key == Keys.RMenu || key == Keys.RControlKey || 
                   key == Keys.Insert || key == Keys.Delete ||
                   key == Keys.Home || key == Keys.End ||
                   key == Keys.PageUp || key == Keys.PageDown ||
                   key == Keys.Up || key == Keys.Down ||
                   key == Keys.Left || key == Keys.Right;
        }

        public bool IsKeyPressed(Keys key)
        {
            return (GetAsyncKeyState((int)key) & 0x8000) != 0;
        }

        public void Dispose()
        {
            // Clean up resources if needed
        }
    }
}