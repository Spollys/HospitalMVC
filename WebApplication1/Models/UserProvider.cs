using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Hospital.Models
{
    public class UserProvider
    {
        private const string DefaultFileName = "users.txt";
        private const string DefaultDataDir = "./App_Data";
        private static string _fullName = string.Empty;
        private static readonly object _lock = new();

        private static readonly Dictionary<string, User> _users = new();

        static UserProvider()
        {
            ReadUsers();
        }

        public static void ClearUsers() => _users.Clear();

        public static bool IsAuthorizedUser(string username, string password) =>
            _users.ContainsKey(username) && _users[username].Password == password;

        public static bool HasAccount(string userName) => _users.ContainsKey(userName);

        public static bool TryAddUser(User user)
        {
            if (_users.ContainsKey(user.Name))
            {
                Trace.WriteLine($"{DateTime.Now:HH:mm:ss}: {user.Name} already exists, not added");
                return false;
            }

            try
            {
                lock (_lock)
                {
                    if (!File.Exists(_fullName))
                    {
                        File.Create(_fullName).Dispose();
                    }

                    using var writer = new StreamWriter(_fullName, append: true);
                    _users.Add(user.Name, user);
                    writer.Write($"{user}");
                }

                Trace.WriteLine($"{DateTime.Now:HH:mm:ss}: {user.Name} is saved in {_fullName}");
                return true;
            }
            catch (IOException e)
            {
                Trace.WriteLine(
                    $"{DateTime.Now.TimeOfDay}: {user.Name} saving in {_fullName} failed with exception: {e.Message}");
                return false;
            }
        }

        public static bool TryRemoveUser(string userName)
        {
            if (!_users.ContainsKey(userName))
            {
                Trace.WriteLine($"{DateTime.Now:HH:mm:ss}: {userName} not found, not removed");
                return false;
            }

            try
            {
                lock (_lock)
                {
                    if (File.Exists(_fullName))
                    {
                        _users.Remove(userName);
                        File.WriteAllLines(_fullName, _users.Values.Select(u => u.ToString()));
                    }
                }

                Trace.WriteLine($"{DateTime.Now:HH:mm:ss}: {userName} is removed from {_fullName}");
                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine($"{DateTime.Now:HH:mm:ss}: {userName} removing from {_fullName} failed: {e.Message}");
                return false;
            }
        }

        public static bool TrySaveUsers(string? dataDir = null, string? fileName = null)
        {
            dataDir ??= DefaultDataDir;
            fileName ??= DefaultFileName;
            var fullName = Path.Combine(dataDir, fileName);
            bool result = false;

            try
            {
                lock (_lock)
                {
                    if (!File.Exists(fullName))
                    {
                        File.Create(fullName).Dispose();
                    }

                    Trace.WriteLine($"{DateTime.Now:HH:mm:ss}: users save started in {fullName}");
                    using var writer = new StreamWriter(fullName, append: false);
                    foreach (var user in _users)
                    {
                        writer.WriteLine(user.Value);
                    }
                }

                result = true;
                Trace.WriteLine($"{DateTime.Now:HH:mm:ss}: users save finished. {_users.Count} users saved in {fullName}");
            }
            catch (Exception e)
            {
                Trace.WriteLine($"Users saving in {fullName} failed: exception {e.Message}");
            }

            return result;
        }

        public static void ReadUsers(string? dataDir = null, string? fileName = null, bool append = true)
        {
            dataDir ??= DefaultDataDir;
            fileName ??= DefaultFileName;
            _fullName = Path.Combine(dataDir, fileName);

            if (!File.Exists(_fullName))
            {
                Trace.WriteLine($"{DateTime.Now.TimeOfDay}: {_fullName} does not exist, not loaded");
                return;
            }

            try
            {
                var userList = DataProvider.ReadData<User>(fileName, dataDir, separator: "\t");
                ClearUsers();

                Trace.WriteLine($"{DateTime.Now:HH:mm:ss}: {userList.Count} users loaded from {_fullName}");

                lock (_lock)
                {
                    foreach (var user in userList)
                    {
                        _users.Add(user.Name, user);
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine($"{DateTime.Now:HH:mm:ss}: Error loading users from {_fullName}: {e.Message}");
            }
        }
    }
}
