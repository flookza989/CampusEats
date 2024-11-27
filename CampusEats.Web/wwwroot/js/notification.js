const showNotification = (message, type = 'success') => {
    const backgroundColor = type === 'success' ? '#28a745' : '#dc3545';

    Toastify({
        text: message,
        duration: 3000,
        gravity: "top",
        position: "right",
        backgroundColor: backgroundColor,
        stopOnFocus: true,
        close: true
    }).showToast();
};