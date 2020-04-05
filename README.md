# SpotifyFunTime

TODO: Make note of appsettings.spotifyclient.json file

---

### Temp - some release publish/build notes

```
cd SpotifyFunTime.Web/ClientApp

npm run build

cd ../../

dotnet publish SpotifyFunTime.Web/SpotifyFunTime.Web.csproj -c Release -o out

cd out

ASPNETCORE_ENVIRONMENT=Release dotnet SpotifyFunTime.Web.dll
```