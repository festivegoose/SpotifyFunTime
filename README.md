# SpotifyFunTime

SpotifyFunTime is a web application that allows you to visualize all sorts of fun stats about your Spotify library and play history.

## Requirements to build

When a user logs into the site, they will be prompted to allow your Spotify app access to their account.

1. Create a developer account and app at https://developer.spotify.com/

    * App Type - Website
    * Redirect URI - http://localhost:5000/sign-in (for local development)

2. Once created, you'll be able to see your app's _Client ID_ and _Client Secret_. Create a file in the root of <span>SpotifyFunTime.</span>Web with the name _appsettings.SpotifyClient.json_ with these values, using the format shown below:

```
{
    "SpotifyClientConfiguration": {
        "ClientId": "<your_client_id>",
        "ClientSecret": "<your_client_secret>"
    }
}
```

## Swagger

Swagger doc can be found at `/swagger`. Authentication is required for all endpoints (see _Backend Notes > Authentication_ below). If you login first at `/login`, your session will become active and you can then test any of the listed endpoints.

## Backend Notes

### Authentication

Once a user authenticates through Spotify, their access/refresh tokens are stored in session, with the access token being used in the `Authorization` header in requests to the Spotify API.

>__Note:__ *Since session is used to store the authenticated state, if you restart the backend while running locally, session will be lost and you will be required to login again.*

Each endpoint has a retry mechanism that will attempt to refresh the access token (using the refresh token) if it has expired. If unable to refresh at any point, the user is redirected back to the /login route.

Default session timeout is set at 10 minutes, this can be changed in [appsettings.json](https://github.com/festivegoose/SpotifyFunTime/blob/master/SpotifyFunTime.Web/appsettings.json)

### Caching

Some of the Spotify calls return a bulky amount of data, and since this data is reused in a lot of cases, it is being cached. Because of this, you'll notice that in some cases (like when calling an endpoint involving the user's library) the initial call may take several seconds, but subsequent calls (even to other endpoints) will be much faster.

The app uses an in-memory cache, which works well on a small scale, but if you plan to have high usage I would recommend switching to something external (Redis, MongoDB, etc.)

Default cache expiration is set at 10 minutes, this can be changed in [appsettings.json](https://github.com/festivegoose/SpotifyFunTime/blob/master/SpotifyFunTime.Web/appsettings.json)

Caching functionality can be changed int [ContentCache.cs](https://github.com/festivegoose/SpotifyFunTime/blob/master/SpotifyFunTime.Application/ContentCache.cs)