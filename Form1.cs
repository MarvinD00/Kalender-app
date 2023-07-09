using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Kalender_app
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
          
            //is user registered and password correct ?
            String userName = this.textBox1.Text;
            String password = this.textBox2.Text;

            if(userExists(userName, password))
            {
                //new user object and set to LoggedInUser
                User user = new User(userName, password, Role.Admin, "Admin", "User",0);
                UserContext.LoggedInUser = user;

                //change form
                this.Hide();
                Form2 form2 = new Form2();
                form2.Show();
            } else
            {
                //show error if invalid user
                MessageBox.Show("Benutzer konnte nicht gefunden werden, bitte überprüfen sie ihre Eingaben", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private bool userExists(String userName, String password) 
        {

            string userFilePath = "UserData.json";
            List<User> userList = new List<User>();


            string loadedUserListJson = File.ReadAllText(userFilePath);

            // Deserialisiere den JSON-String und erhalte eine Liste von Benutzerobjekten
            List<User> loadedUserList = JsonConvert.DeserializeObject<List<User>>(loadedUserListJson);

            // Finde den gewünschten Benutzer anhand des Benutzernamens
            User targetUser = loadedUserList.Find(user => user.userName == userName);

            if (targetUser != null)
            {
                string check = targetUser.password;
                //is password correct?
                if (password.Equals(check))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
