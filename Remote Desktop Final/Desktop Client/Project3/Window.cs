using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Project3
{
    public class Window : Form
    {
        // Разрешение экрана, Full HD  по умолчанию
        int[] resolution = { 1920, 1080 };
        // буффер для хранения массива айт для картинки
        byte[] buffer = new byte[20 * 1024 * 1024];
        // буффер дял хранения предыдущей картинки
        byte[] old_buffer = new byte[20 * 1024 * 1024];
        // буффер дял хранения ещё более предыдущей картинки
        byte[] older_buffer = new byte[20 * 1024 * 1024];
        // Переменная для проверки возможности обновления экрана
        bool flip = false;
        // Таймер для обновления экрана
        System.Windows.Forms.Timer updater;
        // Позиция мыши
        int[] mouse_position = { 0, 0 };
        // Нажатые клавиши
        List<int> keys = new List<int>();
        // Нажатые кнопки мыши
        List<MouseButtons> mouse_buttons = new List<MouseButtons>();
        // Прокрутка колёсика мыши
        int mouse_wheel = 0;

        // Метод для обовления позиции мыши
        void Form1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Обновление координат мыши
            mouse_position[0] = e.X;
            mouse_position[1] = e.Y;
        }
        // Метод вызываемый при прокрутке мыши
        void Form1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Хранение смещения колёсика мыши
            mouse_wheel = e.Delta;
        }
        // Метод вызываемый при нажатии кнопки мыши
        void Form1_MouseButton(object sender, MouseEventArgs e)
        {
            // Добавление в список ажадых кнопок нопку 1 раз
            if (!mouse_buttons.Contains(e.Button))
            {
                mouse_buttons.Add(e.Button);
            }
        }
        // Метод, вызываемый при отпускании кнопки мыши
        void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            // Убирание из списка нажатых клавиш кнопку 1 раз
            if (mouse_buttons.Contains(e.Button))
            {
                mouse_buttons.Remove(e.Button);
            }
        }
        // Метод, вызываемый при нажатии клавиши клавиатуры
        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Добавление в список клавиши 1 раз
            if (!keys.Contains(e.KeyValue))
            {
                keys.Add(e.KeyValue);
            }
        }
        // Метод, вызываемый при отпускании клавиатуры
        void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            // Убирание из списка клавиши 1 раз
            if (keys.Contains(e.KeyValue))
            {
                keys.Remove(e.KeyValue);
            }
        }
        // Конструктор класса Window
        public Window()
        {
            //Параметры для окна
            BackColor = Color.Black; // Черный цвет для неактивного окна
            FormBorderStyle = FormBorderStyle.None; // Задание границ окна
            Bounds = Screen.PrimaryScreen.Bounds; // Задание границ окна
            TopMost = true; // Задание границ окна
            DoubleBuffered = true; // Включение двойной буфферизации, для переключения кадров без "ползущей линии"
            // Обновление экрана
            Paint += OnPaint; // Добавления события отрисовки экрана
            // Посылка действый мыши и клавиатуры
            KeyDown += new KeyEventHandler(Form1_KeyDown); // Событие нажатия клавиши
            KeyUp += new KeyEventHandler(Form1_KeyUp); // Событие отпускания клавиши
            MouseDown += new MouseEventHandler(Form1_MouseButton); // Событие нажатие клавиши мыши
            MouseUp += new MouseEventHandler(Form1_MouseUp); // Событие отпускания клавиши мыши
            MouseWheel += new MouseEventHandler(Form1_MouseWheel); // Событие изменение положения колёсика мыши
            MouseMove += new MouseEventHandler(Form1_MouseMove); // Событие изменения положения мыши
        }
        // Метод, вызываемый каждый тик таймера
        public void Update(object sender, EventArgs ea)
        {
            // обнуление смещения колёсика мыши
            mouse_wheel = 0;
            // Принуждение приложения обновить кран через функцию отрисовки
            Invalidate();
        }
        //Метод для запуска окна
        public void Start()
        {
            // Создание таймера для обновления
            updater = new System.Windows.Forms.Timer();
            // Задание интервала примерно на 60 обновлений в секунду
            updater.Interval = 16;
            // Добавление события обновления к тику таймера
            updater.Tick += Update;
            // Запуск таймера
            updater.Start();
            // Подключение
            start_sending();
            //start_sending();
            // Старт приложение
            Application.EnableVisualStyles();
            Application.Run(this);
        }
        // Метод отрисовки
        private void OnPaint(object sender, PaintEventArgs pea)
        {
            // Проверка на ошибку отрисовки
            try
            {
                // Проверка на возможность обновления экрана
                if (flip)
                {
                    // Отрисовка картинки из буффера с её конвертаией из массива
                    using (var ms = new MemoryStream(buffer))
                    {
                        // Отрисовка картинки из буффера с её конвертаций из массива
                        pea.Graphics.DrawImage((Bitmap)Bitmap.FromStream(ms), new Point(0, 0));
                        // Отрисовка лаймовой линии
                        pea.Graphics.DrawLine(new Pen(Color.Lime, 8), new Point(10, 10), new Point(60, 10));
                    }
                    old_buffer = (byte[])buffer.Clone();
                    older_buffer = (byte[])buffer.Clone();
                }
            }
            catch (Exception)
            {
                // Отрисовка красной линии
                try
                {
                    using (var ms = new MemoryStream(older_buffer))
                    {
                        pea.Graphics.DrawImage((Bitmap)Bitmap.FromStream(ms), new Point(0, 0));
                    }
                }
                catch (Exception) { }
                pea.Graphics.DrawLine(new Pen(Color.Red, 8), new Point(10, 10), new Point(60, 10));
            }
        }
        // Начало посылки
        public async void start_sending()
        {
            // Запуск асинхронной функции, работающей параллельно с программой
            await Task.Run(() =>
            {
                // Небольшое ожидание перед запуском сервера
                System.Threading.Thread.Sleep(1000);
                // Инициализация клиента
                CLient server = new CLient();
                // Создание структуры данных для получения ответа
                ResponceInfo ri = new ResponceInfo();
                // буффер байт для посылки сообщений
                byte[] message;
                // Вечный цикл создания и остановки подключения
                while (true)
                {
                    // Анализ на исключения
                    try
                    {
                        // Запуск сервера
                        Console.WriteLine("Starting server");
                        server.Start();
                        Console.WriteLine("Server started");
                        // Вечный цикл принятия и отправки сообщений
                        while (true)
                        {
                            // Анализ возникающих исключений
                            try
                            {
                                // создание сообщения для передаи серверу
                                message = new byte[200];
                                // Положение мыши на экране
                                message[0] = (byte)(mouse_position[0] % 255);
                                message[1] = (byte)(mouse_position[0] / 255);
                                message[2] = (byte)(mouse_position[1] % 255);
                                message[3] = (byte)(mouse_position[1] / 255);
                                
                                // Данные о нажатых клавишах мыши
                                message[4] = mouse_buttons.Contains(MouseButtons.Left) ? (byte)1 : (byte)0; // ЛКМ
                                message[5] = mouse_buttons.Contains(MouseButtons.Right) ? (byte)1 : (byte)0; // ПКМ
                                message[6] = mouse_buttons.Contains(MouseButtons.Middle) ? (byte)1 : (byte)0; // Колёсико
                                message[7] = mouse_buttons.Contains(MouseButtons.XButton1) ? (byte)1 : (byte)0;
                                message[8] = (byte)(mouse_wheel % 255); // Врещение колёсика мыши
                                message[9] = mouse_wheel >= 0 ? (byte)1 : (byte)0; // Направление вращения колёсика мыши
                                // Нажатые кнопки на экране
                                for (int i = 50; i < 50 + keys.Count; i++)
                                {
                                    // Запись кнопок
                                    message[i] = (byte)keys[i - 50];
                                }
                                // Чтение картинки
                                ri = server.Read(message);
                                // Запись картинки в буффер
                                buffer = (byte[])ri.data.Clone();
                                // Передача глобальной переменной о возможности обновления
                                flip = true;
                                // Вывод информации в консоль
                                Console.Clear();
                                Console.WriteLine("Потрачено Мегабайт: " + Convert.ToString(server.traffic / (1024 * 1024)));
                            }
                            catch (Exception ex)
                            {
                                // Передача глобальной переменной о невозможности обновления
                                flip = false;
                                Console.WriteLine("Error, trying to reconnect");
                                // Выход из цикла приёма и отправки
                                // Возвращение к созданию / пересозданию подключения
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error, server is not working");
                    }
                }
            });
        }
    }
}
