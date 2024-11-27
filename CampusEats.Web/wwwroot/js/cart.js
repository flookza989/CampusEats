// Cart functions
function addToCart(restaurantId, restaurantName, menuItemId, name, price) {
    $.post('/Cart/AddItem', {
        restaurantId: restaurantId,
        restaurantName: restaurantName,
        menuItemId: menuItemId,
        name: name,
        price: price,
        quantity: 1
    })
        .done(function () {
            updateCartDisplay();
            showToast('Item added to cart');
        })
        .fail(function (response) {
            if (response.status === 400) {
                showToast(response.responseText, 'error');
            }
        });
}

function updateCartDisplay() {
    $.get('/Cart/GetCartPartial', function (data) {
        $('#cartContainer').html(data);
        new bootstrap.Offcanvas('#cartOffcanvas').show();
    });
}

function updateQuantity(menuItemId, action) {
    $.post('/Cart/UpdateQuantity', {
        menuItemId: menuItemId,
        action: action
    })
        .done(function () {
            updateCartDisplay();
        });
}

function clearCart() {
    if (confirm('Are you sure you want to clear your cart?')) {
        $.post('/Cart/Clear')
            .done(function () {
                updateCartDisplay();
                showToast('Cart cleared');
            });
    }
}

// Toast notification
function showToast(message, type = 'success') {
    const toast = Toastify({
        text: message,
        duration: 3000,
        close: true,
        gravity: "top",
        position: "right",
        stopOnFocus: true,
        className: type === 'success' ? 'bg-success' : 'bg-danger',
    });

    toast.showToast();
}