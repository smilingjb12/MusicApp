function SongViewModel(json) {
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