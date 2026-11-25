document.addEventListener('DOMContentLoaded', function () {
    // Заглушки для картинок
    document.querySelectorAll('img[data-fallback]').forEach(img => {
        img.onerror = function () {
            this.src = this.getAttribute('data-fallback');
            this.onerror = null;
        };
    });

    // Фильтрация по категориям
    const categoryButtons = document.querySelectorAll('.category-btn');
    const dishesContainer = document.getElementById('dishesContainer');
    const statusMessage = document.getElementById('statusMessage');
    const isAuthenticated = document.getElementById('isAuthenticated')?.value === 'true';

    categoryButtons.forEach(btn => {
        btn.addEventListener('click', function () {
            categoryButtons.forEach(b => b.classList.remove('active'));
            this.classList.add('active');

            const categoryId = this.getAttribute('data-id');

            fetch(`/Home/GetDishesByCategory?categoryId=${categoryId}`)
                .then(response => response.json())
                .then(data => {
                    if (dishesContainer) dishesContainer.innerHTML = '';

                    if (!data.success) {
                        if (statusMessage) statusMessage.textContent = "Ошибка загрузки: " + data.error;
                        return;
                    }

                    if (data.dishes.length === 0) {
                        if (statusMessage) statusMessage.textContent = "В этой категории пока нет блюд";
                        return;
                    }

                    data.dishes.forEach(dish => {
                        const addButton = isAuthenticated
                            ? `<a href="/Cart/AddToCart?dishId=${dish.id}" class="btn-dish">Добавить в корзину</a>`
                            : `<a href="/Account/Login" class="btn-dish-outline">Добавить в корзину</a>`;

                        const card = `
                            <div class="col-md-6 col-lg-4">
                                <div class="dish-card">
                                    <div class="dish-img-container">
                                        <img src="${dish.photoPath || '/images/no-photo.png'}" alt="${dish.name}" class="dish-photo" />
                                    </div>
                                    <div class="dish-body">
                                        <h5 class="dish-title">${dish.name}</h5>
                                        <div class="dish-macros">${dish.description || ''}</div>
                                        <p class="dish-desc">${dish.composition || ''}</p>
                                        <div class="dish-footer">
                                            <p class="dish-price">${dish.price.toLocaleString('ru-RU')} ₽</p>
                                            ${dish.weight ? `<span class="dish-weight">${Math.round(dish.weight)} г</span>` : ''}
                                        </div>
                                        ${addButton}
                                    </div>
                                </div>
                            </div>
                        `;
                        if (dishesContainer) dishesContainer.insertAdjacentHTML('beforeend', card);
                    });

                    if (statusMessage) statusMessage.textContent = `Найдено блюд: ${data.dishes.length}`;
                })
                .catch(err => {
                    if (statusMessage) statusMessage.textContent = "Ошибка сети: " + err;
                });
        });
    });
});