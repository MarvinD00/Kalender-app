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
            User newUser = new User(textBox1.Text, textBox2.Text, convertListboxIndexToRole(listBox1.SelectedIndex), textBox3.Text, textBox4.Text, (int) numericUpDown1.Value);
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
            dataGridView1.Columns["personalNr"].HeaderText = "Personal Nr.";
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
            for (int row = 0; row < tableLayoutPanel1.RowCount; row++)
            {
                for (int column = 0; column < tableLayoutPanel1.ColumnCount; column++)
                {
                    Panel panel = new Panel();
                    tableLayoutPanel1.Controls.Add(panel, column, row);
                    Control control = tableLayoutPanel1.GetControlFromPosition(column, row);
                    System.Windows.Forms.Label label = new System.Windows.Forms.Label();
                    System.Windows.Forms.Label label2 = new System.Windows.Forms.Label();
                    label.Dock = DockStyle.Top;
                    label2.Dock = DockStyle.Bottom;
                    control.Controls.Add(label);
                    control.Controls.Add(label2);
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

            //write number of day + holiday in each cell
            for (int row = 0; row < tableLayoutPanel1.RowCount; row++)
            {
                for (int column = 0; column < tableLayoutPanel1.ColumnCount; column++)
                {
                    Control control = tableLayoutPanel1.GetControlFromPosition(column, row);
                    System.Windows.Forms.Label numLabel = control.Controls[0] as System.Windows.Forms.Label;
                    System.Windows.Forms.Label holidayLabel = control.Controls[1] as System.Windows.Forms.Label;
                    if ((row == 0 && column >= firstDayofMonth - 1) || (row > 0 && cellnumber <= lastDayOfMonth))
                    {
                        numLabel.Text = cellnumber.ToString();
                        holidayLabel.Text = getPublicHoliday(new DateTime(year, month, cellnumber));
                        cellnumber++;
                    }
                    else
                    {
                        numLabel.Text = "";
                        holidayLabel.Text = "";
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
            comboBox1.SelectedIndex = 0;
            int currentMonth = DateTime.Now.Month;
            comboBox2.SelectedIndex = currentMonth - 1;

        }

        private String getPublicHoliday(DateTime date)
        {
            List<DateTime> holidayList = new List<DateTime>();

            holidayList.Add(new DateTime(date.Year, 1, 1));
            holidayList.Add(new DateTime(date.Year, 1, 6));
            holidayList.Add(new DateTime(date.Year, 3, 8));
            holidayList.Add(new DateTime(date.Year, 5, 1));
            holidayList.Add(new DateTime(date.Year, 10, 3));
            holidayList.Add(new DateTime(date.Year, 10, 31));
            holidayList.Add(new DateTime(date.Year, 11, 1));
            holidayList.Add(new DateTime(date.Year, 12, 25));
            holidayList.Add(new DateTime(date.Year, 12, 26));
            
            if (holidayList.Contains(date))
            {
                if(date.Equals(new DateTime(date.Year, 1, 1))) 
                { 
                    return "Neujahr";
                }
                if (date.Equals(new DateTime(date.Year, 1, 6)))
                {
                    return "Heilige Drei Könige";
                }
                if (date.Equals(new DateTime(date.Year, 3, 8)))
                {
                    return "Internationaler Frauentag";
                }
                if (date.Equals(new DateTime(date.Year, 5, 1)))
                {
                    return "Tag der Arbeit";
                }
                if (date.Equals(new DateTime(date.Year, 10, 3)))
                {
                    return "Tag der dt. Einheit";
                }
                if (date.Equals(new DateTime(date.Year, 10, 31)))
                {
                    return "Reformationstag";
                }
                if (date.Equals(new DateTime(date.Year, 11, 1)))
                {
                    return "Allerheiligen";
                }
                if (date.Equals(new DateTime(date.Year, 12, 25)))
                {
                    return "1. Weihnachtsfeiertag";
                }
                if (date.Equals(new DateTime(date.Year, 12, 26)))
                {
                    return "2. Weihnachtsfeiertag";
                }

            }
            return "";
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
