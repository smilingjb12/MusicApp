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
    var type = params.type;
    var data = params.data;
    data = $.extend(data, { timestamp: utils.generateTimestamp() });
    var messageTmpl = $('#chat-msg-tmpl').html();
    
    if (type == 'user.joined') {
        $.extend(data, { text: ' joined the room', type: 'joined'});
    } else if (type == 'user.message') {
        $.extend(data, { type: 'message' });
    } else if (type == 'user.left') {
        $.extend(data, { text: ' left the room', type: 'left' });
    }
    var rendered = Mustache.render(messageTmpl, data);
    $('#chat').append(rendered)
        .scrollTop($('#chat')[0].scrollHeight);
};

$.extend($.connection.musicRoomHub.client, {
    onUserJoined: function(user) {
        console.log('user joined:', user);
        chat.addMessage({ type: 'user.joined', data: user });
    },
    
    onMessageReceived: function(message) {
        console.log('message received:', arguments);
        chat.addMessage({ type: 'user.message', data: message });
    },
    
    onUserLeft: function(user) {
        chat.addMessage({ type: 'user.left', data: user });
    },
    
    onUserListReceived: function(users) {
        console.log('received user list:', users);
        var tmpl = $('#avatars-tmpl').html();
        var rendered = Mustache.render(tmpl, { avatars: users });
        $('#avatars').html(rendered);
    },
    
    onRoomDestroyed: function() {
        window.location = '/room/destroyed/' + roomId;
    },
    
    onPlaylistReceived: function(playlist) {
        console.log('received playlist: ', playlist);
    }
});

$.connection.hub.start().done(function() {
    var hub = $.connection.musicRoomHub;
    hub.server.joinRoom(userId, roomId).fail(function(e) {
        console.error(e);
    });
            
    $('#message-text').keypress(function(e) {
        if (e.which != 13) return; // if not ENTER
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
        });
    }
});