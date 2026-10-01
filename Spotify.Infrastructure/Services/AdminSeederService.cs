using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Spotify.Application.Interfaces;
using Spotify.Domain.Entities.Location;
using Spotify.Domain.Entities.User;
using Spotify.Domain.Enumerations;
using Spotify.Infrastructure.Persistance.Context;
using System.Globalization;
using System.Text;

namespace Spotify.Infrastructure.Services;

public sealed class AdminSeederService : IAdminSeederService
{
    private const string AdminRoleName = "Admin";

    private static readonly Dictionary<string, string[]> CountrySeedData = new()
    {
        ["Albania"] = ["Tirana", "Durres", "Vlore", "Shkoder", "Elbasan"],
        ["Algeria"] = ["Algiers", "Oran", "Constantine", "Annaba", "Blida"],
        ["Argentina"] = ["Buenos Aires", "Cordoba", "Rosario", "Mendoza", "La Plata", "San Miguel de Tucuman"],
        ["Armenia"] = ["Yerevan", "Gyumri", "Vanadzor", "Vagharshapat", "Hrazdan"],
        ["Australia"] = ["Sydney", "Melbourne", "Brisbane", "Perth", "Adelaide", "Canberra"],
        ["Austria"] = ["Vienna", "Graz", "Linz", "Salzburg", "Innsbruck", "Klagenfurt"],
        ["Azerbaijan"] = ["Baku", "Ganja", "Sumqayit", "Mingachevir", "Lankaran"],
        ["Bangladesh"] = ["Dhaka", "Chittagong", "Khulna", "Rajshahi", "Sylhet"],
        ["Belarus"] = ["Minsk", "Gomel", "Mogilev", "Vitebsk", "Grodno", "Brest"],
        ["Belgium"] = ["Brussels", "Antwerp", "Ghent", "Bruges", "Liege", "Charleroi"],
        ["Bolivia"] = ["La Paz", "Santa Cruz de la Sierra", "Cochabamba", "Sucre", "Oruro"],
        ["Bosnia and Herzegovina"] = ["Sarajevo", "Banja Luka", "Tuzla", "Zenica", "Mostar"],
        ["Brazil"] = ["Sao Paulo", "Rio de Janeiro", "Brasilia", "Salvador", "Fortaleza", "Belo Horizonte"],
        ["Bulgaria"] = ["Sofia", "Plovdiv", "Varna", "Burgas", "Ruse", "Stara Zagora"],
        ["Canada"] = ["Toronto", "Montreal", "Vancouver", "Calgary", "Ottawa", "Edmonton"],
        ["Chile"] = ["Santiago", "Valparaiso", "Concepcion", "Antofagasta", "Temuco"],
        ["China"] = ["Beijing", "Shanghai", "Guangzhou", "Shenzhen", "Chengdu", "Wuhan"],
        ["Colombia"] = ["Bogota", "Medellin", "Cali", "Barranquilla", "Cartagena"],
        ["Costa Rica"] = ["San Jose", "Alajuela", "Cartago", "Heredia", "Liberia"],
        ["Croatia"] = ["Zagreb", "Split", "Rijeka", "Osijek", "Zadar", "Dubrovnik"],
        ["Cuba"] = ["Havana", "Santiago de Cuba", "Camaguey", "Holguin", "Varadero"],
        ["Cyprus"] = ["Nicosia", "Limassol", "Larnaca", "Paphos", "Famagusta"],
        ["Czech Republic"] = ["Prague", "Brno", "Ostrava", "Plzen", "Olomouc", "Liberec"],
        ["Denmark"] = ["Copenhagen", "Aarhus", "Odense", "Aalborg", "Esbjerg", "Roskilde"],
        ["Dominican Republic"] = ["Santo Domingo", "Santiago de los Caballeros", "Punta Cana", "La Romana", "Puerto Plata"],
        ["Ecuador"] = ["Quito", "Guayaquil", "Cuenca", "Ambato", "Manta"],
        ["Egypt"] = ["Cairo", "Alexandria", "Giza", "Luxor", "Aswan", "Sharm el-Sheikh"],
        ["Estonia"] = ["Tallinn", "Tartu", "Narva", "Parnu", "Kohtla-Jarve"],
        ["Ethiopia"] = ["Addis Ababa", "Dire Dawa", "Mekelle", "Gondar", "Bahir Dar"],
        ["Finland"] = ["Helsinki", "Espoo", "Tampere", "Vantaa", "Oulu", "Turku"],
        ["France"] = ["Paris", "Marseille", "Lyon", "Toulouse", "Nice", "Bordeaux"],
        ["Georgia"] = ["Tbilisi", "Batumi", "Kutaisi", "Rustavi", "Gori", "Zugdidi"],
        ["Germany"] = ["Berlin", "Hamburg", "Munich", "Cologne", "Frankfurt", "Stuttgart"],
        ["Ghana"] = ["Accra", "Kumasi", "Tamale", "Takoradi", "Cape Coast"],
        ["Greece"] = ["Athens", "Thessaloniki", "Patras", "Heraklion", "Larissa", "Volos"],
        ["Hungary"] = ["Budapest", "Debrecen", "Szeged", "Miskolc", "Pecs", "Gyor"],
        ["Iceland"] = ["Reykjavik", "Kopavogur", "Hafnarfjordur", "Akureyri", "Reykjanesbaer", "Gardabaer"],
        ["India"] = ["Mumbai", "Delhi", "Bangalore", "Hyderabad", "Chennai", "Kolkata"],
        ["Indonesia"] = ["Jakarta", "Surabaya", "Bandung", "Medan", "Semarang", "Denpasar"],
        ["Iran"] = ["Tehran", "Mashhad", "Isfahan", "Shiraz", "Tabriz"],
        ["Iraq"] = ["Baghdad", "Basra", "Mosul", "Erbil", "Najaf"],
        ["Ireland"] = ["Dublin", "Cork", "Limerick", "Galway", "Waterford", "Drogheda"],
        ["Israel"] = ["Jerusalem", "Tel Aviv", "Haifa", "Rishon LeZion", "Beersheba"],
        ["Italy"] = ["Rome", "Milan", "Naples", "Turin", "Florence", "Bologna"],
        ["Japan"] = ["Tokyo", "Osaka", "Yokohama", "Nagoya", "Sapporo", "Kyoto"],
        ["Jordan"] = ["Amman", "Zarqa", "Irbid", "Aqaba", "Madaba"],
        ["Kazakhstan"] = ["Almaty", "Astana", "Shymkent", "Karaganda", "Aktobe", "Atyrau"],
        ["Kenya"] = ["Nairobi", "Mombasa", "Kisumu", "Nakuru", "Eldoret"],
        ["Kuwait"] = ["Kuwait City", "Hawalli", "Salmiya", "Al Ahmadi", "Farwaniya"],
        ["Kyrgyzstan"] = ["Bishkek", "Osh", "Jalal-Abad", "Karakol", "Tokmok"],
        ["Latvia"] = ["Riga", "Daugavpils", "Liepaja", "Jelgava", "Jurmala"],
        ["Lebanon"] = ["Beirut", "Tripoli", "Sidon", "Tyre", "Zahle"],
        ["Lithuania"] = ["Vilnius", "Kaunas", "Klaipeda", "Siauliai", "Panevezys"],
        ["Luxembourg"] = ["Luxembourg City", "Esch-sur-Alzette", "Differdange", "Dudelange", "Ettelbruck"],
        ["Malaysia"] = ["Kuala Lumpur", "George Town", "Johor Bahru", "Ipoh", "Kota Kinabalu"],
        ["Malta"] = ["Valletta", "Sliema", "Birkirkara", "Mosta", "Qormi"],
        ["Mexico"] = ["Mexico City", "Guadalajara", "Monterrey", "Puebla", "Tijuana", "Cancun"],
        ["Moldova"] = ["Chisinau", "Balti", "Tiraspol", "Bender", "Cahul", "Orhei", "Soroca"],
        ["Mongolia"] = ["Ulaanbaatar", "Erdenet", "Darkhan", "Choibalsan", "Murun"],
        ["Morocco"] = ["Casablanca", "Rabat", "Marrakesh", "Fes", "Tangier"],
        ["Nepal"] = ["Kathmandu", "Pokhara", "Lalitpur", "Biratnagar", "Bharatpur"],
        ["Netherlands"] = ["Amsterdam", "Rotterdam", "The Hague", "Utrecht", "Eindhoven", "Groningen"],
        ["New Zealand"] = ["Auckland", "Wellington", "Christchurch", "Hamilton", "Dunedin"],
        ["Nigeria"] = ["Lagos", "Abuja", "Kano", "Ibadan", "Port Harcourt"],
        ["North Macedonia"] = ["Skopje", "Bitola", "Kumanovo", "Prilep", "Ohrid"],
        ["Norway"] = ["Oslo", "Bergen", "Trondheim", "Stavanger", "Drammen", "Tromso"],
        ["Pakistan"] = ["Karachi", "Lahore", "Islamabad", "Faisalabad", "Rawalpindi"],
        ["Panama"] = ["Panama City", "Colon", "David", "Santiago", "Chitre"],
        ["Peru"] = ["Lima", "Arequipa", "Cusco", "Trujillo", "Chiclayo"],
        ["Philippines"] = ["Manila", "Quezon City", "Cebu City", "Davao", "Baguio"],
        ["Poland"] = ["Warsaw", "Krakow", "Lodz", "Wroclaw", "Poznan", "Gdansk"],
        ["Portugal"] = ["Lisbon", "Porto", "Braga", "Coimbra", "Faro", "Funchal"],
        ["Qatar"] = ["Doha", "Al Rayyan", "Al Wakrah", "Umm Salal", "Al Khor"],
        ["Romania"] = ["Bucharest", "Cluj-Napoca", "Timisoara", "Iasi", "Constanta", "Brasov"],
        ["Russia"] = ["Moscow", "Saint Petersburg", "Novosibirsk", "Yekaterinburg", "Kazan", "Nizhny Novgorod"],
        ["Saudi Arabia"] = ["Riyadh", "Jeddah", "Mecca", "Medina", "Dammam"],
        ["Serbia"] = ["Belgrade", "Novi Sad", "Nis", "Kragujevac", "Subotica"],
        ["Singapore"] = ["Singapore", "Jurong", "Woodlands", "Tampines", "Bedok"],
        ["Slovakia"] = ["Bratislava", "Kosice", "Presov", "Zilina", "Nitra", "Banska Bystrica"],
        ["Slovenia"] = ["Ljubljana", "Maribor", "Celje", "Kranj", "Koper"],
        ["South Africa"] = ["Johannesburg", "Cape Town", "Durban", "Pretoria", "Gqeberha"],
        ["South Korea"] = ["Seoul", "Busan", "Incheon", "Daegu", "Daejeon", "Gwangju"],
        ["Spain"] = ["Madrid", "Barcelona", "Valencia", "Seville", "Zaragoza", "Malaga"],
        ["Sri Lanka"] = ["Colombo", "Kandy", "Galle", "Jaffna", "Negombo"],
        ["Sweden"] = ["Stockholm", "Gothenburg", "Malmo", "Uppsala", "Vasteras", "Orebro"],
        ["Switzerland"] = ["Zurich", "Geneva", "Basel", "Bern", "Lausanne", "Lucerne"],
        ["Taiwan"] = ["Taipei", "Kaohsiung", "Taichung", "Tainan", "Hsinchu"],
        ["Tanzania"] = ["Dar es Salaam", "Dodoma", "Mwanza", "Arusha", "Zanzibar City"],
        ["Thailand"] = ["Bangkok", "Chiang Mai", "Phuket", "Pattaya", "Khon Kaen"],
        ["Tunisia"] = ["Tunis", "Sfax", "Sousse", "Kairouan", "Bizerte"],
        ["Turkey"] = ["Istanbul", "Ankara", "Izmir", "Bursa", "Antalya", "Adana"],
        ["Uganda"] = ["Kampala", "Gulu", "Entebbe", "Jinja", "Mbarara"],
        ["Ukraine"] = ["Kyiv", "Kharkiv", "Odesa", "Dnipro", "Lviv", "Zaporizhzhia"],
        ["United Arab Emirates"] = ["Dubai", "Abu Dhabi", "Sharjah", "Ajman", "Al Ain"],
        ["United Kingdom"] = ["London", "Manchester", "Birmingham", "Glasgow", "Liverpool", "Edinburgh"],
        ["United States"] = ["New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "San Francisco"],
        ["Uruguay"] = ["Montevideo", "Salto", "Paysandu", "Punta del Este", "Rivera"],
        ["Uzbekistan"] = ["Tashkent", "Samarkand", "Namangan", "Andijan", "Bukhara", "Nukus"],
        ["Venezuela"] = ["Caracas", "Maracaibo", "Valencia", "Barquisimeto", "Maracay"],
        ["Vietnam"] = ["Hanoi", "Ho Chi Minh City", "Da Nang", "Hai Phong", "Hue"],
    };

    private readonly ApplicationContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<UserRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AdminSeederService(
        ApplicationContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<UserRole> roleManager,
        IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task SeedInitialAdminAsync(CancellationToken cancellationToken = default)
    {
        var email = _configuration["InitialAdmin:Email"];
        var userName = _configuration["InitialAdmin:UserName"];
        var password = _configuration["InitialAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        if (!await _roleManager.RoleExistsAsync(AdminRoleName))
        {
            await _roleManager.CreateAsync(new UserRole
            {
                Id = Guid.NewGuid(),
                Name = AdminRoleName,
                Description = "Administrator role",
                CanRead = true,
                CanCreate = true,
                CanUpdate = true,
                CanDelete = true
            });
        }

        var existingAdmins = await _userManager.GetUsersInRoleAsync(AdminRoleName);
        if (existingAdmins.Count > 0)
        {
            return;
        }

        var defaultSubscriptionId = await _context.Subscriptions
            .Where(s => s.Name == "Default" || s.Name == "Basic")
            .Select(s => s.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var settings = new Settings
        {
            Id = Guid.NewGuid(),
            Language = Language.English
        };

        _context.Settings.Add(settings);
        await _context.SaveChangesAsync(cancellationToken);

        var admin = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = userName,
            EmailConfirmed = true,
            SubscriptionId = defaultSubscriptionId,
            SettingsId = settings.Id
        };

        var result = await _userManager.CreateAsync(admin, password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(admin, AdminRoleName);
        }
    }

    public async Task SeedCountriesAsync(CancellationToken cancellationToken = default)
    {
        var existingCountries = await _context.Countries
            .Include(c => c.Cities)
            .ToListAsync(cancellationToken);

        var countriesByKey = new Dictionary<string, Country>();
        foreach (var existing in existingCountries.OrderByDescending(c => c.Cities.Count))
        {
            countriesByKey.TryAdd(NormalizeName(existing.Name), existing);
        }

        var changed = false;

        foreach (var (countryName, cityNames) in CountrySeedData)
        {
            var countryKey = NormalizeName(countryName);

            if (!countriesByKey.TryGetValue(countryKey, out var country))
            {
                country = new Country
                {
                    Id = Guid.NewGuid(),
                    Name = countryName
                };

                _context.Countries.Add(country);
                countriesByKey[countryKey] = country;
                changed = true;
            }

            var knownCities = country.Cities
                .Select(c => NormalizeName(c.Name))
                .ToHashSet();

            foreach (var cityName in cityNames)
            {
                if (!knownCities.Add(NormalizeName(cityName)))
                {
                    continue;
                }

                _context.Cities.Add(new City
                {
                    Id = Guid.NewGuid(),
                    Name = cityName,
                    CountryId = country.Id
                });
                changed = true;
            }
        }

        if (changed)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    private static string NormalizeName(string value)
    {
        var decomposed = value.Trim().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);

        foreach (var ch in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(ch);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC).ToUpperInvariant();
    }
}