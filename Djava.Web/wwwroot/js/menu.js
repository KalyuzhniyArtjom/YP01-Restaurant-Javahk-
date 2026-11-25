function addToCart(dishId) {
    fetch('/Cart/AddToCart?dishId=' + dishId, { method: 'POST' })
        .then(() => {
            alert('Товар добавлен в корзину!');
        });
}