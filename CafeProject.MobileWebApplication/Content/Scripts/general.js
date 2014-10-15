/* Messages */
function hideMessages() {
    $("div.message").hide();
}

function displayMessage(messageSelector) {
    hideMessages();
    $(messageSelector).show();
}