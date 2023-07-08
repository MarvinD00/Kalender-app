using Newtonsoft.Json;
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
using static System.Net.Mime.MediaTypeNames;
using System.Data.Common;
using System.Xml;
using System.Reflection.Emit;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

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
            tableLayoutPanelInit();
            comboBoxInit();
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

        private void tableLayoutPanelInit()
        {
            int cellnumber = 1;
            
            //add a panel to each cell of tablelayout
            for (int row = 0; row < tableLayoutPanel1.RowCount; row++)
            {
                for (int column = 0; column < tableLayoutPanel1.ColumnCount; column++)
                {
                    Panel panel = new Panel();

                    tableLayoutPanel1.Controls.Add(panel, column, row);
                }
            }
            
            //get selected month and first Day & last day of month
            DateTime dateNow = DateTime.Now;
            int firstDayofMonth = (int)dateNow.DayOfWeek;
            if (firstDayofMonth == 0)
            {
                firstDayofMonth = 7;
            }
            int lastDayOfMonth = DateTime.DaysInMonth(dateNow.Year, dateNow.Month);

            //write number of day in each cell
            for (int row = 0; row < tableLayoutPanel1.RowCount; row++)
            {
                for (int column = 0; column < tableLayoutPanel1.ColumnCount; column++)
                {
                    Control control = tableLayoutPanel1.GetControlFromPosition(column, row);
                    System.Windows.Forms.Label label = new System.Windows.Forms.Label();
                    if ((row == 0 && column >= firstDayofMonth - 1) || row > 0 && (cellnumber <= lastDayOfMonth))
                    {
                        
                        label.Text = cellnumber.ToString();
                        cellnumber++;
                    }
                    else
                    {
                        label.Text = "";
                    }
                    control.Controls.Add(label);
                }
            }
        }

        private void tableLayoutPanelChange()
        {
            int cellnumber = 1;

            
            //get selected month and first Day & last day of month
            DateTime dateNow = DateTime.Now;
            int year;
            int month;
            if(comboBox1.SelectedIndex == -1)
            {
                year = dateNow.Year;
            } else
            {
                year = dateNow.Year + comboBox1.SelectedIndex;
            }
            if (comboBox2.SelectedIndex == -1)
            {
                month = dateNow.Month;
            }else
            {
                month = comboBox2.SelectedIndex+1;
            }

            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            int firstDayofMonth = (int)firstDayOfMonth.DayOfWeek;
            if (firstDayofMonth == 0)
            {
                firstDayofMonth = 7;
            }
            int lastDayOfMonth = DateTime.DaysInMonth(dateNow.Year, month);

            //write number of day in each cell
            for (int row = 0; row < tableLayoutPanel1.RowCount; row++)
            {
                for (int column = 0; column < tableLayoutPanel1.ColumnCount; column++)
                {
                    Control control = tableLayoutPanel1.GetControlFromPosition(column, row);
                    System.Windows.Forms.Label label = control.Controls.OfType<System.Windows.Forms.Label>().FirstOrDefault();
                    if ((row == 0 && column >= firstDayofMonth - 1) || (row > 0 && cellnumber <= lastDayOfMonth))
                    {
                        label.Text = cellnumber.ToString();

                        cellnumber++;
                    }
                    else
                    {
                        label.Text = "";
                    }
                }
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

        private void comboBoxInit()
        {
            int currentYear = DateTime.Now.Year;
            for (int year = currentYear; year < currentYear+3; year++)
            {
                comboBox1.Items.Add(year);
            }
        }

        private void Form2_Deactivate(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();    
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            tableLayoutPanelChange();
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            tableLayoutPanelChange();
        }
    }
}
