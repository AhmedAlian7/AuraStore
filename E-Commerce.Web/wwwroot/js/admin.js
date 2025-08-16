document.addEventListener('DOMContentLoaded', function () {
    const sidebar = document.getElementById('sidebar');
    const mainWrapper = document.getElementById('mainWrapper');
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebarOverlay = document.getElementById('sidebarOverlay');

    // Sidebar toggle functionality
    sidebarToggle.addEventListener('click', function () {
        if (window.innerWidth <= 768) {
            // Mobile: Show/hide sidebar
            sidebar.classList.toggle('show');
            sidebarOverlay.classList.toggle('show');
        } else {
            // Desktop: Collapse/expand sidebar
            sidebar.classList.toggle('collapsed');
            mainWrapper.classList.toggle('sidebar-collapsed');
        }
    });

    // Close sidebar on overlay click (mobile)
    sidebarOverlay.addEventListener('click', function () {
        sidebar.classList.remove('show');
        sidebarOverlay.classList.remove('show');
    });

    // Handle window resize
    window.addEventListener('resize', function () {
        if (window.innerWidth > 768) {
            sidebar.classList.remove('show');
            sidebarOverlay.classList.remove('show');
        } else {
            sidebar.classList.remove('collapsed');
            mainWrapper.classList.remove('sidebar-collapsed');
        }
    });

    // Submenu toggle
    const dropdownToggles = document.querySelectorAll('.dropdown-toggle');
    dropdownToggles.forEach(function (toggle) {
        toggle.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('data-bs-target'));
            const isExpanded = this.getAttribute('aria-expanded') === 'true';

            this.setAttribute('aria-expanded', !isExpanded);
            this.classList.toggle('collapsed');
            target.classList.toggle('show');
        });
    });

    // Set active navigation item based on current URL
    const currentPath = window.location.pathname;
    const navLinks = document.querySelectorAll('.nav-link');

    navLinks.forEach(function (link) {
        if (link.getAttribute('href') === currentPath) {
            // Remove active class from all links
            navLinks.forEach(l => l.classList.remove('active'));
            // Add active class to current link
            link.classList.add('active');

            // If it's a submenu item, expand the parent
            const parentSubmenu = link.closest('.nav-submenu');
            if (parentSubmenu) {
                parentSubmenu.classList.add('show');
                const parentToggle = document.querySelector(`[data-bs-target="#${parentSubmenu.id}"]`);
                if (parentToggle) {
                    parentToggle.classList.remove('collapsed');
                    parentToggle.setAttribute('aria-expanded', 'true');
                }
            }
        }
    });
});