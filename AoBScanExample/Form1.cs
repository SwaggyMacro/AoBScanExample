using Memory;
using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace AoBScanExample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            listView1.View = View.Details;
            listView1.Columns.Add("Id", 100);
            listView1.Columns.Add("Data", 200);
            listView1.LabelEdit = true;
            listView1.ItemActivate += ListView1_ItemActivate;
            listView1.FullRowSelect = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine(AoBScan(textBox2.Text, textBox3.Text, textBox1.Text));
        }
        public string AoBScan(string processName, string windowTitle, string scanContent)
        {
            scanContent = BitConverter.ToString(Encoding.ASCII.GetBytes(scanContent)).Replace("-", " ");
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                if (process.ProcessName.Equals(processName) && process.MainWindowTitle.Equals(windowTitle))
                {
                    Mem m = new Mem();
                    m.OpenProcess(process.Id);
                    var addrList = m.AoBScan(scanContent, true, true, true).Result;
                    foreach (var addr in addrList)
                    {
                        Console.WriteLine(addr);

                        // read addr context
                        var token = m.ReadString(addr.ToString("X"), length: 300).ToString();
                        Console.WriteLine(token);

                        // insert item to listview
                        string[] item = { (listView1.Items.Count + 1).ToString(), token };
                        listView1.Items.Add(new ListViewItem(item));

                        // it should be token if length >= 135
                        if (token.Length >= 135)
                        {
                            return token;
                        }
                    }
                }
            }
            return "false";
        }
        private void ListView1_ItemActivate(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string column2Text = listView1.SelectedItems[0].SubItems[1].Text;
                Clipboard.SetText(column2Text);
                MessageBox.Show("Copy to clipboard: " + column2Text, "Tips", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
