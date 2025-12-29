// Theme management functions
window.themeManager = {
    setTheme: function(theme) {
        document.documentElement.setAttribute('data-bs-theme', theme);
        document.documentElement.setAttribute('data-theme', theme);
        localStorage.setItem('theme', theme);
    },
    
    getTheme: function() {
        return localStorage.getItem('theme') || 'light';
    },
    
    toggleTheme: function() {
        const currentTheme = this.getTheme();
        const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
        this.setTheme(newTheme);
        return newTheme;
    }
};

// Initialize theme on page load
(function() {
    const theme = window.themeManager.getTheme();
    document.documentElement.setAttribute('data-bs-theme', theme);
    document.documentElement.setAttribute('data-theme', theme);
})();
