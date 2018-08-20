# Achievements
Microservice for managing achievements and tracking user actions.

## Development Environment Setup
* Clone the repository
* Configure the app:
  Create a file `appsettings.Development.json` in the same folder as `HiP-Achievements.csproj` with the following content, replacing the values with valid URLs where necessary:
  ```
  {
      "Endpoints": {
          "DataStoreHost": "https://docker-hip.cs.uni-paderborn.de/develop/datastore",
          "ThumbnailServiceHost": "https://docker-hip.cs.upb.de/develop/thumbnailservice",
          "ThumbnailUrlPattern": "achievements/api/image/{0}"
      }
  }
  ```
* Launch the app
  * via Visual Studio: Open the solution (*.sln) and run the app (F5)
  * via Terminal: Execute `dotnet run` from the project folder containing `HiP-Achievements.csproj`

The app is preconfigured to run on dev machines with minimal manual configuration. See [appsettings.json](https://github.com/HiP-App/HiP-Achievements/blob/develop/HiP-Achievements/appsettings.json) for a list of configuration fields and their default values.
To be able to provide test values for local development, make a copy of the appsettings.json and rename it to `appsettings.Development.json` (see [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-2.1) for more information) 
