using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagedNite;
using Microsoft.Xna.Framework;
using System.Threading;

namespace DDW.Kinect.Device
{
    public class KinectDevice
    {
        XnMOpenNIContext context;
        XnMSessionManager session;

        XnMFlowRouter flowRouter;
        XnMPointDenoiser pointDenoiser;
        XnMSelectableSlider2D slider;

        Rectangle bounds;

        public SessionState state = SessionState.Starting;
        public int currentX;
        public int currentY;

        public KinectDevice(Rectangle bounds)
        {
            this.bounds = bounds;
        }

        public void Init()
        {
            try
            {
                state = SessionState.Initialized;
                context = new XnMOpenNIContext();
                context.Init();

                session = new XnMSessionManager(context, "Click,Wave", "RaiseHand");
                session.SessionStarted += new EventHandler<PointEventArgs>(session_SessionStarted);
                session.FocusStartDetected += new EventHandler<FocusStartEventArgs>(session_FocusStartDetected);

                slider = new XnMSelectableSlider2D(bounds.Width, bounds.Height);
                slider.Deactivate += new EventHandler(slider_Deactivate);
                slider.ItemHovered += new EventHandler<SelectableSlider2DHoverEventArgs>(slider_ItemHovered);

                pointDenoiser = new XnMPointDenoiser();
                pointDenoiser.AddListener(slider);

                flowRouter = new XnMFlowRouter();
                flowRouter.SetActiveControl(pointDenoiser);

                session.AddListener(flowRouter);
            }
            catch (XnMException)
            {
                state = SessionState.Starting;
            }
        }

        public void Start()
        {
            Thread th = new Thread(new ThreadStart(ThreadUpdate));
            th.Name = "KinectDeviceThread";
            th.Priority = ThreadPriority.Lowest;
            th.Start();
        }

        public void Close()
        {
            state = SessionState.Ended;
        }

        void session_SessionStarted(object sender, PointEventArgs e)
        {
            state = SessionState.Started;
        }

        void session_FocusStartDetected(object sender, FocusStartEventArgs e)
        {
            state = SessionState.Refocus;
        } 
        
        void slider_Deactivate(object sender, EventArgs e)
        {
            state = SessionState.Ending;
        }

        void slider_ItemHovered(object sender, SelectableSlider2DHoverEventArgs e)
        {
            currentX = e.X;
            currentY = bounds.Height - e.Y;
        }

        private void ThreadUpdate()
        {
            while (state == SessionState.Initialized || state == SessionState.Started || state == SessionState.Refocus)
            {
                uint updateResult = context.Update();
                if (updateResult == 0)
                {
                    session.Update(context);
                }
            }

            if (state == SessionState.Ending)
            {
                state = SessionState.Ended;
                Dispose();
            }
        }

        public void Dispose()
        {
            if (context != null)
            {
                context.Close();
                context.Dispose();
            }

            if (slider != null)
            {
                slider.Dispose();
            }

            if (session != null)
            {
                session.ClearQueue();
                session.EndSession();
                session.Dispose();
            }
        }
    }

    public enum SessionState
    {
        Starting,
        Initialized,
        Started,
        Refocus,
        Ending,
        Ended,
    }
}
