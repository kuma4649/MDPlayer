//  ---------------------------------------------------------------------------
//  This file is part of reSID, a MOS6581 SID emulator engine.
//  Copyright (C) 2010  Dag Lem <resid@nimrod.no>
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//  ---------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Driver.libsidplayfp.builders.resid_builder.reSID
{
    public class siddefs
    {



        // Compilation configuration.
        //#define RESID_INLINING @RESID_INLINING@
        //#define RESID_INLINE @RESID_INLINE@
        //#define RESID_BRANCH_HINTS @RESID_BRANCH_HINTS@

        // Compiler specifics.
        //#define HAVE_BOOL @RESID_HAVE_BOOL@
        //#define HAVE_BUILTIN_EXPECT @HAVE_BUILTIN_EXPECT@
        //#define HAVE_LOG1P @RESID_HAVE_LOG1P@

        // Define bool, true, and false for C++ compilers that lack these keywords.
        //#if !HAVE_BOOL
        //typedef int bool;
        //const bool true = 1;
        //const bool false = 0;
        //#endif

        //#if HAVE_LOG1P
        //#define HAS_LOG1P
        //#endif

        // Branch prediction macros, lifted off the Linux kernel.
        //#if RESID_BRANCH_HINTS && HAVE_BUILTIN_EXPECT
        //#define likely(x)      __builtin_expect(!!(x), 1)
        //#define unlikely(x)    __builtin_expect(!!(x), 0)
        //#else
        //#define likely(x)      (x)
        //#define unlikely(x)    (x)
        //#endif

        // We could have used the smallest possible data type for each SID register,
        // however this would give a slower engine because of data type conversions.
        // An int is assumed to be at least 32 bits (necessary in the types reg24
        // and cycle_count). GNU does not support 16-bit machines
        // (GNU Coding Standards: Portability between CPUs), so this should be
        // a valid assumption.

        //typedef UInt32 reg4;
        //typedef UInt32 reg8;
        //typedef UInt32 reg12;
        //typedef UInt32 reg16;
        //typedef UInt32 reg24;

        //typedef Int32 cycle_count;
        //typedef Int16[] short_point[2];
        //typedef double[] double_point[2];

        public enum chip_model { MOS6581, MOS8580 };

        public enum sampling_method
        {
            SAMPLE_FAST, SAMPLE_INTERPOLATE,
            SAMPLE_RESAMPLE, SAMPLE_RESAMPLE_FASTMEM
        };


        //extern "C"
        //{
        //#ifndef RESID_VERSION_CC
        //extern const char* resid_version_string;
        //#else
        public const string resid_version_string = "1.0-pre2";
        //#endif
        //}


    }
}