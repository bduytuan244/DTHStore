document.getElementById("send-message").addEventListener("click", function () {
    let messageText = document.getElementById("message-text").value;

    if (messageText.trim() === "") return;

    fetch("/Message/SendMessage", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({
            senderId: 1, // ID của người gửi (lấy từ session)
            receiverId: 2, // ID của admin hoặc người nhận
            senderType: "customer",
            receiverType: "admin",
            messageText: messageText
        }),
    })
        .then(response => response.json())
        .then(data => {
            document.getElementById("message-text").value = "";
            loadMessages();
        });
});

// Tải tin nhắn khi mở chat
function loadMessages() {
    fetch("/Message/GetMessages?senderId=1&receiverId=2")
        .then(response => response.json())
        .then(messages => {
            let chatMessages = document.getElementById("chat-messages");
            chatMessages.innerHTML = "";
            messages.forEach(msg => {
                let msgElement = document.createElement("div");
                msgElement.textContent = msg.message_text;
                chatMessages.appendChild(msgElement);
            });
        });
}