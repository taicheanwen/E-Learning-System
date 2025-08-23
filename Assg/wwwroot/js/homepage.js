setTimeout(function() {
        var alert = document.querySelector('.alert');
        if (alert) {
            alert.style.display = 'none';
        }
}, 3000);

// Trim input
$('[data-trim]').on('change', e => {
    e.target.value = e.target.value.trim();
});