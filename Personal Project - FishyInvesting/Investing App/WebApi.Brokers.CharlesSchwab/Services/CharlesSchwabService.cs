using System.Diagnostics;
using System.Globalization;
using System.Text;
using FishyLibrary.Helpers;
using FishyLibrary.Models;
using FishyLibrary.Models.Account;
using FishyLibrary.Models.MarketTime;
using FishyLibrary.Models.Client;
using FishyLibrary.Models.Order;
using MakeTrade.Models;
using Microsoft.IdentityModel.Tokens;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebApi.Template.Contexts;
using WebApi.Template.Models;

namespace WebApi.Template.Services;

public class CharlesSchwabService : ICharlesSchwabService
{
    private static readonly string AppCallbackUrl = "https://127.0.0.1";
    private readonly CharlesSchwabContext _charlesSchwabContext;
    private readonly ICharlesSchwabDataService _charlesSchwabDataService;
    private readonly ILogger<CharlesSchwabService> _logger;

    public CharlesSchwabService(CharlesSchwabContext charlesSchwabContext,
        ICharlesSchwabDataService charlesSchwabDataService,
        ILogger<CharlesSchwabService> logger)
    {
        _charlesSchwabContext = charlesSchwabContext;
        _charlesSchwabDataService = charlesSchwabDataService;
        _logger = logger;
    }

    public void EveryoneSignIn()
    {
        try
        {
            foreach (var clientId in _charlesSchwabContext.Clients.Select(x => x.Id).ToList())
            {
                AutomaticSignIn(clientId);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public string? AutomaticSignIn(int clientId)
    {
        AuthToken? authToken = _charlesSchwabContext.AuthTokens
            .FirstOrDefault(r => r.ClientId == clientId);
        if (authToken == null)
        {
            return null;
        }

        if (authToken.RefreshTokenExpiration < DateTime.Now.AddMinutes(1).ToUniversalTime())
        {
            return GetNewRefreshToken(clientId, authToken);
        }

        if (authToken.AccessTokenExpiration < DateTime.Now.AddMinutes(1).ToUniversalTime())
        {
            return RefreshAccessToken(authToken);
        }

        return authToken.AccessToken;
    }

    public string? ManualSignIn(int clientId)
    {
        AuthToken? authToken = _charlesSchwabContext.AuthTokens
            .FirstOrDefault(r => r.ClientId == clientId);

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

        return UpdateRefreshTokenFromURL(clientId, authToken, returnedUrl, appKey, appSecret);
    }

    private string? GetNewRefreshToken(int clientId, AuthToken authToken)
    {
        Client? users = _charlesSchwabContext.Clients.FirstOrDefault(n => n.Id == clientId);
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

        IWebDriver driver = null;
        string? returnedUrl;
        try
        {
            _logger.LogInformation("Opening Chrome Driver");
            driver = new ChromeDriver(options);
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
                _logger.LogInformation("Mobile approval is required");
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

            returnedUrl = driver.Url;
            _logger.LogInformation("Closed chrome window");
        }
        catch (Exception e)
        {
            if (driver != null)
            {
                driver.Quit();
            }

            _logger.LogError("Issue during driver usage: {DriverError}", e.Message);
            throw;
        }

        driver.Quit();

        if (returnedUrl == null)
        {
            return null;
        }

        _logger.LogDebug("{returnedUrl}", returnedUrl);

        return UpdateRefreshTokenFromURL(clientId, authToken, returnedUrl, appKey, appSecret);
    }

    private string? UpdateRefreshTokenFromURL(int clientId, AuthToken authToken, string returnedUrl, string appKey,
        string appSecret)
    {
        int codeIndex = returnedUrl.IndexOf("code=");
        if (codeIndex <= 0)
        {
            _logger.LogError("Return URL missing 'code='.");
            return null;
        }

        string responseCode = returnedUrl.Substring(codeIndex + 5,
            returnedUrl.IndexOf("%40") - codeIndex - 5) + "@";

        AuthResult? authResult =
            _charlesSchwabDataService.SendRefresh(GetRefreshTokenData(responseCode), appKey, appSecret);
        if (authResult == null)
        {
            return null;
        }

        List<AccountResponse> hashedAccountValues = _charlesSchwabDataService.GetAccountHash(authResult.access_token);
        foreach (var hashedAccountValue in hashedAccountValues)
        {
            if (_charlesSchwabContext.Accounts.Where(a => a.AccountId == hashedAccountValue.accountNumber)
                .IsNullOrEmpty())
            {
                var newAccount = new Account();
                newAccount.AccountId = hashedAccountValue.accountNumber;
                newAccount.HashAccountId = hashedAccountValue.hashValue;
                newAccount.ClientId = clientId;
                _charlesSchwabContext.Accounts.Add(newAccount);
            }
        }

        _charlesSchwabContext.AuthTokens.Add(new AuthToken(clientId, authResult.access_token,
            DateTime.Now.AddSeconds(authResult.expires_in).ToUniversalTime(), authResult.refresh_token,
            DateTime.Now.AddDays(6).AddHours(23).AddMinutes(54).ToUniversalTime(),
            responseCode, appKey, appSecret));
        _charlesSchwabContext.Remove(authToken);
        _charlesSchwabContext.SaveChanges();
        return authResult.access_token;
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

    private const string Salt = "1";

    public static string Encode(string plainText)
    {
        string saltedText = Salt + plainText;

        byte[] plainBytes = Encoding.UTF8.GetBytes(saltedText);
        return Convert.ToBase64String(plainBytes);
    }

    public static string Decode(string base64Text)
    {
        byte[] saltedBytes = Convert.FromBase64String(base64Text);
        string saltedText = Encoding.UTF8.GetString(saltedBytes);

        if (!saltedText.StartsWith(Salt))
        {
            throw new InvalidOperationException("Invalid salt in the input string.");
        }

        return saltedText.Substring(Salt.Length);
    }

    private string? RefreshAccessToken(AuthToken authToken)
    {
        var dict = new Dictionary<string, string>()
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", authToken.RefreshToken }
        };
        AuthResult? result = _charlesSchwabDataService.SendRefresh(dict, authToken.AppKey, authToken.AppSecret);
        if (result == null)
        {
            return null;
        }

        authToken.AccessToken = result.access_token;
        authToken.AccessTokenExpiration = DateTime.Now.AddSeconds(result.expires_in).ToUniversalTime();
        _charlesSchwabContext.SaveChanges();
        return authToken.AccessToken;
    }

    public GenericResponse PlaceTrade(FishyLibrary.Helpers.MakeTrade makeTrade, int accountId, bool dry)
    {
        _logger.LogInformation("Received trade: {trade} for account: {accountId}", makeTrade, accountId);

        Order order = new Order(makeTrade);

        var accountHashValue = GetAccountHashValue(accountId);
        var tokenForAccount = GetTokenForAccount(accountId);
        GenericResponse responseMessage =
            _charlesSchwabDataService.VerifySendMakeTrade(order, accountHashValue,
                tokenForAccount);
        if (!responseMessage.isSuccess)
        {
            _logger.LogError("{}", responseMessage);
            _logger.LogError("Failed to validate: {order}", order);
            return responseMessage;
        }

        _logger.LogInformation(
            "Verifying order successful for trade: {aide} {quantity} {product} at {price} to account: {accountId}",
            makeTrade.Side, makeTrade.Quantity, makeTrade.Product, makeTrade.Price, accountId);

        if (dry)
        {
            _logger.LogInformation("Dry run - trade not placed");
            responseMessage.Message = "0";
            return responseMessage;
        }

        _logger.LogInformation(
            "Sending Order {side} {quantity} {product} at {price} to account: {accountId}",
            makeTrade.Side, makeTrade.Quantity, makeTrade.Product, makeTrade.Price, accountId);

        responseMessage = _charlesSchwabDataService.SendMakeTrade(order, accountHashValue, tokenForAccount);
        if (responseMessage.isSuccess)
        {
            _logger.LogInformation(
                "Extracted number: {number} for {makeTrade.Side} {makeTrade.Quantity} {makeTrade.Product} at " +
                "{makeTrade.Price} to account: {accountId}", responseMessage.Message, makeTrade.Side,
                makeTrade.Quantity, makeTrade.Product, makeTrade.Price, accountId);
        }

        return responseMessage;
    }

    public double GetCurrentMarketPrice(int accountId, string product)
    {
        var tokenForAccount = GetTokenForAccount(accountId);
        PriceData prices = _charlesSchwabDataService.GetCurrentMarketPrice(product, tokenForAccount);
        return (prices.quote.askPrice + prices.quote.bidPrice) / 2;
    }

    private string GetTokenForAccount(int accountId)
    {
        var automaticSignIn = AutomaticSignIn(AccountToUser(accountId));
        if (automaticSignIn == null)
        {
            _logger.LogError("Unable to get account token for account:{accountId}", accountId);
            throw new Exception("Unable to get account token for account: " + accountId);
        }

        return automaticSignIn;
    }

    public AccountInfo GetAccountData(int accountId)
    {
        var accountHashValue = GetAccountHashValue(accountId);
        var tokenForAccount = GetTokenForAccount(accountId);
        var securitiesAccount = _charlesSchwabDataService.GetAccountData(accountHashValue, tokenForAccount)
            .securitiesAccount;
        Dictionary<string, Standing> positions = new Dictionary<string, Standing>();
        foreach (var position in securitiesAccount.positions)
        {
            Standing standing = new Standing(
                position.instrument.symbol,
                (int)position.longQuantity,
                (int)position.shortQuantity,
                position.averagePrice
            );
            positions.Add(position.instrument.symbol, standing);
        }

        var balance = new Balance(securitiesAccount.currentBalances.cashBalance);
        AccountInfo accountInfo = new AccountInfo(balance, positions);
        accountInfo.AccountNumber = securitiesAccount.accountNumber;

        return accountInfo;
    }

    public MarketTime GetMarketTime(int accountId)
    {
        try
        {
            int dayOfMonth = DateTime.Now.Day;
            MarketTime? today = _charlesSchwabContext.MarketTimes.FirstOrDefault(m => m.Date.Day == dayOfMonth);
            if (today == null)
            {
                var tokenForAccount = GetTokenForAccount(accountId);
                var equityResponse = _charlesSchwabDataService.IsMarketOpen(tokenForAccount);

                if (equityResponse.Equity.EQ != null)
                {
                    today = new MarketTime
                    {
                        Date = DateTimeOffset.Now.Date.ToUniversalTime().Date,
                        IsOpen = true,
                        Open = equityResponse.Equity.EQ.SessionHours.RegularMarket[0].Start.TimeOfDay,
                        Close = equityResponse.Equity.EQ.SessionHours.RegularMarket[0].End.TimeOfDay
                    };
                }
                else
                {
                    today = new MarketTime { 
                        Date = DateTimeOffset.Now.Date.ToUniversalTime().Date, 
                        IsOpen = false, 
                        Open = DateTime.Today.ToUniversalTime().TimeOfDay, 
                        Close = DateTime.Today.ToUniversalTime().TimeOfDay
                    };
                }

                // remove previous days
                var previousDays = _charlesSchwabContext.MarketTimes.ToList();
                _charlesSchwabContext.MarketTimes.RemoveRange(previousDays);
                // add a new day
                _charlesSchwabContext.MarketTimes.Add(today);
                _charlesSchwabContext.SaveChangesAsync();
            }

            return today;
        }
        catch (Exception e)
        {
            _logger.LogError("{}", e);
            return new MarketTime { Date = DateTimeOffset.Now.Date, IsOpen = false };
        }
    }

    public List<OrderData> GetOrders(int accountId, string startTime)
    {
        var accountHashValue = GetAccountHashValue(accountId);
        var tokenForAccount = GetTokenForAccount(accountId);
        var startDate = DateTime.ParseExact(startTime, @"MM/dd/yyyy", CultureInfo.InvariantCulture);
        return _charlesSchwabDataService.GetOrders(accountHashValue, tokenForAccount, startDate);
    }

    public OrderData GetOrder(int accountId, string orderNumber)
    {
        var accountHashValue = GetAccountHashValue(accountId);
        var tokenForAccount = GetTokenForAccount(accountId);
        return _charlesSchwabDataService.GetOrder(accountHashValue, tokenForAccount, orderNumber);
    }

    public string GetAccountHashValue(int accountId)
    {
        Account? account = _charlesSchwabContext.Accounts
            .FirstOrDefault(r => r.Id == accountId);
        if (account == null)
        {
            _logger.LogError("Account with id {accountId} not found", accountId);
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
            _logger.LogError("Account with id {accountId} not found", accountId);
            throw new ArgumentException($"Account with id {accountId} not found");
        }

        return account.ClientId;
    }
}