using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Management;


namespace TaskManager
{
    public partial class Form1 : Form
    {
        private List<Process> processes = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void GetProcesses()
        {
            processes.Clear();
            processes = Process.GetProcesses().ToList<Process>();
        }
    
        private void RefresrhProccesList()
        {
            listView1.Items.Clear();

            double memSize = 0;

            foreach (Process p in processes)
            {
                memSize = 0;

                PerformanceCounter pc = new PerformanceCounter();
                pc.CategoryName = "Process";
                pc.CounterName = "Working set - Private";
                pc.InstanceName = p.ProcessName;

                //memSize = (double)pc.NextValue() / (1000 * 1000);

                string[] row = new string[] { p.ProcessName.ToString(), Math.Round(memSize, 1).ToString() };

                listView1.Items.Add(new ListViewItem(row));

                pc.Close();
                pc.Dispose();
            }
            Text = "Запущено процессов " + processes.Count.ToString();
        }
        private void RefresrhProccesList(List<Process> processes, string keyword)
        {
            listView1.Items.Clear();

            double memSize = 0;

            foreach (Process p in processes)
            {
                memSize = 0;

                PerformanceCounter pc = new PerformanceCounter();
                pc.CategoryName = "Process";
                pc.CounterName = "Working set - Private";
                pc.InstanceName = p.ProcessName;

                memSize = (double)pc.NextValue() / (1000 * 1000);

                string[] row = new string[] { p.ProcessName.ToString(), Math.Round(memSize, 1).ToString() };

                listView1.Items.Add(new ListViewItem(row));

                pc.Close();
                pc.Dispose();
            }
            Text = $"Запущено процессов '{keyword}'" + processes.Count.ToString();
        }

        private void KillProcess(Process process)
        {
            process.Kill();

            process.WaitForExit();
        }
        private void KillProcessAndChildren(int pid)
        {
            if (pid == 0)
            {
                return;
            }
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                "Select * From Win32_Process Where ParentProccesID =" + pid);
            ManagementObjectCollection objectCollection = searcher.Get();

            foreach (ManagementObject obj in objectCollection)
            {
                KillProcessAndChildren(Convert.ToInt32(obj["ProcessID"]));
            }
            try
            {
                Process p = Process.GetProcessById(pid);
                p.Kill();
                p.WaitForExit();
            }
            catch (ArgumentException) {}


        }
        private int GetParentProcessId(Process p)
        {
            int ParentId = 0;

            try
            {
                ManagementObject managementObject = new ManagementObject("win32_procces.handle='" + p.Id + "' ");
                managementObject.Get();
                ParentId = Convert.ToInt32(managementObject["ParentProcessId"]);
            }
            catch (Exception) { }

            return ParentId;
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void toolStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            processes = new List<Process>();

            GetProcesses();
            RefresrhProccesList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GetProcesses();
            RefresrhProccesList();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process proccesToKill = processes.Where((x) => x.ProcessName == listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];

                    KillProcess(proccesToKill);
                    GetProcesses();
                    RefresrhProccesList();
                }
            }
            catch (Exception) {}
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems[0] != null)
                {
                    Process proccesToKill = processes.Where((x) => x.ProcessName == listView1.SelectedItems[0].SubItems[0].Text).ToList()[0];

                    KillProcessAndChildren(GetParentProcessId(proccesToKill));
                    GetProcesses();
                    RefresrhProccesList();
                }
            }
            catch (Exception) { }
        }
    }
}
