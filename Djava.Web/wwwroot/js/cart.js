// wwwroot/js/cart.js

// === ЛОГИКА ОТОБРАЖЕНИЯ АДРЕСА ===
function toggleAddress(isDelivery) {
    var addressGroup = document.getElementById('addressGroup');
    var addressInput = document.getElementById('DeliveryAddress');

    if (addressGroup && addressInput) {
        if (isDelivery) {
            addressGroup.style.display = 'block';
            addressInput.required = true;
        } else {
            addressGroup.style.display = 'none';
            addressInput.required = false;
            addressInput.value = '';
        }
    }
}

// Проверка при загрузке страницы (если браузер запомнил выбор "Доставка")
window.addEventListener('load', function () {
    var radios = document.getElementsByName('PickupMethod');
    for (var i = 0; i < radios.length; i++) {
        if (radios[i].value === 'Доставка' && radios[i].checked) {
            toggleAddress(true);
        }
    }
});

// === ЛОГИКА КОРЗИНЫ (Твоя старая логика) ===

// Функция обновления плавающей панели
function updateCartSummary() {
    fetch('/Cart/GetSummary')
        .then(function (response) { return response.json(); })
        .then(function (data) {
            var count = data.TotalItems || data.totalItems || 0;
            var amount = data.TotalAmount || data.totalAmount || 0;

            var countEl = document.getElementById('cart-count');
            var totalEl = document.getElementById('cart-total');
            var floatingCart = document.getElementById('floating-cart');

            if (countEl) countEl.textContent = count.toString();
            if (totalEl) totalEl.textContent = Number(amount).toLocaleString('ru-RU', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

            if (count > 0) {
                if (floatingCart) floatingCart.style.display = 'flex';
            } else {
                if (floatingCart) floatingCart.style.display = 'none';
            }
        })
        .catch(function (err) { console.error('Ошибка корзины:', err); });
}

document.addEventListener('DOMContentLoaded', function () {
    // 1. Запуск обновления панели при загрузке
    updateCartSummary();

    // 2. Перехват кликов по кнопкам "Добавить в корзину"
    document.addEventListener('click', function (e) {
        var target = e.target;
        while (target && target !== document) {
            if (target.tagName === 'A' && target.getAttribute('href') && target.getAttribute('href').indexOf('/Cart/AddToCart') !== -1) {
                setTimeout(updateCartSummary, 500);
                break;
            }
            if (target.tagName === 'BUTTON' && target.getAttribute('data-dish-id')) {
                setTimeout(updateCartSummary, 500);
                break;
            }
            target = target.parentElement;
        }
    });

    // 3. Логика кнопок +/- 
    document.querySelectorAll('.btn-qty').forEach(function (btn) {
        btn.addEventListener('click', function () {
            const cartId = this.getAttribute('data-cartid');
            const qty = parseInt(this.getAttribute('data-qty'));

            if (qty < 1) return;

            fetch('/Cart/UpdateQuantity', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ cartId: parseInt(cartId), quantity: qty })
            }).then(() => location.reload());
        });
    });

    // 4. Логика удаления
    document.querySelectorAll('.btn-remove-item').forEach(function (btn) {
        btn.addEventListener('click', function () {
            const cartId = this.getAttribute('data-cartid');

            fetch('/Cart/RemoveItem', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ cartId: parseInt(cartId) })
            }).then(() => location.reload());
        });
    });
});
