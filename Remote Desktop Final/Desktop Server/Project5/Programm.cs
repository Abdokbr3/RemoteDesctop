using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Project5
{
    public class Program
    {
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;
        private const int MOUSEEVENTF_WHEEL = 0x0800;
        private const int MOUSEEVENTF_XDOWN = 0x0080;
        private const int MOUSEEVENTF_XUP = 0x0100;
		

		
		
        bool working = true;
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        public static void DoMouseClick(int arg)
        {
            Cursor.Position = new Point(Cursor.Position.X, Cursor.Position.Y);
            mouse_event(arg, 0, 0, 0, 0);
        }
        public void ChangeWorkingVar(bool arg)
        {
            working = arg;
        }
        public static void Main(string[] args)
        {
            int[] pressed_mouse_buttons = { 0, 0, 0, 0 }; // Список нажатых кнопок мыши
            List<KeyCode> keys = new List<KeyCode>(); // Список нажатых клавиш
            Server s = new Server(); // Инициализация сервера
            Bitmap screenshot = new Bitmap(1920, 1080); // Буффер для отправки скриншота
            Console.WriteLine("ВВедтите уровень качества сжатия картинки\n");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Качество меньше 10 ужасное");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Качество от 11 до 30 плохое");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Качество от 31 до 50 нормальное");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Качесвто т 60 - хорошее\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Качество влияет на степень сжатия картинки, чем картинка более сжата, тем быстрее она передастся, но при этом хуже выглядит");
            long quality = Convert.ToInt64(Console.ReadLine());
            s.Quality = quality;
            while (true)
            {

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Remote Desktop Server");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Статус:");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" ожидает");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Качество сжатия: ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(s.Quality);
                s.Start(); // Запуск сервера
                // Вывод информации пользователю
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Remote Desktop Server");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Статус:");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(" работает");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Качество сжатия: ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(s.Quality);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("IP Адрес клиента: ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(s.UserIP);

                while (true)
                {
                    using (Graphics g = Graphics.FromImage(screenshot)) // Копирует экран в screenshot
                    {
                        try
                        {
                            g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);
                        }
                        catch (Exception ex) { }
                    }
                    try
                    {
                        ResponceInfo responce = s.Write(screenshot); // Чтение из подключения
                        if (responce.l != 200) // Проверка на неправильный ответ клиента
                        {
                            throw new Exception(); // Выход из try catch
                        }
                        else
                        {
                            // Приминение полученной от клиента информации
                            // Постановка позиции курсора
                            Cursor.Position = new Point(responce.data[0] + responce.data[1] * 256, responce.data[2] + responce.data[3] * 256);

                            if (responce.data[4] == 1)
                            {
                                if (pressed_mouse_buttons[0] == 0)
                                {
                                    DoMouseClick(MOUSEEVENTF_LEFTDOWN);
                                    pressed_mouse_buttons[0] = 1;
                                }
                            }
                            if (responce.data[5] == 1)
                            {
                                if (pressed_mouse_buttons[1] == 0)
                                {
                                    DoMouseClick(MOUSEEVENTF_RIGHTDOWN);
                                    pressed_mouse_buttons[1] = 1;
                                }
                            }
                            if (responce.data[6] == 1)
                            {
                                if (pressed_mouse_buttons[2] == 0)
                                {
                                    DoMouseClick(MOUSEEVENTF_MIDDLEDOWN);
                                    pressed_mouse_buttons[2] = 1;
                                }
                            }
                            if (responce.data[7] == 1)
                            {
                                if (pressed_mouse_buttons[3] == 0)
                                {
                                    DoMouseClick(MOUSEEVENTF_XDOWN);
                                    pressed_mouse_buttons[3] = 1;
                                }
                            }
                            if (responce.data[4] == 0)
                            {
                                if (pressed_mouse_buttons[0] == 1)
                                {
                                    DoMouseClick(MOUSEEVENTF_LEFTUP);
                                    pressed_mouse_buttons[0] = 0;
                                }
                            }
                            if (responce.data[5] == 0)
                            {
                                if (pressed_mouse_buttons[1] == 1)
                                {
                                    DoMouseClick(MOUSEEVENTF_RIGHTUP);
                                    pressed_mouse_buttons[1] = 0;
                                }
                            }
                            if (responce.data[6] == 0)
                            {
                                if (pressed_mouse_buttons[3] == 1)
                                {
                                    DoMouseClick(MOUSEEVENTF_MIDDLEUP);
                                    pressed_mouse_buttons[3] = 0;
                                }
                            }
                            if (responce.data[7] == 0)
                            {
                                if (pressed_mouse_buttons[3] == 1)
                                {
                                    DoMouseClick(MOUSEEVENTF_XUP);
                                    pressed_mouse_buttons[3] = 0;
                                }
                            }
                            if (responce.data[8] != 0)
                            {
                                if (responce.data[9] == 1)
                                    mouse_event(MOUSEEVENTF_WHEEL, 0, 0, responce.data[8], 0);
                                if (responce.data[9] == 1)
                                    mouse_event(MOUSEEVENTF_WHEEL, 0, 0, -responce.data[8], 0);
                            }



                            for (int i = 5; i < 100; i++)
                            {
                                if (responce.data[i] != 0)
                                {
                                    if (!keys.Contains((KeyCode)responce.data[i]))
                                    {
                                        keys.Add((KeyCode)responce.data[i]);
                                        SendKeyDown((KeyCode)responce.data[i]);
                                    }
                                }
                            }
                            for (int i = 0; i < keys.Count; i++)
                            {
                                if (responce.data.Contains((byte)keys[i]))
                                {
                                    SendKeyUp(keys[i]);
                                    keys.Remove(keys[i]);
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        s.CloseClient(); // Закрыть клиент, чтобы его перезапустить
                        break; // Выход из цикла и возвращение к началу, с перезапуском сервера
                    }
                }
            }
        }


        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint numberOfInputs, INPUT[] inputs, int sizeOfInputStructure);

        /// <summary>
        /// simulate key press
        /// </summary>
        /// <param name="keyCode"></param>
        public static void SendKeyPress(KeyCode keyCode)
        {
            INPUT input = new INPUT
            {
                Type = 1
            };
            input.Data.Keyboard = new KEYBDINPUT()
            {
                Vk = (ushort)keyCode,
                Scan = 0,
                Flags = 0,
                Time = 0,
                ExtraInfo = IntPtr.Zero,
            };

            INPUT input2 = new INPUT
            {
                Type = 1
            };
            input2.Data.Keyboard = new KEYBDINPUT()
            {
                Vk = (ushort)keyCode,
                Scan = 0,
                Flags = 2,
                Time = 0,
                ExtraInfo = IntPtr.Zero
            };
            INPUT[] inputs = new INPUT[] { input, input2 };
            if (SendInput(2, inputs, Marshal.SizeOf(typeof(INPUT))) == 0)
                throw new Exception();
        }

        /// <summary>
        /// Send a key down and hold it down until sendkeyup method is called
        /// </summary>
        /// <param name="keyCode"></param>
        public static void SendKeyDown(KeyCode keyCode)
        {
            INPUT input = new INPUT
            {
                Type = 1
            };
            input.Data.Keyboard = new KEYBDINPUT();
            input.Data.Keyboard.Vk = (ushort)keyCode;
            input.Data.Keyboard.Scan = 0;
            input.Data.Keyboard.Flags = 0;
            input.Data.Keyboard.Time = 0;
            input.Data.Keyboard.ExtraInfo = IntPtr.Zero;
            INPUT[] inputs = new INPUT[] { input };
            if (SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT))) == 0)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Release a key that is being hold down
        /// </summary>
        /// <param name="keyCode"></param>
        public static void SendKeyUp(KeyCode keyCode)
        {
            INPUT input = new INPUT
            {
                Type = 1
            };
            input.Data.Keyboard = new KEYBDINPUT();
            input.Data.Keyboard.Vk = (ushort)keyCode;
            input.Data.Keyboard.Scan = 0;
            input.Data.Keyboard.Flags = 2;
            input.Data.Keyboard.Time = 0;
            input.Data.Keyboard.ExtraInfo = IntPtr.Zero;
            INPUT[] inputs = new INPUT[] { input };
            if (SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT))) == 0)
                throw new Exception();

        }
    }
}
[StructLayout(LayoutKind.Sequential)]
internal struct INPUT
{
    public uint Type;
    public MOUSEKEYBDHARDWAREINPUT Data;
}

[StructLayout(LayoutKind.Explicit)]
internal struct MOUSEKEYBDHARDWAREINPUT
{
    [FieldOffset(0)]
    public HARDWAREINPUT Hardware;
    [FieldOffset(0)]
    public KEYBDINPUT Keyboard;
    [FieldOffset(0)]
    public MOUSEINPUT Mouse;
}

[StructLayout(LayoutKind.Sequential)]
internal struct HARDWAREINPUT
{
    public uint Msg;
    public ushort ParamL;
    public ushort ParamH;
}

[StructLayout(LayoutKind.Sequential)]
internal struct KEYBDINPUT
{
    public ushort Vk;
    public ushort Scan;
    public uint Flags;
    public uint Time;
    public IntPtr ExtraInfo;
}

[StructLayout(LayoutKind.Sequential)]
internal struct MOUSEINPUT
{
    public int X;
    public int Y;
    public uint MouseData;
    public uint Flags;
    public uint Time;
    public IntPtr ExtraInfo;
}

public enum KeyCode : ushort
{
    #region Media
    MEDIA_NEXT_TRACK = 0xb0,
    MEDIA_PLAY_PAUSE = 0xb3,
    MEDIA_PREV_TRACK = 0xb1,
    MEDIA_STOP = 0xb2,
    #endregion
    #region math
    ADD = 0x6b,
    MULTIPLY = 0x6a,
    DIVIDE = 0x6f,
    SUBTRACT = 0x6d,
    #endregion
    #region Browser
    BROWSER_BACK = 0xa6,
    BROWSER_FAVORITES = 0xab,
    BROWSER_FORWARD = 0xa7,
    BROWSER_HOME = 0xac,
    BROWSER_REFRESH = 0xa8,
    BROWSER_SEARCH = 170,
    BROWSER_STOP = 0xa9,
    #endregion
    #region Numpad numbers
    NUMPAD0 = 0x60,
    NUMPAD1 = 0x61,
    NUMPAD2 = 0x62,
    NUMPAD3 = 0x63,
    NUMPAD4 = 100,
    NUMPAD5 = 0x65,
    NUMPAD6 = 0x66,
    NUMPAD7 = 0x67,
    NUMPAD8 = 0x68,
    NUMPAD9 = 0x69,
    #endregion
    #region Fkeys
    F1 = 0x70,
    F10 = 0x79,
    F11 = 0x7a,
    F12 = 0x7b,
    F13 = 0x7c,
    F14 = 0x7d,
    F15 = 0x7e,
    F16 = 0x7f,
    F17 = 0x80,
    F18 = 0x81,
    F19 = 130,
    F2 = 0x71,
    F20 = 0x83,
    F21 = 0x84,
    F22 = 0x85,
    F23 = 0x86,
    F24 = 0x87,
    F3 = 0x72,
    F4 = 0x73,
    F5 = 0x74,
    F6 = 0x75,
    F7 = 0x76,
    F8 = 0x77,
    F9 = 120,
    #endregion
    #region Other
    OEM_1 = 0xba,
    OEM_102 = 0xe2,
    OEM_2 = 0xbf,
    OEM_3 = 0xc0,
    OEM_4 = 0xdb,
    OEM_5 = 220,
    OEM_6 = 0xdd,
    OEM_7 = 0xde,
    OEM_8 = 0xdf,
    OEM_CLEAR = 0xfe,
    OEM_COMMA = 0xbc,
    OEM_MINUS = 0xbd,
    OEM_PERIOD = 190,
    OEM_PLUS = 0xbb,
    #endregion
    #region KEYS
    KEY_0 = 0x30,
    KEY_1 = 0x31,
    KEY_2 = 50,
    KEY_3 = 0x33,
    KEY_4 = 0x34,
    KEY_5 = 0x35,
    KEY_6 = 0x36,
    KEY_7 = 0x37,
    KEY_8 = 0x38,
    KEY_9 = 0x39,
    KEY_A = 0x41,
    KEY_B = 0x42,
    KEY_C = 0x43,
    KEY_D = 0x44,
    KEY_E = 0x45,
    KEY_F = 70,
    KEY_G = 0x47,
    KEY_H = 0x48,
    KEY_I = 0x49,
    KEY_J = 0x4a,
    KEY_K = 0x4b,
    KEY_L = 0x4c,
    KEY_M = 0x4d,
    KEY_N = 0x4e,
    KEY_O = 0x4f,
    KEY_P = 80,
    KEY_Q = 0x51,
    KEY_R = 0x52,
    KEY_S = 0x53,
    KEY_T = 0x54,
    KEY_U = 0x55,
    KEY_V = 0x56,
    KEY_W = 0x57,
    KEY_X = 0x58,
    KEY_Y = 0x59,
    KEY_Z = 90,
    #endregion
    #region volume
    VOLUME_DOWN = 0xae,
    VOLUME_UP = 0xaf,
    #endregion
    SNAPSHOT = 0x2c,
    RightClick = 0x5d,
    BACKSPACE = 8,
    CANCEL = 3,
    CAPS_LOCK = 20,
    CONTROL = 0x11,
    ALT = 18,
    DECIMAL = 110,
    DELETE = 0x2e,
    DOWN = 40,
    END = 0x23,
    ESC = 0x1b,
    HOME = 0x24,
    INSERT = 0x2d,
    LAUNCH_APP1 = 0xb6,
    LAUNCH_APP2 = 0xb7,
    LAUNCH_MAIL = 180,
    LAUNCH_MEDIA_SELECT = 0xb5,
    LCONTROL = 0xa2,
    LEFT = 0x25,
    LSHIFT = 160,
    LWIN = 0x5b,
    PAGEDOWN = 0x22,
    NUMLOCK = 0x90,
    PAGE_UP = 0x21,
    RCONTROL = 0xa3,
    ENTER = 13,
    RIGHT = 0x27,
    RSHIFT = 0xa1,
    RWIN = 0x5c,
    SHIFT = 0x10,
    SPACE_BAR = 0x20,
    TAB = 9,
    UP = 0x26,
}




"a"
a = KeyCode.KEY_A = 0x041