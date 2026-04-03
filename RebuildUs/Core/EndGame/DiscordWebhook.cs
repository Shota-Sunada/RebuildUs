namespace RebuildUs.Core.EndGame;

internal static class DiscordWebhook
{
    private static readonly HttpClient Client = new();

    internal static HttpResponseMessage SendResult(string winnerText, CustomGamemode gamemode, Field[] fields)
    {
        var gmTr = gamemode switch
        {
            CustomGamemode.HideNSeek => TrKey.GameModeHideNSeek,
            CustomGamemode.HotPotato => TrKey.GameModeHotPotato,
            CustomGamemode.BattleRoyale => TrKey.GameModeBattleRoyale,
            _ => TrKey.GameModeNormal,
        };

        var footer = new Footer { Text = "Rebuild Us ©2026 Shota-Sunada" };
        var author = new Author { Name = string.Format(Tr.Get(TrKey.GameIsOverBecause), winnerText) };
        var embed = new Embed
        {
            Title = Tr.Get(TrKey.WebhookResult),
            Timestamp = DateTime.Now,
            Footer = footer,
            Author = author,
            Fields = fields,
        };
        var data = new Webhook
        {
            Content = new StringBuilder(Tr.Get(TrKey.WebhookGameIsOver)).Append('\n').Append(Tr.Get(TrKey.GameMode)).Append(": ").Append(Tr.Get(gmTr)).ToString(),
            Tts = false,
            Username = RebuildUs.MOD_NAME,
            AvatarUrl = "https://cdn.discordapp.com/avatars/1265311488147460279/d09b1711dfbb97ec82581880e28d57cb.webp?size=1024",
            Embeds = [embed],
        };

        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return Client.PostAsync(RebuildUs.WebhookUrl.Value, content).Result;
    }
}

public class Webhook
{
    [JsonPropertyName("content")]
    public string Content { get; set; }
    [JsonPropertyName("tts")]
    public bool Tts { get; set; }
    [JsonPropertyName("embeds")]
    public Embed[] Embeds { get; set; }
    [JsonPropertyName("username")]
    public string Username { get; set; }
    [JsonPropertyName("avatar_url")]
    public string AvatarUrl { get; set; }
}

public class Embed
{
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
    [JsonPropertyName("footer")]
    public Footer Footer { get; set; }
    [JsonPropertyName("author")]
    public Author Author { get; set; }
    [JsonPropertyName("fields")]
    public Field[] Fields { get; set; }
}

public class Footer
{
    [JsonPropertyName("text")]
    public string Text { get; set; }
}

public class Author
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class Field
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("value")]
    public string Value { get; set; }
    [JsonPropertyName("inline")]
    public bool Inline { get; set; }
}