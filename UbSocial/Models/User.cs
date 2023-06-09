﻿using UbSocial.Models.Helpers;

namespace UbSocial.Models
{
    public class User
    {
        private string? _email;
        private string? _password;
        private string? _name;
        private string? _surname;
        private bool? _admin;
        private int? _id;
        private int? _numDownloadable;
        private bool? _canDownloable;
        private int? _countFilesUploaded;  

        public User() {
        }
        public string? Email { get => _email; set => _email = value; }
        public string? Password { get => _password; set => _password = value; }
        public string? Name { get => _name; set => _name = value; }
        public string? Surname { get => _surname; set => _surname = value; }
        public int? Id { get => _id; set => _id = value; }
        public int? NumDownloadable { get => _numDownloadable; set => _numDownloadable = value; }
        public bool? CanDownloable { get => _canDownloable; set => _canDownloable = value; }
        public int? CountFilesUploaded { get => _countFilesUploaded; set => _countFilesUploaded = value; }
        public bool? Admin { get => _admin; set => _admin = value; }
    }
}
