using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Kalender_app
{
    public class User
    {
        public String userName { get; set; }
        public String password { get; set; }
        public Role Role { get; set; }
        public String Vorname { get; set; }
        public String Nachname { get; set; }


        public User(String name, String password, Role role, String Vorname, String Nachname) {
            this.userName = name;
            this.password = password;
            this.Role = role;
            this.Vorname = Vorname;
            this.Nachname = Nachname;
        }

        public void saveUser()
        {
            string userFilePath = "UserData.json";
            
            string loadedUserListJson = File.ReadAllText(userFilePath);
            List<User> loadedUserList = JsonConvert.DeserializeObject<List<User>>(loadedUserListJson);

            
            loadedUserList.Add(this);

            string updatedUserListJson = JsonConvert.SerializeObject(loadedUserList);

            File.WriteAllText(userFilePath, updatedUserListJson);
        }

        public void removeUser()
        {
            string userFilePath = "UserData.json";

            string loadedUserListJson = File.ReadAllText(userFilePath);
            List<User> loadedUserList = JsonConvert.DeserializeObject<List<User>>(loadedUserListJson);

            loadedUserList.Remove(this);

            string updatedUserListJson = JsonConvert.SerializeObject(loadedUserList);

            File.WriteAllText(userFilePath, updatedUserListJson);
        }
    }
}

public enum Role
{
    Mitarbeiter,
    Admin
}
