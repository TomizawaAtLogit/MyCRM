// Language management module for Blazor app
// Uses localStorage for immediate feedback and syncs with database for cross-device persistence

export function getLanguage() {
    const language = localStorage.getItem('language') || getBrowserLanguage();
    console.log('Getting language:', language);
    return language;
}

export function setLanguage(language) {
    console.log('Setting language to:', language);
    localStorage.setItem('language', language);
}

export function getBrowserLanguage() {
    // Get browser language, default to 'en' if not 'en' or 'ja'
    const browserLang = navigator.language || navigator.userLanguage || 'en';
    const langCode = browserLang.split('-')[0]; // Get just 'en' from 'en-US'
    
    // Only return 'en' or 'ja', default to 'en' for others
    if (langCode === 'ja') {
        return 'ja';
    }
    return 'en';
}

export function initialize() {
    const language = getLanguage();
    console.log('Initializing language:', language);
    document.documentElement.setAttribute('lang', language);
}

// Initialize on load
initialize();

// Listen for Blazor enhanced navigation events
if (typeof window !== 'undefined') {
    window.addEventListener('pageshow', function(event) {
        initialize();
    });
    
    if (window.Blazor) {
        window.Blazor.addEventListener('enhancedload', function() {
            console.log('Module: Blazor enhanced navigation detected, reapplying language');
            initialize();
        });
    }
}
