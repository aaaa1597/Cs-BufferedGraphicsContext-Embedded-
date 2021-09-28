using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImagingSolution
{
    namespace Drawing
    {
        class DoubleBuffer : IDisposable
        {
            public Graphics Graphics
            {
                get { return mBufferedGraphics.Graphics; }
            }

            BufferedGraphics mBufferedGraphics;
            Control mControl;

            public DoubleBuffer(Control control)
            {
                mControl = control;
                this.Dispose();

                System.Drawing.BufferedGraphicsContext currentContext;
                currentContext = BufferedGraphicsManager.Current;
                mBufferedGraphics = currentContext.Allocate(control.CreateGraphics(), control.DisplayRectangle);

                mControl.Paint += new System.Windows.Forms.PaintEventHandler(this.Paint);
                return;
            }

            ~DoubleBuffer()
            {
                Dispose();
                return;
            }

            public void Dispose()
            {
                if (mBufferedGraphics != null)
                {
                    mBufferedGraphics.Dispose();
                    mBufferedGraphics = null;
                }
                mControl.Paint -= new System.Windows.Forms.PaintEventHandler(this.Paint);
                return;
            }

            public void Refresh()
            {
                if (mBufferedGraphics != null)
                    mBufferedGraphics.Render();
                return;
            }

            private void Paint(object sender, PaintEventArgs e)
            {
                Refresh();
                return;
            }
        }
    }
}
