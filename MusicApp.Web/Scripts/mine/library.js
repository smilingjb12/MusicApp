function SongModel(json) {
    var self = this;

    self.id = ko.observable(json.id);
    self.title = ko.observable(json.title);
    self.artist = ko.observable(json.artist);
    self.album = ko.observable(json.album);
    self.likes = ko.observable(json.likes);
    self.tags = ko.observableArray(json.tags);
    self.albumCoverPicturePath = ko.observable(json.albumCoverPicturePath);
    self.duration = ko.observable(json.duration);
    self.fileSizeInMegaBytes = ko.observable(json.fileSizeInMegaBytes);
    self.bitrate = ko.observable(json.bitrate);
    self.filePath = ko.observable(json.filePath);

    self.artistDisplay = ko.computed(function() {
        return self.artist() == null || self.artist() == '' ? 'Unknown' : self.artist();
    });

    self.titleDisplay = ko.computed(function() {
        return self.title() == null || self.artist() == '' ? 'Unknown' : self.title();
    });
    
    self.fullTitle = ko.computed(function() {
        return self.artist() + ' ' + self.title();
    });

    self.durationDisplay = ko.computed(function() {
        if (!self.duration()) return null;
        var parts = self.duration().split(':');
        var mapped = parts.filter(function(part) {
            return parseInt(part) != 0;
        }).map(function(x) {
            return parseInt(x);
        });
        var seconds = mapped[mapped.length - 1];
        if (seconds.toString().length == 1) {
            seconds = '0' + seconds;
            mapped[mapped.length - 1] = seconds;
        }
        return mapped.join(':');
    });
    
    self.bitrateDisplay = ko.computed(function() {
        return self.bitrate() + ' kbps';
    });
    
    self.fileSizeInMegaBytesDisplay = ko.computed(function() {
        return self.fileSizeInMegaBytes() + ' MB';
    });
    
    self.downloadUrl = ko.computed(function() {
        return '/song/download/' + self.id();
    });
}

function AppModel() {
    var self = this;

    self.song = ko.observable(new SongModel({}));
    self.songs = ko.observableArray([]);
    self.filter = ko.observable('');

    window.songs = self.songs; // TODO: REMOVE
    window.song = self.song; // TODO: REMOVE

    window.filteredSongs = self.filteredSongs = ko.computed(function() {
        var filter = self.filter().trim().toLowerCase();
        console.log('filter: ', filter);
        if (!filter) return self.songs();
        return ko.utils.arrayFilter(self.songs(), function(song) {
            var search = new RegExp(filter, 'i');
            return song.fullTitle().match(search);
        });
    });

    self.resetUploadState = function() {
        $('#upload-modal').html($('#browse-song-template').html());
    };

    self.fileSelected = function() {
        var file = document.querySelector('#upload-modal [type=file]').files[0];
        $('#upload-modal .modal-body').fadeOut(function() {
            var modalBody = $(this);
            var progressBarHtml = $('#upload-song-template').html();
            modalBody.html(progressBarHtml).fadeIn();
            var songUploader = new SongUploader(file);
            songUploader.upload(function done() { // TODO: handle errors
                ko.mapping.fromJS(this.uploadedSong, {}, self.song);
                self.songs.push(new SongModel(this.uploadedSong));
                setTimeout(function() { // emulating processing
                    $('#upload-modal').modal('hide');
                    self.resetUploadState();
                    ko.applyBindings(self, $('#upload-modal')[0]);
                    $('#song-edit-modal').modal('show');
                }, 500);
            });
        });
    };

    self.updateSong = function() {
        console.log('saving song to server:', self.song());
        $.ajax({
            type: 'POST',
            url: '/song/update',
            data: ko.toJSON(self.song()),
            contentType: 'application/json',
        })
    };
    
    self.deleteSong = function() {
        $.post('/song/delete', { id: self.song().id }).done(function(resp) {
            self.songs.remove(self.song());
        });
    };

    self.fetchSongs = function() {
        $.get('/user/uploadedsongs').done(function(songs) {
            console.log('fetched songs:', songs);
            ko.utils.arrayForEach(songs, function(song) {
                self.songs.push(new SongModel(song));
            });
        });
    };
    
    self.initializeUi = function() {
        $(".song-list").tooltip({
            selector: '[data-toggle="tooltip"]'
        });
        $('#song-edit-modal').on('hide.bs.modal', function() {
            var tags = $('#song-tags').val().split(',');
            console.log('tags:', tags);
            self.song().tags([]);
            if (tags.length != 1 || tags[0] != '') { // has any tags
                ko.utils.arrayForEach(tags, function(tag) {
                    self.song().tags.push({ id: 0, name: tag });
                });
            }
            self.updateSong();
        });
    };
    
    self.editSong = function(song) {
        self.song(song);
        var tagInput = $('#song-tags');
        tagInput.tagsinput();
        ko.utils.arrayForEach(self.song().tags(), function(tag) {
            tagInput.tagsinput('add', tag.name);
        });
        $('#song-edit-modal').modal('show');
    };
    
    self.confirmDelete = function(song) {
        self.song(song);
        $('#song-delete-confirmation').modal('show');
    };
    
    self.noSongsPresent = ko.computed(function() {
        return self.songs().length == 0;
    });

    self.fetchSongs();
    self.initializeUi();
    self.resetUploadState();
}

ko.applyBindings(new AppModel());