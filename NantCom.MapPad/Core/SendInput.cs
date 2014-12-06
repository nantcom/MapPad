using NantCom.MapPad.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NantCom.MapPad.Core
{
    /// <summary>
    /// Class which simulates input
    /// </summary>
    /// Solution is from: http://stackoverflow.com/questions/3644881/simulating-keyboard-with-sendinput-api-in-directinput-applications
    public class SimulateInput
    {
        /// <summary>
        /// Synthesizes keystrokes, mouse motions, and button clicks.
        /// </summary>
        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs,
            [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs,
            int cbSize);

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public INPUT_TYPE type;
            public InputUnion U;
            public static int Size
            {
                get { return Marshal.SizeOf(typeof(INPUT)); }
            }
        }

        [Flags]
        private enum MOUSEEVENTF : uint
        {
            ABSOLUTE = 0x8000,
            HWHEEL = 0x01000,
            MOVE = 0x0001,
            MOVE_NOCOALESCE = 0x2000,
            LEFTDOWN = 0x0002,
            LEFTUP = 0x0004,
            RIGHTDOWN = 0x0008,
            RIGHTUP = 0x0010,
            MIDDLEDOWN = 0x0020,
            MIDDLEUP = 0x0040,
            VIRTUALDESK = 0x4000,
            WHEEL = 0x0800,
            XDOWN = 0x0080,
            XUP = 0x0100
        }
        
        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public MOUSEEVENTF dwFlags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public VirtualKeyShort wVk;
            public ScanCode wScan;
            public KEYEVENTF dwFlags;
            public int time;
            public UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        [Flags]
        private enum KEYEVENTF : uint
        {
            EXTENDEDKEY = 0x0001,
            KEYUP = 0x0002,
            SCANCODE = 0x0008,
            UNICODE = 0x0004
        }

        private enum INPUT_TYPE : uint
        {
            MOUSE = 0,
            KEYBOARD,
            HARDWARE
        }

        /// <summary>
        /// Simulate Keydown using Scan code
        /// </summary>
        /// <param name="scanCode">The scan code.</param>
        public static void KeyDown(ScanCode scanCode )
        {
            INPUT[] InputData = new INPUT[1];

            InputData[0].type = INPUT_TYPE.KEYBOARD;
            InputData[0].U = new InputUnion()
            {
                ki = new KEYBDINPUT()
                {
                    wScan = scanCode,
                    dwFlags = KEYEVENTF.SCANCODE
                }
            };

            // send keydown
            if (SendInput(1, InputData, Marshal.SizeOf(InputData[0])) == 0)
            {
                System.Diagnostics.Debug.WriteLine("SendInput failed with code: " +
                Marshal.GetLastWin32Error().ToString());
            }
        }

        /// <summary>
        /// Simulate Keyup using Scan code
        /// </summary>
        /// <param name="scanCode">The scan code.</param>
        public static void KeyUp(ScanCode scanCode)
        {
            INPUT[] InputData = new INPUT[1];
            
            InputData[0].type = INPUT_TYPE.KEYBOARD;
            InputData[0].U = new InputUnion()
            {
                ki = new KEYBDINPUT()
                {
                    wScan = scanCode,
                    dwFlags = KEYEVENTF.SCANCODE | KEYEVENTF.KEYUP
                }
            };

            // send keydown
            if (SendInput(1, InputData, Marshal.SizeOf(InputData[0])) == 0)
            {
                System.Diagnostics.Debug.WriteLine("SendInput failed with code: " +
                Marshal.GetLastWin32Error().ToString());
            }
        }

    }
}
