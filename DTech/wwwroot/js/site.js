// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//Hide or show the password
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

//Switch Tab in profile page
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

//Review Image
function img_pathUrl(input) {
    $("img#imgpreview")[0].src = URL.createObjectURL(input.files[0]);
}

//Open form to add addresss
function openForm() {
    const form = document.getElementById("addressForm");
    const overlay = document.getElementById("overlay");
    form.classList.remove('d-none');
    form.classList.add('d-block');
    overlay.classList.remove('d-none');
}

//Close add addresss form
function closeForm() {
    const form = document.getElementById("addressForm");
    const overlay = document.getElementById("overlay");
    form.classList.remove('d-block');
    form.classList.add('d-none');
    overlay.classList.add('d-none');
}

//Datable for coupon and order table
document.addEventListener("DOMContentLoaded", function () {
    if ($('#myTable').length) {
        $('#myTable').DataTable();
    }

    if ($('#myCouponTable').length) {
        $('#myCouponTable').DataTable();
    }
});

//Edit address
function loadEditAddress(url) {
    fetch(url)
        .then(response => {
            if (!response.ok) throw new Error("Failed to load edit form");
            return response.text();
        })
        .then(html => {
            // Find the edit address form element
            const editForm = document.getElementById("editAddressForm");
            const overlay = document.getElementById("editOverlay");

            // Replace the partial view content
            editForm.innerHTML = html;

            // Show the overlay and form
            overlay.classList.remove("d-none");
            editForm.classList.remove("d-none");
        })
        .catch(error => {
            console.error(error);
            alert("Could not load edit form: " + error.message);
        });
}

function closeEditForm() {
    const overlay = document.getElementById("editOverlay");
    const container = document.getElementById("editAddressForm");

    container.innerHTML = "";  // Clear the content
    container.classList.remove("d-block");
    container.classList.add("d-none");
    overlay.classList.add("d-none");
}

//Horizontal card slider navigation
function slideLeft(id) {
    const container = document.getElementById(id);
    container.scrollBy({ left: -220, behavior: 'smooth' });
}

function slideRight(id) {
    const container = document.getElementById(id);
    container.scrollBy({ left: 220, behavior: 'smooth' });
}

//// product-purchase
//document.addEventListener('DOMContentLoaded', function () {
//    // DOM Elements
//    const quantityInput = document.getElementById('quantity');
//    const decreaseBtn = document.getElementById('decreaseQuantity');
//    const increaseBtn = document.getElementById('increaseQuantity');
//    const addToCartBtn = document.getElementById('addToCart');
//    const buyNowBtn = document.getElementById('buyNow');
//    const productIdElement = document.getElementById('productId');

//    // Quantity adjustment
//    decreaseBtn.addEventListener('click', function () {
//        let quantity = parseInt(quantityInput.value);
//        if (quantity > 1) {
//            quantityInput.value = quantity - 1;
//        }
//    });

//    increaseBtn.addEventListener('click', function () {
//        let quantity = parseInt(quantityInput.value);
//        quantityInput.value = quantity + 1;
//    });

//    // Add to cart button
//    addToCartBtn.addEventListener('click', function () {
//        let quantity = parseInt(quantityInput.value);
//        let productId = productIdElement ? productIdElement.value : null;

//        const formData = new FormData();
//        formData.append('productId', productId);
//        formData.append('quantity', quantity);

//        fetch('/Cart/AddToCart', {
//            method: 'POST',
//            headers: {
//                'Content-Type': 'application/x-www-form-urlencoded',
//            },
//            body: new URLSearchParams(formData)
//        })
//            .then(response => response.json())
//            .then(data => {
//                if (data.success) {
//                    const cartCountElement = document.getElementById('cartCount');
//                    if (cartCountElement) {
//                        cartCountElement.textContent = data.cartCount;
//                    }
//                    showNotification("Added to Cart", "success");
//                } else {
//                    if (data.notLoggedIn) {
//                        alert("Please log in to add products to your cart.");
//                        window.location.href = '/Authentication/Login';
//                    } else {
//                        showNotification(data.message, "error");
//                    }
//                }
//            })
//            .catch(error => {
//                showNotification("Error, please try again!", "error");
//            });
//    });

//    // Buy now button
//    buyNowBtn.addEventListener('click', function () {
//        let quantity = parseInt(quantityInput.value);
//        let productId = productIdElement ? productIdElement.value : null;

//        // Create form data
//        const formData = new FormData();
//        formData.append('productId', productId);
//        formData.append('quantity', quantity);

//        // Send AJAX request
//        fetch('/Order/BuyNow', {
//            method: 'POST',
//            headers: {
//                'Content-Type': 'application/x-www-form-urlencoded',
//            },
//            body: new URLSearchParams(formData)
//        })
//            .then(response => response.json())
//            .then(data => {
//                if (data.success) {
//                    window.location.href = '/Order/Checkout';
//                } else {
//                    showNotification(data.message, "error");
//                }
//            })
//            .catch(error => {
//                showNotification("Có lỗi xảy ra. Vui lòng thử lại sau", "error");
//            });
//    });

//    // Notification function
//    function showNotification(message, type) {
//        // Check if toastr is available
//        if (typeof toastr !== 'undefined') {
//            toastr[type](message);
//        } else {
//            // Create a simple notification element
//            const notification = document.createElement('div');
//            notification.className = 'alert alert-' + (type === 'success' ? 'success' : 'danger');
//            notification.textContent = message;
//            notification.style.position = 'fixed';
//            notification.style.top = '20px';
//            notification.style.right = '20px';
//            notification.style.zIndex = '9999';
//            notification.style.padding = '10px 20px';
//            notification.style.borderRadius = '4px';
//            notification.style.opacity = '0';
//            notification.style.transition = 'opacity 0.3s ease-in-out';

//            document.body.appendChild(notification);

//            // Fade in
//            setTimeout(() => {
//                notification.style.opacity = '1';
//            }, 10);

//            // Fade out and remove after 3 seconds
//            setTimeout(() => {
//                notification.style.opacity = '0';
//                setTimeout(() => {
//                    document.body.removeChild(notification);
//                }, 300);
//            }, 3000);
//        }
//    }
//});

//Toggle see more
document.addEventListener('DOMContentLoaded', function () {
    const toggleBtn = document.getElementById('toggleDescriptionBtn');
    const preview = document.getElementById('descriptionPreview');
    const full = document.getElementById('descriptionFull');
    const textSpan = document.getElementById('toggleText');
    const icon = document.getElementById('toggleIcon');

    if (!toggleBtn) return;

    const chevronDown = `
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-chevron-down" viewBox="0 0 16 16">
          <path fill-rule="evenodd" d="M1.646 4.646a.5.5 0 0 1 .708 0L8 10.293l5.646-5.647a.5.5 0 0 1 .708.708l-6 6a.5.5 0 0 1-.708 0l-6-6a.5.5 0 0 1 0-.708"/>
        </svg>`;

    const chevronUp = `
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-chevron-up" viewBox="0 0 16 16">
          <path fill-rule="evenodd" d="M7.646 4.646a.5.5 0 0 1 .708 0l6 6a.5.5 0 0 1-.708.708L8 5.707l-5.646 5.647a.5.5 0 0 1-.708-.708z"/>
        </svg>`;

    toggleBtn.addEventListener('click', () => {
        setTimeout(() => {
            const isExpanded = full.classList.contains('show');
            if (isExpanded) {
                textSpan.textContent = 'See Less';
                icon.innerHTML = chevronUp;
            } else {
                textSpan.textContent = 'See More';
                icon.innerHTML = chevronDown;
            }
        }, 350);
    });
});

//Open full specification
function openWindow() {
    const form = document.getElementById("SpecWindow");
    const overlay = document.getElementById("overlay");
    form.classList.remove('d-none');
    form.classList.add('d-block');
    overlay.classList.remove('d-none');
}

//Close full specification
function closeWindow() {
    const form = document.getElementById("SpecWindow");
    const overlay = document.getElementById("overlay");
    form.classList.remove('d-block');
    form.classList.add('d-none');
    overlay.classList.add('d-none');
}

//Display sub list of accessory
document.addEventListener("DOMContentLoaded", function () {
    const setupHoverDropdown = (triggerId, listId) => {
        const trigger = document.getElementById(triggerId);
        const list = document.getElementById(listId);

        if (!trigger || !list) return;

        trigger.addEventListener('mouseover', () => {
            list.classList.remove('d-none');
            list.classList.add('d-block');
        });

        trigger.addEventListener('mouseout', () => {
            list.classList.remove('d-block');
            list.classList.add('d-none');
        });

        list.addEventListener('mouseover', () => {
            list.classList.remove('d-none');
            list.classList.add('d-block');
        });

        list.addEventListener('mouseout', () => {
            list.classList.remove('d-block');
            list.classList.add('d-none');
        });
    };

    setupHoverDropdown('mouse', 'mouselist');
    setupHoverDropdown('keyboard', 'keyboardlist');
    setupHoverDropdown('headphone', 'headphonelist');
});

function handleNotLoggedIn() {
    showNotification("Please log in to add products to your cart.", "info");

    // Store current page to return after login
    const returnUrl = encodeURIComponent(window.location.pathname);

    // Brief delay for notification visibility
    setTimeout(() => {
        window.location.href = `/Authentication/Login?returnUrl=${returnUrl}`;
    }, 1000);
}

// Add product to cart via API
function addProductToCart(productId, quantity) {
    const params = new URLSearchParams();
    params.append('productId', productId);
    params.append('quantity', quantity);

    fetch('/Cart/AddToCart', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'X-Requested-With': 'XMLHttpRequest'
        },
        body: params.toString()
    })
        .then(response => {
            if (!response.ok) throw new Error('Network response was not ok');
            return response.json();
        })
        .then(data => {
            if (data.success) {
                updateCartCount(data.cartCount);
                showNotification("Added to Cart", "success");
            } else if (data.notLoggedIn) {
                handleNotLoggedIn();
            } else {
                showNotification(data.message, "error");
            }
        })
        .catch(error => {
            showNotification("Error, please try again!", "error");
            console.error("Error:", error);
        });
}

// Update cart count in the UI
function updateCartCount(count) {
    const cartCountElement = document.getElementById('cartCount');
    if (cartCountElement) {
        cartCountElement.textContent = count;
    }
}

// Notification function
function showNotification(message, type) {
    // Check if toastr is available
    if (typeof toastr !== 'undefined') {
        toastr[type](message);
    } else {
        // Create a simple notification element
        const notification = document.createElement('div');
        notification.className = 'alert alert-' + (type === 'success' ? 'success' : 'danger');
        notification.textContent = message;
        notification.style.position = 'fixed';
        notification.style.top = '20px';
        notification.style.right = '20px';
        notification.style.zIndex = '9999';
        notification.style.padding = '10px 20px';
        notification.style.borderRadius = '4px';
        notification.style.opacity = '0';
        notification.style.transition = 'opacity 0.3s ease-in-out';

        document.body.appendChild(notification);

        // Fade in
        setTimeout(() => {
            notification.style.opacity = '1';
        }, 10);

        // Fade out and remove after 3 seconds
        setTimeout(() => {
            notification.style.opacity = '0';
            setTimeout(() => {
                document.body.removeChild(notification);
            }, 300);
        }, 3000);
    }
}