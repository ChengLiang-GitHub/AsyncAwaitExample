using System;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox2.AppendText(DateTime.Now.ToString() + "\n");
            richTextBox2.ScrollToCaret();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            string RunLongTask(string taskName, int delayTime)
            {
                string str = taskName + DateTime.Now.ToString() + $"\tstart to delay {delayTime}s" + "\n";
                Task.Delay(delayTime).Wait();
                str  += taskName + DateTime.Now.ToString() + $"\t{delayTime}s later" + "\n";
                return str;
            }
            async void waitTaskWithAsync(Task<string> childTask)
            {

                string str = await childTask;
                richTextBox2.Invoke((Action)(() =>
                {
                    richTextBox2.AppendText(str);
                    richTextBox2.ScrollToCaret();
                }));
            }
            void waitTaskWithSync(Task<string> childTask)
            {

                string str = childTask.Result;
                richTextBox2.Invoke((Action)(() =>
                {
                    richTextBox2.AppendText(str);
                    richTextBox2.ScrollToCaret();
                }));
            }
            Task.Run(async () =>
            {

                Task<string> childTask1 = Task.Run(() => RunLongTask("Child task 1 ", 15000));
                Task<string> childTask2 = Task.Run(() => RunLongTask("Child task 2 ", 30000));
                bool onlyOnce = false;
                while (true)
                {
                    richTextBox1.Invoke((Action)(() =>
                    {

                        richTextBox1.AppendText("Parent task " + DateTime.Now.ToString() + "\n");
                        richTextBox1.ScrollToCaret();
                    }));
                    await Task.Delay(1000);
                    if (!onlyOnce)
                    {
                        waitTaskWithSync(childTask1); // While child task 1 is not finished yet, to wait child task 1 with synchronization let parent task block.
                        waitTaskWithAsync(childTask2); // While child task 2 is not finished yet. to wait child task 2 with asynchronization let parent task work smoothly. 
                        onlyOnce = true;
                    }
                }


            });
            Task.Delay(5000).Wait();
            //richTextBox2.Text += "Ui Thread \n";
            //MessageBox.Show(Task.CurrentId.ToString());
        }

        private async void button2_Click(object sender, EventArgs e)
        {

            richTextBox2.AppendText("UI thread block wait " + DateTime.Now.ToString() + "\n");
            richTextBox2.ScrollToCaret();
            Task.Delay(10000).Wait();
            richTextBox2.AppendText("UI thread non-block wait " + DateTime.Now.ToString() + "\n");
            richTextBox2.ScrollToCaret();
            await Task.Delay(10000);
            richTextBox2.AppendText("UI thread Test finish " + DateTime.Now.ToString() + "\n");
            richTextBox2.ScrollToCaret();
        }
    }
}
