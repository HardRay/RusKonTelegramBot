using Application.Models;
using Telegram.Bot.Types.InlineQueryResults;

namespace Bot.Api.Helpers;

/// <summary>
/// Инструменты построения инлайн-ответов
/// </summary>
public static class InlineHelper
{
    public static List<InlineQueryResultArticle> GenerateInlineListAsync(IEnumerable<string> models, string? paramSearch = "")
    {
        if (string.IsNullOrEmpty(paramSearch))
        {
            return models
              .Select(r => new InlineQueryResultArticle(r, r, new InputTextMessageContent(r)))
              .Take(10)
              .ToList();
        }

        var result = models
          .Where(c => c.Contains(paramSearch, StringComparison.OrdinalIgnoreCase))
          .Select(r => new InlineQueryResultArticle(r, r, new InputTextMessageContent(r)))
          .ToList();

        if (result.Count > 50)
        {
            return result.Take(10).ToList();
        }

        return result;
    }

    public static List<InlineQueryResultArticle> GenerateCitiesInlineListAsync(IEnumerable<CityModel> models, string? paramSearch = "")
    {
        if (string.IsNullOrEmpty(paramSearch))
        {
            return models
              .Select(r => new InlineQueryResultArticle(r.Name, r.Name, new InputTextMessageContent(r.Name))
              {
                  ThumbHeight = 320,
                  ThumbWidth = 10,
                  ThumbUrl = r.PhotoUrl
              })
              .Take(10)
              .ToList();
        }

        var result = models
          .Where(c => c.Name.Contains(paramSearch, StringComparison.OrdinalIgnoreCase))
          .Select(r => new InlineQueryResultArticle(r.Name, r.Name, new InputTextMessageContent(r.Name))
          {
              ThumbHeight = 320,
              ThumbWidth = 10,
              ThumbUrl = r.PhotoUrl
          })
          .ToList();

        if (result.Count > 50)
        {
            return result.Take(10).ToList();
        }

        return result;
    }
}
