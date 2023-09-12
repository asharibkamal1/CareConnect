var chatbotIcon = document.getElementById("chatbot-icon");
var chatbotPopup = document.getElementById("chatbot-popup");
var chatMessages = document.getElementById("chat-messages");
var userInput = document.getElementById("user-input");
var chatbotIcon = document.getElementById("chatbot-icon");
var chatbotPopup = document.getElementById("chatbot-popup");

chatbotIcon.addEventListener("click", function () {
    if (chatbotPopup.style.display === "block") {
        chatbotPopup.style.display = "none";
        chatbotIcon.innerHTML = '<i class="fas fa-comment"></i>';
    } else {
        chatbotPopup.style.display = "block";
        chatbotIcon.innerHTML = '<i class="fas fa-times"></i>';
    }
});
//chatbotIcon.addEventListener("click", function() {
//    chatbotPopup.style.display = "block";
//});

//document.getElementById("close-btn").addEventListener("click", function() {
//    chatbotPopup.style.display = "none";
//});

function sendMessage(message) {
    var messageDiv = document.createElement("div");
    messageDiv.className = "user-message";
    messageDiv.textContent = message;
    chatMessages.appendChild(messageDiv);

    // Simulate a bot response after a delay
    setTimeout(function() {
        var botResponse = "Hello! I'm a chatbot.";
        var botMessageDiv = document.createElement("div");
        botMessageDiv.className = "bot-message";
        botMessageDiv.textContent = botResponse;
        chatMessages.appendChild(botMessageDiv);
    }, 1000);
}
