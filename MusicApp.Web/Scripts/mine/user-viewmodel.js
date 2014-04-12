function UserViewModel(json) {
    var self = this;

    self.Id = ko.observable(json.Id);
    self.FullName = ko.observable(json.FullName);
    self.Login = ko.observable(json.Login);
    self.PictureFilePath = ko.observable(json.PictureFilePath);
   
}