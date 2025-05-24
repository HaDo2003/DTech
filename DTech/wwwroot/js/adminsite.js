//Control discount code
document.addEventListener("DOMContentLoaded", function () {
    const discountTypeSelect = document.getElementById('discountTypeSelect');
    const discountInput = document.getElementById('discountInput');
    const maxDiscountInput = document.getElementById('Maxdiscount');
    const discountValidation = document.createElement('span');
    discountValidation.className = 'text-danger';
    discountValidation.id = 'discountValidation';
    discountInput.parentNode.appendChild(discountValidation);

    function handleDiscountTypeChange() {
        const discountType = discountTypeSelect.value;
        if (discountType === 'Direct') {
            maxDiscountInput.value = discountInput.value;
            maxDiscountInput.setAttribute('disabled', 'disabled');
        } else {
            maxDiscountInput.removeAttribute('disabled');
        }
    }

    discountTypeSelect.addEventListener('change', handleDiscountTypeChange);
    discountInput.addEventListener('input', function () {
        if (discountTypeSelect.value === 'Direct') {
            maxDiscountInput.value = discountInput.value;
        }
    });

    document.getElementById('couponForm').addEventListener('submit', function (e) {
        var discountType = discountTypeSelect.value;
        var discount = parseFloat(discountInput.value);
        discountValidation.textContent = '';

        if (discountType === 'Percentage') {
            if (isNaN(discount) || discount >= 50) {
                e.preventDefault();
                discountValidation.textContent = 'For percentage type, discount must be less than 50.';
                discountInput.focus();
            }
            maxDiscountInput.removeAttribute('disabled');
        }
        else if (discountType === 'Direct') {
            if (isNaN(discount) || discount < 0) {
                e.preventDefault();
                discountValidation.textContent = 'For direct discount type, discount must be a positive number.';
                discountInput.focus();
            }
            maxDiscountInput.value = discountInput.value;
            maxDiscountInput.setAttribute('disabled', 'disabled');
        }
    });
});