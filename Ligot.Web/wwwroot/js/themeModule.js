export function setTheme(theme) {
    console.log('Setting theme to:', theme);
    document.documentElement.setAttribute('data-bs-theme', theme);
    document.documentElement.setAttribute('data-theme', theme);
    localStorage.setItem('theme', theme);
}

export function getTheme() {
    const theme = localStorage.getItem('theme') || 'light';
    console.log('Getting theme:', theme);
    return theme;
}

export function toggleTheme() {
    const currentTheme = getTheme();
    const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
    console.log('Toggling theme from', currentTheme, 'to', newTheme);
    setTheme(newTheme);
    return newTheme;
}

export function initialize() {
    const theme = getTheme();
    console.log('Initializing theme:', theme);
    document.documentElement.setAttribute('data-bs-theme', theme);
    document.documentElement.setAttribute('data-theme', theme);
}

// Initialize on load and on Blazor navigation
initialize();

// Listen for Blazor enhanced navigation events
if (typeof window !== 'undefined') {
    window.addEventListener('pageshow', function(event) {
        initialize();
    });
    
    if (window.Blazor) {
        window.Blazor.addEventListener('enhancedload', function() {
            console.log('Module: Blazor enhanced navigation detected, reapplying theme');
            initialize();
        });
    }
}
