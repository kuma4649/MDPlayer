/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2013 Leandro Nini <drfiemost@users.sourceforge.net>
 * Copyright 2007-2010 Antti Lankila
 * Copyright 2000-2001 Simon White
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Driver.libsidplayfp.sidplayfp
{
    /**
     * Base class for sid builders.
     */
    public class sidbuilder
    {




        //# include <set>
        //# include <string>
        //# include "sidplayfp/SidConfig.h"
        //class sidemu;
        //class EventScheduler;

        //protected typedef std::set<libsidplayfp::sidemu*> emuset_t;

        private string m_name;

        protected string m_errorBuffer;

        //protected SortedSet<libsidplayfp.sidemu> sidobjs = new SortedSet<sidemu>();
        protected List<libsidplayfp.sidemu> sidobjs = new List<sidemu>();

        protected bool m_status;

        /**
         * Utility class for setting emu parameters in builders.
         */
        //template<class Temu, typename Tparam>
        //protected class applyParameter
        //{
        //    protected Tparam m_param;
        //    //protected void (Temu::* m_method) (Tparam);
        //    public delegate void dlgM_method(Tparam a);
        //    public dlgM_method m_method;

        //    //public applyParameter(void (Temu::* method)(Tparam), Tparam param)
        //    public applyParameter(dlgM_method method, Tparam param)
        //    {
        //        m_param = param;
        //        m_method = method;
        //    }

        //    public void opeKakko(sidemu e)
        //    {
        //        ((Temu)e).m_method(m_param);
        //    }
        //}

        protected class applyParameter_LibsidplayfpReSID_bool
        {
            protected bool m_param;
            public delegate void dlgM_method(bool a);
            public dlgM_method m_method;

            public applyParameter_LibsidplayfpReSID_bool(dlgM_method method, bool param)
            {
                m_param = param;
                m_method = method;
            }

            public void opeKakko(sidemu e)
            {
                m_method(m_param);
            }
        }

        protected class applyParameter_LibsidplayfpReSID_double
        {
            protected double m_param;
            public delegate void dlgM_method(double a);
            public dlgM_method m_method;

            public applyParameter_LibsidplayfpReSID_double(dlgM_method method, double param)
            {
                m_param = param;
                m_method = method;
            }

            public void opeKakko(sidemu e)
            {
                m_method(m_param);
            }
        }

        public sidbuilder(string name)
        {
            m_name = name;
            m_errorBuffer = "N/A";
            m_status = (true);
        }

        ~sidbuilder() { }

        /**
         * The number of used devices.
         *
         * @return number of used sids, 0 if none.
         */
        public UInt32 usedDevices() { return (UInt32)sidobjs.Count; }

        /**
         * Available devices.
         *
         * @return the number of available sids, 0 = endless.
         */
        public virtual UInt32 availDevices() { return 0; }

        /**
         * Create the sid emu.
         *
         * @param sids the number of required sid emu
         * @return the number of actually created sid emus
         */
        public virtual UInt32 create(UInt32 sids) { return 0; }

        /**
         * Find a free SID of the required specs
         *
         * @param env the event context
         * @param model the required sid model
         * @return pointer to the locked sid emu
         */
        //public sidemu lock_(EventScheduler scheduler, sid_model_t model) { return null; }

        /**
         * Release this SID.
         *
         * @param device the sid emu to unlock
         */
        //public void unlock(sidemu device) { }

        /**
         * Remove all SID emulations.
         */
        //public void remove() { }

        /**
         * Get the builder's name.
         *
         * @return the name
         */
        public string name() { return m_name; }

        /**
         * Error message.
         *
         * @return string error message.
         */
        public string error() { return m_errorBuffer; }

        /**
         * Determine current state of object.
         *
         * @return true = okay, false = error
         */
        public bool getStatus() { return m_status; }

        /**
         * Get the builder's credits.
         *
         * @return credits
         */
        public virtual string credits() { return null; }

        /**
         * Toggle sid filter emulation.
         *
         * @param enable true = enable, false = disable
         */
        public virtual void filter(bool enable) { }




        /*
 * This file is part of libsidplayfp, a SID player engine.
 *
 * Copyright 2011-2015 Leandro Nini <drfiemost@users.sourceforge.net>
 * Copyright 2007-2010 Antti Lankila
 * Copyright 2000-2001 Simon White
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

        //# include "sidbuilder.h"
        //# include <algorithm>
        //# include "sidemu.h"
        //# include "sidcxx11.h"

        public sidemu lock_(EventScheduler scheduler, libsidplayfp.sidplayfp.SidConfig.sid_model_t model)
        {
            m_status = true;

            foreach (libsidplayfp.sidemu it in sidobjs)
            {
                sidemu sid = it;
                if (sid.lock_(scheduler))
                {
                    sid.model(model);
                    return sid;
                }
            }

            // Unable to locate free SID
            m_status = false;
            m_errorBuffer = name() + " ERROR: No available SIDs to lock";
            return null;
        }

        public void unlock(sidemu device)
        {
            libsidplayfp.sidemu oldSe = null;
            foreach(libsidplayfp.sidemu se in sidobjs)
            {
                if (oldSe != se)
                {
                    se.unlock();
                }
                oldSe = se;
            }
            //libsidplayfp.sidemu it = sidobjs[device];
            //if (it != sidobjs[sidobjs.Count - 1])
            //{
            //    it.unlock();
            //}
        }

        //template<class T>
        public void Delete<T>(T s) { s = default(T); }

        public void remove()
        {
            //foreach (libsidplayfp.sidemu it in sidobjs)
            //{
                //Delete<emuset_t>(it);
            //}
            //sidobjs.clear();
            sidobjs.Clear();
        }

    }
}
