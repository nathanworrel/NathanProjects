using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using FishyLibrary.Models;
using FishyLibrary.Models.Account;
using FishyLibrary.Models.MarketTime;
using Microsoft.IdentityModel.Tokens;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebApi.Template.Contexts;
using WebApi.Template.Models;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WebApi.Template.Services;

public class CharlesSchwabService : ICharlesSchwabService
{
    private static readonly string RefreshAccessTokenUrl = "https://api.schwabapi.com/v1/oauth/token";
    private static readonly string AppCallbackUrl = "https://127.0.0.1";
    private static readonly string TraderBaseUrl = "https://api.schwabapi.com/trader/v1";
    private static readonly string MarketDataBaseUrl = "https://api.schwabapi.com/marketdata/v1";
    private readonly CharlesSchwabContext _charlesSchwabContext;
    private readonly HttpClient _httpClient;
    private readonly ILogger<CharlesSchwabService> _logger;

    public CharlesSchwabService(CharlesSchwabContext charlesSchwabContext, IHttpClientFactory httpClientFactory,
        ILogger<CharlesSchwabService> logger)
    {
        _charlesSchwabContext = charlesSchwabContext;
        _httpClient = httpClientFactory.CreateClient();
        _logger = logger;
    }

    public void EveryoneSignIn()
    {
        try
        {
            foreach (var userId in _charlesSchwabContext.UsersDatabase.Select(x => x.Id).ToList())
            {
                AutomaticSignIn(userId);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public string? AutomaticSignIn(int userId)
    {
        AuthToken? authToken = _charlesSchwabContext.AuthTokens
            .FirstOrDefault(r => r.UserId == userId);
        if (authToken == null)
        {
            return null;
        }

        if (authToken.RefreshTokenExpiration < DateTime.Now.AddMinutes(1).ToUniversalTime())
        {
            return GetTokens(userId, authToken);
        }

        if (authToken.AccessTokenExpiration < DateTime.Now.AddMinutes(1).ToUniversalTime())
        {
            return RefreshAccessToken(authToken);
        }

        return authToken.AccessToken;
    }

    public string? ManualSignIn(int userId)
    {
        AuthToken? authToken = _charlesSchwabContext.AuthTokens
            .FirstOrDefault(r => r.UserId == userId);

        string? appKey;
        string? appSecret;

        if (authToken != null)
        {
            appKey = authToken.AppKey;
            appSecret = authToken.AppSecret;
        }
        else
        {
            Console.WriteLine("Enter App Key:");
            appKey = Console.ReadLine();
            Console.WriteLine("Enter App Secret:");
            appSecret = Console.ReadLine();
        }

        var authUrl = $"https://api.schwabapi.com/v1/oauth/authorize?client_id={appKey}&redirect_uri={AppCallbackUrl}";

        Process.Start(new ProcessStartInfo
        {
            FileName =
                authUrl,
            UseShellExecute = true
        });

        Console.WriteLine("Enter Redirect URL:");
        string? returnedUrl = Console.ReadLine();

        string responseCode = returnedUrl.Substring(returnedUrl.IndexOf("code=") + 5,
            returnedUrl.IndexOf("%40") - returnedUrl.IndexOf("code=") - 5) + "@";

        AuthResult? authResult = SendRefresh(GetRefreshTokenData(responseCode), appKey, appSecret).Result;
        if (authResult == null)
        {
            return null;
        }

        HttpResponseMessage accountHash = GetAccountHash(authResult.access_token).Result;
        if (!accountHash.IsSuccessStatusCode)
        {
            throw new Exception(accountHash.Content.ReadAsStringAsync().Result);
        }

        if (authToken != null)
        {
            _charlesSchwabContext.AuthTokens.Remove(authToken);
        }

        _charlesSchwabContext.AuthTokens.Add(new AuthToken(userId, authResult.access_token,
            DateTime.Now.AddSeconds(authResult.expires_in).ToUniversalTime(), authResult.refresh_token,
            DateTime.Now.AddDays(6).ToUniversalTime(),
            responseCode, appKey, appSecret));

        var accountHashData = accountHash.Content.ReadAsStringAsync().Result;
        var hashedAccountValues = JsonConvert.DeserializeObject<List<AccountResponse>>(accountHashData);
        if (hashedAccountValues == null)
        {
            _logger.LogInformation($"No Accounts found for user {userId}");
            return null;
        }

        foreach (var hashedAccountValue in hashedAccountValues)
        {
            if (_charlesSchwabContext.Accounts.Where(a => a.AccountId == hashedAccountValue.AccountNumber)
                .IsNullOrEmpty())
            {
                var newAccount = new Account();
                newAccount.AccountId = hashedAccountValue.AccountNumber;
                newAccount.HashAccountId = hashedAccountValue.HashValue;
                newAccount.UserId = userId;
                _charlesSchwabContext.Accounts.Add(newAccount);
            }
        }

        _charlesSchwabContext.SaveChanges();
        return authResult.access_token;
    }

    private string? GetTokens(int userId, AuthToken authToken)
    {
        Users? users = _charlesSchwabContext.UsersDatabase.FirstOrDefault(n => n.Id == userId);
        if (users is not { IsAutomatic: true })
        {
            return null;
        }

        var appKey = authToken.AppKey;
        var appSecret = authToken.AppSecret;

        var userName = Decode(users.Username);
        var password = Decode(users.Password);

        var authUrl = $"https://api.schwabapi.com/v1/oauth/authorize?client_id={appKey}&redirect_uri={AppCallbackUrl}";

        var options = new ChromeOptions();
        options.AddArgument("--disable-gpu"); // Disable GPU acceleration (may cause issues in headless)
        options.AddArgument("--headless");
        // options.AddArgument("--user-data-dir=/temp");
        options.AddArgument("--no-sandbox"); // Required for some environments
        options.AddArgument("window-size=1920,1080");
        options.AddArgument("--disable-dev-shm-usage"); // Overcome limited resource problems
        options.AddArgument("--disable-blink-features=AutomationControlled");
        options.AddExcludedArgument("enable-automation");
        options.AddArgument(
            "user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36");
        options.AddUserProfilePreference("profile.default_content_setting_values.notifications", 1);
        options.AddUserProfilePreference("credentials_enable_service", false);

        IWebDriver driver = new ChromeDriver(options);

        driver.Navigate().GoToUrl(authUrl);

        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

        IWebElement idInput = driver.FindElement(By.CssSelector("input#loginIdInput"));
        foreach (char c in userName)
        {
            idInput.SendKeys(c.ToString()); // Send one character at a time
            Thread.Sleep(10); // Delay in milliseconds (adjust as needed)
        }

        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

        IWebElement passwordInput = driver.FindElement(By.Id("passwordInput"));
        foreach (char c in password)
        {
            passwordInput.SendKeys(c.ToString()); // Send one character at a time
            Thread.Sleep(10); // Delay in milliseconds (adjust as needed)
        }

        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

        IWebElement loginButton = driver.FindElement(By.Id("btnLogin"));
        loginButton.Click();

        Thread.Sleep(5000);

        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

        var mobileApprove = driver.FindElements(By.Id("mobile_approve"));
        if (mobileApprove.Count > 0)
        {
            mobileApprove[0].Click();
            Thread.Sleep(60000);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(60);

            IWebElement rememberDevice = driver.FindElement(By.Id("remember-device-yes-content"));
            rememberDevice.Click();
            Thread.Sleep(1000);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            IWebElement buttonContinue = driver.FindElement(By.Id("btnContinue"));
            buttonContinue.Click();
            Thread.Sleep(1000);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        IWebElement acceptTermsButton = driver.FindElement(By.Id("acceptTerms"));
        acceptTermsButton.Click();
        Thread.Sleep(1000);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

        IWebElement submitButton = driver.FindElement(By.Id("submit-btn"));
        submitButton.Click();
        Thread.Sleep(1000);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

        IWebElement acceptButton = driver.FindElement(By.Id("agree-modal-btn-"));
        acceptButton.Click();
        Thread.Sleep(1000);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

        IWebElement finalSubmitButton = driver.FindElement(By.Id("submit-btn"));
        finalSubmitButton.Click();
        Thread.Sleep(1000);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

        IWebElement cancelButton = driver.FindElement(By.Id("cancel-btn"));
        cancelButton.Click();
        Thread.Sleep(3000);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

        string? returnedUrl = driver.Url;

        driver.Quit();

        if (returnedUrl == null)
        {
            return null;
        }

        int codeIndex = returnedUrl.IndexOf("code=");
        if (codeIndex <= 0)
        {
            _logger.LogError("Return URL missing 'code='.");
            return null;
        }
        string responseCode = returnedUrl.Substring(codeIndex + 5,
            returnedUrl.IndexOf("%40") - codeIndex - 5) + "@";

        AuthResult? authResult = SendRefresh(GetRefreshTokenData(responseCode), appKey, appSecret).Result;
        if (authResult == null)
        {
            return null;
        }

        HttpResponseMessage accountHash = GetAccountHash(authResult.access_token).Result;
        if (!accountHash.IsSuccessStatusCode)
        {
            throw new Exception(accountHash.Content.ReadAsStringAsync().Result);
        }

        var accountHashData = accountHash.Content.ReadAsStringAsync().Result;
        var hashedAccountValues = JsonConvert.DeserializeObject<List<AccountResponse>>(accountHashData);
        if (hashedAccountValues == null)
        {
            _logger.LogInformation($"No Accounts found for user {userId}");
            return null;
        }

        foreach (var hashedAccountValue in hashedAccountValues)
        {
            if (_charlesSchwabContext.Accounts.Where(a => a.AccountId == hashedAccountValue.AccountNumber)
                .IsNullOrEmpty())
            {
                var newAccount = new Account();
                newAccount.AccountId = hashedAccountValue.AccountNumber;
                newAccount.HashAccountId = hashedAccountValue.HashValue;
                newAccount.UserId = userId;
                _charlesSchwabContext.Accounts.Add(newAccount);
            }
        }

        _charlesSchwabContext.AuthTokens.Add(new AuthToken(userId, authResult.access_token,
            DateTime.Now.AddSeconds(authResult.expires_in).ToUniversalTime(), authResult.refresh_token,
            DateTime.Now.AddDays(6).ToUniversalTime(),
            responseCode, appKey, appSecret));
        _charlesSchwabContext.Remove(authToken);
        _charlesSchwabContext.SaveChanges();
        return authResult.access_token;
    }
    
    public static string Decode(string base64Text)
    {
        return null;
    }

    private string? RefreshAccessToken(AuthToken authToken)
    {
        var dict = new Dictionary<string, string>()
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", authToken.RefreshToken }
        };
        AuthResult? result = SendRefresh(dict, authToken.AppKey, authToken.AppSecret).Result;
        if (result == null)
        {
            return null;
        }

        authToken.AccessToken = result.access_token;
        authToken.AccessTokenExpiration = DateTime.Now.AddSeconds(result.expires_in).ToUniversalTime();
        _charlesSchwabContext.SaveChanges();
        return authToken.AccessToken;
    }

    private async Task<AuthResult?> SendRefresh(Dictionary<string, string> dict, string appKey, string appSecret)
    {
        var req = new HttpRequestMessage(HttpMethod.Post, RefreshAccessTokenUrl);
        string credentials = $"{appKey}:{appSecret}";
        string base64Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
        req.Headers.Add("Authorization", $"Basic {base64Credentials}");
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        req.Content = new FormUrlEncodedContent(dict);
        HttpResponseMessage response = await _httpClient.SendAsync(req);
        if (response.IsSuccessStatusCode)
        {
            string jsonResponse = await response.Content.ReadAsStringAsync();
            AuthResult? authResult = JsonConvert.DeserializeObject<AuthResult>(jsonResponse);
            return authResult;
        }

        return null;
    }

    public async Task<HttpResponseMessage> SendMakeTrade(Order order, int accountId)
    {
        var req = new HttpRequestMessage(HttpMethod.Post,
            $"{TraderBaseUrl}/accounts/{GetAccountHashValue(accountId)}/orders");
        var jsonOrder = JsonConvert.SerializeObject(order);
        req.Content = new StringContent(jsonOrder, Encoding.UTF8, "application/json");
        req.Headers.Add("Authorization", $"Bearer {GetTokenForAccount(accountId)}");
        return await _httpClient.SendAsync(req);
    }

    public async Task<HttpResponseMessage> VerifySendMakeTrade(Order order, int accountId)
    {
        var req = new HttpRequestMessage(HttpMethod.Post,
            $"{TraderBaseUrl}/accounts/{GetAccountHashValue(accountId)}/previewOrder");
        var jsonOrder = JsonConvert.SerializeObject(order);
        req.Content = new StringContent(jsonOrder, Encoding.UTF8, "application/json");
        req.Headers.Add("Authorization", $"Bearer {GetTokenForAccount(accountId)}");
        return await _httpClient.SendAsync(req);
    }

    public async Task<HttpResponseMessage> GetCurrentMarketPrice(int accountId, string product)
    {
        var req = new HttpRequestMessage(HttpMethod.Get,
            $"{MarketDataBaseUrl}/{product}/quotes");
        req.Headers.Add("Authorization", $"Bearer {GetTokenForAccount(accountId)}");
        return await _httpClient.SendAsync(req);
    }

    private string? GetTokenForAccount(int accountId)
    {
        return AutomaticSignIn(AccountToUser(accountId));
    }

    public async Task<HttpResponseMessage> GetAccountData(int accountId)
    {
        var req = new HttpRequestMessage(HttpMethod.Get,
            $"{TraderBaseUrl}/accounts/{GetAccountHashValue(accountId)}?fields=positions");
        req.Headers.Add("Authorization", $"Bearer {GetTokenForAccount(accountId)}");
        return await _httpClient.SendAsync(req);
    }

    public async Task<HttpResponseMessage> GetAccountHash(string accessToken)
    {
        var req = new HttpRequestMessage(HttpMethod.Get,
            $"{TraderBaseUrl}/accounts/accountNumbers");
        req.Headers.Add("Authorization", $"Bearer {accessToken}");
        return await _httpClient.SendAsync(req);
    }

    private async Task<HttpResponseMessage> IsMarketOpen(int accountId)
    {
        var req = new HttpRequestMessage(HttpMethod.Get,
            $"{MarketDataBaseUrl}/markets/equity?date={DateTime.Now.Date:yyyy-MM-dd}");
        req.Headers.Add("Authorization", $"Bearer {GetTokenForAccount(accountId)}");
        return await _httpClient.SendAsync(req);
    }

    public async Task<MarketTime> GetMarketTime(int accountId)
    {
        try
        {
            int dayOfMonth = DateTime.Now.Day;
            MarketTime? today = _charlesSchwabContext.MarketTimes.FirstOrDefault(m => m.Date.Day == dayOfMonth);
            if (today == null)
            {
                var result = IsMarketOpen(accountId).Result.Content.ReadAsStringAsync().Result;
                var equityResponse = JsonSerializer.Deserialize<EquityResponse>(result);
                if (equityResponse == null || equityResponse.Equity == null || equityResponse.Equity.EQ == null)
                {
                    today = new MarketTime { Date = DateTimeOffset.Now.Date.ToUniversalTime().Date, IsOpen = false };
                }
                else
                {
                    today = new MarketTime
                    {
                        Date = DateTimeOffset.Now.Date.ToUniversalTime().Date,
                        IsOpen = true,
                        Open = equityResponse.Equity.EQ.SessionHours.RegularMarket[0].Start.TimeOfDay,
                        Close = equityResponse.Equity.EQ.SessionHours.RegularMarket[0].End.TimeOfDay
                    };
                }
                // remove previous days
                var previousDays = _charlesSchwabContext.MarketTimes.ToList();
                _charlesSchwabContext.MarketTimes.RemoveRange(previousDays);
                // add a new day
                _charlesSchwabContext.MarketTimes.Add(today);
                await _charlesSchwabContext.SaveChangesAsync();
            }

            return today;
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            return new MarketTime { Date = DateTimeOffset.Now.Date, IsOpen = false };
        }
    }

    public async Task<HttpResponseMessage> GetOrders(int accountId, DateTime startDate)
    {
        var req = new HttpRequestMessage(HttpMethod.Get,
            $"{TraderBaseUrl}/accounts/{GetAccountHashValue(accountId)}/orders?fromEnteredTime={startDate.ToString("yyyy-MM-ddTHH:mm:ss.fff'Z'").Replace(":", "%3A")}&toEnteredTime={DateTime.Now.Date.AddDays(1).Date.ToString("yyyy-MM-ddTHH:mm:ss.fff'Z'").Replace(":", "%3A")}");
        req.Headers.Add("Authorization", $"Bearer {GetTokenForAccount(accountId)}");
        return await _httpClient.SendAsync(req);
    }

    public async Task<HttpResponseMessage> GetOrder(int accountId, string orderNumber)
    {
        var req = new HttpRequestMessage(HttpMethod.Get,
            $"{TraderBaseUrl}/accounts/{GetAccountHashValue(accountId)}/orders/{orderNumber}");
        req.Headers.Add("Authorization", $"Bearer {GetTokenForAccount(accountId)}");
        return await _httpClient.SendAsync(req);
    }

    public string GetAccountHashValue(int accountId)
    {
        Account? account = _charlesSchwabContext.Accounts
            .FirstOrDefault(r => r.Id == accountId);
        if (account == null)
        {
            _logger.LogError($"Account with id {accountId} not found");
            throw new ArgumentException($"Account with id {accountId} not found");
        }

        return account.HashAccountId;
    }

    public int AccountToUser(int accountId)
    {
        Account? account = _charlesSchwabContext.Accounts
            .FirstOrDefault(r => r.Id == accountId);
        if (account == null)
        {
            _logger.LogError($"Account with id {accountId} not found");
            throw new ArgumentException($"Account with id {accountId} not found");
        }

        return account.UserId;
    }

    private Dictionary<string, string> GetRefreshTokenData(string code)
    {
        return new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "redirect_uri", AppCallbackUrl },
            { "code", code }
        };
    }
}