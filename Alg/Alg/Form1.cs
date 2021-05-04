using System;
using System.Windows.Forms;

namespace Alg
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            dataGridView1.RowCount = 1;
            dataGridView1[0, 0].Value = "P" + 1;
            dataGridView1.ColumnCount = 4;
        }

        QueueProcess queue;
        int G, I;

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            dataGridView1.RowCount = (int)numericUpDown1.Value;
            for (int i = 0; i < (int)numericUpDown1.Value; i++)
                dataGridView1[0, i].Value = "P" + (i+1);
            
        }

        private void fillColumn()
        {
            for (int i = 0; i < dataGridView2.ColumnCount; i++)
            {
                dataGridView2.Columns[i].HeaderCell.Value = (i + 1).ToString();
                dataGridView2.Columns[i].Width = 22;
            }
            dataGridView2.ColumnCount -= 1;
        }

        private void fillLabel()
        {
            label1.Text = "Ср. вр. ожидания " + ((float)G / dataGridView2.RowCount).ToString();
            label2.Text = "Ср. вр. исполнения " + ((float)(G + I) / dataGridView2.RowCount).ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            createQueue();
            dataGridView2.ColumnCount = 0;
            dataGridView2.RowCount = queue.length();
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
                dataGridView2.Rows[i].HeaderCell.Value = queue[i].name;
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    algFCFS();
                    break;
                case 1:
                    algRR();
                    break;
                case 2:
                    algPP();
                    break;
                case 3:
                    algNP();
                    break;
                default:
                    break;
            }
            
        }

        private void algNP()
        {
            queue.delete();
            queue.shellSort();
            int last = 0;
            while (queue.length() > 0)
            {
                fill(last);
                queue.sortNP();
                queue.delete();
                last += 1;
            }
            fillColumn();
            fillLabel();
        }

        private void algPP()
        {
            queue.delete();
            queue.shellSort();
            int last = 0;
            while (queue.length() > 0)
            {
                fill(last);
                queue.delete();
                queue.shellSort();
                queue.sortPP();
                last += 1;
            }
            fillColumn();
            fillLabel();
        }

        private void algRR()
        {
            int k = 2;
            int c = 0;
            queue.delete();
            queue.shellSort();
            int last = 0;
            while (queue.length() > 0)
            {
                fill(last);
                if (c == k)
                {
                    c = 0;
                    queue.sortRR();
                }
                c++;
                queue.delete();
                last += 1;
            }
            fillColumn();
            fillLabel();
        }

        private void algFCFS()
        {
            
            queue.delete();
            queue.shellSort();
            int last = 0;
            while (queue.length() > 0)
            {

                fill(last);
                queue.delete();
                queue.shellSort();
                last += 1;
            }
            fillColumn();
            fillLabel();
        }

        private void fill(int last)
        {
            int column = dataGridView2.ColumnCount - 1;
            dataGridView2.ColumnCount += 1;
            if (queue[0].arrive <= last)
            {
                queue[0].ready = true;
                dataGridView2[column, queue[0].number].Value = "И";
                I++;
                queue[0].perf -= 1;
                for (int i = 1; i < queue.length(); i++)
                {
                    if (queue[i].perf > 0 && queue[i].arrive <= last)
                    {
                        dataGridView2[column, queue[i].number].Value = "Г";
                        G++;
                    }
                    if (queue[i].arrive <= last + 1)
                        queue[i].ready = true;
                    else
                        queue[i].ready = false;
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    dataGridView1.ColumnCount = 3;
                    break;
                case 1:
                    dataGridView1.ColumnCount = 3;
                    break;
                case 2:
                    dataGridView1.ColumnCount = 4;
                    break;
                case 3:
                    dataGridView1.ColumnCount = 4;
                    break;
                default:
                    break;
            }
        }

        private void createQueue()
        {
            queue = new QueueProcess();
            Process p;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                p = new Process(dataGridView1[0, i].Value.ToString(), i,
                    int.Parse(dataGridView1[1, i].Value.ToString()), 
                    int.Parse(dataGridView1[2, i].Value.ToString()));
                if (dataGridView1.ColumnCount == 4)
                    p.priority = int.Parse(dataGridView1[3, i].Value.ToString());
                queue.enqueue(p);
            }
        }
    }

    public class QueueProcess
    {
        public Process[] queue;

        public QueueProcess()
        {
            queue = new Process[0];
        }

        public Process this[int index]
        {
            get
            {
                return queue[index];
            }
            set
            {
                queue[index] = value;
            }
        }

        public int length()
        {
            return queue.Length;
        }

        public Process dequeue()
        {
            Process process = queue[0];
            for (int i = 1; i < queue.Length; i++)
                queue[i - 1] = queue[i];
            Array.Resize(ref queue, queue.Length - 1);
            return process;
        }

        public void enqueue(Process p)
        {
            Array.Resize(ref queue, queue.Length + 1);
            queue[queue.Length - 1] = p;
        }

        public void swap(int i, int j)
        {
            Process p = queue[i];
            queue[i] = queue[j];
            queue[j] = p;
        }

        public void sortNP()
        {
            int min = 100;
            if (queue[0].perf == 0)
                for (int i = 1; i < queue.Length; i++)
                    if (queue[i].ready && queue[i].priority < min)
                    {
                            min = queue[i].priority;
                            swap(0, i);
                    }
            
        }

        public void sortRR()
        {
            int j = 0;
            for (int i = 0; i < queue.Length; i++)
                if (queue[i].ready)
                {
                    swap(j, i);
                    j = i;
                }
        }

        public void sortPP()
        {
            int min = 100;
            for (int i = 0; i < queue.Length; i++)
                if (queue[i].ready && queue[i].priority < min)
                {
                    min = queue[i].priority;
                    swap(0, i);
                }
        }

        public void shellSort()
        {
            int step = queue.Length / 2;
            while (step > 0)
            {
                int i, j;
                for (i = step; i < queue.Length; i++)
                {
                    Process value = queue[i];
                    for (j = i - step; (j >= 0) && (queue[j].arrive > value.arrive); j -= step)
                        queue[j + step] = queue[j];
                    queue[j + step] = value;
                }
                step /= 2;
            }
        }

        public void message()
        {
            for (int i = 0; i < queue.Length; i++)
                MessageBox.Show(queue[i].arrive.ToString());
        }

        public void delete()
        {
            for (int i = 0; i < queue.Length; i++)
                if (queue[i].perf == 0)
                {
                    for (int j = i + 1; j < queue.Length; j++)
                        queue[j - 1] = queue[j];
                    Array.Resize(ref queue, queue.Length - 1);
                }
        }
    }

    public class Process
    {
        public string name { get; set; }
        public int arrive { get; set; }
        public int perf { get; set; }
        public int number { get; set; }
        public int priority = 0;
        public bool ready = false;

        public Process(string name,int number, int arrive, int perf)
        {
            this.name = name;
            this.number = number;
            this.arrive = arrive;
            this.perf = perf;
        }

         
    }
}
