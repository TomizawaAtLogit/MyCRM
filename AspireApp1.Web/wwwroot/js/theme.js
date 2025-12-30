// Theme management functions
window.themeManager = {
    setTheme: function(theme) {
        console.log('Setting theme to:', theme);
        document.documentElement.setAttribute('data-bs-theme', theme);
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem('theme', theme);
    },
    
    getTheme: function() {
        const theme = localStorage.getItem('theme') || 'light';
        console.log('Getting theme:', theme);
        return theme;
    },
    
    toggleTheme: function() {
        const currentTheme = this.getTheme();
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
        console.log('Toggling theme from', currentTheme, 'to', newTheme);
        this.setTheme(newTheme);
        return newTheme;
    },
    
    initialize: function() {
        const theme = this.getTheme();
        console.log('Initializing theme:', theme);
        document.documentElement.setAttribute('data-bs-theme', theme);
        document.documentElement.setAttribute('data-theme', theme);
    }
};

// Initialize theme on page load
(function() {
    console.log('Theme script loaded');
    window.themeManager.initialize();
    
    // Listen for Blazor enhanced navigation events
    if (window.Blazor) {
        // For Blazor Server
        window.Blazor.addEventListener('enhancedload', function() {
            console.log('Blazor enhanced navigation detected, reapplying theme');
            window.themeManager.initialize();
        });
    }
    
    // Also listen for standard navigation events as fallback
    window.addEventListener('pageshow', function(event) {
        // Reapply theme when page is shown (including back/forward navigation)
        window.themeManager.initialize();
    });
})();
