using System.Net.Sockets;
using System.Net;
using System.Windows;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace Server;


public partial class MainWindow : Window
{
    static int _port = 27001;
    static IPAddress _ip = IPAddress.Parse("192.168.1.8");
    private TcpListener? _listener;

    static IPEndPoint _endpoint = new(_ip, _port);

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        Start();
    }


    private async void Start()
    {
        try
        {
            _listener = new TcpListener(_ip, _port);
            _listener.Start();
            Dispatcher.Invoke(() => ListBoxInfarmation.Items.Add($"Listening on: {_listener.LocalEndpoint}"));

            while (true)
            {
                var client = await _listener.AcceptTcpClientAsync();
                HandleClient(client);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

    }

    private async void HandleClient(TcpClient client)
    {
        try
        {
            Dispatcher.Invoke(() => ListBoxInfarmation.Items.Add($"{client.Client.RemoteEndPoint} connected"));
            using var networkRead = client.GetStream();
            using (var reader = new StreamReader(networkRead, Encoding.UTF8))
            {
                var baseImage = await reader.ReadToEndAsync();
                var image = BitmapImageFromBase64(baseImage);
                Dispatcher.Invoke(() => ListBoxImages.Items.Add(image));
            };
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }


    }

    private BitmapImage BitmapImageFromBase64(string base64Image)
    {
        try
        {
            var imageBytes = Convert.FromBase64String(base64Image);
            var imageStream = new MemoryStream(imageBytes);

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = imageStream;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return null!;
        }


    }
}