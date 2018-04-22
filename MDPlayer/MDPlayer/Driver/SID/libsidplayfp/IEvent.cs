using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Driver.libsidplayfp
{
    public interface IEvent
    {
        void SetM_next(IEvent val);
        IEvent GetM_next();
        void SetTriggerTime(Int64 val);
        Int64 GetTriggerTime();
        void SetM_name(string val);
        string GetM_name();

        void event_();
    }
}
