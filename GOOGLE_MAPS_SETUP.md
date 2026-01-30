# Google Maps Integration Setup Guide

This application integrates Google Maps to display customer site locations on the Customers page.

## Prerequisites

You need a Google Maps API key to use this feature. Follow these steps to obtain one:

1. Go to the [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select an existing one
3. Enable the following APIs:
   - Maps JavaScript API
   - Geocoding API (optional, for address geocoding)
4. Create credentials:
   - Navigate to "Credentials" in the left menu
   - Click "Create Credentials" → "API Key"
   - Copy the generated API key

## Configuration

### Development Environment

1. Copy `Ligot.Web/appsettings.Development.json.template` to `Ligot.Web/appsettings.Development.json`
2. Replace `YOUR_DEVELOPMENT_GOOGLE_MAPS_API_KEY_HERE` with your actual API key:

```json
{
  "GoogleMaps": {
    "ApiKey": "YOUR_ACTUAL_API_KEY_HERE"
  }
}
```

**Important:** The `appsettings.Development.json` file is ignored by git to prevent accidentally committing your API key.

### Production Environment

1. Set the Google Maps API key as an environment variable or in your production configuration
2. Update `Ligot.Web/appsettings.json` or use secrets management:

```json
{
  "GoogleMaps": {
    "ApiKey": "YOUR_PRODUCTION_API_KEY_HERE"
  }
}
```

## Features

- **Interactive Map Display**: View site locations on an interactive Google Map
- **Site Selection Sync**: Click on a site in the list to see it highlighted on the map
- **Automatic Geocoding**: Addresses can be geocoded to coordinates (optional feature)
- **Single Marker Display**: Shows one marker for the currently selected site

## Usage

1. Navigate to the Customers page
2. Select a customer to view their details
3. Click on the "Sites" tab
4. Select a site from the list to see its location on the map
5. The map will automatically pan and zoom to the selected site

## Database Schema

The following fields have been added to the `customer_sites` table:

- `Latitude` (double precision, nullable)
- `Longitude` (double precision, nullable)

These fields store the geographic coordinates of each site.

## Security Notes

- **Never commit API keys to version control**
- The `appsettings.Development.json` file is listed in `.gitignore`
- Use environment variables or Azure Key Vault for production secrets
- Consider implementing API key restrictions in Google Cloud Console:
  - HTTP referrer restrictions for web applications
  - IP address restrictions for backend services

## Troubleshooting

### Map not displaying

1. Check browser console for errors
2. Verify your API key is correctly configured
3. Ensure the Maps JavaScript API is enabled in Google Cloud Console
4. Check for any API quota or billing issues

### Geocoding not working

1. Ensure the Geocoding API is enabled
2. Check API quota limits
3. Verify the address format is correct

## Future Enhancements

- Automatic geocoding when creating/updating sites
- Bulk coordinate updates for existing sites
- Multiple marker display option
- Custom map styles and themes

