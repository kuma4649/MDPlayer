/*
 * This file is part of libsidplayfp, a SID player engine.
 *
 *  Copyright (C) 2011-2015 Leandro Nini
 *  Copyright (C) 2009 Antti S. Lankila
 *  Copyright (C) 2001 Simon White
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Driver.libsidplayfp
{
    /**
     * C64 system runs actions at system clock high and low
     * states. The PHI1 corresponds to the auxiliary chip activity
     * and PHI2 to CPU activity. For any clock, PHI1s are before
     * PHI2s.
     */
    public enum event_phase_t : int
    {
        EVENT_CLOCK_PHI1 = 0,
        EVENT_CLOCK_PHI2 = 1
    }

    /**
     * Fast EventScheduler, which maintains a linked list of Events.
     * This scheduler takes neglible time even when it is used to
     * schedule events for nearly every clock.
     *
     * Events occur on an internal clock which is 2x the visible clock.
     * The visible clock is divided to two phases called phi1 and phi2.
     *
     * The phi1 clocks are used by VIC and CIA chips, phi2 clocks by CPU.
     *
     * Scheduling an event for a phi1 clock when system is in phi2 causes the
     * event to be moved to the next phi1 cycle. Correspondingly, requesting
     * a phi1 time when system is in phi2 returns the value of the next phi1.
     */
    public class EventScheduler
    {



        //# include "Event.h"
        //# include "sidcxx11.h"


        /// The first event of the chain.
        private IEvent firstEvent;

        /// EventScheduler's current clock.
        private Int64 currentTime;

        /**
         * Scan the event queue and schedule event for execution.
         *
         * @param event The event to add
         */
        private void schedule(IEvent event_)
        {

            //Console.WriteLine("schedule:" + event_.GetM_name());
            //IEvent s = firstEvent;
            //int cnt = 0;
            //while (s!=null)
            //{
            //    s = s.GetM_next();
            //    cnt++;
            //}
            //Console.WriteLine("task counter:" + cnt);

            // find the right spot where to tuck this new event
            IEvent scan = firstEvent;
            IEvent bscan = null;
            for (;;)
            {
                //if (*scan == nullptr || (*scan)->triggerTime > event.triggerTime)
                //{
                //    event.next = *scan;
                //    *scan = &event;
                //    break;
                //}
                //scan = &((* scan)->next);

                if (scan == null)
                {
                    if (bscan == null)
                    {
                        firstEvent = event_;
                    }
                    else
                    {
                        bscan.SetM_next(event_);
                    }
                    event_.SetM_next(null);
                    break;
                }
                else if (scan.GetTriggerTime() > event_.GetTriggerTime())
                {
                    event_.SetM_next(scan);
                    if (bscan == null)
                    {
                        firstEvent = event_;
                    }
                    else
                    {
                        bscan.SetM_next(event_);
                    }
                    break;
                }

                bscan = scan;
                scan = scan.GetM_next();

            }
        }

        public EventScheduler()
        {
            firstEvent = null;
            currentTime = 0;
        }

        /**
         * Add event to pending queue.
         *
         * At PHI2, specify cycles=0 and Phase=PHI1 to fire on the very next PHI1.
         *
         * @param event the event to add
         * @param cycles how many cycles from now to fire
         * @param phase the phase when to fire the event
         */
        public void schedule(IEvent event_, UInt32 cycles, event_phase_t phase)
        {
            // this strange formulation always selects the next available slot regardless of specified phase.
            event_.SetTriggerTime(currentTime + ((currentTime & 1) ^ (int)phase) + (cycles << 1));
            schedule(event_);
        }

        /**
         * Add event to pending queue in the same phase as current event.
         *
         * @param event the event to add
         * @param cycles how many cycles from now to fire
         */
        public void schedule(Event event_, UInt32 cycles)
        {
            event_.SetTriggerTime(currentTime + (cycles << 1));
            schedule(event_);
        }

        /**
         * Cancel event if pending.
         *
         * @param event the event to cancel
         */
        public void cancel(Event event_)
        {
            //Console.WriteLine("cancel:" + event_.GetM_name());

            IEvent scan = firstEvent;
            IEvent bscan = null;

            while (scan != null)
            {
                if (event_ == scan)
                {
                    if (bscan != null)
                    {
                        bscan.SetM_next(scan.GetM_next());
                    }
                    else
                    {
                        firstEvent = scan.GetM_next();
                    }
                    //scan.SetM_next(null);
                    break;
                }
                bscan = scan;
                scan = scan.GetM_next();
            }
        }

        /**
         * Cancel all pending events and reset time.
         */
        public void reset()
        {
            //Console.WriteLine("reset:" );
            firstEvent = null;
            currentTime = 0;
        }

        /**
         * Fire next event, advance system time to that event.
         * イベントをリストの最初からひとつ切り出し、それを実行
         */
        public void clock()
        {
            //Console.WriteLine("clock:" );
            if (firstEvent == null) return;
            IEvent event_ = firstEvent;
            firstEvent = firstEvent.GetM_next(); //次のイベントが最初になる
            currentTime = event_.GetTriggerTime();
            //MDPlayer.log.Write(string.Format("{0} {1}",currentTime,event_.GetM_name()));

            event_.event_();
        }

        /**
         * Check if an event is in the queue.
         *
         * @param event the event
         * @return true when pending
         */
        public bool isPending(Event event_)
        {
            //Console.WriteLine("isPending:" + event_.GetM_name());
            IEvent scan = firstEvent;
            while (scan != null)
            {
                if (event_ == scan)
                {
                    return true;
                }
                scan = scan.GetM_next();
            }
            return false;
        }

        /**
         * Get time with respect to a specific clock phase.
         *
         * @param phase the phase
         * @return the time according to specified phase.
         */
        public Int64 getTime(event_phase_t phase)
        {
            return (currentTime + ((Int64)phase ^ 1)) >> 1;
        }

        /**
         * Get clocks since specified clock in given phase.
         *
         * @param clock the time to compare to
         * @param phase the phase to comapre to
         * @return the time between specified clock and now
         */
        public Int64 getTime(Int64 clock, event_phase_t phase)
        {
            return getTime(phase) - clock;
        }

        /**
         * Return current clock phase.
         *
         * @return The current phase
         */
        public event_phase_t phase()
        {
            //return static_cast<event_phase_t>(currentTime & 1);
            return (event_phase_t)(currentTime & 1);
        }





    }
}