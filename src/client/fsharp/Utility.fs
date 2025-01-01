module AAG.Client.Utility

open Microsoft.JSInterop

let disableButton (id: string) (state: bool) (jsRuntime: IJSRuntime) =
    jsRuntime.InvokeVoidAsync("setButtonDisabled", id, state)
