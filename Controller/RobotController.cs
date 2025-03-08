using MQTTnet;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Warehouse.Model;

namespace Warehouse.Controller;

public class RobotController
{
    public async Task SendMessageAsync(Robot robotId, string message)
    {
        // Triển khai logic gửi thông điệp, ví dụ qua MQTT, API, hoặc console
        Console.WriteLine($"Sending message to robot {robotId.Robot_ID}: {message}");
        await Task.CompletedTask; // Thay bằng logic thực tế của bạn
    }

}
