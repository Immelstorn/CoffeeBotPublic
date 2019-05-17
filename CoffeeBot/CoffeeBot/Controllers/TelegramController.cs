using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

using CoffeeBot.Models;
using CoffeeBot.Models.DB;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

using User = CoffeeBot.Models.DB.User;

namespace CoffeeBot.Controllers
{
    public class TelegramController : ApiController
    {
        #region Fields
        private readonly List<string> _logs;
        private readonly TelegramBotClient _bot = new TelegramBotClient(ConfigurationManager.AppSettings["Token"]);
        

        private const int DefaultCount = 5;
        private const int DefaultDistance = 1000;

        private Language _lang = Language.UA;

        #endregion

        #region Public
        public TelegramController()
        {
            _logs = new List<string>();
        }

        public async Task Post([FromBody]Update update)
        {
            try
            {
                using (var db = new DataContext())
                {
                    await ProcessUpdate(update, db);
                }
            }
            catch (AggregateException a)
            {
                var chatId = update.Message?.Chat.Id ?? update.CallbackQuery.Message.Chat.Id;
                foreach (var exception in a.InnerExceptions)
                {
                    await WriteLogs("================================================================");
                    await WriteLogs(exception.Message, chatId);
                    await WriteLogs(exception.StackTrace, chatId);
                }
                await SendMessageToAuthor(string.Join("\n", _logs), null);
            }
            catch (Exception e) when (e.Message != "query is too old and response timeout expired or query ID is invalid" &&
                                      e.Message != "message is not modified: specified new message content and reply markup are exactly the same as a current content and reply_markup of the message")
            {
                await WriteLogs("Error from:");
                await WriteLogs("update.Message?.Chat.Id: "+ update.Message?.Chat?.Id);
                await WriteLogs("update.CallbackQuery.Message?.Chat.Id: " + update.CallbackQuery?.Message?.Chat?.Id);

                var chatId = update.Message?.Chat?.Id ?? update.CallbackQuery?.Message?.Chat?.Id;

                await WriteLogs("================================================================");
                await WriteLogs(e.Message, chatId);
                await WriteLogs(e.StackTrace, chatId);
                await SendMessageToAuthor(string.Join("\n", _logs), null);
            }
        }

        public async Task<string> Get()
        {
            return "1";
        }
        #endregion

        #region Privates
        private async Task ProcessUpdate(Update update, DataContext db)
        {
            var user = await SetStats(update, db);
            if (user == null)
            {
                return;
            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                var data = update.CallbackQuery.Data.Split(' ');
                await WriteLogs("Callback data", user.ChatId, update.CallbackQuery.Data);

                switch (data[0])
                {
                    case "info":
                        var id = int.Parse(data[1]);
                        await Info(id, user, db);
                        break;

                    case "infoback":
                        id = int.Parse(data[1]);
                        await Info(id, user, db, update.CallbackQuery.Message.MessageId);
                        break;

                    case "setrate":
                        id = int.Parse(data[1]);
                        var markup = new InlineKeyboardMarkup(
                            new List<IEnumerable<InlineKeyboardButton>> {
                                new List<InlineKeyboardButton> {
                                    InlineKeyboardButton.WithCallbackData(await Helper.GetLocalizationAsync(LocalizationKeys.STARS_1, _lang, db), $"rate {id} 1"),
                                    InlineKeyboardButton.WithCallbackData(await Helper.GetLocalizationAsync(LocalizationKeys.STARS_2, _lang, db), $"rate {id} 2"),
                                    InlineKeyboardButton.WithCallbackData(await Helper.GetLocalizationAsync(LocalizationKeys.STARS_3, _lang, db), $"rate {id} 3"),
                                },
                                new List<InlineKeyboardButton> {
                                    InlineKeyboardButton.WithCallbackData(await Helper.GetLocalizationAsync(LocalizationKeys.STARS_4, _lang, db), $"rate {id} 4"),
                                    InlineKeyboardButton.WithCallbackData(await Helper.GetLocalizationAsync(LocalizationKeys.STARS_5, _lang, db), $"rate {id} 5")
                                },
                                new List<InlineKeyboardButton> {
                                    InlineKeyboardButton.WithCallbackData(await Helper.GetLocalizationAsync(LocalizationKeys.COMMENT, _lang, db), $"comment {id}"),
                                    InlineKeyboardButton.WithCallbackData(await Helper.GetLocalizationAsync(LocalizationKeys.BACK, _lang, db), $"infoback {id} "),
                                }
                            });
                        await _bot.EditMessageReplyMarkupAsync(user.ChatId, update.CallbackQuery.Message.MessageId, markup);
                        break;

                    case "comments": //read comments
                        id = int.Parse(data[1]);
                        await SendComments(user, db, id, 0, null);
                        break;

                    case "commentspage": //read comments
                        id = int.Parse(data[1]);
                        var page = int.Parse(data[2]);
                        await SendComments(user, db, id, page, update.CallbackQuery.Message.MessageId);
                        break;

                    case "comment": //leave comments
                        await SetUserMode(user, UserMode.WaitForComment, db, data[1]);
                        await _bot.SendTextMessageAsync(user.ChatId,
                            await Helper.GetLocalizationAsync(LocalizationKeys.WRITE_COMMENT, _lang, db), 
                            replyMarkup: await LocationKeyboardMarkup(db));
                        break;

                    case "comment2": //leave comments
                        await SetUserMode(user, UserMode.WaitForComment, db, data[1]);
                        await _bot.EditMessageTextAsync(
                            user.ChatId,
                            update.CallbackQuery.Message.MessageId,
                            await Helper.GetLocalizationAsync(LocalizationKeys.WRITE_COMMENT, _lang, db),
                            ParseMode.Markdown);
                        break;

                    case "rate":
                        id = int.Parse(data[1]);
                        var rating = int.Parse(data[2]);
                        await SetRating(user, id, rating, db);
                        await Info(id, user, db, update.CallbackQuery.Message.MessageId);
                        break;

                    case "page":
                        await SendPlacesPage(user, new MessageParams(data[1].Split(';')), db, update.CallbackQuery.Message.MessageId);
                        break;

                    case "distance":
                        var param = new MessageParams(data[1].Split(';'))
                        {
                            Page = 0 //distance is changed, setting page to 0
                        };
                        await SendPlacesPage(user, param, db, update.CallbackQuery.Message.MessageId);
                        break;
                }

                await _bot.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                return;
            }

            var text = update.Message.Text;

            if (!string.IsNullOrEmpty(text))
            {
                await WriteLogs("Message text", user.ChatId, text);

                if (user.IsNew || text.Equals("/language"))
                {
                    await SetUserMode(user, UserMode.None, db);
                    await ChooseLangauge(db, user);
                    return;
                }

                if (text.Equals("/where"))
                {
                    await SetUserMode(user, UserMode.None, db);
                    var sb = new StringBuilder();
                    sb.AppendLine(await Helper.GetLocalizationAsync(LocalizationKeys.INFO_ABOUT_CITIES, _lang, db));
                    var cities = await db.Places.Where(p => p.Active)
                        .Select(c => c.City)
                        .OrderBy(c => c.Country)
                        .ToListAsync();

                    var groups = cities.GroupBy(c => c.Country).ToList();
                    var ua = groups.First(g => g.Key == "UA")
                        .Select(c => $"{Helper.IsoCountryCodeToFlagEmoji(c.Country)} {c.GetName(_lang)}")
                        .Distinct()
                        .OrderBy(n => n)
                        .ToList();

                    foreach (var city in ua)
                    {
                        sb.AppendLine($"• {city}");
                    }

                    sb.AppendLine();

                    foreach (var group in groups.Where(g => g.Key != "UA"))
                    {
                        var names = group
                            .Select(c => $"{Helper.IsoCountryCodeToFlagEmoji(c.Country)} {c.GetName(_lang)}")
                            .Distinct()
                            .OrderBy(n => n)
                            .ToList();

                        foreach (var name in names)
                        {
                            sb.AppendLine($"• {name}");
                        }
                    }

                    await _bot.SendTextMessageAsync(user.ChatId, sb.ToString(), replyMarkup: await LocationKeyboardMarkup(db));
                    return;
                }

                if (text.Equals("/suggest"))
                {
                    await SetUserMode(user, UserMode.None, db);
                    await _bot.SendTextMessageAsync(user.ChatId, await Helper.GetLocalizationAsync(LocalizationKeys.SUGGEST, _lang, db), replyMarkup: await LocationKeyboardMarkup(db));
                    await SetUserMode(user, UserMode.Suggest, db);
                    return;
                }

                if (text.Equals("/help"))
                {
                    await SetUserMode(user, UserMode.None, db);
                    await SendHelp(user, db);
                    return;
                }

                if (user.ChatId.ToString() == ConfigurationManager.AppSettings["MyTelegramChatId"])
                {
                    if (text.Equals("/stat"))
                    {
                        var lastUsers = await db.Users.OrderByDescending(u => u.Id).
                            Take(10).
                            ToListAsync();

                        var usersCount = await db.Users.CountAsync();
                        await SendAdminStat(lastUsers, user, usersCount);
                        return;
                    }

                    if (text.Equals("/last"))
                    {
                        var lastUsers = await db.Users.OrderByDescending(u => u.LastMessageTime).
                            Take(10).
                            ToListAsync();

                        await SendAdminStat(lastUsers, user);
                        return;
                    }

                    if (text.Equals("/top"))
                    {
                        var topUsers = await db.Users.OrderByDescending(u => u.MessageCount).
                            Take(10).
                            ToListAsync();

                        await SendAdminStat(topUsers, user);
                        return;
                    }
                }

                switch (user.UserMode)
                {
                    case UserMode.None:
                        //if nothing else
                        await SendHelp(user, db);
                        return;

                    case UserMode.WaitForComment:
                        var comment = text.Length > 300 ? text.Substring(0, 300) : text;
                        await SetComment(user, int.Parse(user.UserModeParams), comment, db);
                        await SetUserMode(user, UserMode.None, db, null);
                        await _bot.SendTextMessageAsync(
                            user.ChatId,
                            await Helper.GetLocalizationAsync(LocalizationKeys.THANKS_FOR_COMMENT, _lang, db),
                            replyMarkup: await LocationKeyboardMarkup(db));
                        return;

                    case UserMode.SetLang:

                        if (text.Equals("🇺🇦 Українська"))
                        {
                            user.Settings.Language = _lang = Language.UA;
                        }
                        else if(text.Equals("🇺🇸 English"))
                        {
                            user.Settings.Language = _lang = Language.EN;
                        }
                        else if (text.Equals("🇷🇺 Русский"))
                        {
                            user.Settings.Language = _lang = Language.RU;
                        }
                        else
                        {
                            await ChooseLangauge(db, user);
                            return;
                        }

                        await SetUserMode(user, UserMode.None, db);

                        var replyKeyboard = new ReplyKeyboardMarkup(
                            new List<KeyboardButton> {
                                new KeyboardButton {
                                    RequestLocation = true,
                                    Text = await Helper.GetLocalizationAsync(LocalizationKeys.SEND_LOCATION, _lang, db)
                                }},
                            resizeKeyboard: true);

                        await _bot.SendTextMessageAsync(user.ChatId, 
                            await Helper.GetLocalizationAsync(LocalizationKeys.HI_MESSAGE, _lang, db),
                            replyMarkup: replyKeyboard);

                        return;

                    case UserMode.Suggest:
                        await SetUserMode(user, UserMode.None, db);
                        await SaveSuggestion(user, text, db);
                        await _bot.SendTextMessageAsync(user.ChatId,
                            await Helper.GetLocalizationAsync(LocalizationKeys.THANKS_FOR_SUGGESTION, _lang, db),
                            replyMarkup: await LocationKeyboardMarkup(db));

                        return;
                }

            }

            if (update.Message.Location != null)
            {
                await WriteLogs("Location", user.ChatId, $"{update.Message.Location.Longitude};{update.Message.Location.Latitude}");

                await SendPlacesPage(
                    user, 
                    new MessageParams {
                        Longitude = update.Message.Location.Longitude,
                        Latitude = update.Message.Location.Latitude,
                        Page = 0,
                        Distance = DefaultDistance
                    }, 
                    db);
            }
        }

        private async Task SendAdminStat(List<User> users, User userToSend, int count = 0)
        {
            var sb = new StringBuilder();
            if (count != 0)
            {
                sb.AppendLine("Users: " + count);
            }

            foreach (var u in users)
            {
                var name = u.Username != null ? $@"{u.Username}" : $"{u.FirstName} {u.LastName}";
                if (string.IsNullOrEmpty(name.Trim()))
                {
                    name = u.ChatId.ToString();
                }
                sb.AppendLine($"{name}\t\t\t{u.Created:g}\t\t\t{u.LastMessageTime:g}\t\t\t{u.MessageCount}");
            }
            await _bot.SendTextMessageAsync(userToSend.ChatId, sb.ToString());
        }

        private async Task SaveSuggestion(User user, string text, DataContext db)
        {
            db.Suggestions.Add(
                new Suggestion {
                    DateTime = DateTime.UtcNow,
                    Text = text,
                    User = user
                });
            await db.SaveChangesAsync();

            var name = user.Username ?? $"{user.FirstName} {user.LastName}";
            if (string.IsNullOrEmpty(name))
            {
                name = user.ChatId.ToString();
            }

            var sb = new StringBuilder();
            sb.AppendLine("<b>New suggestion:</b>");
            sb.AppendLine($"<b>User:</b> {name}");
            sb.AppendLine($"<b>Text:</b> {text}");
            await SendMessageToAuthor(sb.ToString(), db, ParseMode.Html);
        }

        private async Task ChooseLangauge(DataContext db, User user)
        {
            await SetUserMode(user, UserMode.SetLang, db);

            var replyKeyboard = new ReplyKeyboardMarkup(
                new List<KeyboardButton> {
                    new KeyboardButton {
                        Text = "🇺🇦 Українська"
                    },
                    new KeyboardButton {
                        Text = "🇺🇸 English"
                    },
                    new KeyboardButton {
                        Text = "🇷🇺 Русский"
                    }
                },
                resizeKeyboard: true);

            await _bot.SendTextMessageAsync(user.ChatId,
                "🇺🇦 Обери мову спілкування\n🇺🇸 Choose your language\n🇷🇺 Выбери язык",
                replyMarkup: replyKeyboard);
        }

        private async Task SendComments(User user, DataContext db, int id, int page, int? messageId = null)
        {
            var place = await db.Places.FirstAsync(p => p.Id == id);
            var sb = new StringBuilder();
            sb.AppendLine($"<b>{place.Name}</b>");

            var comments = place.Ratings
                .Where(r => !string.IsNullOrEmpty(r.Comment) && r.NeedReview == false)
                .OrderByDescending(c => c.Timestamp)
                .ToList();

            if (!comments.Any())
            {
                var replyMarkup = new InlineKeyboardMarkup(
                    new List<InlineKeyboardButton> {
                        InlineKeyboardButton.WithCallbackData(await Helper.GetLocalizationAsync(LocalizationKeys.LEAVE_COMMENT, _lang, db), $"comment2 {id}")
                    });

                sb.AppendLine(await Helper.GetLocalizationAsync(LocalizationKeys.NO_COMMENTS_YET, _lang, db));
                await _bot.SendTextMessageAsync(
                    user.ChatId,
                    sb.ToString(),
                    ParseMode.Html,
                    disableWebPagePreview: true,
                    replyMarkup: replyMarkup);
                return;
            }

            var commentsInlineKeyboard = new List<InlineKeyboardButton>();
            if (page != 0)
            {
                commentsInlineKeyboard.Add(InlineKeyboardButton.WithCallbackData("⬅️", $"commentspage {id} {page - 1}"));
            }


            var lastItemToShow = page * DefaultCount + DefaultCount;
            for (var i = page * DefaultCount; i < lastItemToShow; i++)
            {
                if (i == comments.Count)
                {
                    break;
                }

                var comment = comments[i];
                var stars = comment.Stars == 0 ? await Helper.GetLocalizationAsync(LocalizationKeys.NO_RATING, _lang, db) : Helper.GetStars(comment.Stars);

                sb.AppendLine();
                sb.AppendLine($"<b>{comment.Timestamp:d}</b>");
                sb.AppendLine($"{i + 1}. {stars}");
                sb.AppendLine(comment.Comment);
            }

            if (lastItemToShow < comments.Count)
            {
                commentsInlineKeyboard.Add(InlineKeyboardButton.WithCallbackData("➡️", $"commentspage {id} {page + 1}"));
            }

            var markup = new InlineKeyboardMarkup(
                new List<IEnumerable<InlineKeyboardButton>> {
                    commentsInlineKeyboard,
                });

            if (messageId.HasValue)
            {
                await _bot.EditMessageTextAsync(user.ChatId, messageId.Value, sb.ToString(), ParseMode.Html, replyMarkup: markup, disableWebPagePreview: true);
            }
            else
            {
                await _bot.SendTextMessageAsync(
                    user.ChatId,
                    sb.ToString(),
                    ParseMode.Html,
                    replyMarkup: markup,
                    disableWebPagePreview: true);
            }
        }

        private async Task SetRating(User user, int id, int rating, DataContext db)
        {
            var place = await db.Places.FindAsync(id);
            var dbUser = await db.Users.Where(u => u.ChatId == user.ChatId).FirstOrDefaultAsync();
            if (place != null)
            {
                var userRating = place.Ratings.FirstOrDefault(r => r.User.ChatId == user.ChatId);
                if (userRating != null)
                {
                    userRating.Stars = rating;
                }
                else
                {
                    place.Ratings.Add(
                        new Rating {
                            Stars = rating,
                            User = dbUser
                        });
                }

                await db.SaveChangesAsync();
            }
        }

        private async Task SetComment(User user, int id, string comment, DataContext db)
        {
            var place = await db.Places.FindAsync(id);
            if (place != null)
            {
                var userRating = place.Ratings.FirstOrDefault(r => r.User.Id == user.Id);
                if (userRating != null)
                {
                    userRating.Comment = comment;
                }
                else
                {
                    place.Ratings.Add(
                        new Rating {
                            Comment = comment,
                            User = user
                        });
                }

                await db.SaveChangesAsync();

                var name = user.Username ?? $"{user.FirstName} {user.LastName}";
                if (string.IsNullOrEmpty(name))
                {
                    name = user.ChatId.ToString();
                }
                var sb = new StringBuilder();
                sb.AppendLine("<b>New comment:</b>");
                sb.AppendLine($"<b>User:</b> {name}");
                sb.AppendLine($"<b>Place:</b> {place.Name}");
                sb.AppendLine($"<b>COmment:</b> {comment}");
                await SendMessageToAuthor(sb.ToString(), db, ParseMode.Html);
            }
        }

       
        private async Task Info(int id, User user, DataContext db, int? messageId = null)
        {
            var place = await db.Places.FirstAsync(p => p.Id == id);
            decimal avgRating = place.Ratings.Any() ? place.Ratings.Sum(r => r.Stars) / place.Ratings.Count : 0;
            var avgRatingStr = avgRating > 0
                ? $"*{await Helper.GetLocalizationAsync(LocalizationKeys.RATING, _lang, db)}:* {Helper.GetStars(Convert.ToInt32(Math.Ceiling(avgRating)))}"
                : string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine($"☕ *{place.Name}*");
            sb.AppendLine(Helper.GetAddressLink(place, _lang));
            if (!string.IsNullOrEmpty(place.GetDescription(_lang)))
            {
                sb.AppendLine();
                sb.AppendLine(place.GetDescription(_lang));
            }

            if (avgRating > 0)
            {
                sb.AppendLine();
                sb.AppendLine(avgRatingStr);
            }

            sb.AppendLine();
            sb.AppendLine($"*{await Helper.GetLocalizationAsync(LocalizationKeys.FEATURES, _lang, db)}* {Helper.GetPerks(place.Perks)}");

            if (place.OpenTime.HasValue && place.OpenTimeWeekend.HasValue && place.CloseTime.HasValue && place.CloseTimeWeekend.HasValue)
            {
                sb.AppendLine();
                sb.AppendLine($"*{await Helper.GetLocalizationAsync(LocalizationKeys.WORKING_HOURS, _lang, db)}*");
                sb.AppendLine($"🕓 {await Helper.GetLocalizationAsync(LocalizationKeys.WEEKDAYS, _lang, db)} {place.OpenTime.Value:hh\\:mm} - {place.CloseTime.Value:hh\\:mm}");
                sb.AppendLine($"🕓 {await Helper.GetLocalizationAsync(LocalizationKeys.WEEKEND, _lang, db)} {place.OpenTimeWeekend.Value:hh\\:mm} - {place.CloseTimeWeekend.Value:hh\\:mm}");
            }
           

            var userRating = place.Ratings.FirstOrDefault(r => r.User.Id == user.Id);
            if (userRating != null)
            {
                sb.AppendLine();
                if (userRating.Stars > 0)
                {
                    sb.AppendLine($"*{await Helper.GetLocalizationAsync(LocalizationKeys.YOUR_RATING, _lang, db)}* {Helper.GetStars(userRating.Stars)}");
                }

                if (!string.IsNullOrEmpty(userRating.Comment))
                {
                    sb.AppendLine($"*{await Helper.GetLocalizationAsync(LocalizationKeys.YOUR_FEEDBACK, _lang, db)}* {userRating.Comment}");
                }
            }

            var markup = new InlineKeyboardMarkup(
                new List<IEnumerable<InlineKeyboardButton>> {
                    new List<InlineKeyboardButton> {
                        InlineKeyboardButton.WithCallbackData(userRating == null 
                            ? await Helper.GetLocalizationAsync(LocalizationKeys.LEAVE_FEEDBACK, _lang, db) 
                            : await Helper.GetLocalizationAsync(LocalizationKeys.EDIT_FEEDBACK, _lang, db), 
                            $"setrate {id}"),
                    },
                    new List<InlineKeyboardButton> {
                        InlineKeyboardButton.WithCallbackData(await Helper.GetLocalizationAsync(LocalizationKeys.READ_REVIEWS, _lang, db), $"comments {id}"),
                    }
                });

            if (messageId.HasValue)
            {
                await _bot.EditMessageTextAsync(user.ChatId, messageId.Value, $"{sb}", replyMarkup: markup, parseMode: ParseMode.Markdown, disableWebPagePreview: true);
            }
            else
            {
                await _bot.SendTextMessageAsync(
                    user.ChatId,
                    $"{sb}",
                    replyMarkup: markup,
                    parseMode: ParseMode.Markdown,
                    disableWebPagePreview: true);
            }
        }

        private async Task SendPlacesPage(User user, MessageParams param, DataContext db, int? messageId = null)
        {
            var places = await db.Places.Where(p => p.Active).ToListAsync();
            var distances = places
                .Select(p => new Tuple<Place, double>(p, Helper.GetDistance(param.Longitude, param.Latitude, p.Longitude, p.Latitude)))
                .OrderBy(i => i.Item2).ToList();

            var inRadius = distances.Where(p => p.Item2 < param.Distance).ToList();

            var sb = new StringBuilder();

            var infoInlineKeyboard = new List<InlineKeyboardButton>();
            if (param.Page != 0)
            {
                infoInlineKeyboard.Add(InlineKeyboardButton.WithCallbackData("⬅️", $"page {param.Page - 1};{param.Longitude};{param.Latitude};{param.Distance}"));
            }

            if (!inRadius.Any())
            {
                sb.AppendLine(await Helper.GetLocalizationAsync(LocalizationKeys.NOTHING_FOUND_NEARBY, _lang, db));
                sb.AppendLine(await Helper.GetLocalizationAsync(LocalizationKeys.TRY_INCREASE_RADIUS, _lang, db));
                sb.AppendLine();
                sb.AppendLine(await Helper.GetLocalizationAsync(LocalizationKeys.THE_CLOSEST_PLACE, _lang, db));

                var item = distances.First();
                var dist = item.Item2 < 1000
                    ? Math.Round((item.Item2), 1) + $" {await Helper.GetLocalizationAsync(LocalizationKeys.M, _lang, db)}"
                    : Math.Round((item.Item2 / 1000), 1) + $" {await Helper.GetLocalizationAsync(LocalizationKeys.KM, _lang, db)}";

                var open = Helper.GetOpenStatus(item.Item1);
                sb.AppendLine($"{open}*{item.Item1.Name}* ({char.ConvertFromUtf32(0x1F9B6)}{dist})");
                sb.AppendLine($"{Helper.GetAddressLink(item.Item1, _lang)}");
            }
            else
            {
                sb.AppendLine("🔍 " + await Helper.GetLocalizationAsync(LocalizationKeys.LIST_HEADER, _lang, db));
                var lastItemToShow = param.Page * DefaultCount + DefaultCount;
                for (var i = param.Page * DefaultCount; i < lastItemToShow; i++)
                {
                    if (i == inRadius.Count)
                    {
                        break;
                    }

                    var item = inRadius[i];
                    var dist = item.Item2 < 1000
                        ? Math.Round((item.Item2), 1) + $" {await Helper.GetLocalizationAsync(LocalizationKeys.M, _lang, db)}"
                        : Math.Round((item.Item2 / 1000), 1) + $" {await Helper.GetLocalizationAsync(LocalizationKeys.KM, _lang, db)}";

                    var open = Helper.GetOpenStatus(item.Item1);

                    sb.AppendLine();
                    sb.AppendLine($"{i + 1}. {open}*{item.Item1.Name}* ({char.ConvertFromUtf32(0x1F9B6)}{dist})");
                    sb.AppendLine($"{Helper.GetAddressLink(item.Item1, _lang)}");
                   

                    infoInlineKeyboard.Add(InlineKeyboardButton.WithCallbackData((i + 1).ToString(), $"info {item.Item1.Id.ToString()}"));
                }

                if (lastItemToShow < inRadius.Count)
                {
                    infoInlineKeyboard.Add(InlineKeyboardButton.WithCallbackData("➡️", $"page {param.Page + 1};{param.Longitude};{param.Latitude};{param.Distance}"));
                }

                sb.AppendLine();
                sb.AppendLine(await Helper.GetLocalizationAsync(LocalizationKeys.DESC_AND_REVIEWS, _lang, db));

            }

            var distanceInlineKeyboard = new List<InlineKeyboardButton> {
                InlineKeyboardButton.WithCallbackData(
                    param.Distance == 500 ? $"✅ 0.5 {await Helper.GetLocalizationAsync(LocalizationKeys.KM, _lang, db)}" : $"0.5 {await Helper.GetLocalizationAsync(LocalizationKeys.KM, _lang, db)}",
                    $"distance {param.Page};{param.Longitude};{param.Latitude};500"),
                InlineKeyboardButton.WithCallbackData(
                    param.Distance == 1000 ? $"✅ 1 {await Helper.GetLocalizationAsync(LocalizationKeys.KM, _lang, db)}" : $"1 {await Helper.GetLocalizationAsync(LocalizationKeys.KM, _lang, db)}",
                    $"distance {param.Page};{param.Longitude};{param.Latitude};1000"),
                InlineKeyboardButton.WithCallbackData(
                    param.Distance == 2000 ? $"✅ 2 {await Helper.GetLocalizationAsync(LocalizationKeys.KM, _lang, db)}" : $"2 {await Helper.GetLocalizationAsync(LocalizationKeys.KM, _lang, db)}",
                    $"distance {param.Page};{param.Longitude};{param.Latitude};2000")
            };

            var markup = new InlineKeyboardMarkup(
                new List<IEnumerable<InlineKeyboardButton>> {
                    infoInlineKeyboard,
                    distanceInlineKeyboard
                });

            if (messageId.HasValue)
            {
                await _bot.EditMessageTextAsync(user.ChatId, messageId.Value, sb.ToString(), ParseMode.Markdown, replyMarkup: markup, disableWebPagePreview: true);
            }
            else
            {
                await _bot.SendTextMessageAsync(
                    user.ChatId,
                    sb.ToString(),
                    ParseMode.Markdown,
                    replyMarkup: markup,
                    disableWebPagePreview: true);
            }
        }

        private async Task<ReplyKeyboardMarkup> LocationKeyboardMarkup(DataContext db)
        {
            return new ReplyKeyboardMarkup(
                new List<KeyboardButton> {
                    new KeyboardButton {
                        RequestLocation = true,
                        Text = await Helper.GetLocalizationAsync(LocalizationKeys.SEND_LOCATION, _lang, db)
                    }
                },
                resizeKeyboard: true);
        }

        private async Task SendHelp(User user, DataContext db)
        {
            var sb = new StringBuilder();
            sb.AppendLine(await Helper.GetLocalizationAsync(LocalizationKeys.HELP_TEXT, _lang, db));
            sb.AppendLine();
            sb.AppendLine(await Helper.GetLocalizationAsync(LocalizationKeys.BOT_COMMANDS, _lang, db));
            sb.AppendLine();
            sb.AppendLine(await Helper.GetLocalizationAsync(LocalizationKeys.EMOJI_DESC, _lang, db));
            sb.AppendLine(await Helper.GetLocalizationAsync(LocalizationKeys.EMOJI_LIST, _lang, db));

            await _bot.SendTextMessageAsync(
                user.ChatId,
                sb.ToString(),
                replyMarkup: await LocationKeyboardMarkup(db));
        }

        private async Task SetUserMode(User user, UserMode mode, DataContext db, string param = null)
        {
            var dbUser = await db.Users.FirstAsync(s => s.ChatId == user.ChatId);
            dbUser.UserMode = mode;
            dbUser.UserModeParams = param;
            await db.SaveChangesAsync();
        }

        private async Task<User> SetStats(Update update, DataContext db)
        {
            var userId = update.Message?.Chat?.Id ?? update.CallbackQuery?.Message?.Chat?.Id;
            var username = update.Message?.Chat?.Username ?? update.CallbackQuery?.Message?.Chat?.Username;
            var firstName = update.Message?.Chat?.FirstName ?? update.CallbackQuery?.Message?.Chat?.FirstName;
            var lastName = update.Message?.Chat?.LastName ?? update.CallbackQuery?.Message?.Chat?.LastName;

            if (userId == null)
            {
                return null;
            }

            var user = await db.Users.FirstOrDefaultAsync(s => s.ChatId == userId)
                ?? db.Users.Add(
                    new User {
                        ChatId = userId.Value,
                        Created = DateTime.UtcNow,
                        UserMode = UserMode.None,
                        MessageCount = 0,
                        IsNew = true,
                        Settings = new UserSettings { Language = Language.UA}
                    });

            user.FirstName = firstName;
            user.LastName = lastName;
            user.Username = username;
            user.MessageCount++;
            user.LastMessageTime = DateTime.UtcNow;
            await db.SaveChangesAsync();

            _lang = user.Settings.Language;
            return user;
        }

        private async Task SendMessageToAuthor(string text, DataContext db, ParseMode parseMode = ParseMode.Default)
        {
            if (db != null)
            {
                await _bot.SendTextMessageAsync(ConfigurationManager.AppSettings["MyTelegramChatId"], text, parseMode, replyMarkup: await LocationKeyboardMarkup(db));
            }
            else
            {
                await _bot.SendTextMessageAsync(ConfigurationManager.AppSettings["MyTelegramChatId"], text, parseMode);
            }
        }

        private async Task WriteLogs(string text, long? chatId = null, string additional = null)
        {
            _logs.Add($"{DateTime.UtcNow}: {text}");
            Trace.TraceInformation(text);

            using (var database = new DataContext())
            {
                var user = await database.Users.FirstOrDefaultAsync(u => u.ChatId == chatId);

                database.Logs.Add(
                    new Log {
                        Message = text,
                        User = user,
                        Addtitional = additional
                    });
                await database.SaveChangesAsync();
            }
        }
        #endregion
    }
}
