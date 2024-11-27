class NotificationService {
    constructor() {
        this.toastContainer = document.createElement('div');
        this.toastContainer.className = 'toast-container position-fixed top-0 end-0 p-3';
        document.body.appendChild(this.toastContainer);
    }

    show(message, type = 'info') {
        const toastElement = document.createElement('div');
        const toastId = `toast-${Date.now()}`;

        toastElement.className = `toast align-items-center border-0 show`;
        toastElement.setAttribute('role', 'alert');
        toastElement.id = toastId;

        // Set background color based on type
        switch (type) {
            case 'success':
                toastElement.classList.add('bg-success', 'text-white');
                break;
            case 'error':
                toastElement.classList.add('bg-danger', 'text-white');
                break;
            case 'warning':
                toastElement.classList.add('bg-warning', 'text-dark');
                break;
            default:
                toastElement.classList.add('bg-primary', 'text-white');
        }

        toastElement.innerHTML = `
            <div class="d-flex">
                <div class="toast-body">
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" 
                        data-bs-dismiss="toast">
                </button>
            </div>
        `;

        this.toastContainer.appendChild(toastElement);

        // Auto-hide after 5 seconds
        setTimeout(() => {
            const toast = document.getElementById(toastId);
            if (toast) {
                toast.remove();
            }
        }, 5000);
    }

    success(message) {
        this.show(message, 'success');
    }

    error(message) {
        this.show(message, 'error');
    }

    warning(message) {
        this.show(message, 'warning');
    }

    info(message) {
        this.show(message, 'info');
    }
}

// Create global instance
window.notificationService = new NotificationService();