/* Simple smart pointer class. */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Driver.libsidplayfp.sidtune
{
    public class SmartPtrBase_sidtt<T>
    {



        //# include "sidcxx11.h"

        public UInt64 ulint_smartpt;

        public SmartPtrBase_sidtt(T[] buffer, UInt64 bufferLen, bool bufOwner = false)
        {
            bufBegin = null;
            bufEnd = null;
            pBufCurrent = null;
            bufLen = 0;
            status = false;
            doFree = bufOwner;
            dummy = null;

            if ((buffer != null) && (bufferLen != 0))
            {
                bufBegin = new Ptr<T>(buffer,0);
                pBufCurrent = new Ptr<T>(buffer, 0);
                bufEnd = new Ptr<T>(bufBegin.buf, bufBegin.ptr + (Int32)bufferLen);
                bufLen = bufferLen;
                status = true;
            }
        }

        ~SmartPtrBase_sidtt()
        {
            if (doFree && (bufBegin != null))
            {
                bufBegin = null;
            }
        }

        /* --- public member functions --- */

        public virtual Ptr<T> tellBegin() { return bufBegin; }
        public virtual UInt64 tellLength() { return bufLen; }
        public virtual UInt64 tellPos() { return (UInt64)(pBufCurrent.ptr - bufBegin.ptr); }

        public virtual bool checkIndex(UInt64 index)
        {
            return (pBufCurrent.ptr + (Int32)index) < bufEnd.ptr;
        }

        public virtual bool reset()
        {
            if (bufLen != 0)
            {
                pBufCurrent = bufBegin;
                status = true;
            }
            else
            {
                status = false;
            }
            return status;
        }

        public virtual bool good()
        {
            return pBufCurrent.ptr < bufEnd.ptr;
        }

        public virtual bool fail()
        {
            return pBufCurrent == bufEnd;
        }

        public virtual void opePlusPlus()
        {
            if (good())
            {
                pBufCurrent.ptr++;
            }
            else
            {
                status = false;
            }
        }

        public virtual void opePlusPlus(int m)
        {
            if (good())
            {
                pBufCurrent.ptr++;
            }
            else
            {
                status = false;
            }
        }

        public virtual void opeMinusMinus()
        {
            if (!fail())
            {
                pBufCurrent.ptr--;
            }
            else
            {
                status = false;
            }
        }

        public virtual void opeMinusMinus(int m)
        {
            if (!fail())
            {
                pBufCurrent.ptr--;
            }
            else
            {
                status = false;
            }
        }

        public virtual void opePlusEquel(UInt64 offset)
        {
            if (checkIndex(offset))
            {
                pBufCurrent.ptr += (Int32)offset;
            }
            else
            {
                status = false;
            }
        }

        public virtual void opeMinusEquel(UInt64 offset)
        {
            if ((pBufCurrent.ptr - (Int32)offset) >= bufBegin.ptr)
            {
                pBufCurrent.ptr -= (Int32)offset;
            }
            else
            {
                status = false;
            }
        }

        public virtual Ptr<T> opePtr()
        {
            if (good())
            {
                return pBufCurrent;
            }
            else
            {
                status = false;
                return dummy;
            }
        }

        public virtual Ptr<T> opeDKakko(UInt64 index)
        {
            if (checkIndex(index))
            {
                return new Ptr<T>(pBufCurrent.buf, pBufCurrent.ptr + (Int32)index);
            }
            else
            {
                status = false;
                return dummy;
            }
        }

        public virtual bool opeBool() { return status; }

        protected Ptr<T> bufBegin;
        protected Ptr<T> bufEnd;
        protected Ptr<T> pBufCurrent;
        protected UInt64 bufLen;
        protected bool status;

        protected bool doFree;
        protected Ptr<T> dummy;
    }



    public sealed class SmartPtr_sidtt<T> : SmartPtrBase_sidtt<T>
    {
        public SmartPtr_sidtt(T[] buffer, UInt64 bufferLen, bool bufOwner = false) : base(buffer, bufferLen, bufOwner)
        { }


        SmartPtr_sidtt() : base(null, 0)
        { }

        public void setBuffer(T[] buffer, UInt64 bufferLen)
        {
            if ((buffer != null) && (bufferLen != 0))
            {
                this.bufBegin = new Ptr<T>(buffer, 0);
                this.pBufCurrent = new Ptr<T>(buffer, 0);
                this.bufEnd = new Ptr<T>(buffer, (Int32)bufferLen);
                this.bufLen = bufferLen;
                this.status = true;
            }
            else
            {
                this.bufBegin = null;
                this.pBufCurrent = null;
                this.bufEnd = null;
                this.bufLen = 0;
                this.status = false;
            }
        }





    }
}