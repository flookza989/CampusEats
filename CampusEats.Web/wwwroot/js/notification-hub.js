class NotificationHub {
    constructor() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationHub")
            .withAutomaticReconnect()
            .build();

        this.connection.onreconnected(() => {
            this.joinUserGroup();
        });

        this.setupHandlers();
        this.start();
    }

    async start() {
        try {
            await this.connection.start();
            this.joinUserGroup();
        } catch (err) {
            console.error("Error establishing SignalR connection:", err);
            setTimeout(() => this.start(), 5000);
        }
    }

    joinUserGroup() {
        const userId = document.querySelector('meta[name="user-id"]').content;
        this.connection.invoke("JoinUserGroup", userId);
    }

    setupHandlers() {
        // Handle new notification
        this.connection.on("ReceiveNotification", (notification) => {
            this.updateNotificationBadge();
            this.showNotificationToast(notification);
            this.updateNotificationDropdown();
        });

        // Handle notification count update
        this.connection.on("UpdateNotificationCount", (count) => {
            const badge = document.getElementById("notification-badge");
            if (badge) {
                badge.textContent = count;
                badge.style.display = count > 0 ? "block" : "none";
            }
        });
    }

    updateNotificationBadge() {
        fetch('/Notification/GetUnreadCount')
            .then(response => response.json())
            .then(data => {
                const badge = document.getElementById("notification-badge");
                if (badge) {
                    badge.textContent = data.count;
                    badge.style.display = data.count > 0 ? "block" : "none";
                }
            });
    }

    updateNotificationDropdown() {
        fetch('/Notification/GetNotificationPartial')
            .then(response => response.text())
            .then(html => {
                const dropdown = document.querySelector('.notification-dropdown');
                if (dropdown) {
                    dropdown.innerHTML = html;
                }
            });
    }

    showNotificationToast(notification) {
        window.notificationService.show(
            `${notification.message}`,
            notification.type.toLowerCase()
        );
    }
}

// Initialize when document is ready
document.addEventListener('DOMContentLoaded', () => {
    window.notificationHub = new NotificationHub();
});