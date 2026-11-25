// wwwroot/js/cart.js

// Функция обновления плавающей панели
function updateCartSummary() {
    fetch('/Cart/GetSummary')
        .then(function (response) { return response.json(); })
        .then(function (data) {
            var count = data.TotalItems || data.totalItems || 0;
            var amount = data.TotalAmount || data.totalAmount || 0;

            var countEl = document.getElementById('cart-count');
            var totalEl = document.getElementById('cart-total');
            var floatingCart = document.getElementById('floating-cart'); // Получаем саму панель

            if (countEl) countEl.textContent = count.toString();
            if (totalEl) totalEl.textContent = Number(amount).toLocaleString('ru-RU', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

            // ГЛАВНОЕ ИЗМЕНЕНИЕ: Показываем или скрываем панель
            if (count > 0) {
                floatingCart.style.display = 'flex';
            } else {
                floatingCart.style.display = 'none';
            }
        })
        .catch(function (err) { console.error('Ошибка корзины:', err); });
}

document.addEventListener('DOMContentLoaded', function () {
    // 1. Запуск обновления панели при загрузке
    updateCartSummary();

    // 2. Перехват кликов по кнопкам "Добавить в корзину" (для мгновенного обновления панели)
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
