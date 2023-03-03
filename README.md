# App_Template

## Config

Add the following configuration to the appsettings.json or create matching enviroment variables.

```json
{
    "ApiSettings":  {
        "ConnectionString": MySql_ConnectionString,
        "AdminEmail": email,
        "AdminPassword": password,
        "DefaultPassword": password,
        "TokenKey": GUID
  }
}
```

Add **CORS_ORIGIN** = https://localhost/ to your environment variables
