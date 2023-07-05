using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kalender_app
{
    internal class User
    {
        String name;
        String password;
        Role role;


        public User(String name, String password, Role role) {
            this.name = name;
            this.password = password;
            this.role = role;
        }


    }
}

public enum Role
{
    Mitarbeiter,
    Arbeitgeber,
    Admin
}
