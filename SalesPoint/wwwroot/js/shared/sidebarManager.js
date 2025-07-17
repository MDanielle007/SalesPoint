function toggleSidebar(show = null) {
    const sidebar = $('#sidebar');
    const overlay = $('#sidebarOverlay');

    if (show === null) {
        show = sidebar.hasClass('-translate-x-full');
    }

    if (show) {
        sidebar.removeClass('-translate-x-full');
        overlay.removeClass('opacity-0 invisible').addClass('opacity-50 visible');
    } else {
        sidebar.addClass('-translate-x-full');
        overlay.removeClass('opacity-50 visible').addClass('opacity-0 invisible');
    }
}

function handleResponsiveSidebar() {
    if ($(window).width() < 768) {
        $('#sidebar').addClass('-translate-x-full');
        $('#sidebarOverlay').addClass('opacity-0 invisible');
    } else {
        $('#sidebar').removeClass('-translate-x-full');
        $('#sidebarOverlay').addClass('opacity-0 invisible');
    }
}

export function initializeSidebar() {
    // Toggle sidebar on button click
    $('#sidebarToggle').click(() => toggleSidebar());

    // Close sidebar when clicking overlay
    $('#sidebarOverlay').click(() => toggleSidebar(false));

    // Close sidebar when clicking the close button inside sidebar
    $('#sidebarClose').click(() => toggleSidebar(false));

    // Handle window resize
    $(window).resize(handleResponsiveSidebar);

    // Initial check
    handleResponsiveSidebar();
}