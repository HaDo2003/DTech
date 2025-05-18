//Real-time product updates using SignalR
function initializeProductSignalR(currentCategorySlug) {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationsHub")
        .build();

    connection.start()
        .then(() => {
            console.log("Connected to NotificationsHub");
        })
        .catch(err => console.error(err.toString()));

    connection.on("ReceiveNewProduct", function (product) {
        console.log("Received product:", product);

        if (product.categorySlug !== currentCategorySlug) {
            console.log(`Product category '${product.categorySlug}' does not match current category '${currentCategorySlug}'. Skipping.`);
            return;
        }

        const container = document.getElementById("product-list");
        if (!container) {
            console.error("Element with id 'product-list' not found");
            return;
        }

        const hasDiscount = product.discount > 0;
        const finalPrice = product.price * (1 - product.discount / 100);
        const formattedFinalPrice = finalPrice.toLocaleString('en-US', { minimumFractionDigits: 0 });
        const formattedOriginalPrice = product.price.toLocaleString('en-US', { minimumFractionDigits: 0 });
        const discountPercent = product.discount.toFixed(1).replace(/\.0$/, '');
        const productUrl = `/${product.categorySlug}/${product.brandSlug}/${product.slug}`;

        const productCardHtml = `
            <div class="col">
                <div class="card h-100 product-card position-relative border-0">
                    ${hasDiscount ? `
                        <div class="position-absolute end-0 top-0 m-2">
                            <span class="badge bg-danger rounded-pill">-${discountPercent}%</span>
                        </div>` : ''}

                    <a href="${productUrl}"
                       class="text-decoration-none">
                        <div class="custom-container-img">
                            <img src="${product.photo}" class="card-img-top p-1 w-100 mx-auto d-block" alt="${product.name}">
                        </div>
                        <div class="card-body p-2">
                            <p class="text-start small fw-semibold mb-1 text-dark">
                                ${product.name.length > 40 ? product.name.substring(0, 40) + "..." : product.name}
                            </p>
                            <div class="d-flex flex-column text-start">
                                ${hasDiscount ? `
                                    <p class="mb-0"><del class="text-muted small">$${formattedOriginalPrice}</del></p>
                                    <p class="text-danger fw-bold mb-1">$${formattedFinalPrice}</p>
                                ` : `
                                    <br />
                                    <p class="text-dark fw-bold mb-1">$${formattedOriginalPrice}</p>
                                `}
                            </div>

                            ${product.promotionalGift ? `
                                <div class="custom-text">
                                    ${product.promotionalGift.length > 50
                    ? product.promotionalGift.substring(0, 50) + "..."
                    : product.promotionalGift}
                                </div>
                            ` : ''}
                        </div>
                    </a>
                </div>
            </div>
        `;

        container.insertAdjacentHTML("afterbegin", productCardHtml);
    });
}