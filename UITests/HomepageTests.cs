using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace UITests
{
    [Parallelizable(ParallelScope.Self)]
    // Beginning with NUnit 2.5, the TestFixture attribute is optional for non-parameterized, non-generic fixtures.
    // So long as the class contains at least one method marked with the Test, TestCase or TestCaseSource attribute, it will be treated as a test fixture.
    [TestFixture]
    public class HomepageTests : PageTest
    {
        [Test]
        public async Task homepage_should_contain_the_welcome_text()
        {
            var welcomeText = Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Welcome" });

            await Expect(welcomeText).ToContainTextAsync("Welcome");
        }

        [Test]
        public async Task homepage_should_contain_the_welcome_text_with_tracing()
        {
            // Start tracing before creating / navigating a page.
            await Context.Tracing.StartAsync(new()
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });

            var welcomeText = Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Welcome" });

            await Expect(welcomeText).ToContainTextAsync("Welcome");

            // Stop tracing and export it into a zip archive.
            await Context.Tracing.StopAsync(new()
            {
                Path = "trace.zip"
            });
        }

        [SetUp]
        public async Task Setup()
        {
            var webAppUrl = TestContext.Parameters["webAppUrl"];

            if (string.IsNullOrWhiteSpace(webAppUrl))
            {
                throw new Exception("The webAppUrl test parameter could not be found. Did you forget to set your runsettings test parameters? see https://learn.microsoft.com/en-us/visualstudio/test/configure-unit-tests-by-using-a-dot-runsettings-file?view=vs-2022");
            }

            await Page.GotoAsync(webAppUrl);
        }
    }
}