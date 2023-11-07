using FluentAssertions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace UITests;

[Parallelizable(ParallelScope.Self)]
// Beginning with NUnit 2.5, the TestFixture attribute is optional for non-parameterized, non-generic fixtures.
// So long as the class contains at least one method marked with the Test, TestCase or TestCaseSource attribute, it will be treated as a test fixture.
[TestFixture]
public class ContosoPagesTests : PageTest
{
    public const int ProductId = 1;

    [Test]
    public async Task a_user_should_be_able_to_search_by_text()
    {
        await Page.GetByPlaceholder("Search by product name or search by image").FillAsync("laptops");
        await Page.GetByPlaceholder("Search by product name or search by image").PressAsync("Enter");
        await Expect(Page).ToHaveURLAsync("/suggested-products-list");
    }

    [Test]
    public async Task a_user_should_be_able_to_select_a_category()
    {
        await Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions {Name = "All Categories" }).ClickAsync();
        await Page.GetByRole(AriaRole.Menuitem, new PageGetByRoleOptions {Name = "Laptops" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync("/list/laptops");
    }

    [Test]
    public async Task a_user_should_be_able_to_hover_over_header_menus()
    {
        await Page.GetByRole(AriaRole.Navigation).GetByRole(AriaRole.Link, new LocatorGetByRoleOptions {Name = "All Products" }).HoverAsync();
        await Page.GetByRole(AriaRole.Navigation).GetByRole(AriaRole.Link, new LocatorGetByRoleOptions {Name = "Laptops" }).HoverAsync();
        await Page.GetByRole(AriaRole.Navigation).GetByRole(AriaRole.Link, new LocatorGetByRoleOptions {Name = "Controllers" }).HoverAsync();
        await Page.GetByRole(AriaRole.Navigation).GetByRole(AriaRole.Link, new LocatorGetByRoleOptions {Name = "Mobiles" }).HoverAsync();
        await Page.GetByRole(AriaRole.Navigation).GetByRole(AriaRole.Link, new LocatorGetByRoleOptions {Name = "Monitors" }).HoverAsync();
    }

    [Test]
    public async Task a_user_should_be_able_to_select_a_header_menu()
    {
        await Page.GetByRole(AriaRole.Navigation).GetByRole(AriaRole.Link, new LocatorGetByRoleOptions { Name = "All Products" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync("/list/all-products");
    }

    [Test]
    public async Task the_prev_and_next_buttons_should_change_the_slider()
    {
        var carousel = Page.GetByTestId("carousel");
        await Expect(carousel.GetByText("The Fastest, Most Powerful Xbox Ever.")).ToBeVisibleAsync();
        await carousel.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions {Name = "Next" }).ClickAsync();
        await Expect(carousel.GetByText("Xbox Wireless Controller - Mineral Camo Special Edition")).ToBeVisibleAsync();
        await carousel.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Previous" }).ClickAsync();
    }

    [Test]
    public async Task the_carousel_buttons_should_change_the_slider()
    {
        var carousel = Page.GetByTestId("carousel");
        await carousel.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "carousel indicator 2" }).ClickAsync();
        await Expect(carousel.GetByText("Xbox Wireless Controller - Mineral Camo Special Edition")).ToBeVisibleAsync();
        await carousel.GetByRole(AriaRole.Button, new LocatorGetByRoleOptions {Name = "carousel indicator 1" }).ClickAsync();
        await Expect(carousel.GetByText("The Fastest, Most Powerful Xbox Ever.")).ToBeVisibleAsync();
    }

    [Test]
    public async Task the_buy_button_links_to_the_product_page()
    {
        await Page.GetByTestId("carousel").GetByRole(AriaRole.Button, new LocatorGetByRoleOptions {Name = "Buy Now" }).First.ClickAsync();
        await Expect(Page).ToHaveURLAsync("/product/detail/1");
    }

    [Test]
    public async Task the_more_details_links_to_the_list_page()
    {
        await Page.GetByTestId("carousel").GetByRole(AriaRole.Button, new LocatorGetByRoleOptions {Name = "More Details" }).First.ClickAsync();
        await Expect(Page).ToHaveURLAsync("/list/controllers");
    }

    [Test]
    public async Task should_be_able_to_select_product_to_view_details()
    {
        await Page.GotoAsync("/list/all-products");
        await Page.GetByRole(AriaRole.Img, new PageGetByRoleOptions { Name = "Xbox Wireless Controller Lunar Shift Special Edition" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync($"/product/detail/{ProductId}");
    }

    [Test]
    public async Task a_user_should_be_able_to_filter_product_by_brands()
    {
        await Page.GotoAsync("/list/all-products");
        await Page.Locator("[id=\"\\32 \"]").CheckAsync();
    }

    [Test]
    public async Task the_image_should_not_break_the_ui()
    {
        // Navigate to the page with the image
        await Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = "Mobiles" }).ClickAsync();
        await Page.GetByRole(AriaRole.Img, new PageGetByRoleOptions { Name = "Asus Zenfone 5Z" }).ClickAsync();

        // Get the dimensions of the image
        var image = Page.Locator(".productdetailsimage");
        var imageSize = await image.BoundingBoxAsync();

        // Assert that the image fits within the container
        (imageSize?.Height).Should().BeLessThanOrEqualTo(600);
    }

    [Test]
    public async Task a_user_should_be_able_to_select_the_footer_menu()
    {
        await Page.GetByRole(AriaRole.Listitem).Filter(new LocatorFilterOptions {HasText = "Monitors" }).GetByRole(AriaRole.Link, new LocatorGetByRoleOptions {Name = "Monitors" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync("/list/monitors");
    }

    [SetUp]
    public async Task Setup()
    {
        await Context.Tracing.StartAsync(new TracingStartOptions
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });

        await Page.GotoAsync("/");
    }

    [TearDown]
    public async Task Teardown()
    {
        await Context.Tracing.StopAsync(new TracingStopOptions
        {
            Path = $"trace_pagestests_{TestContext.CurrentContext.Test.Name}.zip"
        });
    }

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            BaseURL = "https://cloudtesting.contosotraders.com"
        };
    }
}