// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function togglePassword(inputId, iconId) {
    var input = document.getElementById(inputId);
    var icon = document.getElementById(iconId);

    if (input.type === "password") {
        input.type = "text";
        icon.classList.remove("fa-eye");
        icon.classList.add("fa-eye-slash");
    } else {
        input.type = "password";
        icon.classList.remove("fa-eye-slash");
        icon.classList.add("fa-eye");
    }
}

document.addEventListener("DOMContentLoaded", function () {
    const menuItems = document.querySelectorAll('#menu li');
    const tabContents = document.querySelectorAll('.tab-content');

    menuItems.forEach(item => {
        item.addEventListener('click', () => {
            // Remove 'active' from all menu items
            menuItems.forEach(li => li.classList.remove('active'));

            // Add 'active' to the clicked item
            item.classList.add('active');

            // Hide all tab contents
            tabContents.forEach(tab => tab.classList.add('d-none'));

            // Show the selected tab content
            const tabId = item.getAttribute('data-tab');
            const activeTab = document.getElementById(tabId);
            if (activeTab) {
                activeTab.classList.remove('d-none');
            }
        });
    });
});

function img_pathUrl(input) {
    $("img#imgpreview")[0].src = URL.createObjectURL(input.files[0]);
}

function openForm() {
    const form = document.getElementById("addressForm");
    const overlay = document.getElementById("overlay");
    form.classList.remove('d-none');
    form.classList.add('d-block');
    overlay.classList.remove('d-none');
}

function closeForm() {
    const form = document.getElementById("addressForm");
    const overlay = document.getElementById("overlay");
    form.classList.remove('d-block');
    form.classList.add('d-none');
    overlay.classList.add('d-none');
}