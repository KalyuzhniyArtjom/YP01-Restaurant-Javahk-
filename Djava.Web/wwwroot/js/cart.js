function updateQty(cartId, qty) {
    if (qty < 1) return;
    fetch('/Cart/UpdateQuantity', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ cartId: cartId, quantity: qty })
    }).then(() => location.reload());
}

function removeItem(cartId) {
    fetch('/Cart/RemoveItem', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ cartId: cartId })
    }).then(() => location.reload());
}