# HARDWARIO UI Components Library

This project contains Blazor UI components to simplify building of your application.
These components uses the [Radzen Components Library](https://blazor.radzen.com/get-started?theme=material3) as base.

Note: *This library is still in its start. If you are missing some specific component please let us know by creating the issue or [contact us](https://www.hardwario.com/contact).*

## Using the Library in your project
You can use this UI library in any Blazor project (Blazor Server, WebAssembly or MAUI Blazor Hybrid App). You need to do few first steps to use this library. 

The project has not been published as nuget package yet. So you need to clone the HARDWARIO .NET SDK solution. Then you can go to your solution and "Add Existing Project". There you can add the project "hio-dotnet.UI.BlazorComponents.Radzen" project into your solution. 

Then you can add the library to your specific project as dependency:

Go to your project in Solution Explorer, then Right Click to the project "Dependencies" item and choose "Add Project Reference...". There you can check the UI components project and click Add.

Second step is registering the Radzen components in the main program. For example for MAUI application it is in file MauiProgram.cs, in the WebAssembly (WASM) it is Program.cs:

```csharp
using Radzen;
...
...
builder.Services.AddRadzenComponents();
...
```

Third step is adding the css and js resources. You must add them into index.html file in the "wwwroot" folder:

Add this to the header:
```html
<link rel="stylesheet" href="_content/Radzen.Blazor/css/material-base.css">
```

And this to the body before the blazor.webassembly.js:

```html
<script src="_content/hio-dotnet.UI.BlazorComponents.Radzen/hiodotnet_interop.js"></script>
<script src="_content/Radzen.Blazor/Radzen.Blazor.js"></script>
```

Next step is adding the imports in the _Imports.razor file:

```csharp
@using global::Radzen
@using global::Radzen.Blazor
```
Note: *Here you can add also other namespaces for specific components for example "@using global::hio_dotnet.UI.BlazorComponents.Radzen.CHESTER" or you can add them later in specific page/component.*

And finally last step is adding the services component to the "MainLayout.razor":
Add this at the end of the file:

```csharp
<RadzenComponents />
```
This step is necessary for displaying services such as popup dialogs or notifications. Later you can use for example notification by injecting notification service in your component:

```csharp
@inject NotificationService NotificationService
```

and then calling:

```csharp
void ShowNotification(NotificationMessage message)
{
    NotificationService.Notify(message);
}

private void OnDevEUIChangedHandler(string devEUI)
{
    DevEUI = devEUI;
    ShowNotification(new NotificationMessage
        {
            Severity = NotificationSeverity.Success,
            Summary = "DevEUI Changed",
            Detail = $"New set DevEUI is: {devEUI}",
            Duration = 4000
        });

    console.Log($"DevEUI changed to {devEUI}");
}
```
To display the popup notification in your app. 

Or this part will open you the dialog with OK and Cancel to input the password:

```csharp
@inject DialogService DialogService
```

and code:

```csharp
private string password = string.Empty;
private RadzenPassword? passwordInput;

private async Task Unlock()
{
    var result = await DialogService.OpenAsync("Unlock Device", ds =>
@<RadzenStack Gap="1.5rem">
    <RadzenStack Orientation="Orientation.Vertical">
        <RadzenStack Orientation="Orientation.Horizontal" Gap="0.5rem" AlignItems="AlignItems.Start">
            <RadzenText Text="Password" />
            <RadzenPassword @ref=passwordInput @bind-Value="@password" Style="width: 100%;" onkeydown="@(async (KeyboardEventArgs e) => OnKeyDown(e, ds))" />
        </RadzenStack>
        <RadzenStack Orientation="Orientation.Horizontal">
            <RadzenButton Text="Unlock" Click="() => ds.Close(true)" />
            <RadzenButton Text="Cancel" Click="() => ds.Close(false)" ButtonStyle="ButtonStyle.Light" />
        </RadzenStack>
    </RadzenStack>
</RadzenStack>
);

    if (result == true)
    {
        // DO YOUR UNLOCK ROUTINE...
        await Task.Delay(250);

        if (DeviceService.IsConfigLocked)
        {
            ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Unlock Failed", Detail = "Device is still locked. Please check if the password is correct.", Duration = 4000 });
        }
        else
        {
            ShowNotification(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Unlock Successfull", Detail = "Device is unlocked. You can change configuration now.", Duration = 4000 });
        }
    }
}

private async Task OnKeyDown(KeyboardEventArgs e, DialogService ds)
{
    if (e.Key == "Enter")
    {
	    // this will solve the "loosing focus" trouble for Enter key confirmation of the password. The loose of the focus from textedit is important to propagate newely typed value into the password variable.
	    // if you do not injected JS inject it by @inject IJSRuntime JSRuntime together with @using Microsoft.JSInterop
        await JSRuntime.InvokeVoidAsync("document.activeElement.blur");
		// close the dialog
        ds.Close(true);
    }
}
```


## Example projects
Example project with the components is called [hio-dotnet.Demos.BlazorComponents.Radzen.WASM](). You can find the application of practically all the components in the library because we are using this project for the testing components during the development. They are not connected to the HW specific drivers. It means that for example Console component is using just dummy values.

Another projects which are using these components are:

- [hio-dotnet.Demos.HardwarioManger]() - MAUI phone application for BLE communication with CHESTER
- [hio-dotnet.Demos.HardwarioMonitor]() - MAUI desktop application for J-Link and PPKII communication with CHESTER

