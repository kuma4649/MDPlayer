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
    //template< class This >
    public sealed class EventCallback<This> : Event
    {



        //# include "Event.h"
        //# include "sidcxx11.h"


        public delegate void Callback();

        private This m_this;
        public Callback m_callback;

        public override void event_()
        {
            m_callback();
        }

        /// <summary>
        /// 注意：callbackはobject_インスタンスのメソッドをセットすること
        /// </summary>
        /// <param name="name"></param>
        /// <param name="object_"></param>
        /// <param name="callback"></param>
        public EventCallback(string name, This object_, Callback callback) : base(name)
        {
            m_this = object_;
            m_callback = callback;
        }


    }
}