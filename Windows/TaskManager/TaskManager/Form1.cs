using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskMananger
{
    public partial class Form1 : Form
    {
        private int counter;
        Process current = Process.GetCurrentProcess();
        List<total> tm = new List<total>();
        ProcessStartInfo startInfo = new ProcessStartInfo("iexplore.exe", "www.microsoft.com");
        Process ieProc = null;
        private Timer timer = null;
        public bool Close = false;
        public Form1()
        {
            InitializeComponent();
            ListAllRunningProcesses();

        }
        public class total
        {
            public string NAME;
            public string PRIORITY;
            public string PID;
            public string RAM;
            public string TIME;
            public total(string a, string b, string c,string d, string e)
            {
                PID = a;
                NAME = b;
                PRIORITY = c;
                RAM = d;
                TIME = e;
            }
        }
        void printProcess()
        {
            listView1.Clear();
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.GridLines = true;
            listView1.AllowColumnReorder = true;
            listView1.Columns.Add("PID", 93, HorizontalAlignment.Center);
            listView1.Columns.Add("Name", 150, HorizontalAlignment.Left);
            listView1.Columns.Add("Priority", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Ram", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Time", 150, HorizontalAlignment.Center);

            listView1.Items.Clear();

            for (int i = 0; i < tm.Count; i++)
            {
                listView1.Items.Add(tm[i].PID);
                listView1.Items[i].SubItems.Add(tm[i].NAME);
                listView1.Items[i].SubItems.Add(tm[i].PRIORITY);
                listView1.Items[i].SubItems.Add(tm[i].RAM);
                listView1.Items[i].SubItems.Add(tm[i].TIME);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            printProcess();
            InitializeTimer();
        }
        int prMemory(Process proc)
        {
            int memsize = 0; // memsize in Megabyte
            PerformanceCounter PC = new PerformanceCounter();
            PC.CategoryName = "Process";
            PC.CounterName = "Working Set - Private";
            PC.InstanceName = proc.ProcessName;
            memsize = Convert.ToInt32(PC.NextValue()) / (int)(1024);
            PC.Close();
            PC.Dispose();
            return memsize;
        }
        public void ListAllRunningProcesses()
        {
            Process[] runmngProcs = System.Diagnostics.Process.GetProcesses().Where(p =>
            {
                bool hasException = false;
                try { IntPtr x = p.Handle; }
                catch { hasException = true; }
                return !hasException;
            }).ToArray();
            //Process[] runmngProcs = Process.GetProcesses();
            // var runmngProcs = from proc in Process.GetProcesses(".") orderby proc.Id select proc;
            int i = 0;
            foreach (var p in runmngProcs)
            {
                this.tm.Add(new total(p.Id.ToString(), p.ProcessName.ToString(), p.BasePriority.ToString(), (p.WorkingSet64/ 1024).ToString(), p.PrivilegedProcessorTime.ToString()));
                i++;
            }
            label1.Text = string.Format("Выполняется {0} процесса", i.ToString());
        }

        private void InitializeTimer()
        {
            // Run this procedure in an appropriate event.
            counter = 10;
            this.timer1 = new Timer();
            timer1.Interval = 300;
            timer1.Enabled = true;
            // Hook up timer's tick event handler.
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
        }
        private void timer1_Tick(object sender, System.EventArgs e)
        {
            if (counter ==0)
            {
                // Exit loop code.
                timer1.Enabled = false;
                counter = 10;
                ListAllRunningProcesses();
                printProcess();
                InitializeTimer();
            }
            else
            {
                // Run your procedure here.
                // Increment counter.
                counter -= 1;
                label2.Text = "Обновление через: " + counter.ToString();
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                timer1.Enabled = false;
                button1.Text = "StartTimer";
            }
            else
            {
                timer1.Enabled = true;
                button1.Text = "StartTimer";
            }
        }
    }
}