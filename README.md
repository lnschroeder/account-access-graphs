# account-access-graphs

A tool for modeling and analyzing account access graphs

## Build application for development
```sh
dotnet watch
# or
dotnet build
```

## Build application for deployment
This will create a production ready deployment of the application. It compiles and bundles all files, which then can be served as a static webserver.
```sh
# generate static content
dotnet publish -c Release -o release src/client/AAG.Client.fsproj

cd release/wwwroot/

# serve it as a webserver
dotnet serve --port 8080 # requires https://github.com/natemcmaster/dotnet-serve
# or
python3 -m http.server 8080
```
