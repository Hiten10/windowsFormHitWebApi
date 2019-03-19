using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        CancellationTokenSource cts;
        Stopwatch sw;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           // sw = new Stopwatch();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            resultsTextBox.Clear();

            // Instantiate the CancellationTokenSource.
            cts = new CancellationTokenSource();

            try
            {
                await AccessTheWebAsync(cts.Token);
                resultsTextBox.Text += "\r\nDownloads complete.";
            }
            catch (OperationCanceledException)
            {
                resultsTextBox.Text += "\r\nDownloads canceled.\r\n";
            }
            catch (Exception)
            {
                resultsTextBox.Text += "\r\nDownloads failed.\r\n";
            }

            cts = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel();
            }
        }

        async Task AccessTheWebAsync(CancellationToken ct)
        {
            HttpClient client = new HttpClient();

            // Make a list of web addresses.
            List<string> urlList = SetUpURLList();

            // ***Create a query that, when executed, returns a collection of tasks.
            //IEnumerable<Task<int>> downloadTasksQuery =
            //    from url in urlList select ProcessURL(url, client, ct);

            IEnumerable<Task<Tuple<int,string,string>>> downloadTasksQuery =
             from url in urlList select ProcessURL(url, client, ct);

            // ***Use ToList to execute the query and start the tasks.
            //List<Task<int>> downloadTasks = downloadTasksQuery.ToList();
            List<Task < Tuple<int, string, string> >> downloadTasks = downloadTasksQuery.ToList();

            // ***Add a loop to process the tasks one at a time until none remain.
            while (downloadTasks.Count > 0)
            {
                // Identify the first task that completes.
                // Task<int> firstFinishedTask = await Task.WhenAny(downloadTasks);
                Task < Tuple<int, string, string> > firstFinishedTask = await Task.WhenAny(downloadTasks);
                // ***Remove the selected task from the list so that you don't
                // process it more than once.
                downloadTasks.Remove(firstFinishedTask);

                // Await the completed task.
                //int length = await firstFinishedTask;
               // int responseCode = firstFinishedTask.Result.Item1;
                resultsTextBox.Text += $"\r\nLength of the download:  {firstFinishedTask.Result.Item1}";
                int rowId = dataGridView1.Rows.Add();
                DataGridViewRow row = dataGridView1.Rows[rowId];
                row.Cells[0].Value = firstFinishedTask.Result.Item2;
                row.Cells[1].Value = firstFinishedTask.Result.Item3;
                row.Cells[2].Value = firstFinishedTask.Result.Item1;
            }
        }

        private List<string> SetUpURLList()
        {
            List<string> urls = new List<string>
            {
                "https://msdn.microsoft.com",
                "https://google.co.in",
                "http://yahoo.com",
                "https://youtube.com",
                "https://msdn.microsoft.com/library/windows/apps/br211380.aspx",
                "http://codeproject.com",
                "https://msdn.microsoft.com/library/hh290136.aspx",
                "http://csharp.com",
                "https://msdn.microsoft.com/library/dd470362.aspx",
                "https://msdn.microsoft.com/library/aa578028.aspx",
                "http://scotch.io",
                "https://msdn.microsoft.com/library/ms404677.aspx",
                "http://www.cricbuzz.com",
                "https://msdn.microsoft.com/library/ff730837.aspx",
                "http://desktop-rhvibtv/reports/browse/",
                "http://localhost/vdi/index.html"
            };
            return urls;
        }

        //async Task<int> ProcessURL(string url, HttpClient client, CancellationToken ct)
        //{
        //    // GetAsync returns a Task<HttpResponseMessage>.
        //    HttpResponseMessage response = await client.GetAsync(url, ct);

        //    // Retrieve the website contents from the HttpResponseMessage.
        //    byte[] urlContents = await response.Content.ReadAsByteArrayAsync();

        //    return urlContents.Length;
        //}

        async Task<Tuple<int,string,string>> ProcessURL(string url, HttpClient client, CancellationToken ct)
        {
            sw = new Stopwatch();
            // GetAsync returns a Task<HttpResponseMessage>.
            sw.Start();
            HttpResponseMessage response = await client.GetAsync(url, ct);
            sw.Stop();
            TimeSpan timeToResponse = sw.Elapsed;
            // Retrieve the website contents from the HttpResponseMessage.
            // byte[] urlContents = await response.Content.ReadAsByteArrayAsync();

            return Tuple.Create<int, string, string>((int)response.StatusCode, url, Math.Floor(timeToResponse.TotalMilliseconds).ToString());

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            
            //if (e.RowIndex != 0)
            //{
                if (e.ColumnIndex ==1)
                {
                    int amount = Convert.ToInt32(e.Value);                    
                    // return if rowCount = 0
                    if (this.dataGridView1.Rows.Count == 0)
                        return;

                    if (amount > 700)
                        e.CellStyle.BackColor = Color.Red;
                    else
                        e.CellStyle.BackColor = Color.Green;
                }
          //  }

        }
    }
}

