using MQTTnet;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Controller
{
    public class RobotController
    {
        private readonly IMqttClient _mqttClient;
        private readonly MqttClientOptions _mqttOptions;
        private bool _isReconnecting = false;

        public RobotController()
        {
            _mqttClient = new MqttClientFactory().CreateMqttClient();

            _mqttOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost", 1883)
                .WithClientId($"RobotController_{Guid.NewGuid().ToString()[..8]}")
                .WithTimeout(TimeSpan.FromSeconds(10))
                .Build();

            _mqttClient.DisconnectedAsync += async e =>
            {
                Console.WriteLine("Disconnected from MQTT broker.");
                if (!_isReconnecting)
                {
                    await ReconnectAsync();
                }
            };

            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                //Console.WriteLine($"Received message: {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                await Task.CompletedTask;
            };

            Task.Run(() => ConnectAsync()).GetAwaiter().GetResult();
        }

        private async Task ConnectAsync()
        {
            try
            {
                await _mqttClient.ConnectAsync(_mqttOptions);
                //Console.WriteLine("Connected to MQTT broker successfully!");
                await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("robot/robot/command").Build());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection error: {ex.Message}");
                await ReconnectAsync();
            }
        }

        private async Task ReconnectAsync()
        {
            _isReconnecting = true;
            int retryCount = 0;
            const int maxRetries = 5;
            int delayBetweenRetries = 2000;

            while (!_mqttClient.IsConnected && retryCount < maxRetries)
            {
                try
                {
                    retryCount++;
                    //Console.WriteLine($"Attempting to reconnect... (try {retryCount}/{maxRetries})");
                    await _mqttClient.ConnectAsync(_mqttOptions);
                    await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("robot/robot/command").Build());
                    Console.WriteLine("Reconnected successfully!");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Reconnection error: {ex.Message}");
                    if (retryCount == maxRetries)
                    {
                        Console.WriteLine("Maximum retry attempts reached!");
                        return;
                    }
                    await Task.Delay(delayBetweenRetries);
                    delayBetweenRetries *= 2;
                }
            }
            _isReconnecting = false;
        }

        public async Task SendMessageAsync(string message)
        {
            if (!_mqttClient.IsConnected)
            {
                Console.WriteLine("MQTT client is not connected. Attempting to reconnect...");
                await ReconnectAsync();
                if (!_mqttClient.IsConnected)
                {
                    Console.WriteLine("Failed to reconnect. Message not sent.");
                    return;
                }
            }

            try
            {
                var mqttMessage = new MqttApplicationMessageBuilder()
                    .WithTopic("robot/robot/command")
                    .WithPayload(Encoding.UTF8.GetBytes(message))
                    .Build();

                await _mqttClient.PublishAsync(mqttMessage);
                //Console.WriteLine($"Published message: {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                await ReconnectAsync();
            }
        }

        public async Task DisconnectAsync()
        {
            if (_mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync();
                Console.WriteLine("Disconnected from MQTT broker.");
            }
        }

        // Hàm Main để chạy thử
        //public static async Task Main(string[] args)
        //{
        //    ArgumentNullException.ThrowIfNull(args);

        //    var controller = new RobotController();
        //    await controller.SendMessageAsync("Test message from RobotController");
        //    Console.WriteLine("Press any key to exit...");
        //    Console.ReadKey();
        //    await controller.DisconnectAsync();
        //}
    }
}