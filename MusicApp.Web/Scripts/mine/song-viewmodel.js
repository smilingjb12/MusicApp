function SongViewModel(json) {
    window.json = json;
    var self = this;

    self.Id = ko.observable(json.Id);
    self.Title = ko.observable(json.Title);
    self.Artist = ko.observable(json.Artist);
    self.Album = ko.observable(json.Album);
    self.Likes = ko.observable(json.Likes);
    self.Tags = ko.observableArray(json.Tags);
    self.AlbumCoverPicturePath = ko.observable(json.AlbumCoverPicturePath);
    self.Duration = ko.observable(json.Duration);
    self.FileSizeInMegaBytes = ko.observable(json.FileSizeInMegaBytes);
    self.Bitrate = ko.observable(json.Bitrate);
    self.FilePath = ko.observable(json.FilePath);

    self.ArtistDisplay = ko.computed(function() {
        return self.Artist() == null || self.Artist() == '' ? 'Unknown' : self.Artist();
    });

    self.TitleDisplay = ko.computed(function() {
        return self.Title() == null || self.Artist() == '' ? 'Unknown' : self.Title();
    });

    self.FullTitle = ko.computed(function() {
        return self.Artist() + ' ' + self.Title();
    });
    
    self.FullTitleDisplay = ko.computed(function() {
        return '"' + self.Title() + '"' + ' by ' + self.Artist();
    });

    self.DurationDisplay = ko.computed(function() {
        if (!self.Duration()) return null;
        var duration = self.Duration();
        var hours = duration.Hours;
        var minutes = duration.Minutes;
        var seconds = duration.Seconds;
        if (hours != 0) {
            return hours + ':' + (minutes > 9 ? minutes : '0' + minutes) + ':' + seconds;
        } else {
            return minutes + ':' + (seconds > 9 ? seconds : '0' + seconds);
        }
    });

    self.BitrateDisplay = ko.computed(function() {
        return self.Bitrate() + ' kbps';
    });

    self.FileSizeInMegaBytesDisplay = ko.computed(function() {
        return self.FileSizeInMegaBytes() + ' MB';
    });

    self.DownloadUrl = ko.computed(function() {
        return '/song/download/' + self.Id();
    });
}