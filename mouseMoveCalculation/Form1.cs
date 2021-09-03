using System;
using System.Drawing;
using System.Windows.Forms;

namespace mouseMoveCalculation
{
    public partial class Form1 : Form
    {
        Pen redP = new Pen(Color.Red, 2f);
        Pen greenP = new Pen(Color.Lime, 2f);
        Pen blueP = new Pen(Color.LightBlue, 2f);
        Pen ornageP = new Pen(Color.Orange, 2f);
        Pen yellowP = new Pen(Color.Yellow, 2f);
        Pen aquaP = new Pen(Color.Aqua, 2f);

        int[] enemyLastFrame = new int[] { 398, 112 };
        int[] enemyThisFrame = new int[] { 306, 129 };
        int[] wantedCursorPos = new int[] { 337, 177 };
        int[] center = new int[] { 250, 250 };

        bool redraw = true;
        bool ignoreClick = true;

        int trackClickNumb = 0;

        public Form1()
        {
            InitializeComponent();
            panel1.Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (redraw == false)
                return;
            panel1.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
            if (timer1.Enabled)
                (sender as Button).Text = "Stop Simulation";
            else
                (sender as Button).Text = "Start Simulation";
            redraw = true;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Pen redL = new Pen(Color.Red, (float)numericUpDown1.Value);
            Pen greenL = new Pen(Color.Lime, (float)numericUpDown1.Value);
            Pen blueL = new Pen(Color.LightBlue, (float)numericUpDown1.Value);

            float notInt = trackBar2.Value + 1;

            var predicted = predictNewPoint(enemyLastFrame, enemyThisFrame, (float)(notInt / 10 - 0.1));
            var mid = getMidPoint(predicted, wantedCursorPos);
            var sens = getPercentInPoints(wantedCursorPos, predicted,  trackBar1.Value);

            e.Graphics.Clear(Color.Black);

            if (checkBox1.Checked)
                e.Graphics.DrawLine(redL, center[0], center[1], predicted[0], predicted[1]);
            if (checkBox2.Checked)
                e.Graphics.DrawLine(greenL, center[0], center[1], sens[0], sens[1]);
            if (checkBox3.Checked)
                e.Graphics.DrawLine(blueL, center[0], center[1], wantedCursorPos[0], wantedCursorPos[1]);

            e.Graphics.DrawRectangle(redP, new Rectangle(predicted[0] - 2, predicted[1] - 2, 4, 4));
            e.Graphics.DrawRectangle(aquaP, new Rectangle(mid[0] - 2, mid[1] - 2, 4, 4));
            e.Graphics.DrawRectangle(greenP, new Rectangle(sens[0] - 2, sens[1] - 3, 4, 4));

            e.Graphics.DrawRectangle(yellowP, new Rectangle(enemyLastFrame[0] - 3, enemyLastFrame[1] - 2, 6, 6));
            e.Graphics.DrawRectangle(ornageP, new Rectangle(enemyThisFrame[0] - 3, enemyThisFrame[1] - 2, 6, 6));
            e.Graphics.DrawRectangle(blueP, new Rectangle(wantedCursorPos[0] - 2, wantedCursorPos[1] - 2, 4, 4));

            richTextBox1.Text = $"Yellow  ENLF  {enemyLastFrame[0]},{enemyLastFrame[1]}\nOrange  ENCF  {enemyThisFrame[0]},{enemyThisFrame[1]}\nRedP	PRED  {predicted[0]},{predicted[1]}\nLBlue   USER  {wantedCursorPos[0]},{wantedCursorPos[1]}\nGreen   CALC  {sens[0]},{sens[1]}\nBlue    MIDP  {mid[0]},{mid[1]}\n";

            redraw = false;
        }

        private int[] getPercentInPoints(int[] a, int[] b, int sens)
        {
            if (a.Length != 2 && b.Length != 2)
                return new int[] { 0, 0 };
            float notInt1 = (b[0] - a[0]);
            float notInt2 = (b[1] - a[1]);
            return new int[] { a[0] + (int)(notInt1 / 100 * sens), a[1] + (int)(notInt2 / 100 * sens) };
        }

        private int[] getMidPoint(int[] a, int[] b)
        {
            if (a.Length != 2 && b.Length != 2)
                return new int[] { 0, 0 };

            return new int[] { (a[0] + b[0]) / 2, (a[1] + b[1]) / 2 };
        }

        private int[] predictNewPoint(int[] a, int[] b, float sens)
        {
            if (a.Length != 2 && b.Length != 2 && sens > 0)
                return new int[] { 0, 0 };

            float notInt1 = (b[0] - a[0]);
            float notInt2 = (b[1] - a[1]);
            return new int[] { b[0] + (int)(notInt1 * sens), b[1] + (int)(notInt2 * sens) };
        }

        private void button4_Click(object sender, EventArgs e)
        {
            float notInt = trackBar2.Value + 1;
            var predicted1 = predictNewPoint(enemyLastFrame, enemyThisFrame, (float)(notInt / 10 - 0.1));
            var predicted2 = predictNewPoint(enemyLastFrame, enemyThisFrame, 1f);
            var sens = getPercentInPoints(predicted1, wantedCursorPos, trackBar1.Value);
            enemyLastFrame = enemyThisFrame;
            enemyThisFrame = predicted2;
            wantedCursorPos = sens;
            panel1.Invalidate();
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            if (ignoreClick)
                return;

            Point point = (sender as Panel).PointToClient(Cursor.Position);
            int[] set = new int[] { point.X, point.Y };
            switch (trackClickNumb)
            {
                case 0:
                    enemyLastFrame = set;
                    label4.Text = "Click to set enemyThisFrame pos";
                    break;
                case 1:
                    enemyThisFrame = set;
                    label4.Text = "Click to set playerCursorPos pos";
                    break;
                case 2:
                    wantedCursorPos = set;
                    label4.Text = "Waiting";
                    break;
            }
            panel1.Invalidate();
            trackClickNumb++;

            if (trackClickNumb == 3)
                ignoreClick = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            label4.Text = "Click to set enemyLastFrame pos";
            trackClickNumb = 0;
            ignoreClick = false;
            redraw = true;
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            redraw = true;
            label6.Text = (((float)trackBar2.Value) / 10).ToString();
            label7.Text = trackBar1.Value.ToString();
        }
    }
}
