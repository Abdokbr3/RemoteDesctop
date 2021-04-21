using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace Project5
{
    struct ResponceInfo // Структура, которая хранит ответ клиента
    {
        public int l; // Длина ответа
        public Byte[] data; // Данные в байтах ответа
    }
    class Server
    {
        private ResponceInfo ri = new ResponceInfo(); // Структура, которая хранит ответ клиента
        private TcpListener server = null; // Сервер
        private TcpClient client; // Клиент
        private NetworkStream stream = null; // Поток для обмена даными
        private IPAddress localAddr = IPAddress.Any; // IP адресс для обмена сообщениями (0.0.0.0) по умолчанию 

        public Int32 port = 10000; // Порт для передачи
        public Int64 Quality = 16L; // Качество сжатья картинки
        public Int32 pause = 0; // Пауза (в мс), требуемая для сжатья картинки
        public string UserIP;
        public void Start() // Метод запуска сервера
        {
            server = new TcpListener(localAddr, port);
            server.Start();
            client = server.AcceptTcpClient();
            stream = client.GetStream();
        }
        // Метод обмена сервера с клиентом:
        // Клиент сначала посылает данные серверу
        // Далее сервер посылает картинку клиенту
        public ResponceInfo Write(Bitmap pic)
        {
            // Память выделенная для сообщения от клиента
            Byte[] bytes = new Byte[8 * 1024];
            // Получение длины сообщения и его содержания
            ri.l = stream.Read(bytes, 0, bytes.Length);
            ri.data = bytes;
            // Получение IP адреса клиента
            UserIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            // Отправка данных клиенту
            if (client.Connected)
            {
                // Сжатие картинки перед отправкой
                EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Quality);
                ImageCodecInfo imageCodec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(o => o.FormatID == ImageFormat.Jpeg.Guid);
                EncoderParameters parameters = new EncoderParameters(1);
                parameters.Param[0] = qualityParam;
                pic.Save(stream, imageCodec, parameters);
                // Ожидание конца операций с картинкой
                System.Threading.Thread.Sleep(30);
                // Очистка потока
                stream.Flush();
            }
            // Возвращение данных, полученых с клиента
            return ri;
        }
        // Метод завершения работы сервера
        public void CloseClient()
        {
            client.Close(); 
            client.Dispose();
            server.Stop();
        }
    }
}
