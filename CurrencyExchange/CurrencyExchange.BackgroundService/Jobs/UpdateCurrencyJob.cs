using CurrencyExchange.Core.Entities;
using CurrencyExchange.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using Quartz;

namespace CurrencyExchange.BackgroundService.Jobs
{
    public class UpdateCurrencyJob : IJob
    {
        private readonly AppDbContext _dbContext;

        public UpdateCurrencyJob(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var httpClient = new HttpClient();
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://www.cbr.ru/scripts/XML_daily.asp");
                var response = await httpClient.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();

                var xml = await response.Content.ReadAsStringAsync();
                var rates = ParseCbrXml(xml);


                await UpdateDatabaseWithRatesAsync(rates);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Currency update failed: {ex.Message}");
            }
        }

        private List<(string code, decimal rate)> ParseCbrXml(string xml)
        {
            var rates = new List<(string, decimal)>();
            var doc = XDocument.Parse(xml);


            foreach (var valuteElement in doc.Descendants("Valute"))
            {
                var charCode = valuteElement.Element("CharCode")?.Value;
                var valueStr = valuteElement.Element("Value")?.Value;
                var nominalStr = valuteElement.Element("Nominal")?.Value;

                if (string.IsNullOrEmpty(charCode) ||
                    string.IsNullOrEmpty(valueStr) ||
                    string.IsNullOrEmpty(nominalStr))
                    continue;

                valueStr = valueStr.Replace(',', '.');
                nominalStr = nominalStr.Replace(',', '.');

                if (decimal.TryParse(valueStr, out var value) &&
                    decimal.TryParse(nominalStr, out var nominal))
                {
                    var rate = value / nominal;
                    rates.Add((charCode, rate));
                }
            }

            return rates;
        }

        private async Task UpdateDatabaseWithRatesAsync(List<(string code, decimal rate)> rates)
        {
            foreach (var (code, rate) in rates)
            {
                var currency = await _dbContext.Currencies
                    .FirstOrDefaultAsync(c => c.Name == code);

                if (currency != null)
                {
                    currency.Rate = rate;
                    currency.UpdatedAt = DateTime.Now;
                }
                else
                    await _dbContext.Currencies.AddAsync(new Currency
                    {
                        Id = Guid.NewGuid(),
                        Name = code,
                        Rate = rate,
                        UpdatedAt = DateTime.Now
                    });
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}