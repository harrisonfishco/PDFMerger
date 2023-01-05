using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PdfSharp.Pdf;

namespace pdfw
{
    public partial class Form1 : Form
    {

        private List<string> filenames;

        public Form1()
        {
            InitializeComponent();
            this.Text = "PDF Merger";
            filenames = new List<string>();

            typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(listView1, true, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Test");

            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.FileOk += fileDialogOk;
            fileDialog.Filter = "Portable Document Format|*.pdf";
            fileDialog.Multiselect = true;
            fileDialog.ShowDialog();
        }

        private void fileDialogOk(object sender, CancelEventArgs e)
        {
            if(!e.Cancel)
            {
                string[] files = ((OpenFileDialog)sender).FileNames;

                foreach (string file in files)
                {
                    filenames.Add(file);
                    ListViewItem item = new ListViewItem(new FileInfo(file).Name);
                    item.ToolTipText = file;
                    item.ImageIndex = 0;
                    listView1.Items.Add(item);
                }

                if(filenames.Count > 1)
                    mergeButton.Enabled = true;
            }
        }

        private string[] getItemsInOrder()
        {
            string[] ordered = new string[listView1.Items.Count];
            SortedDictionary<Tuple<int, int>, string> points = new SortedDictionary<Tuple<int, int>, string>();
            foreach(ListViewItem item in listView1.Items)
            {
                Tuple<int, int> tp = new Tuple<int, int>(item.Position.Y, item.Position.X);
                points.Add(tp, item.ToolTipText);
            }

            int i = 0;
            foreach(KeyValuePair<Tuple<int, int>, string> kvp in points)
            {
                ordered[i] = kvp.Value;
                ++i;
            }

            return ordered;
        }

        private void mergeButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileOk += saveFileOk;
            saveFileDialog.Filter = "Portal Document Format|*.pdf";
            saveFileDialog.ShowDialog();
        }

        private void saveFileOk(object sender, CancelEventArgs e)
        {
            PdfDocument[] docs = new PdfDocument[filenames.Count];
            int i = 0;

            PdfDocument doc = new PdfDocument();

            foreach (string filename in getItemsInOrder())
            {
                docs[i] = PdfSharp.Pdf.IO.PdfReader.Open(filename, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                foreach (PdfPage page in docs[i].Pages)
                {
                    doc.AddPage(page);
                }
                ++i;
            }

            doc.Save(((SaveFileDialog)sender).FileName);

            filenames.Clear();
            listView1.Clear();
            mergeButton.Enabled = false;
        }

        ListViewItem heldDownItem;
        Point heldDownPoint;

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            listView1.AutoArrange = false;
            heldDownItem = listView1.GetItemAt(e.X, e.Y);
            if(heldDownItem != null)
            {
                heldDownPoint = new Point(e.X - heldDownItem.Position.X, e.Y - heldDownItem.Position.Y);
            }
        }

        private void listView1_MouseMove(object sender, MouseEventArgs e)
        {
            if(heldDownItem != null)
            {
                heldDownItem.Position = new Point(e.Location.X - heldDownPoint.X, e.Location.Y - heldDownPoint.Y);
            }
        }

        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            heldDownItem = null;
            listView1.AutoArrange = true;
        }
    }
}
