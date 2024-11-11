module AAG.Client.MainPage

open Bolero

type Main = Template<"wwwroot/main.html">

let menuItem currentPage page (text: string) =
    Main
        .MenuItem()
        .Active(
            if currentPage = page then
                "is-active"
            else
                ""
        )
        .Url(Routing.router.Link page)
        .Text(text)
        .Elt()

