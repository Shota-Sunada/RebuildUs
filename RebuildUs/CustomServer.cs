using System.Text.Json;

namespace RebuildUs;

internal static class CustomServer
{
    private const string CUSTOM_SERVER_FILE_NAME = "CustomServer.txt";
    private static readonly string CUSTOM_SERVER_FILE_PATH = Path.Combine(Directory.GetCurrentDirectory(), CUSTOM_SERVER_FILE_NAME);
    internal static CustomServerType ServerData { get; private set; } = new("127.0.0.1", 22023);

    internal static void Initialize()
    {
        if (!File.Exists(CUSTOM_SERVER_FILE_PATH))
        {
            CreateSettingFile();
        }

        using var stream = new FileStream(CUSTOM_SERVER_FILE_PATH, FileMode.Open);
        ServerData = JsonSerializer.Deserialize<CustomServerType>(CUSTOM_SERVER_FILE_PATH);
    }

    private static void CreateSettingFile()
    {
        var json = JsonSerializer.Serialize(ServerData, new JsonSerializerOptions { WriteIndented = true, });
        File.WriteAllText(CUSTOM_SERVER_FILE_PATH, json);
    }
}

internal class CustomServerType
{
    internal string IP;
    internal ushort Port;

    internal CustomServerType(string ip, ushort port)
    {
        IP = ip;
        Port = port;
    }
}