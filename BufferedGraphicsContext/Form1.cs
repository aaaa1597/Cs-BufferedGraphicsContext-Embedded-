using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BufferedGraphicsContext
{
    public partial class Form1 : Form
    {
        private ImagingSolution.Drawing.DoubleBuffer myDoubleBuffer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // DoubleBufferオブジェクトの作成
            myDoubleBuffer = new ImagingSolution.Drawing.DoubleBuffer(pictureBox1);

            Task.Run(async () => {
                await Task.Delay(100);
                /************ 遅延実行させたい処理 ************/
                this.Invoke((MethodInvoker)(() =>
                {
                    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                    sw.Start();

                    /* 拡縮率を求める */
                    mScale = pictureBox1.Height / (double)mOrgBitmap.Height;
                    /* 左上オフセットを求める */
                    mLeftTopScaleOffset = new Point(-(((int)(mOrgBitmap.Width * mScale) - pictureBox1.Width) / 2), -(((int)(mOrgBitmap.Height * mScale) - pictureBox1.Height) / 2));

                    // Graphics描画
                    Graphics g = myDoubleBuffer.Graphics;
                    g.Clear(pictureBox1.BackColor);
                    g.DrawImage(mOrgBitmap, mLeftTopScaleOffset.X + mLeftTopMoveOffset.X, mLeftTopScaleOffset.Y + mLeftTopMoveOffset.Y, (int)(mOrgBitmap.Width * mScale), (int)(mOrgBitmap.Height * mScale));
//                  g.Dispose();

                    // 更新
                    myDoubleBuffer.Refresh();

                    sw.Stop();
                    double time = sw.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency * 1000.0;
                    System.IO.File.AppendAllText(@"../../../aaaalog.log", $"Form1_Load {DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} DrawTime =	{ time.ToString()}	[ms]\n");
                }));
            });
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            // リサイズ時は再確保
            if (myDoubleBuffer != null) myDoubleBuffer.Dispose();
            myDoubleBuffer = new ImagingSolution.Drawing.DoubleBuffer(pictureBox1);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 解放
            myDoubleBuffer.Dispose();
        }

        Bitmap mOrgBitmap = new Bitmap(@"../../../4k4k-67.png");
        double mScale = 1;
        Point mLeftTopScaleOffset = new Point();
        Point mLeftTopMoveOffset = new Point();

        private void mnuTest_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            /* 拡縮率を求める */
            mScale = pictureBox1.Height / (double)mOrgBitmap.Height;
            /* 左上オフセットを求める */
            mLeftTopScaleOffset = new Point(-(((int)(mOrgBitmap.Width * mScale) - pictureBox1.Width) / 2), -(((int)(mOrgBitmap.Height * mScale) - pictureBox1.Height) / 2));
            /* 左上移動オフセットを初期化 */
            mLeftTopMoveOffset.X = 0;
            mLeftTopMoveOffset.Y = 0;

            // Graphics描画
            Graphics g = myDoubleBuffer.Graphics;
            g.Clear(pictureBox1.BackColor);
            g.DrawImage(mOrgBitmap, mLeftTopScaleOffset.X + mLeftTopMoveOffset.X, mLeftTopScaleOffset.Y + mLeftTopMoveOffset.Y, (int)(mOrgBitmap.Width * mScale), (int)(mOrgBitmap.Height * mScale));
//            g.Dispose();

            // 更新
            myDoubleBuffer.Refresh();

            sw.Stop();
            double time = sw.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency * 1000.0;
            System.IO.File.AppendAllText(@"../../../aaaalog.log", $"mnuTest_Click {DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} DrawTime =	{ time.ToString()}	[ms]\n");
            MessageBox.Show("DrawTime = " + time.ToString() + "[ms]");
        }

        Point mPrevMousePoint = new Point();
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            Point deltamove = new Point(e.X - mPrevMousePoint.X, e.Y - mPrevMousePoint.Y);
            /* 左上移動オフセットを求める */
            mLeftTopMoveOffset.X += deltamove.X;
            mLeftTopMoveOffset.Y += deltamove.Y;

            // Graphics描画
            Graphics g = myDoubleBuffer.Graphics;
            g.Clear(pictureBox1.BackColor);
            g.DrawImage(mOrgBitmap, mLeftTopScaleOffset.X + mLeftTopMoveOffset.X, mLeftTopScaleOffset.Y + mLeftTopMoveOffset.Y, (int)(mOrgBitmap.Width * mScale), (int)(mOrgBitmap.Height * mScale));
//            g.Dispose();

            // 更新
            myDoubleBuffer.Refresh();

            mPrevMousePoint = new Point(e.X, e.Y);

            sw.Stop();
            double time = sw.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency * 1000.0;
            System.IO.File.AppendAllText(@"../../../aaaalog.log", $"pictureBox1_MouseMove {DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} DrawTime =	{ time.ToString()}	[ms] ");
            System.IO.File.AppendAllText(@"../../../aaaalog.log", $"deltamove={deltamove} mLeftTopMoveOffset.X={mLeftTopMoveOffset.X} mLeftTopMoveOffset.Y={mLeftTopMoveOffset.Y}\n");
        }

        const double SCALE_MIN = 0.01;

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            /* 拡縮率を求める */
            mScale += (e.Delta / 120 * 0.01);
            if (mScale < SCALE_MIN) { mScale = SCALE_MIN; }
            /* 左上拡縮オフセットを求める */
            mLeftTopScaleOffset = new Point(-(((int)(mOrgBitmap.Width * mScale) - pictureBox1.Width) / 2), -(((int)(mOrgBitmap.Height * mScale) - pictureBox1.Height) / 2));

            // Graphics描画
            Graphics g = myDoubleBuffer.Graphics;
            g.Clear(pictureBox1.BackColor);
            g.DrawImage(mOrgBitmap, mLeftTopScaleOffset.X + mLeftTopMoveOffset.X, mLeftTopScaleOffset.Y + mLeftTopMoveOffset.Y, (int)(mOrgBitmap.Width * mScale), (int)(mOrgBitmap.Height * mScale));
//            g.Dispose();

            // 更新
            myDoubleBuffer.Refresh();

            sw.Stop();
            double time = sw.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency * 1000.0;
            System.IO.File.AppendAllText(@"../../../aaaalog.log", $"pictureBox1_MouseWheel {DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss.fff")} DrawTime =	{ time.ToString()}	[ms]\n");
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) { return; }
            mPrevMousePoint = new Point(e.X, e.Y);
            return;
        }
    }
}
