using PuppeteerSharp;

namespace VotingDominator.Services
{
    public class VotingDominatorService
    {
        public async Task DominateVotes()
        {
            string chromePath = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";

            using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                ExecutablePath = chromePath,
                Headless = false, // Set to false if you want to watch it work
                Args = new[] { "--no-sandbox", "--disable-popup-blocking" }
            });

            int counter = 999999;
            int antiBotTracker = 0;

            string voteValue = "71388936";
            string voteButtonId = "#pd-vote-button16241029";
            string returnPollSelector = "a.pds-return-poll";
            string radioSelector = $"input[value='{voteValue}']";

            using (var page = await browser.NewPageAsync())
            {
                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = 1920,
                    Height = 1080
                });

                await page.GoToAsync("https://prepdig.com/2025/11/2026-defensive-poy/");
                await page.GoToAsync("https://prepdig.com/2025/11/2026-defensive-poy/");

                for (int i = 0; i < counter; i++)
                {
                    if (antiBotTracker >= 25)
                    {
                        Thread.Sleep(90000);
                        antiBotTracker = 0;
                    }

                    // Click the radio input
                    await page.WaitForSelectorAsync(radioSelector);
                    await page.ClickAsync(radioSelector);

                    Thread.Sleep(500);

                    // Click Vote button
                    await page.WaitForSelectorAsync(voteButtonId);
                    await page.ClickAsync(voteButtonId);

                    Thread.Sleep(500);

                    await page.WaitForSelectorAsync(returnPollSelector);
                    await page.ClickAsync(returnPollSelector);

                    Thread.Sleep(500);

                    antiBotTracker++;
                }
            }
        }

        async Task AwaitSelector(IPage page, string selector)
        {
            await Task.WhenAny(
                page.WaitForSelectorAsync(selector, new WaitForSelectorOptions { Timeout = 4000 }),
                Task.Run(async () =>
                {
                    while (true)
                    {
                        var popup = await page.QuerySelectorAsync("button.bz-close-btn");
                        if (popup != null)
                        {
                            await popup.ClickAsync();
                            return;
                        }
                        await Task.Delay(500);
                    }
                })
            );
        }

        async Task ClosePopupIfExists(IPage page)
        {
            var popupButton = await page.QuerySelectorAsync("button.bz-close-btn");
            if (popupButton != null)
            {
                await popupButton.ClickAsync();
            }
        }

        async Task<IElementHandle> WaitForSelectorWithPopupHandlingAsync(IPage page, string selector, string popupSelector, int timeoutMs)
        {
            var start = DateTime.Now;
            while ((DateTime.Now - start).TotalMilliseconds < timeoutMs)
            {
                try
                {
                    // 1. Check if our main selector exists
                    var element = await page.QuerySelectorAsync(selector);
                    if (element != null)
                        return element;

                    // 2. If popup appears, close it
                    var popup = await page.QuerySelectorAsync(popupSelector);
                    if (popup != null)
                    {
                        await popup.ClickAsync();
                        Console.WriteLine("Popup closed ✅");
                    }
                }
                catch
                {
                    // ignore transient navigation / detach errors
                }

                await Task.Delay(500);
            }

            throw new TimeoutException($"Timeout waiting for selector: {selector}");
        }
    }
}

//try
//{
//    // Click the radio input
//    await page.WaitForSelectorAsync(radioSelector, new WaitForSelectorOptions
//    {
//        Timeout = 6000
//    });
//}
//catch (Exception)
//{
//    await ClosePopupIfExists(page);
//    await page.WaitForSelectorAsync(radioSelector, new WaitForSelectorOptions
//    {
//        Timeout = 6000
//    });
//}

//await page.ClickAsync(radioSelector);

//try
//{
//    // Click Vote button
//    await page.WaitForSelectorAsync(voteButtonId, new WaitForSelectorOptions
//    {
//        Timeout = 6000
//    });
//}
//catch (Exception)
//{
//    await ClosePopupIfExists(page);
//    await page.WaitForSelectorAsync(voteButtonId, new WaitForSelectorOptions
//    {
//        Timeout = 6000
//    });
//}

//await page.ClickAsync(voteButtonId);

//try
//{
//    // Click Vote button
//    await page.WaitForSelectorAsync(returnPollSelector, new WaitForSelectorOptions
//    {
//        Timeout = 6000
//    });
//}
//catch (Exception)
//{
//    await ClosePopupIfExists(page);
//    await page.WaitForSelectorAsync(returnPollSelector, new WaitForSelectorOptions
//    {
//        Timeout = 6000
//    });
//}

//await page.ClickAsync(returnPollSelector);