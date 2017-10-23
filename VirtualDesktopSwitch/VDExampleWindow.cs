using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VirtualDesktopSwitch
{
    /// <summary>
    /// Example form
    /// </summary>
    public partial class VDExampleWindow : Form
    {
        public VDExampleWindow()
        {
            InitializeComponent();
        }
        private VirtualDesktopManager vdm;
        private void VDExampleWindow_Load(object sender, EventArgs e)
        {
            //Create IVirtualDesktopManager on load
            vdm = new VirtualDesktopManager();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            //Show details on click
            MessageBox.Show("Virtual Desktop ID: " + vdm.GetWindowDesktopId(Handle).ToString("X") + Environment.NewLine +
                "IsCurrentVirtualDesktop: " + vdm.IsWindowOnCurrentVirtualDesktop(Handle).ToString()
                );
        }
        //Timer tick to check if the window is on the current virtual desktop and change it otherwise
        //A timer does not have to be used, but something has to trigger the check
        //If the window was active before the vd change, it would trigger 
        //the deactivated and lost focus events when the vd changes
        //The timer always gets triggered which makes the example hopefully less confusing
        private void VDCheckTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Trace.WriteLine("IsVisible:\t" + Visible.ToString());
                if (!vdm.IsWindowOnCurrentVirtualDesktop(Handle))
                {
                    using (NewWindow nw = new NewWindow())
                    {
                        nw.Show(null);
                        vdm.MoveWindowToDesktop(Handle, vdm.GetWindowDesktopId(nw.Handle));
                    }
                }
            }
            catch
            {
                //This will fail due to race conditions as currently written on occassion
            }
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.VDCheckTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(1112, 368);
            this.label1.TabIndex = 0;
            this.label1.Text = "Example Contents";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // VDCheckTimer
            // 
            this.VDCheckTimer.Enabled = true;
            this.VDCheckTimer.Interval = 1000;
            this.VDCheckTimer.Tick += new System.EventHandler(this.VDCheckTimer_Tick);
            // 
            // VDExampleWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1112, 368);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "VDExampleWindow";
            this.Text = "VD Example";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.VDExampleWindow_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer VDCheckTimer;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new VDExampleWindow());
        }
    }
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("a5cd92ff-29be-454c-8d04-d82879fb3f1b")]
    [System.Security.SuppressUnmanagedCodeSecurity]
    public interface IVirtualDesktopManager
    {
        [PreserveSig]
        int IsWindowOnCurrentVirtualDesktop(
            [In] IntPtr TopLevelWindow,
            [Out] out int OnCurrentDesktop
            );
        [PreserveSig]
        int GetWindowDesktopId(
            [In] IntPtr TopLevelWindow,
            [Out] out Guid CurrentDesktop
            );

        [PreserveSig]
        int MoveWindowToDesktop(
            [In] IntPtr TopLevelWindow,
            [MarshalAs(UnmanagedType.LPStruct)]
            [In]Guid CurrentDesktop
            );
    }

    public class NewWindow : Form
    {
    }
    [ComImport, Guid("aa509086-5ca9-4c25-8f95-589d3c07b48a")]
    public class CVirtualDesktopManager
    {

    }
    public class VirtualDesktopManager
    {
        public VirtualDesktopManager()
        {
            cmanager = new CVirtualDesktopManager();
            manager = (IVirtualDesktopManager)cmanager;
        }
        ~VirtualDesktopManager()
        {
            manager = null;
            cmanager = null;
        }
        private CVirtualDesktopManager cmanager = null;
        private IVirtualDesktopManager manager;

        public bool IsWindowOnCurrentVirtualDesktop(IntPtr TopLevelWindow)
        {
            int result;
            int hr;
            if ((hr = manager.IsWindowOnCurrentVirtualDesktop(TopLevelWindow, out result)) != 0)
            {
                Marshal.ThrowExceptionForHR(hr);
            }
            return result != 0;
        }

        public Guid GetWindowDesktopId(IntPtr TopLevelWindow)
        {
            Guid result;
            int hr;
            if ((hr = manager.GetWindowDesktopId(TopLevelWindow, out result)) != 0)
            {
                Marshal.ThrowExceptionForHR(hr);
            }
            return result;
        }

        public void MoveWindowToDesktop(IntPtr TopLevelWindow, Guid CurrentDesktop)
        {
            int hr;
            if ((hr = manager.MoveWindowToDesktop(TopLevelWindow, CurrentDesktop)) != 0)
            {
                Marshal.ThrowExceptionForHR(hr);
            }
        }
    }
}
