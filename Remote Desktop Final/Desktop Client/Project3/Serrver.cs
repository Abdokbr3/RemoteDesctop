using System;
using System.Net.Sockets;
using System.Drawing;

namespace Project3
{
    // Структура для хранения настроек сервера
    public struct Settings
    {
        public Size resolution; // Разрешение экрана
        public int port; // Порт для отпраки картинки
        public string ip; // Порт для IP Адреса
    }
    // Структура для хранения ответа сервера
    struct ResponceInfo
    {
        public int l; // Длина сообщения
        public Byte[] data; // Сообщение
    }
    
    class CLient
    {
        private TcpClient client = null; // Клиент
        private NetworkStream stream = null; // Поток для обмена данными с сервером
        private ResponceInfo ri = new ResponceInfo(); // Сообщение от сервера
        public Int32 port = 10000; // Порт для картинки
        public long traffic = 0; // Хранение трафика 

        // Чтение настроек из файла
        public static Settings ReadFromFile(string filename)
        {
            // Проверка на исключения при чтении файла
            try
            {
                // Настройки из текстового файла
                Settings sets = new Settings();
                // Строка для строки
                string line = "";
                // Строка для имени аргументно
                string arg_name = "";
                // Строка для значения
                string value = "";
                // Файл для чтения настроек из него
                System.IO.StreamReader file = new System.IO.StreamReader(filename);
                // Чтение файла построчно
                while ((line = file.ReadLine()) != null)
                {
                    // Проверка на привильность написания файла
                    if (line.Split(':').Length == 2)
                    {
                        // Задания имени аргумента
                        arg_name = line.Split(':')[0];
                        // Задание значения аргумента
                        value = line.Split(':')[1];
                        // Запись данных в структуру
                        if (arg_name == "port")
                        {
                            sets.port = Convert.ToInt32(value);
                        }
                        if (arg_name == "server_ip")
                        {
                            sets.ip = value;
                        }
                        if (arg_name == "width")
                        {
                            sets.resolution.Width = Convert.ToInt32(value);
                        }
                        if (arg_name == "height")
                        {
                            sets.resolution.Height = Convert.ToInt32(value);
                        }
                    }
                }
                // Возвращение заполненной структуры
                return sets;
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке
                Console.WriteLine("Error while reading settings file: " + ex.Message);
                // Возвращение путого файла с настройками
                return new Settings();
            }
        }
        // Метод запуска подключения
        public void Start()
        {
            // Заполнение настроек
            Settings settings = ReadFromFile("settings.txt");
            // Заполнение IP Адреса сервера
            string server = settings.ip;
            // Создание подключения
            client = new TcpClient(server, port);
            // Создание потока обмена данными с сервером
            stream = client.GetStream();
        }
        // Чтение данных с сервера и отправка своих
        public ResponceInfo Read(byte[] message)
        {
            // Отправка данных серверу
            stream.Write(message, 0, 200);
            // Создание буффера в 20 МБ для принятия картинки
            Byte[] bytes = new Byte[20 * 1024 * 1024];
            //  чтение длины и данных в структуру
            ri.l = stream.Read(bytes, 0, bytes.Length);
            ri.data = bytes;
            // Очистка потока
            stream.Flush();
            traffic += ri.l;
            return ri;
        }
        // Остановка клиента
        public void CloseClient()
        {
            client.Close();
            client.Dispose();
        }
    }
}