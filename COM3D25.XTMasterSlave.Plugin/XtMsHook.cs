using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CM3D2.XtMasterSlave.Plugin
{
    class Hook
    {
        static XtMasterSlave xtms;

        public static void hookPreAutoTwist(TBody tbody)
        {
            if (!xtms)
                xtms = GameObject.FindObjectOfType<XtMasterSlave>();

            if (xtms)
                xtms.preTBodyAutoTwist(tbody);
        }

        /*public static void hookBodyPreLateUpdate(TBody body)
        {
            if (!xtms)
                xtms = GameObject.FindObjectOfType<XtMasterSlave>();

            if (xtms)
                xtms.preBodyLateUpdate(body);
        }*/
        /*
        public static void hookIKPreLateUpdate(object ikctrl)
        {
            if (!xtms)
                xtms = GameObject.FindObjectOfType<XtMasterSlave>();

            if (xtms)
                xtms.postIKUpdate(ikctrl);
        }*/
    }
}
