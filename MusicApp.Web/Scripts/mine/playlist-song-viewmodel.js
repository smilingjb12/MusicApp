function PlaylistSongViewModel(json) {
    var self = this;

    self.Id = ko.observable(json.Id);
    self.Title = ko.observable(json.Title);
    self.Artist = ko.observable(json.Artist);
    self.AlbumCoverPicturePath = ko.observable(json.AlbumCoverPicturePath
        || "/Content/images/default-album-cover.gif");
    self.Duration = ko.observable(json.Duration);
    self.Index = ko.observable(json.Index);
    self.Bitrate = ko.observable(json.Bitrate);

    self.IsPlaying = ko.observable(false);

    self.DurationDisplay = ko.computed(function () {
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

    self.FullTitle = ko.computed(function () {
        return self.Artist() + ' ' + self.Title();
    });

    self.FullTitleDisplay = ko.computed(function () {
        return '"' + self.Title() + '"' + ' by ' + self.Artist();
    });

    self.ArtistDisplay = ko.computed(function () {
        return self.Artist() == null || self.Artist() == '' ? 'Unknown' : self.Artist();
    });

    self.TitleDisplay = ko.computed(function () {
        return self.Title() == null || self.Artist() == '' ? 'Unknown' : self.Title();
    });

    self.BitrateDisplay = ko.computed(function () {
        return self.Bitrate() + ' kbps';
    });

    self.DownloadUrl = ko.computed(function () {
        return '/song/download/' + self.Id();
    });
}