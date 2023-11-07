using FluentAssertions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace UITests;

[Parallelizable(ParallelScope.Self)]
// Beginning with NUnit 2.5, the TestFixture attribute is optional for non-parameterized, non-generic fixtures.
// So long as the class contains at least one method marked with the Test, TestCase or TestCaseSource attribute, it will be treated as a test fixture.
[TestFixture]
[Skip(SkipAttribute.Targets.Firefox | SkipAttribute.Targets.Webkit)]
public class ContosoMapTests : PageTest
{
    private const float Latitude = 41.890221f;
    private const float Longitude = 12.492348f;

    [Test]
    public async Task the_map_should_display_bing_maps_iframe()
    {
        await Expect(Page.Locator("input#latitude")).ToHaveValueAsync(Latitude.ToString());
        await Expect(Page.Locator("input#longitude")).ToHaveValueAsync(Longitude.ToString());
        await Expect(Page.Locator("#current-location")).ToBeVisibleAsync();

        var boundingBox = await Page.FrameLocator("iframe[title=\"geolocation\"]").Locator("canvas[aria-label=\"Interactive Map\"]").BoundingBoxAsync();
        (boundingBox?.Width).Should().BeGreaterThan(100);
        (boundingBox?.Height).Should().BeGreaterThan(100);
    }

    [Test]
    public async Task the_map_should_be_able_to_be_zoomed_in_in_the_iframe()
    {
        await Page.FrameLocator("iframe[title=\"geolocation\"]").GetByRole(AriaRole.Button, new FrameLocatorGetByRoleOptions { Name = "Zoom avanti" }).ClickAsync();
    }

    [Test]
    public async Task the_map_should_be_able_to_be_zoomed_out_in_the_iframe()
    {
        await Page.FrameLocator("iframe[title=\"geolocation\"]").GetByRole(AriaRole.Button, new FrameLocatorGetByRoleOptions { Name = "Zoom indietro" }).ClickAsync();
    }

    [SetUp]
    public async Task Setup()
    {
        await Page.GotoAsync("/");
        await Page.Mouse.WheelAsync(0, 4000);
    }

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            Locale = "it-IT",
            TimezoneId = "Europe/Rome",
            Geolocation = new Geolocation
            {
                Latitude = Latitude,
                Longitude = Longitude
            },
            Permissions = new List<string> { "geolocation" },
            BaseURL = "https://cloudtesting.contosotraders.com"
        };
    }
}