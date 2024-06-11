namespace Bot.Api.Helpers;

/// <summary>
/// Инструменты работы с файлами
/// </summary>
public static class FileHelper
{
    public static Stream GetImageAsync(string fileName)
    {
        return File.OpenRead(Path.Combine("Images", fileName));
    }
}
