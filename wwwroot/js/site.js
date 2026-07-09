document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.getElementById("sidebar");
    const sidebarToggle = document.getElementById("sidebarToggle");
    const themeToggle = document.getElementById("themeToggle");

    if (sidebarToggle && sidebar) {
        sidebarToggle.addEventListener("click", function () {
            if (window.innerWidth <= 850) {
                sidebar.classList.toggle("mobile-open");
            } else {
                sidebar.classList.toggle("collapsed");
            }
        });
    }

    if (themeToggle) {
        themeToggle.addEventListener("click", function () {
            document.body.classList.toggle("light-mode");

            const isLight = document.body.classList.contains("light-mode");
            localStorage.setItem("theme", isLight ? "light" : "dark");
        });
    }

    const savedTheme = localStorage.getItem("theme");

    if (savedTheme === "light") {
        document.body.classList.add("light-mode");
    }

    const currentPath = window.location.pathname.toLowerCase();

    document.querySelectorAll(".menu-link").forEach(link => {
        const href = link.getAttribute("href");

        if (!href || href === "#") return;

        if (currentPath.startsWith(href.toLowerCase())) {
            link.classList.add("active");
        }
    });
});