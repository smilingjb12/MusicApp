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
        } else if (type == 'song.changed') {
            $.extend(data, { text: 'Now playing ' + data.song, type: 'misc' });
        } else if (type == 'song.voteskip') {
            $.extend(data, { text: data.user + ' voted to skip current song', type: 'misc' });
        } else if (type == 'song.skip') {
            $.extend(data, { text: 'Skipping current song...', type: 'misc' });
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
    self.currentSong = ko.observable(new PlaylistSongViewModel({}));
    self.message = ko.observable('');
    self.canVoteSkip = ko.observable(true);
    self.skipVotes = ko.observable(0);

    self.friendSearchString = ko.observable('');
    self.searchedFriends = ko.observableArray([]);

    self.songSearchString = ko.observable('');
    self.searchedSongs = ko.observableArray([]);

    self.setCurrentSong = function (song) {
        console.log('setting current song at server to be:', song.FullTitleDisplay());
        hub.server.setCurrentSongIndex(window.userId, window.roomId, song.Index());
    };

    self.playNextSong = function () {
        console.log('playNextSong()');
        var currSongIndex = app.playlist.indexOf(app.currentSong());
        var nextSongIndex = currSongIndex + 1;
        if (nextSongIndex == app.playlist().length) {
            nextSongIndex = 0;
        }
        console.log('next song index:', nextSongIndex);
        var nextSong = app.playlist()[nextSongIndex];
        console.log('nextSong:', nextSong);
        self.setCurrentSong(nextSong); // server callback will start playback automatically
    };

    self.voteSkipSong = function () {
        console.log('sending voteSkipSong message to server');
        hub.server.voteSkipSong(window.roomId, window.userId);
        self.canVoteSkip(false);
    };

    self.playSong = function (song) {
        console.log('playing AUDIO of song', song.FullTitleDisplay());
        ko.utils.arrayForEach(self.playlist(), function (s) {
            s.IsPlaying(false);
        });
        song.IsPlaying(true);
        self.currentSong(song);
        player.src = song.FilePath();
        $('#player').on('loadedmetadata', function () {
            console.log('metadata loaded => playing song');
            player.play();
        });
    }

    self.idsOfPeopleIntheRoom = function() {
        var ids = $('#avatars [data-id]').map(function() {
            return $(this).attr('data-id');
        }).map(function() { return parseInt(this); }).get();
        console.log('ids of people in the room:', ids);
        return ids;
    };

    self.notify = function(msg) {
        $.notify(msg, {
            position: 'top center',
            style: 'bootstrap',
            className: 'success'
        });
    };

    self.addSongToPlaylist = function(song) {
        console.log('adding song to playlist:', song.Id());
        hub.server.addSongToPlaylist(song.Id(), window.roomId).fail(function(e) {
            console.error(e);
        });
        self.notify(song.Title() + ' added to playlist');
    };

    self.inviteFriend = function(user) {
        console.log('inviting', user);
        var inviteUrl = '/Room/InviteUserToRoom?senderId=' + window.userId +
            '&receiverId=' + user.Id() +
            '&roomId=' + window.roomId;
        console.log('invite url:', inviteUrl);
        $.get(inviteUrl).done(function(resp) {
            console.log('response from inviting user:', resp);
        }).fail(function(e) {
            console.error(e);
        });
        self.searchedFriends.remove(user);
        self.notify('Invitation has been sent to ' + user.Login() + '(' + user.FullName() + ')');
    };

    self.searchForSongs = function() {
        console.log('searching for', self.songSearchString());
        self.songSearchString(self.songSearchString().trim());
        if (self.songSearchString().length == 0) return true;
        $.getJSON('/song/search?term=' + self.songSearchString()).done(function(songs) {
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

    self.searchForFriends = function() {
        console.log('searching for friends, term:', self.friendSearchString());
        self.friendSearchString(self.friendSearchString().trim());
        if (self.friendSearchString().length == 0) return true;
        $.getJSON('/user/SearchUsersExceptCurrent?term=' + self.friendSearchString()).done(function (friends) {
            console.log('received friends:', friends);
            self.searchedFriends.removeAll();
            var peopleInRoomIds = self.idsOfPeopleIntheRoom();
            ko.utils.arrayForEach(friends, function(friend) {
                if (!peopleInRoomIds.some(function(id) { return id == friend.Id; })) { // user is not already in this room
                    self.searchedFriends.push(new UserViewModel(friend));
                }
            });
        }).fail(function(e) {
            console.error(e);
        });
    };
    
    self.sendMessage = function() {
        if (self.message().length == 0) return;
        console.log('sending', self.message());
        hub.server.sendMessage(window.userId, window.roomId, self.message()).fail(function(e) {
            console.error(e);
        });
        self.message('');
    };

    self.showAddSongModal = function() {
        $('#add-song-modal').modal({
            backdrop: 'static'
        });
    };

    self.showInviteAFriendModal = function() {
        console.log('showing invite a friend modal');
        $('#invite-friend-modal').modal('show');
    };

    self.initialize = function() {
        $("#avatars").tooltip({
            selector: '[data-toggle="tooltip"]'
        });
        console.log('currentUserIsHost:', window.currentUserIsHost);
        if (window.currentUserIsHost) {
            $(window).on('beforeunload', function() {
                return 'If you leave this page your room will be destroyed';
            });
            document.getElementById('player').addEventListener("ended", function () {
                console.log('song ended => playing next song');
                self.playNextSong();
            });
        }
    };

    self.startSendingUpdateSongMessages = function() {
        setInterval(function () {
            console.log('updating song time:', player.currentTime);
            hub.server.updateSongTime(window.roomId, player.currentTime);
        }, 1000);
    };

    self.initialize();
}

window.app = new AppViewModel();
$.connection.hub.start().done(function() {
    ko.applyBindings(app);
    var hub = $.connection.musicRoomHub;
    
    hub.server.joinRoom(window.userId, window.roomId).fail(function(e) {
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

    onPlaylistReceived: function(data) {
        console.log('received playlist:', data.playlist,
            'currentSongIndex:', data.currentSongIndex);
        app.playlist.removeAll();
        var i = 0;
        ko.utils.arrayForEach(data.playlist, function (song) {
            var vm = new PlaylistSongViewModel(song);
            vm.Index(i);
            i++;
            app.playlist.push(vm);
        });
        if (app.playlist().length > 0) {
            app.playSong(app.playlist()[data.currentSongIndex]);
        }        
    },

    onRoomDestroyed: function() {
        window.location = '/room/destroyed/' + window.roomId;
    },
    
    onSongAddedToPlaylist: function(whoAdded, song) {
        console.log('onSongAddedToPlaylist(), song:', song, ', who added:', whoAdded);
        var songViewModel = new PlaylistSongViewModel(song);
        songViewModel.Index(app.playlist().length);
        window.song = song;
        window.vm = songViewModel;
        console.log('songViewModel:', songViewModel);
        app.playlist.push(songViewModel);
        if (app.playlist().length == 1) { // this is the first song added
            app.setCurrentSong(songViewModel);
        }
        chat.addMessage({ type: 'song.added', data: { username: whoAdded, song: songViewModel.FullTitleDisplay() } });
    },

    onCurrentSongChanged: function (data) {
        app.canVoteSkip(true);
        console.log('onCurrentSongChanged: whoChanged:', data.whoChanged, ', index:', data.index);
        var currentSongViewModel = app.playlist()[data.index];
        console.log('current song:', currentSongViewModel);
        app.playSong(currentSongViewModel);
        chat.addMessage({
            type: 'song.changed',
            data: { user: data.whoChanged, song: currentSongViewModel.TitleDisplay() }
        });
        if (window.currentUserIsHost) {
            console.log('starting to send update song time messages');
            app.startSendingUpdateSongMessages();
        }
    },

    onCurrentSongTimeReceived: function (data) {
        if (window.currentUserIsHost) { // host always has actual song time
            return;
        }
        console.log('onCurrentSongTimeReceived()');
        $('#player').on('loadedmetadata', function () {
            console.log('metadata loaded => sending request to get current time');
            $.get('/Room/CurrentSongTime?roomId=' + window.roomId).done(function (time) {
                console.log('received current time:', time);
                player.currentTime = time;
            });
        });
    },

    onSkipVoteReceived: function (data) {
        console.log('onSkipVoteReceived => data:', data);
        chat.addMessage({
            type: 'song.voteskip',
            data: { user: data.voter }
        });
    },

    onSkipVotesCountReceived: function(data) {
        console.log('received skip votes count:', data.count);
        app.skipVotes(data.count);
        if (window.currentUserIsHost && data.count >= app.idsOfPeopleIntheRoom().length) {
            console.log('vote limit exceeded => playing next song');
            chat.addMessage({
                type: 'song.skip',
            });
            app.playNextSong();
        }
    }
});