const showNotification = (message, type = 'success') => {
    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer);
            toast.addEventListener('mouseleave', Swal.resumeTimer);
        }
    });

    const iconMap = {
        success: 'success',
        error: 'error',
        warning: 'warning',
        info: 'info'
    };

    const backgroundColors = {
        success: '#28a745',
        error: '#dc3545',
        warning: '#ffc107',
        info: '#17a2b8'
    };

    Toast.fire({
        icon: iconMap[type] || 'info',
        title: message,
        background: backgroundColors[type] || backgroundColors.info,
        color: type === 'warning' ? '#000' : '#fff',
        customClass: {
            popup: 'colored-toast',
            title: 'toast-title'
        }
    });
};