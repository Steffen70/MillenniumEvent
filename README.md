# Millennium Event Ticket Generator Site

## Config

Add the following configuration to the appsettings.json or create matching enviroment variables.

```json
{
    "ApiSettings":  {
        "ConnectionString": "MySql_ConnectionString",
        "AdminEmail": "email",
        "AdminPassword": "password",
        "DefaultPassword": "password",
        "TokenKey": "GUID",
        "FlyerImageName": "base flyer.png name in data directory",
        "EmailConfiguration": {
            "From": "email address",
            "SmtpServer": "smtp.gmail.com",
            "Port": 465,
            "Username": "email address",
            "Password": "email password"
        },
        "Views": [
            "Index",
            "Login",
            "Tickets",
            "Impressum",
            "Dashboard"
        ]
    },
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    }
}
```

Add **CORS_ORIGIN** = https://localhost/ to your environment variables
