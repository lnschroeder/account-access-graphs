module AAG.Client.Utility

open Microsoft.JSInterop

let toggleButtonEnabled (id: string) (enable: bool) (jsRuntime: IJSRuntime) =
    jsRuntime.InvokeVoidAsync("setButtonDisabled", id, not enable)
