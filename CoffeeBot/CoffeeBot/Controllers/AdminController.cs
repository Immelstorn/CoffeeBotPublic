using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

using CoffeeBot.Models;
using CoffeeBot.Models.Admin;
using CoffeeBot.Models.DB;

namespace CoffeeBot.Controllers
{
    public class AdminController:Controller
    {
        public async Task<ActionResult> Comments()
        {
            var model = new List<CommentViewModel>();
            using (var db = new DataContext())
            {
                model = await db.Ratings.Select(
                    r => new CommentViewModel {
                        Id = r.Id,
                        Stars = r.Stars,
                        Comment = r.Comment,
                        User = r.User.Username ?? r.User.FirstName + " " + r.User.LastName,
                        Place = r.Place.Name,
                        Timestamp = r.Timestamp,
                        NeedReview = r.NeedReview
                    }).OrderByDescending(r => r.NeedReview)
                    .ThenByDescending(r => r.Timestamp).ToListAsync();
            }

            return View(model);
        }

        public async Task<ActionResult> PlacesInCity(int id)
        {
            var model = new List<PlaceViewModel>();
            using (var db = new DataContext())
            {
                var places = await db.Places.Where(c => c.City.Id == id).ToListAsync();
                model = places.Select(
                    c => new PlaceViewModel
                    {
                        Id = c.Id,
                        DescriptionEn = c.GetDescription(Language.EN),
                        DescriptionRu = c.GetDescription(Language.RU),
                        DescriptionUa = c.GetDescription(Language.UA),
                        AddressEn = c.GetAddress(Language.EN),
                        AddressRu = c.GetAddress(Language.RU),
                        AddressUa = c.GetAddress(Language.UA),
                        Name = c.Name,
                        Longitude = c.Longitude,
                        Latitude = c.Latitude,
                        Active = c.Active
                    }).ToList();
            }

            return View(model);
        }

        public async Task<ActionResult> EditCities(List<CityViewModel> model)
        {
            using (var db = new DataContext())
            {
                foreach (var item in model)
                {
                    var city = await db.Cities.FirstOrDefaultAsync(p => p.Id == item.Id);
                    city?.SetName(Language.EN, item.NameEn);
                    city?.SetName(Language.UA, item.NameUa);
                    city?.SetName(Language.RU, item.NameRu);
                    if (city != null)
                    {
                        city.Country = item.Country;
                    }
                }

                await db.SaveChangesAsync();
            }

            return RedirectToAction("Cities");

        }

        public async Task<ActionResult> Cities()
        {
            var model = new List<CityViewModel>();
            using (var db = new DataContext())
            {
                var cities = await db.Cities.Where(c => c.Places.Any()).ToListAsync();
                model = cities.Select(
                    c => new CityViewModel {
                        Id = c.Id,
                        NameEn = c.GetName(Language.EN),
                        NameRu = c.GetName(Language.RU),
                        NameUa = c.GetName(Language.UA),
                        Country = c.Country,
                        Flag = Helper.IsoCountryCodeToFlagEmoji(c.Country),
                        Count = c.Places.Count
                    }).ToList();
            }

            return View(model);

        }

        public async Task<ActionResult> Index()
        {
            return RedirectToAction("Cities");
        }

        public async Task<ActionResult> AddPlace(AddViewModel model)
        {
            using (var db = new DataContext())
            {
                var ci = await  db.Cities.FirstOrDefaultAsync(c => c.Id == model.CityId);

                const string format = "h\\:mm";
                var isOpenParsed = TimeSpan.TryParseExact(model.OpenTime, format, null, out var openParsed);
                var isCloseParsed = TimeSpan.TryParseExact(model.CloseTime, format, null, out var closeParsed);
                var isOpenWeekendParsed = TimeSpan.TryParseExact(model.OpenTimeWeekend, format, null, out var openWeekendParsed);
                var isCloseWeekendParsed = TimeSpan.TryParseExact(model.CloseTimeWeekend, format, null, out var closeWeekendParsed);

                var pl = new Place
                {
                    City = ci,
                    Name = model.Name,
                    Latitude = double.Parse(model.Lat),
                    Longitude = double.Parse(model.Long),
                    OpenTime = isOpenParsed ? openParsed : new TimeSpan(),
                    CloseTime = isCloseParsed ? closeParsed : new TimeSpan(),
                    OpenTimeWeekend = isOpenWeekendParsed ? openWeekendParsed : new TimeSpan(),
                    CloseTimeWeekend = isCloseWeekendParsed ? closeWeekendParsed : new TimeSpan()
                };
                
                pl.SetDescription(Language.EN, model.DescEn);
                pl.SetDescription(Language.UA, model.DescUa);
                pl.SetDescription(Language.RU, model.DescRu);
                
                pl.SetAddress(Language.EN, model.AddressEn);
                pl.SetAddress(Language.UA, model.AddressUa);
                pl.SetAddress(Language.RU, model.AddressRu);

                if (model.Alcohol)
                {
                    pl.Perks = pl.Perks | Perk.Alcohol;
                }
                if (model.CoWorking)
                {
                    pl.Perks = pl.Perks | Perk.CoWorking;
                }
                if (model.CoffeeToGo)
                {
                    pl.Perks = pl.Perks | Perk.CoffeeToGo;
                }
                if (model.Kitchen)
                {
                    pl.Perks = pl.Perks | Perk.Kitchen;
                }
                if (model.NonDairyMilk)
                {
                    pl.Perks = pl.Perks | Perk.NonDairyMilk;
                }
                if (model.PetFriendly)
                {
                    pl.Perks = pl.Perks | Perk.PetFriendly;
                }
                if (model.Restroom)
                {
                    pl.Perks = pl.Perks | Perk.Restroom;
                }
                if (model.SaleOfCoffeeBeans)
                {
                    pl.Perks = pl.Perks | Perk.SaleOfCoffeeBeans;
                }
                if (model.WiFi)
                {
                    pl.Perks = pl.Perks | Perk.WiFi;
                }

                pl.Active = model.Active;

                db.Places.Add(pl);
                
               await db.SaveChangesAsync();
            }
            return RedirectToAction("PlacesInCity", new { id = model.CityId });
        }

        public async Task<ActionResult> Add()
        {
            return View();
        }

        public async Task<ActionResult> Delete(int id)
        {
            using (var db = new DataContext())
            {
                var place = await db.Places.FirstOrDefaultAsync(p => p.Id == id);
                if (place != null)
                {
                    db.Ratings.RemoveRange(place.Ratings);
                    db.Places.Remove(place);
                    await db.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> DeleteComment(int id)
        {
            using (var db = new DataContext())
            {
                var comment = await db.Ratings.FirstOrDefaultAsync(p => p.Id == id);
                if (comment != null)
                {
                    db.Ratings.Remove(comment);
                    await db.SaveChangesAsync();
                }
            }
            return RedirectToAction("Comments");
        }

        public async Task<ActionResult> EditComments(List<CommentViewModel> model)
        {
            using (var db = new DataContext())
            {
                for (var i = 0; i < model.Count; i++)
                {
                    var item = model[i];
                    var rating = await db.Ratings.FirstOrDefaultAsync(p => p.Id == item.Id);
                    if (rating != null)
                    {
                        rating.Comment = item.Comment;
                        rating.NeedReview = item.NeedReview;
                    }

                    if (i % 50 == 0)
                    {
                        await db.SaveChangesAsync();
                    }
                }
                await db.SaveChangesAsync();
            }

            return RedirectToAction("Comments");
        }

        public async Task<ActionResult> Edit(List<PlaceViewModel> model)
        {
            using (var db = new DataContext())
            {
                for (var i = 0; i < model.Count; i++)
                {
                    var item = model[i];
                    var place = await db.Places.FirstOrDefaultAsync(p => p.Id == item.Id);
                    if (place != null)
                    {
                        place.SetAddress(Language.EN, item.AddressEn);
                        place.SetAddress(Language.UA, item.AddressRu);
                        place.SetAddress(Language.RU, item.AddressRu);
                        place.SetDescription(Language.EN, item.DescriptionEn);
                        place.SetDescription(Language.UA, item.DescriptionUa);
                        place.SetDescription(Language.RU, item.DescriptionRu);
                        place.Name = item.Name;
                        place.Longitude = item.Longitude;
                        place.Latitude = item.Latitude;
                        place.Active = item.Active;
                    }

                    if (i % 20 == 0)
                    {
                        await db.SaveChangesAsync();
                    }
                }
                await db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

    }
}