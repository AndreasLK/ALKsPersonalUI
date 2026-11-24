using ALKsPersonalUI; //Force reload due to packages not building
using ALKsPersonalUI.Core;
using ALKsPersonalUI.Showcase;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddPersonalUILibrary(options =>
{
    options.DefaultPressEffect = ButtonPressEffect.Auto;
    options.DefaultTouchInputMode = TouchInputMode.Auto;

    options.AddSeedTheme(
        name: "SunsetTouch",
        accentColor: "#f97316",
        applicationBackgroundColor: "#16100c",
        actionColor: "#fb923c",
        isDarkTheme: true,
        defaultPressEffect: ButtonPressEffect.Auto);
});

await builder.Build().RunAsync();
