function AppViewModel() {
    var self = this;

    self.song = ko.observable(new SongViewModel({}));
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
            return song.FullTitle().match(search);
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
                self.songs.push(new SongViewModel(this.uploadedSong));
                setTimeout(function() { // emulating processing
                    $('#upload-modal').modal('hide');
                    self.resetUploadState();
                    ko.applyBindings(self, $('#upload-modal')[0]);
                    self.editSong(self.song());
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
        });
    };
    
    self.deleteSong = function() {
        $.post('/song/delete', { id: self.song().Id }).done(function(resp) {
            self.songs.remove(self.song());
        });
    };

    self.fetchSongs = function() {
        $.get('/user/uploadedsongs').done(function(songs) {
            console.log('fetched songs:', songs);
            ko.utils.arrayForEach(songs, function(song) {
                self.songs.push(new SongViewModel(song));
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
            self.song().Tags([]);
            if (tags.length != 1 || tags[0] != '') { // has any tags
                ko.utils.arrayForEach(tags, function(tag) {
                    self.song().Tags.push({ id: 0, name: tag });
                });
            }
            self.updateSong();
        });
    };
    
    self.editSong = function(song) {
        console.log('editing song:', song);
        self.song(song);
        var tagInput = $('#song-tags');
        tagInput.tagsinput();
        ko.utils.arrayForEach(self.song().Tags(), function(tag) {
            tagInput.tagsinput('add', tag.name);
        });
        console.log('showing edit song modal');
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

ko.applyBindings(new AppViewModel());