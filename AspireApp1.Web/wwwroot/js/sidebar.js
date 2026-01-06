// Sidebar collapse functionality
window.sidebarManager = {
    toggle: function(isCollapsed) {
        const sidebar = document.querySelector('.sidebar');
        console.log('Toggle called, isCollapsed:', isCollapsed, 'sidebar found:', !!sidebar);
        
        if (sidebar) {
            if (isCollapsed) {
                sidebar.classList.add('collapsed');
            } else {
                sidebar.classList.remove('collapsed');
            }
            console.log('Sidebar classes:', sidebar.className);
            return true;
        } else {
            console.error('Sidebar element not found in DOM');
            return false;
        }
    }
};

console.log('Sidebar manager loaded');
