using System;

namespace KinectBasic
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (KinectGame game = KinectGame.GetInstance())
            {
                game.Run();
            }
        }
    }
#endif
}

