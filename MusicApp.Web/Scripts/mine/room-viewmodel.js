var utils = {
    generateTimestamp: function() {
        var d = new Date();
        var hours = d.getHours();
        var minutes = d.getMinutes();
        var seconds = d.getSeconds();
        if (hours < 10) hours = '0' + hours;
        if (minutes < 10) minutes = '0' + minutes;
        if (seconds < 10) seconds = '0' + seconds;
        return '[' + hours + ':' + minutes + ':' + seconds + ']';
    }
};

var chat = {
    addMessage: function(params) {
        var type = params.type;
        var data = params.data;
        data = $.extend(data, { timestamp: utils.generateTimestamp() });
        var messageTmpl = $('#chat-msg-tmpl').html();

        if (type == 'user.joined') {
            $.extend(data, { text: ' joined the room', type: 'joined' });
        } else if (type == 'user.message') {
            $.extend(data, { type: 'message' });
        } else if (type == 'user.left') {
            $.extend(data, { text: ' left the room', type: 'left' });
        } else if (type == 'song.added') {
            $.extend(data, { text: ' added ' + data.song + ' to playlist', type: 'misc' });
        }
        var rendered = Mustache.render(messageTmpl, data);
        $('#chat').append(rendered)
            .scrollTop($('#chat')[0].scrollHeight);
    }
};

function AppViewModel() {
    var hub = $.connection.musicRoomHub;
    var self = this;

    self.playlist = ko.observableArray([]);
    self.message = ko.observable('');
    self.searchString = ko.observable('');
    self.searchedSongs = ko.observableArray([]);
    window.songs = self.searchedSongs; // TODO: REMOVE THIS

    self.addSongToPlaylist = function(song) {
        console.log('adding song to playlist:', song.Id());
        hub.server.addSongToPlaylist(song.Id(), roomId).fail(function(e) {
            console.error(e);
        });
    };

    self.search = function() {
        console.log('searching for', self.searchString());
        self.searchString(self.searchString().trim());
        if (self.searchString().length == 0) return true;
        $.getJSON('/song/search?term=' + self.searchString()).done(function(songs) {
            console.log('received songs:', songs);
            self.searchedSongs.removeAll();
            ko.utils.arrayForEach(songs, function(song) {
                self.searchedSongs.push(new SongViewModel(song));
            });
        }).fail(function(e) {
            console.error(e);
        });
        return true;
    };
    
    self.sendMessage = function() {
        if (self.message().length == 0) return;
        console.log('sending', self.message());
        hub.server.sendMessage(userId, roomId, self.message()).fail(function(e) {
            console.error(e);
        });
        self.message('');
    };

    self.showAddSongModal = function() {
        $('#add-song-modal').modal({
            backdrop: 'static'
        })
    };

    self.initialize = function() {
        $("#avatars").tooltip({
            selector: '[data-toggle="tooltip"]'
        });
        if (currentUserIsHost) {
            $(window).on('beforeunload', function() {
                return 'If you leave this page your room will be destroyed';
            });
        }
    };

    self.initialize();
}

app = new AppViewModel(); // noooooooo! a global variable! oh my god! why?!
$.connection.hub.start().done(function() {
    ko.applyBindings(app);
    var hub = $.connection.musicRoomHub;
    
    hub.server.joinRoom(userId, roomId).fail(function(e) {
        console.error(e);
    });
});

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
    
    onSongAddedToPlaylist: function(whoAdded, song) {
        console.log('onSongAddedToPlaylist(), song:', song, ', who added:', whoAdded);
        var songViewModel = new SongViewModel(song);
        window.song = song;
        window.vm = songViewModel;
        console.log('songViewModel:', songViewModel);
        app.playlist.push(songViewModel);
        chat.addMessage({ type: 'song.added', data: { username: whoAdded, song: songViewModel.FullTitleDisplay() } });
    },

    onPlaylistReceived: function(data) {
        console.log('received playlist:', data.playlist);
        app.playlist.removeAll();
        ko.utils.arrayForEach(data.playlist, function(song) {
            app.playlist.push(new SongViewModel(song));
        });
    }
});