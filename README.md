# Install the browsers
Build the UITests project. Once built, open a PowerShell window and run the following:
```powershell
./UITests/bin/Debug/net7.0/playwright.ps1 install
```

# Run the test generator
Playwright comes with the ability to generate tests out of the box and is a great way to quickly get started with testing. It will open two windows, a browser window where you interact with the website you wish to test and the Playwright Inspector window where you can record your tests, copy the tests, clear your tests as well as change the language of your tests.
```powershell
./UITests/bin/Debug/net7.0/playwright.ps1 codegen https://localhost:7092
```

change `net7.0` depending of your .net version

# Start the project
To be able run the tests, you need to have the project running. To do so, open a PowerShell window and run the following
```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run --launch-profile "https" --project .\E2ETestingDemo\E2ETestingDemo.csproj
```

# Tracing
You can use Playwrights tracing capabilities by running the test `homepage_should_contain_the_welcome_text_with_tracing`. The file will be saved in the bin/Debug folder of the UITests project.
To view the tracing results, you can execute the following in a PowerShell window:
```powershell
./UITests/bin/Debug/net7.0/playwright.ps1 show-trace <path_to_tracing_zip>
```

replace `<path_to_tracing_zip>` with the tracing zip instance path location i.e. `./UITests/bin/Debug/net7.0/trace.zip`