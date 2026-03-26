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

        var json = File.ReadAllText(CUSTOM_SERVER_FILE_PATH);
        ServerData = JsonSerializer.Deserialize<CustomServerType>(json);

        // Logger.LogInfo("[CustomServer] Read Json: {0}", json);
        Logger.LogInfo("[CustomServer] IP: {0}, Port: {1}", ServerData.IP, ServerData.Port);
    }

    private static void CreateSettingFile()
    {
        var json = "{\n\t\"IP\": \"127.0.0.1\",\n\t\"Port\": 22023\n}";
        File.WriteAllText(CUSTOM_SERVER_FILE_PATH, json);
    }
}

public class CustomServerType
{
    public string IP { get; set; } = "127.0.0.1";
    public ushort Port { get; set; } = 22023;

    public CustomServerType() { }

    public CustomServerType(string ip, ushort port)
    {
        IP = ip;
        Port = port;
    }
}