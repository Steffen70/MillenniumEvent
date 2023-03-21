# Demo Application: Motorcycle Renting Platform based on Millennium Ticket Generator Site

## Config

Add the following configuration to the appsettings.json or create matching enviroment variables.

```json
{
    "ApiSettings":  {
        "ConnectionString": "MySql_ConnectionString",
        "AdminUsername": "email",
        "AdminPassword": "password",
        "DefaultPassword": "password",
        "TokenKey": "GUID"
    }
}
```

Add **CORS_ORIGIN** = ```https://localhost/``` and **APPSETTINGS** = ```Data/appsettings.json``` to your environment variables
