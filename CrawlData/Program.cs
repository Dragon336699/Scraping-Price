using Newtonsoft.Json.Linq;
using PuppeteerSharp;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml.Linq;

var domains = new[]
{
    new { Domain = "laptop88.vn",Price = "price", oldPrice = "old-price" },
    new { Domain = "phongvu.vn" ,Price = "att-product-detail-latest-price", oldPrice = "att-product-detail-retail-price"},
    new { Domain = "cellphones.com.vn" ,Price = "product__price--show", oldPrice = "product__price--through"},
    new { Domain = "fptshop.com.vn" ,Price = "st-price-main", oldPrice = "st-price-sub"},
    new { Domain = "hacom.vn" ,Price = "giakm", oldPrice = "giany"},
};
Console.WriteLine("Nhap so link: ");
string t = Console.ReadLine();
int.TryParse(t, out int link);
Console.WriteLine("URL:");
string[] Urls = new string[link];
for (int i = 0; i < link; i++)
{
    Urls[i] = Console.ReadLine();
}

using var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();
var browser = await Puppeteer.LaunchAsync(new LaunchOptions
{
    Headless = true,
});

await Parallel.ForEachAsync(Urls, async (url,token) =>
{
    Uri uri = new Uri(url);
    var myDomain = uri.Host;
    var objectDomain = domains.FirstOrDefault(item => item.Domain == myDomain);
    var price = objectDomain.Price;
    var oldPrice = objectDomain.oldPrice;
    var page = await browser.NewPageAsync();
    await page.GoToAsync(url);
    var priceNode = await page.WaitForSelectorAsync($".{price}");
    var oldPriceNode = await page.WaitForSelectorAsync($".{oldPrice}");
    if (priceNode != null && oldPriceNode != null)
    {
        var priceText = await page.EvaluateFunctionAsync<string>("element => element.textContent", priceNode);
        var oldPriceText = await page.EvaluateFunctionAsync<string>("element => element.textContent", oldPriceNode);
        var priceNumber = Regex.Replace(priceText.Trim(), "[^0-9]", "");
        var oldPriceNumber = Regex.Replace(oldPriceText.Trim(), "[^0-9]", "");
        Console.WriteLine("Current Price: " + priceNumber + "VND");
        Console.WriteLine("Old Price: " + oldPriceNumber + "VND");
        Console.WriteLine();
    } else 
    {
        Console.WriteLine("Not exist");
    }
    await page.CloseAsync();
});
browserFetcher.Dispose();

