$(document).ready(function () {

    var audio = new Audio("/Content/adminlte/audio/newmessage.mp3");

    var hub = $.connection.notification;

    hub.client.ReceiveMessageFromD1 = function (response) {
        console.log(response);
        console.log(`this is the message : ${response.Body}`);
        var conversationId = response.ConversationId;
        var conversationMessageId = response.ConversationMessageId;
        var messageBody = response.Body;
        var senderName = response.SenderName;
        console.log(`this is the sender name : ${senderName}`);
        var senderProfile = (response.SenderProfile != null) ? response.SenderProfile : '/images/user.png';
        var sentDate = response.SentDate;
        var participants = response.Participants;

        var threadListTableBody = $('#ConversationThreadListTable > tbody');
        var threadListTableRow = $(threadListTableBody).find('tr.conversation-thread-info');
        var selectedThreadInfoRow = $(threadListTableBody).find('tr.conversation-header.selected');

        var conversationIdFromD1 = selectedThreadInfoRow.attr('data-conversation-id');
        var conversationIdFromExternal = $('#conversation_id').val();
        var finalconversationid = "";
        if (conversationIdFromD1 == undefined)
            finalconversationid = conversationIdFromExternal
        if (conversationIdFromExternal == undefined)
            finalconversationid = conversationIdFromD1

        console.log(`this is the current conversation id : ${finalconversationid}`)

        if (conversationId == finalconversationid) {
            var directChatMessagesContainer = $('.direct-chat-messages-container');
            var directCharMessageHtml = `
                <div class="direct-chat-msg">
                    <div class="direct-chat-infos clearfix">
                        <span class="direct-chat-name float-left">${response.SenderName}</span>
                        <span class="direct-chat-timestamp float-right">${response.SentDate}</span>
                    </div>
                    <img class="direct-chat-img" src="${response.SenderProfile}" alt="Message User Image">
                    <div class="direct-chat-text">
                        ${response.Body}
                    </div>
                </div>
            `;
            $(directChatMessagesContainer).append(directCharMessageHtml);

            $('#messagesContainer').animate({ scrollTop: 9999 });
            $('#messagesContainer').animate({ scrollTop: 9999 }).delay(000);
            $('#messagesContainer').animate({ scrollTop: 9999 });
        }

        audio.play();
    }
});