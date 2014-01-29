function SongUploader(file) {
    this.file = file;
}

SongUploader.prototype = {
    UPLOAD_URL: '/song/upload',

    upload: function(callback) {
        console.log('upload()');
        var self = this;
        var xhr = new XMLHttpRequest();
        xhr.upload.addEventListener('progress', function(e) {
            console.log('progress changed');
            var percent = 100 * e.loaded / e.total;
            $('#upload-progress').css('width', percent + '%');
        });
        xhr.onreadystatechange = function(e) {
            console.log('onreadystatechange e: ', e);
            if (e.target.readyState != 4) return;
            if (e.target.status != 200) {
                console.error(e);
                return;
            }
            self.uploadedSong = e.target.response;
            callback.call(self);
        };
        xhr.responseType = 'json';
        xhr.open('POST', this.UPLOAD_URL);
        xhr.send(this.file);
        console.log('sent file');
    }
};