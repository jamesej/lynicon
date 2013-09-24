using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Lynicon.Extensibility;

namespace Lynicon.Test.Models
{
    public class DalTrack
    {
        static readonly DalTrack instance = new DalTrack();
        public static DalTrack Instance { get { return instance; } }

        static DalTrack() { }

        public DalTrack()
        {

        }

        public void Initialise()
        {
            EventHub.Instance.RegisterEventProcessor("",
                ehd => {
                    Debug.WriteLine(">>Evt. " + ehd.EventName + " on " + ehd.Sender.GetType().FullName); return ehd.Data;
                }, "daltrack", null);
        }
    }
}