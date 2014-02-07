utils = {};
utils.generateTimestamp = function() {
    var d = new Date();
    var hours = d.getHours();
    var minutes = d.getMinutes();
    var seconds = d.getSeconds();
    if (hours < 10) hours = '0' + hours;
    if (minutes < 10) minutes = '0' + minutes;
    if (seconds < 10) seconds = '0' + seconds;
    return '[' + hours + ':' + minutes + ':' + seconds + ']';
};

var chat = {};
chat.addMessage = function(params) {
    console.log('chat.addMessage()');
    var type = params.type;
    var data = params.data;
    console.log('type:', type);
    console.log('data:', data);

    // TODO: use template engine
    if (type == 'user.joined') { 
        var message = $($('#chat-event-tmpl').html());
        message.find('.username')
            .text(data.username)
            .end()
            .find('.text')
            .text(' joined the room')
            .end()
            .find('.timestamp')
            .text(utils.generateTimestamp())
            .end()
            .find('.info')
            .addClass('joined');
                
    } else if (type == 'user.message') {
        var message = $($('#chat-msg-tmpl').html());
        message.find('.timestamp')
            .text(utils.generateTimestamp())
            .end()
            .find('.username')
            .text(data.username)
            .end()
            .find('.message')
            .text(data.text);
    } else if (type == 'user.left') {
        var message = $($('#chat-event-tmpl').html());
        message.find('.username')
            .text(data.username)
            .end()
            .find('.text')
            .text(' left the room')
            .end()
            .find('.timestamp')
            .text(utils.generateTimestamp())
            .end()
            .find('.info')
            .addClass('left');
    }
    $('#chat').append(message)
        .scrollTop($('#chat')[0].scrollHeight);
};

var hub = $.connection.musicRoomHub;

hub.client.onUserJoined = function(user) {
    console.log('user joined:', user);
    chat.addMessage({ type: 'user.joined', data: user });
};
        
hub.client.onMessageReceived = function(message) {
    console.log('message received:', arguments);
    chat.addMessage({ type: 'user.message', data: message });
};
        
hub.client.onUserLeft = function(user) {
    chat.addMessage({ type: 'user.left', data: user });
};
        
hub.client.onUserListReceived = function(users) {
    console.log('received user list:', users);
    var avatars = $('#avatars');
    avatars.empty();
    users.forEach(function(user) {
        var avatar = $($('#avatar-tmpl').html());
        avatar.attr({ title: user.username, src: user.picture });
        avatars.append(avatar);
    });
};

$.connection.hub.start().done(function() {
    hub.server.joinRoom(userId, roomId).fail(function(e) {
        console.error(e);
    });
            
    $('#message-text').keypress(function(e) {
        if (e.which != 13) return;
        var text = $(this).val();
        $(this).val('');
        hub.server.sendMessage(userId, roomId, text).fail(function(e) {
            console.error(e);
        });
    });
    
    $("#avatars").tooltip({
        selector: '[data-toggle="tooltip"]'
    });
    
    if (isHost) {
        $(window).on('beforeunload', function() {
            return 'If you leave this page your room will be destroyed';
        })
    }
});