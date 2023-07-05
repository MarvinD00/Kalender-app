﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace Kalender_app
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            updateGridViewData();
            addContextMenuStrip();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            User newUser = new User(textBox1.Text, textBox2.Text, convertListboxIndexToRole(listBox1.SelectedIndex), textBox3.Text, textBox4.Text);
            newUser.saveUser();
            updateGridViewData();
            textBox1.Text = "";
            textBox2.Text = "";
            listBox1.SelectedIndex = -1;
            textBox3.Text = "";
            textBox4.Text = "";

        }

        private void updateGridViewData()
        {
            string userFilePath = "UserData.json";
            string loadedUserListJson = File.ReadAllText(userFilePath);
            List<User> loadedUserList = JsonConvert.DeserializeObject<List<User>>(loadedUserListJson);

            dataGridView1.DataSource = loadedUserList;
            dataGridView1.Columns["Password"].Visible = false;
            dataGridView1.Columns["Role"].HeaderText = "Rolle";
            dataGridView1.Columns["UserName"].HeaderText = "Benutzername";
        }

        private void addContextMenuStrip()
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();

            ToolStripMenuItem deleteRowMenuItem = new ToolStripMenuItem("Zeile löschen");
            deleteRowMenuItem.Click += DeleteRowMenuItem_Click;

            // Füge das Menüelement zum ContextMenuStrip hinzu
            contextMenu.Items.Add(deleteRowMenuItem);

            // Weise das ContextMenuStrip dem DataGridView zu
            dataGridView1.ContextMenuStrip = contextMenu;
        }

        private Role convertListboxIndexToRole(int index)
        {
            if(index == 0)
            {
                return Role.Mitarbeiter;
            } else if(index == 1)
            {
                return Role.Admin;
            } else 
            { 
                return Role.Mitarbeiter;
            }
        }

        private void DeleteRowMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string userFilePath = "UserData.json";

                string userListJson = File.ReadAllText(userFilePath);
                List<User> userList = JsonConvert.DeserializeObject<List<User>>(userListJson);

                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                string userNameToDelete = selectedRow.Cells["UserName"].Value.ToString();

                User userToDelete = userList.FirstOrDefault(u => u.userName == userNameToDelete);

                if (userToDelete != null)
                {
                    userList.Remove(userToDelete);

                    string updatedUserListJson = JsonConvert.SerializeObject(userList);

                    File.WriteAllText(userFilePath, updatedUserListJson);
                }
                updateGridViewData();
            }
        }

        private void Form2_Deactivate(object sender, EventArgs e)
        {
            Application.Exit();    
        }
    }
}
