using System.Collections.Generic;

using CoffeeBot.Models.DB;

namespace CoffeeBot.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<CoffeeBot.Models.DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CoffeeBot.Models.DataContext context)
        {
        //     context.Database.ExecuteSqlCommand(@"DELETE coffee.Texts
        //                                         FROM coffee.Texts AS T
        //                                             JOIN coffee.LocalizationTexts AS LT
        //                                                 ON LT.Text_Id = T.Id
        //     
        //                                         DELETE coffee.Localizations");

            context.Localization.AddOrUpdate(new Localization
            {
                Name = LocalizationKeys.FEATURES.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "Features:"},
                    new Text { Language = Language.UA, Value = "Особливості:"},
                    new Text { Language = Language.RU, Value = "Особенности:"},
                }
            });
            context.Localization.AddOrUpdate(new Localization
            {
                Name = LocalizationKeys.LIST_HEADER.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "Here are places I found nearby you:"},
                    new Text { Language = Language.UA, Value = "Ось що я знайшов поруч з тобою:"},
                    new Text { Language = Language.RU, Value = "Вот, что я нашел рядом с тобой:"},
                }
            });
            context.Localization.AddOrUpdate(new Localization
            {
                Name = LocalizationKeys.THANKS_FOR_SUGGESTION.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "Thanks! Your comment will be reviewed as soon as possible!"},
                    new Text { Language = Language.UA, Value = "Дякуємо! Твій коментар буде розглянуто найближчим часом!"},
                    new Text { Language = Language.RU, Value = "Спасибо! Твой комментарий будет рассмотрен в ближайшее время!"},
                }
            });
            context.Localization.AddOrUpdate(new Localization
            {
                Name = LocalizationKeys.SUGGEST.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "Send your suggestion or comment"},
                    new Text { Language = Language.UA, Value = "Напиши свою пропозицію або зауваження"},
                    new Text { Language = Language.RU, Value = "Напиши свое предложение или замечание"},
                }
            });
            context.Localization.AddOrUpdate(new Localization
            {
                Name = LocalizationKeys.EMOJI_LIST.ToString(),
                Texts = new List<Text> {
                    new Text {
                        Language = Language.EN,
                        Value = $"{Helper.Emoji[Perk.WiFi]} - WiFi\n" +
                                $"{Helper.Emoji[Perk.Alcohol]} - you can buy alcohol here\n" +
                                $"{Helper.Emoji[Perk.CoWorking]} - you can work from here\n" +
                                $"{Helper.Emoji[Perk.CoffeeToGo]} - you can take coffee to go\n" +
                                $"{Helper.Emoji[Perk.Kitchen]} - you can eat here\n" +
                                $"{Helper.Emoji[Perk.NonDairyMilk]} - this place has vegetable milk\n" +
                                $"{Helper.Emoji[Perk.PetFriendly]} - pet friendly\n" +
                                $"{Helper.Emoji[Perk.Restroom]} - restroom\n" +
                                $"{Helper.Emoji[Perk.SaleOfCoffeeBeans]} - you can buy coffee beans here"
                    },
                    new Text {
                        Language = Language.UA,
                        Value = $"{Helper.Emoji[Perk.WiFi]} - є WiFi\n" +
                                $"{Helper.Emoji[Perk.Alcohol]} - є алкоголь\n" +
                                $"{Helper.Emoji[Perk.CoWorking]} - можна попрацювати\n" +
                                $"{Helper.Emoji[Perk.CoffeeToGo]} - можна взяти каву з собою\n" +
                                $"{Helper.Emoji[Perk.Kitchen]} - є кухня\n" +
                                $"{Helper.Emoji[Perk.NonDairyMilk]} - є рослинне молоко\n" +
                                $"{Helper.Emoji[Perk.PetFriendly]} - pet friendly\n" +
                                $"{Helper.Emoji[Perk.Restroom]} - є вбиральня\n" +
                                $"{Helper.Emoji[Perk.SaleOfCoffeeBeans]} - продається кава в зернах"
                    },
                    new Text {
                        Language = Language.RU,
                        Value = $"{Helper.Emoji[Perk.WiFi]} - есть WiFi\n" +
                                $"{Helper.Emoji[Perk.Alcohol]} - есть алкоголь\n" +
                                $"{Helper.Emoji[Perk.CoWorking]} - можно поработать\n" +
                                $"{Helper.Emoji[Perk.CoffeeToGo]} - можно взять кофе с собой\n" +
                                $"{Helper.Emoji[Perk.Kitchen]} - есть кухня\n" +
                                $"{Helper.Emoji[Perk.NonDairyMilk]} - есть растительное молоко\n" +
                                $"{Helper.Emoji[Perk.PetFriendly]} - pet friendly\n" +
                                $"{Helper.Emoji[Perk.Restroom]} - есть уборная\n" +
                                $"{Helper.Emoji[Perk.SaleOfCoffeeBeans]} - продается кофе в зернах"
                    }
                }
            });
            context.Localization.AddOrUpdate(new Localization
            {
                Name = LocalizationKeys.EMOJI_DESC.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "What do emoji in place's features mean?"},
                    new Text { Language = Language.UA, Value = "Що означають емоджі в особливостях закладу?"},
                    new Text { Language = Language.RU, Value = "Что означают эмоджи в особенностях заведения?"},
                }
            });
            context.Localization.AddOrUpdate(
                new Localization {
                    Name = LocalizationKeys.BOT_COMMANDS.ToString(),
                    Texts = new List<Text> {
                        new Text {
                            Language = Language.EN,
                            Value = "/language - language selection\n" +
                                "/where - list of cities\n" +
                                "/suggest - send a suggestion or comment to the author\n" +
                                "/help - read help"
                        },
                        new Text {
                            Language = Language.UA,
                            Value = "/language - вибір мови\n" +
                                "/where - перелік міст\n" +
                                "/suggest - відправити пропозицію або зауваження автору\n" +
                                "/help - виклик довідки"
                        },
                        new Text {
                            Language = Language.RU,
                            Value = "/language - выбор языка\n" +
                                "/where - список городов\n" +
                                "/suggest - отправить предложение или замечание автору\n" +
                                "/help - вызов справки"
                        },
                    }
                });
            context.Localization.AddOrUpdate(new Localization
            {
                Name = LocalizationKeys.DESC_AND_REVIEWS.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "Description and reviews of found coffee places:"},
                    new Text { Language = Language.UA, Value = "Опис та відгуки про знайдені кав'ярні:"},
                    new Text { Language = Language.RU, Value = "Описание и отзывы найденных кофеен:"},
                }
            });
            context.Localization.AddOrUpdate(
                new Localization {
                    Name = LocalizationKeys.HELP_TEXT.ToString(),
                    Texts = new List<Text> {
                        new Text {
                            Language = Language.EN,
                            Value = "Share your location to find a third wave coffee house nearby.\n" +
                                "Any questions? Ask @immelstornn\n" +
                                "\n" +
                                "Click on the address of the coffee shop in the list to open the map.\n" +
                                "Use the buttons below the list to read information about coffee shops or leave a review.\n"
                        },
                        new Text {
                            Language = Language.UA,
                            Value = "Відправ свою локацію, щоб знайти кав'ярню третьої хвилі поруч.\n" +
                                "Є питання? Пиши @immelstornn\n" +
                                "\n" +
                                "Натисни на адресу кав'ярні у списку, щоб відкрити карту.\n" +
                                "Використовуй кнопки під списком, щоб прочитати інформацію про кав'ярні або залишити відгук.\n"
                        },
                        new Text {
                            Language = Language.RU,
                            Value = "Отправь свою локацию, чтобы найти кофейню третьей волны рядом.\n" +
                                "Есть вопросы? Пиши @immelstorn\n" +
                                "\n" +
                                "Нажми на адрес кофейни в списке, чтобы открыть карту.\n" +
                                "Используй кнопки под списком, чтобы прочитать информацию про кофейни или оставить отзыв.\n"
                        },
                    }
                });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.M.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "m"},
                    new Text { Language = Language.UA, Value = "м"},
                    new Text { Language = Language.RU, Value = "м"},
                }
            });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.KM.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "km"},
                    new Text { Language = Language.UA, Value = "км"},
                    new Text { Language = Language.RU, Value = "км"},
                }
            });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.THE_CLOSEST_PLACE.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "The closest place:"},
                    new Text { Language = Language.UA, Value = "Найближчий заклад:"},
                    new Text { Language = Language.RU, Value = "Ближайшее заведение:"},
                }
            });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.TRY_INCREASE_RADIUS.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "Try to increase the search radius."},
                    new Text { Language = Language.UA, Value = "Спробуй збільшити радіус пошуку."},
                    new Text { Language = Language.RU, Value = "Попробуй увеличить радиус поиска."},
                }
            });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.NOTHING_FOUND_NEARBY.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "Nothing found nearby 🙁"},
                    new Text { Language = Language.UA, Value = "Поруч з тобою нічого не знайдено 🙁"},
                    new Text { Language = Language.RU, Value = "Рядом с тобой ничего не найдено 🙁"},
                }
            });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.READ_REVIEWS.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "Read reviews"},
                    new Text { Language = Language.UA, Value = "Прочитати відгуки"},
                    new Text { Language = Language.RU, Value = "Прочитать отзывы"},
                }
            });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.EDIT_FEEDBACK.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "Edit feedback:"},
                    new Text { Language = Language.UA, Value = "Змінити відгук"},
                    new Text { Language = Language.RU, Value = "Изменить отзыв"},
                }
            });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.YOUR_FEEDBACK.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "Your feedback:"},
                    new Text { Language = Language.UA, Value = "Твій відгук:"},
                    new Text { Language = Language.RU, Value = "Твой отзыв:"},
                }
            });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.LEAVE_FEEDBACK.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "Leave feedback"},
                    new Text { Language = Language.UA, Value = "Залишити відгук"},
                    new Text { Language = Language.RU, Value = "Оставить отзыв"},
                }
            });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.YOUR_RATING.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "Your rating:"},
                    new Text { Language = Language.UA, Value = "Твоя оцінка:"},
                    new Text { Language = Language.RU, Value = "Твоя оценка:"},
                }
            });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.WEEKEND.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "Weekend:"},
                    new Text { Language = Language.UA, Value = "Bихідні:"},
                    new Text { Language = Language.RU, Value = "Выходные:"},
                }
            });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.WEEKDAYS.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "Weekdays:"},
                    new Text { Language = Language.UA, Value = "Будні:"},
                    new Text { Language = Language.RU, Value = "Будние:"},
                }
            });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.WORKING_HOURS.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "Working hours:"},
                    new Text { Language = Language.UA, Value = "Час роботи:"},
                    new Text { Language = Language.RU, Value = "Время работы:"},
                }
            });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.NO_RATING.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "No rating"},
                    new Text { Language = Language.UA, Value = "Оцінка відсутня"},
                    new Text { Language = Language.RU, Value = "Оценка отсутствует"},
                }
            });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.RATING.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "Rating"},
                    new Text { Language = Language.UA, Value = "Оцінка"},
                    new Text { Language = Language.RU, Value = "Оценка"},
                }
            });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.NO_COMMENTS_YET.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "Nobody has left a comment yet, you can be the first!"},
                    new Text { Language = Language.UA, Value = "Ніхто ще не залишив коментарів, ти можеш бути першим!"},
                    new Text { Language = Language.RU, Value = "Никто еще не оставил комментариев, ты можешь быть первым!"},
                }
            });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.THANKS_FOR_COMMENT.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "Your comment has been saved, thanks!"},
                    new Text { Language = Language.UA, Value = "Коментар збережений, дякую!"},
                    new Text { Language = Language.RU, Value = "Комментарий сохранен, спасибо!"},
                }
            });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.INFO_ABOUT_CITIES.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "I can find coffee place for you in these cities:"},
                    new Text { Language = Language.UA, Value = "Я можу знайти кав‘ярню для тебе у таких містах:"},
                    new Text { Language = Language.RU, Value = "Я могу найти кофейню для тебя в таких городах:"},
                }
            });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.HI_MESSAGE.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "Hi, share your location to find third wave coffee house nearby."},
                    new Text { Language = Language.UA, Value = "Привіт, надішли свою локацію, щоб знайти кав‘ярню третьої хвилі поруч."},
                    new Text { Language = Language.RU, Value = "Привет, отправь свою локацию, чтобы найти кофейню третьей волны рядом."},
                }
            });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.SEND_LOCATION.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "📍 Share location"},
                    new Text { Language = Language.UA, Value = "📍 Надіслати місцезнаходження"},
                    new Text { Language = Language.RU, Value = "📍 Отправить местоположение"},
                }
            });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.WRITE_COMMENT.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "Write your thoughts about this place. No more than 300 characters, please."},
                    new Text { Language = Language.UA, Value = "Напиши, що ти думаєш про це місце. Не більше 300 символів, будь ласка."},
                    new Text { Language = Language.RU, Value = "Напиши, что ты думаешь об этом месте. Не больше 300 символов, пожалуйста."},
                }
            });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.BACK.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "Back"},
                    new Text { Language = Language.UA, Value = "Назад"},
                    new Text { Language = Language.RU, Value = "Назад"},
                }
            });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.COMMENT.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "Comment"},
                    new Text { Language = Language.UA, Value = "Коментар"},
                    new Text { Language = Language.RU, Value = "Комментарий"},
                }
            });
            context.Localization.AddOrUpdate(new Localization
            {
                Name = LocalizationKeys.LEAVE_COMMENT.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "Leave comment"},
                    new Text { Language = Language.UA, Value = "Залишити коментар"},
                    new Text { Language = Language.RU, Value = "Оставить комментарий"},
                }
            });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.STARS_5.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "⭐⭐⭐⭐⭐"},
                    new Text { Language = Language.UA, Value = "⭐⭐⭐⭐⭐"},
                    new Text { Language = Language.RU, Value = "⭐⭐⭐⭐⭐"},
                }
            });
            context.Localization.AddOrUpdate(new Localization {
                Name = LocalizationKeys.STARS_4.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "⭐⭐⭐⭐"},
                    new Text { Language = Language.UA, Value = "⭐⭐⭐⭐"},
                    new Text { Language = Language.RU, Value = "⭐⭐⭐⭐"},
                }
            });
            context.Localization.AddOrUpdate(new Localization
            {
                Name = LocalizationKeys.STARS_3.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "⭐⭐⭐"},
                    new Text { Language = Language.UA, Value = "⭐⭐⭐"},
                    new Text { Language = Language.RU, Value = "⭐⭐⭐"},
                }
            });
            context.Localization.AddOrUpdate(new Localization
            {
                Name = LocalizationKeys.STARS_2.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "⭐⭐"},
                    new Text { Language = Language.UA, Value = "⭐⭐"},
                    new Text { Language = Language.RU, Value = "⭐⭐"},
                }
            });
            context.Localization.AddOrUpdate(new Localization
            {
                Name = LocalizationKeys.STARS_1.ToString(),
                Texts = new List<Text> {
                    new Text { Language = Language.EN, Value = "⭐"},
                    new Text { Language = Language.UA, Value = "⭐"},
                    new Text { Language = Language.RU, Value = "⭐"},
                }
            });
        }
    }
}
