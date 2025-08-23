document.addEventListener('DOMContentLoaded', () => {
    if (!window.notificationConnection) {
        window.notificationConnection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationHub")
            .build();
    }

    const connection = window.notificationConnection;

    async function startConnection() {
        try {
            if (connection.state === signalR.HubConnectionState.Disconnected) {
                await connection.start();
                console.log("SignalR Connected.");
            }
        } catch (err) {
            console.error("SignalR Connection Error:", err);
            setTimeout(startConnection, 5000);
        }
    }

    if (!connection._hasReceiveNotificationListener) {
        connection.on("ReceiveNotification", (message, courseId) => {
            console.log('Notification received:', message);

            const notificationBadge = document.getElementById('notificationBadge');
            const notificationList = document.getElementById('notificationList');

            if (notificationBadge && notificationList) {
                let currentCount = parseInt(notificationBadge.textContent || "0");
                notificationBadge.textContent = currentCount + 1;
                notificationBadge.classList.remove('d-none');

                const newNotification = document.createElement('li');
                const courseUrl = `/Course/Details/${courseId}`;
                newNotification.innerHTML = `<a class="dropdown-item" href="${courseUrl}">${message}</a>`;
                notificationList.prepend(newNotification);

                const noNotificationItem = notificationList.querySelector('.text-muted');
                if (noNotificationItem) {
                    noNotificationItem.remove();
                }
            }
        });

        connection._hasReceiveNotificationListener = true;
    }

    startConnection();
});
