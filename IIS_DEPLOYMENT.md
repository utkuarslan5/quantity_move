# IIS Deployment Guide for Quantity Move API and React Frontend

This guide explains how to deploy the ASP.NET Core API and React frontend on the same IIS site.

## Architecture Overview

- **React Frontend**: Served as static files at the root path `/`
- **ASP.NET Core API**: Deployed as an application at `/api` virtual directory
- **Same Origin**: Both served from the same domain, no CORS needed

## Prerequisites

1. IIS 10.0 or later with ASP.NET Core Hosting Bundle installed
2. .NET 8.0 Runtime installed on the server
3. SQL Server access configured
4. Appropriate permissions for the IIS application pool

## Step 1: Build the React Frontend

```bash
cd frontend
npm install
npm run build
```

This creates a `dist` folder with static files ready for deployment.

## Step 2: Publish the ASP.NET Core API

```bash
cd api
dotnet publish -c Release -o ./publish
```

This creates a `publish` folder with the compiled API.

## Step 3: Configure IIS Site

### 3.1 Create IIS Site

1. Open IIS Manager
2. Right-click on "Sites" → "Add Website"
3. Configure:
   - **Site name**: `QuantityMove` (or your preferred name)
   - **Physical path**: Point to the React `dist` folder (e.g., `C:\inetpub\wwwroot\quantity-move\dist`)
   - **Binding**: Configure HTTP/HTTPS as needed
   - **Port**: 80 (or your preferred port)

### 3.2 Configure API Virtual Directory

1. Right-click on the site → "Add Application"
2. Configure:
   - **Alias**: `api`
   - **Application pool**: Create a new application pool or use existing (must be "No Managed Code" or ".NET CLR Version" set appropriately)
   - **Physical path**: Point to the API `publish` folder (e.g., `C:\inetpub\wwwroot\quantity-move\api\publish`)

### 3.3 Configure Application Pool

1. Select the application pool for the API
2. Set:
   - **.NET CLR Version**: "No Managed Code" (for ASP.NET Core)
   - **Managed Pipeline Mode**: Integrated
   - **Identity**: Use ApplicationPoolIdentity or a specific service account with database access

## Step 4: Configure web.config for API

Create or update `web.config` in the API publish folder:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet" 
                  arguments=".\quantity_move_api.dll" 
                  stdoutLogEnabled="false" 
                  stdoutLogFile=".\logs\stdout" 
                  hostingModel="inprocess" />
      <rewrite>
        <rules>
          <rule name="API Path Base" stopProcessing="true">
            <match url=".*" />
            <conditions>
              <add input="{REQUEST_URI}" pattern="^/api/" />
            </conditions>
            <action type="Rewrite" url="{R:0}" />
          </rule>
        </rules>
      </rewrite>
    </system.webServer>
  </location>
</configuration>
```

## Step 5: Configure API Settings

Update `appsettings.json` in the API publish folder with production settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=YOUR_SERVER;Initial Catalog=SL10P;User ID=YOUR_USER;Password=YOUR_PASSWORD;Connect Timeout=0"
  },
  "Jwt": {
    "SecretKey": "YOUR_SECRET_KEY_MUST_BE_AT_LEAST_32_CHARACTERS_LONG_AND_SECURE",
    "Issuer": "quantity-move-api",
    "Audience": "quantity-move-client",
    "ExpirationInHours": 24
  }
}
```

**Important**: 
- Use a strong, unique secret key for JWT in production
- Store sensitive configuration in environment variables or Azure Key Vault
- Never commit production connection strings or secrets to source control

## Step 6: URL Rewrite Rules (Optional)

If you want to handle client-side routing for the React app, add URL rewrite rules:

1. Install URL Rewrite module for IIS (if not already installed)
2. In IIS Manager, select the site
3. Open "URL Rewrite"
4. Add rule:
   - **Pattern**: `^(?!api).*`
   - **Action type**: Rewrite
   - **Rewrite URL**: `/index.html`
   - **Stop processing**: Yes

This ensures that all non-API routes are handled by the React app.

## Step 7: Test Deployment

1. **Test API**: Navigate to `http://your-server/api/swagger` (if in development mode) or test endpoints directly
2. **Test Frontend**: Navigate to `http://your-server/` - should load React app
3. **Test Authentication**: 
   - Login via frontend
   - Verify JWT token is stored
   - Test API calls with token

## Step 8: Security Considerations

1. **HTTPS**: Configure SSL certificate for production
2. **CORS**: Not needed since same origin, but ensure API doesn't allow all origins
3. **JWT Secret**: Use a strong, randomly generated secret key
4. **Database**: Use least-privilege database user account
5. **Firewall**: Restrict access to database server
6. **Logging**: Configure proper logging and monitoring

## Troubleshooting

### API Not Responding

1. Check application pool is running
2. Verify ASP.NET Core Hosting Bundle is installed
3. Check Windows Event Viewer for errors
4. Verify `web.config` is correct
5. Check file permissions on publish folder

### Frontend Not Loading

1. Verify physical path points to `dist` folder
2. Check `index.html` exists in dist folder
3. Verify static file serving is enabled in IIS
4. Check browser console for errors

### Authentication Issues

1. Verify JWT secret key matches in configuration
2. Check token expiration settings
3. Verify database connection string
4. Check user table structure matches AuthService queries

### Database Connection Issues

1. Verify connection string is correct
2. Check SQL Server is accessible from IIS server
3. Verify database user has appropriate permissions
4. Check firewall rules allow SQL Server port (1433)

## File Structure on Server

```
C:\inetpub\wwwroot\quantity-move\
├── dist\                    (React static files - site root)
│   ├── index.html
│   ├── assets\
│   └── ...
└── api\publish\              (API application - /api virtual directory)
    ├── quantity_move_api.dll
    ├── web.config
    ├── appsettings.json
    └── ...
```

## Maintenance

- **Updates**: Rebuild and republish both frontend and API
- **Logs**: Check `logs\stdout` folder in API publish directory
- **Backups**: Regularly backup database and configuration files
- **Monitoring**: Set up application performance monitoring

## Additional Resources

- [ASP.NET Core IIS Hosting](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/)
- [IIS URL Rewrite Module](https://www.iis.net/downloads/microsoft/url-rewrite)
- [ASP.NET Core Hosting Bundle](https://dotnet.microsoft.com/download/dotnet/8.0)

