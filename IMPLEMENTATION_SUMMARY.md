# Google Maps Integration - Implementation Summary

## Overview
This implementation adds Google Maps functionality to the Customers page, allowing users to view site locations on an interactive map.

## What Was Implemented

### 1. Database Changes
- **New Fields**: Added `Latitude` (double) and `Longitude` (double) to the `customer_sites` table
- **Migration**: Created `AddSiteCoordinates` migration to update the database schema
- **Nullable Fields**: Coordinates are optional, allowing gradual adoption

### 2. Backend Changes
- **DTOs Updated**:
  - `CustomerSiteDto`: Added Latitude/Longitude fields
  - `CustomerSiteCreateDto`: Added Latitude/Longitude fields  
  - `CustomerSiteUpdateDto`: Added Latitude/Longitude fields
- **Controller**: Updated `CustomersController` to persist coordinates when creating/updating sites
- **API**: All site CRUD operations now support coordinate data

### 3. Frontend Changes
- **New Service**: Created `GoogleMapsService` to manage API key configuration
- **JavaScript Helper**: Added `google-maps-helper.js` with functions for:
  - Initializing Google Maps
  - Displaying markers
  - Syncing map with site selection
  - Geocoding support (prepared for future use)
- **Enhanced SiteTab Component**:
  - Split into two-column layout (site list on left, map on right)
  - Clickable site rows to update map view
  - Automatic map pan/zoom to selected site
  - Visual highlighting of selected site
- **Script Loading**: Added Google Maps helper to main app layout

### 4. Configuration & Security
- **API Key Configuration**: Added to `appsettings.json` with placeholder
- **Template File**: Created `appsettings.Development.json.template` for developer setup
- **Gitignore**: Added specific ignore rule for `Ligot.Web/appsettings.Development.json`
- **Documentation**: Created comprehensive `GOOGLE_MAPS_SETUP.md`

## How It Works

### User Flow
1. User navigates to Customers page
2. Selects a customer to view details
3. Clicks on "Sites" tab
4. Sees split view: site list (left) and map (right)
5. Clicks on any site in the list
6. Map automatically pans and zooms to show that site's location
7. Selected site is highlighted in the list

### Technical Flow
1. On page load, `SiteTab` component initializes Google Maps
2. API key is retrieved from `GoogleMapsService`
3. If API key is valid, map is created in the designated div
4. When a site is selected:
   - Component calls `googleMapsHelper.showSite()`
   - If site has coordinates, marker is placed directly
   - If site only has address, geocoding can convert it to coordinates
   - Map pans and zooms to the location
5. Marker includes an info window with site name and address

## Requirements Fulfilled

✅ **Display Google Map**: Interactive map shown on Sites tab  
✅ **Persist Coordinates**: Latitude/Longitude stored in database  
✅ **Map Sync**: Map updates when site is selected in list  
✅ **Secure API Key**: Stored in appsettings, not committed to git  
✅ **No Backfill**: Coordinates can be added gradually (not required for existing sites)  
✅ **Single Marker**: Only one marker shown at a time (for selected site)  
✅ **No Diagnostics**: Implementation is straightforward without extra logging

## Future Enhancements (Not Implemented)
These are prepared but not actively implemented:
- Automatic geocoding when creating/editing sites
- Bulk coordinate update for existing sites
- Multiple marker display option
- Custom map styles

## Setup Instructions
See `GOOGLE_MAPS_SETUP.md` for detailed setup instructions.

## Files Changed
```
.gitignore
Ligot.DbApi/
  ├── Controllers/CustomersController.cs
  ├── DTOs/CustomerDTOs.cs
  ├── Migrations/20260115054913_AddSiteCoordinates.cs
  └── Models/CustomerSite.cs
Ligot.Web/
  ├── Components/
  │   ├── App.razor
  │   └── Pages/SiteTab.razor
  ├── CustomerApiClient.cs
  ├── Program.cs
  ├── Services/GoogleMapsService.cs
  ├── appsettings.json
  ├── appsettings.Development.json.template
  └── wwwroot/js/google-maps-helper.js
GOOGLE_MAPS_SETUP.md
IMPLEMENTATION_SUMMARY.md (this file)
```

## Testing Notes
- Build completes successfully
- Existing test failure is unrelated (Aspire infrastructure issue)
- Manual testing recommended after setting up API key

## Security Considerations
- API keys are never committed to repository
- Template file approach prevents accidental exposure
- Gitignore is specific to prevent over-blocking
- No credentials or secrets in code
- Google Maps API key should have restrictions configured in Google Cloud Console

## Support
If you encounter issues:
1. Verify API key is correctly configured
2. Check browser console for JavaScript errors
3. Ensure Maps JavaScript API is enabled in Google Cloud Console
4. Review `GOOGLE_MAPS_SETUP.md` for troubleshooting tips

