using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

        // Virtual key codes from Win32 API
        public enum VirtualKeys : byte
        {
            // Modifier keys
            LControlKey = 0xA2,
            RControlKey = 0xA3,
            LShiftKey = 0xA0,
            RShiftKey = 0xA1,
            LAltKey = 0xA4,
            RAltKey = 0xA5,
            LWinKey = 0x5B,
            RWinKey = 0x5C,
            
            // Letters
            A = 0x41, B = 0x42, C = 0x43, D = 0x44, E = 0x45, F = 0x46, G = 0x47, H = 0x48,
            I = 0x49, J = 0x4A, K = 0x4B, L = 0x4C, M = 0x4D, N = 0x4E, O = 0x4F, P = 0x50,
            Q = 0x51, R = 0x52, S = 0x53, T = 0x54, U = 0x55, V = 0x56, W = 0x57, X = 0x58,
            Y = 0x59, Z = 0x5A,
            
            // Numbers
            D0 = 0x30, D1 = 0x31, D2 = 0x32, D3 = 0x33, D4 = 0x34,
            D5 = 0x35, D6 = 0x36, D7 = 0x37, D8 = 0x38, D9 = 0x39,
            
            // Function keys
            F1 = 0x70, F2 = 0x71, F3 = 0x72, F4 = 0x73, F5 = 0x74, F6 = 0x75,
            F7 = 0x76, F8 = 0x77, F9 = 0x78, F10 = 0x79, F11 = 0x7A, F12 = 0x7B,
            
            // Special keys
            Enter = 0x0D,
            Space = 0x20,
            Tab = 0x09,
            Back = 0x08,
            Delete = 0x2E,
            Insert = 0x2D,
            Home = 0x24,
            End = 0x23,
            PageUp = 0x21,
            PageDown = 0x22,
            Escape = 0x1B,
            
            // Arrow keys
            Up = 0x26,
            Down = 0x28,
            Left = 0x25,
            Right = 0x27
        }

        private Dictionary<string, VirtualKeys> _keyMappings;

        public KeyboardHook()
        {
            InitializeKeyMappings();
        }

        private void InitializeKeyMappings()
        {
            _keyMappings = new Dictionary<string, VirtualKeys>
            {
                // Modifier keys
                { "LControlKey", VirtualKeys.LControlKey },
                { "RControlKey", VirtualKeys.RControlKey },
                { "LShiftKey", VirtualKeys.LShiftKey },
                { "RShiftKey", VirtualKeys.RShiftKey },
                { "LAltKey", VirtualKeys.LAltKey },
                { "RAltKey", VirtualKeys.RAltKey },
                { "LWinKey", VirtualKeys.LWinKey },
                { "RWinKey", VirtualKeys.RWinKey },
                
                // Letters
                { "A", VirtualKeys.A }, { "B", VirtualKeys.B }, { "C", VirtualKeys.C }, { "D", VirtualKeys.D },
                { "E", VirtualKeys.E }, { "F", VirtualKeys.F }, { "G", VirtualKeys.G }, { "H", VirtualKeys.H },
                { "I", VirtualKeys.I }, { "J", VirtualKeys.J }, { "K", VirtualKeys.K }, { "L", VirtualKeys.L },
                { "M", VirtualKeys.M }, { "N", VirtualKeys.N }, { "O", VirtualKeys.O }, { "P", VirtualKeys.P },
                { "Q", VirtualKeys.Q }, { "R", VirtualKeys.R }, { "S", VirtualKeys.S }, { "T", VirtualKeys.T },
                { "U", VirtualKeys.U }, { "V", VirtualKeys.V }, { "W", VirtualKeys.W }, { "X", VirtualKeys.X },
                { "Y", VirtualKeys.Y }, { "Z", VirtualKeys.Z },
                
                // Numbers
                { "0", VirtualKeys.D0 }, { "1", VirtualKeys.D1 }, { "2", VirtualKeys.D2 }, { "3", VirtualKeys.D3 },
                { "4", VirtualKeys.D4 }, { "5", VirtualKeys.D5 }, { "6", VirtualKeys.D6 }, { "7", VirtualKeys.D7 },
                { "8", VirtualKeys.D8 }, { "9", VirtualKeys.D9 },
                
                // Function keys
                { "F1", VirtualKeys.F1 }, { "F2", VirtualKeys.F2 }, { "F3", VirtualKeys.F3 }, { "F4", VirtualKeys.F4 },
                { "F5", VirtualKeys.F5 }, { "F6", VirtualKeys.F6 }, { "F7", VirtualKeys.F7 }, { "F8", VirtualKeys.F8 },
                { "F9", VirtualKeys.F9 }, { "F10", VirtualKeys.F10 }, { "F11", VirtualKeys.F11 }, { "F12", VirtualKeys.F12 },
                
                // Special keys
                { "Enter", VirtualKeys.Enter },
                { "Space", VirtualKeys.Space },
                { "Tab", VirtualKeys.Tab },
                { "Backspace", VirtualKeys.Back },
                { "Delete", VirtualKeys.Delete },
                { "Insert", VirtualKeys.Insert },
                { "Home", VirtualKeys.Home },
                { "End", VirtualKeys.End },
                { "PageUp", VirtualKeys.PageUp },
                { "PageDown", VirtualKeys.PageDown },
                { "Escape", VirtualKeys.Escape },
                
                // Arrow keys
                { "Up", VirtualKeys.Up },
                { "Down", VirtualKeys.Down },
                { "Left", VirtualKeys.Left },
                { "Right", VirtualKeys.Right }
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
            foreach (var key in keys.AsEnumerable().Reverse())
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
            foreach (var key in keys.AsEnumerable().Reverse())
            {
                SendKeyUpInternal(key);
            }
        }

        private List<VirtualKeys> ParseKeyCombination(string combination)
        {
            var result = new List<VirtualKeys>();
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

        private void SendKeyDownInternal(VirtualKeys key)
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

        private void SendKeyUpInternal(VirtualKeys key)
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

        private bool IsExtendedKey(VirtualKeys key)
        {
            return key == VirtualKeys.RAltKey || key == VirtualKeys.RControlKey || 
                   key == VirtualKeys.Insert || key == VirtualKeys.Delete ||
                   key == VirtualKeys.Home || key == VirtualKeys.End ||
                   key == VirtualKeys.PageUp || key == VirtualKeys.PageDown ||
                   key == VirtualKeys.Up || key == VirtualKeys.Down ||
                   key == VirtualKeys.Left || key == VirtualKeys.Right;
        }

        public bool IsKeyPressed(VirtualKeys key)
        {
            return (GetAsyncKeyState((int)key) & 0x8000) != 0;
        }

        public void Dispose()
        {
            // Clean up resources if needed
        }
    }
}