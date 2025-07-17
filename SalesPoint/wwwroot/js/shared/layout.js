import { initializeSidebar } from './sidebarManager.js';

$(document).ready(function () {
    initializeSidebar();

    const areaScript = document.body.getAttribute('data-area-script');
    if (areaScript) {
        import(areaScript).then(module => {
            if (module.initialize) module.initialize();
        }).catch(err => {
            console.error('Error loading area script:', err);
        });
    }
});