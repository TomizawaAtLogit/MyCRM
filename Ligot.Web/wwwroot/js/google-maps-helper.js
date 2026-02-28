// Google Maps Integration
window.googleMapsHelper = {
    map: null,
    marker: null,
    geocoder: null,
    _pendingElementId: null,
    _pendingResolve: null,
    _pendingReject: null,
    _scriptLoading: false,

    initMap: function (elementId, apiKey) {
        console.log('Initializing Google Maps...');

        return new Promise((resolve, reject) => {
            // Check if Google Maps API is already loaded
            if (typeof google !== 'undefined' && google.maps) {
                this._createMap(elementId);
                resolve();
                return;
            }

            // Prevent duplicate script injection if already loading
            if (this._scriptLoading) {
                this._pendingElementId = elementId;
                this._pendingResolve = resolve;
                this._pendingReject = reject;
                return;
            }

            // Load Google Maps API
            this._scriptLoading = true;
            this._pendingElementId = elementId;
            this._pendingResolve = resolve;
            this._pendingReject = reject;

            const script = document.createElement('script');
            script.src = `https://maps.googleapis.com/maps/api/js?key=${apiKey}&callback=googleMapsHelper._onGoogleMapsLoaded&v=weekly`;
            script.async = true;
            script.defer = true;
            script.onerror = () => {
                this._scriptLoading = false;
                if (this._pendingReject) {
                    this._pendingReject(new Error('Failed to load Google Maps API'));
                    this._pendingResolve = null;
                    this._pendingReject = null;
                }
            };

            document.head.appendChild(script);
        });
    },

    _onGoogleMapsLoaded: function () {
        console.log('Google Maps API loaded');
        this._scriptLoading = false;
        if (this._pendingElementId) {
            this._createMap(this._pendingElementId);
            this._pendingElementId = null;
        }
        if (this._pendingResolve) {
            this._pendingResolve();
            this._pendingResolve = null;
            this._pendingReject = null;
        }
    },

    _createMap: function (elementId) {
        const mapElement = document.getElementById(elementId);
        if (!mapElement) {
            console.error('Map element not found:', elementId);
            return;
        }

        // Default to a general location if no marker is set yet
        const defaultLocation = { lat: 35.6762, lng: 139.6503 }; // Tokyo

        this.map = new google.maps.Map(mapElement, {
            zoom: 12,
            center: defaultLocation,
            mapTypeControl: true,
            streetViewControl: true,
            fullscreenControl: true
        });

        this.geocoder = new google.maps.Geocoder();
        console.log('Map created successfully');
    },

    showSite: function (siteName, address, latitude, longitude) {
        console.log('Showing site:', siteName, address, latitude, longitude);
        
        if (!this.map) {
            console.error('Map not initialized');
            return;
        }

        // Clear existing marker
        if (this.marker) {
            this.marker.setMap(null);
        }

        // If we have coordinates, use them
        if (latitude && longitude) {
            const position = { lat: latitude, lng: longitude };
            
            this.marker = new google.maps.Marker({
                position: position,
                map: this.map,
                title: siteName,
                animation: google.maps.Animation.DROP
            });

            // Create info window
            const infoWindow = new google.maps.InfoWindow({
                content: `<div><strong>${siteName}</strong><br>${address || 'No address'}</div>`
            });

            this.marker.addListener('click', () => {
                infoWindow.open(this.map, this.marker);
            });

            // Pan and zoom to the marker
            this.map.setCenter(position);
            this.map.setZoom(15);
        } else if (address) {
            // Try to geocode the address
            this.geocodeAndShowSite(siteName, address);
        } else {
            console.warn('No coordinates or address provided for site');
        }
    },

    geocodeAndShowSite: function (siteName, address) {
        if (!this.geocoder) {
            console.error('Geocoder not initialized');
            return;
        }

        this.geocoder.geocode({ address: address }, (results, status) => {
            if (status === 'OK' && results[0]) {
                const location = results[0].geometry.location;
                const lat = location.lat();
                const lng = location.lng();
                
                console.log('Geocoded address:', address, 'to', lat, lng);
                
                this.showSite(siteName, address, lat, lng);
            } else {
                console.error('Geocoding failed:', status);
            }
        });
    },

    geocodeAddress: function (address) {
        return new Promise((resolve, reject) => {
            if (!this.geocoder) {
                reject('Geocoder not initialized');
                return;
            }

            this.geocoder.geocode({ address: address }, (results, status) => {
                if (status === 'OK' && results[0]) {
                    const location = results[0].geometry.location;
                    resolve({
                        latitude: location.lat(),
                        longitude: location.lng()
                    });
                } else {
                    reject(`Geocoding failed: ${status}`);
                }
            });
        });
    },

    clearMap: function () {
        if (this.marker) {
            this.marker.setMap(null);
            this.marker = null;
        }
        
        if (this.map) {
            // Reset to default location
            const defaultLocation = { lat: 35.6762, lng: 139.6503 };
            this.map.setCenter(defaultLocation);
            this.map.setZoom(12);
        }
    }
};

// Make the callback accessible globally
window.googleMapsHelper._onGoogleMapsLoaded = window.googleMapsHelper._onGoogleMapsLoaded.bind(window.googleMapsHelper);
