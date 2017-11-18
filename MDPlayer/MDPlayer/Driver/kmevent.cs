using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver
{
    public class kmevent
    {
        /*
          KMxxx event timer header
          by Mamiya
        */

        //# ifndef KMEVENT_H_
        //#define KMEVENT_H_

        //# include "kmtypes.h"
        //# ifdef __cplusplus
        //extern "C" {
        //#endif

        public const Int32 KMEVENT_ITEM_MAX = 31; /* MAX 255 */

        //public KMEVENT_TAG KMEVENT=new KMEVENT_TAG();
        //public KMEVENT_ITEM_TAG KMEVENT_ITEM=new KMEVENT_ITEM_TAG();
        //public UInt32 KMEVENT_ITEM_ID;
        //public delegate void dlgProc(KMEVENT _event, UInt32 curid, object user);
        public delegate void dlgProc(KMEVENT _event, UInt32 curid, m_hes.HESHES user);
        public class KMEVENT_ITEM
        {
            /* メンバ直接アクセス禁止 */
            //public object user;
            public m_hes.HESHES user;
            public dlgProc proc;
            public UInt32 count;   /* イベント発生時間 */
            public byte prev;     /* 双方向リンクリスト */
            public byte next;     /* 双方向リンクリスト */
            public byte sysflag;  /* 内部状態フラグ */
            public byte flag2;    /* 未使用 */
        };
        public class KMEVENT
        {
            /* メンバ直接アクセス禁止 */
            public KMEVENT_ITEM[] item = new KMEVENT_ITEM[KMEVENT_ITEM_MAX + 1];
        };

        //public void kmevent_init(KMEVENT kme) { }
        //public UInt32 kmevent_alloc(KMEVENT kme) { return 0; }
        //public void kmevent_free(KMEVENT kme, UInt32 curid) { }
        //public void kmevent_settimer(KMEVENT kme, UInt32 curid, UInt32 time) { }
        //public UInt32 kmevent_gettimer(KMEVENT kme, UInt32 curid, ref UInt32 time) { return 0; }
        //public void kmevent_setevent(KMEVENT kme, UInt32 curid, dlgProc proc, object user) { }
        //public void kmevent_process(KMEVENT kme, UInt32 cycles) { }

        //# ifdef __cplusplus
        //}
        //#endif
        //#endif


        /*
  KMxxx event timer
  by Mamiya
*/

//# include "kmevent.h"

        private enum KMEVENT_FLAG : byte
        {
            KMEVENT_FLAG_BREAKED = 0x01,//(1 << 0),
            KMEVENT_FLAG_DISPATCHED = 0x02,//(1 << 1),
            KMEVENT_FLAG_ALLOCED = 0x80//(1 << 7)
        }

        private void kmevent_reset(KMEVENT kme)
        {
            UInt32 id;
            kme.item[0].count = 0;
            for (id = 0; id <= KMEVENT_ITEM_MAX; id++)
            {
                kme.item[id].sysflag &= (byte)~KMEVENT_FLAG.KMEVENT_FLAG_ALLOCED;
                kme.item[id].count = 0;
                kme.item[id].next = (byte)id;
                kme.item[id].prev = (byte)id;
            }
        }

        public void kmevent_init(KMEVENT kme)
        {
            UInt32 id;
            for (id = 0; id <= KMEVENT_ITEM_MAX; id++)
            {
                kme.item[id] = new KMEVENT_ITEM();
                kme.item[id].sysflag = 0;
            }
            kmevent_reset(kme);
        }

        public UInt32 kmevent_alloc(KMEVENT kme)
        {
            UInt32 id;
            for (id = 1; id <= KMEVENT_ITEM_MAX; id++)
            {
                if (kme.item[id].sysflag == 0)
                {
                    kme.item[id].sysflag = (byte)KMEVENT_FLAG.KMEVENT_FLAG_ALLOCED;
                    return id;
                }
            }
            return 0;
        }

        /* リストから取り外す */
        private void kmevent_itemunlist(KMEVENT kme, UInt32 curid)
        {
            KMEVENT_ITEM cur, next, prev;
            cur = kme.item[curid];
            next = kme.item[cur.next];
            prev = kme.item[cur.prev];
            next.prev = cur.prev;
            prev.next = cur.next;
        }

        /* リストの指定位置(baseid)の直前に挿入 */
        private void kmevent_itemlist(KMEVENT kme, UInt32 curid, UInt32 baseid)
        {
            KMEVENT_ITEM cur, next, prev;
            cur = kme.item[curid];
            next = kme.item[baseid];
            prev = kme.item[next.prev];
            cur.next = (byte)baseid;
            cur.prev = next.prev;
            prev.next =(byte) curid;
            next.prev = (byte)curid;
        }

        /* ソート済リストに挿入 */
        private void kmevent_iteminsert(KMEVENT kme, UInt32 curid)
        {
            UInt32 baseid;
            for (baseid = kme.item[0].next; baseid!=0; baseid = kme.item[baseid].next)
            {
                if (kme.item[baseid].count!=0)
                {
                    if (kme.item[baseid].count > kme.item[curid].count) break;
                }
            }
            kmevent_itemlist(kme, curid, baseid);
        }

        public void kmevent_free(KMEVENT kme, UInt32 curid)
        {
            kmevent_itemunlist(kme, curid);
            kme.item[curid].sysflag = 0;
        }

        public void kmevent_settimer(KMEVENT kme, UInt32 curid, UInt32 time)
        {
            kmevent_itemunlist(kme, curid); /* 取り外し */
            kme.item[curid].count = time != 0 ? kme.item[0].count + time : 0;
            if (kme.item[curid].count != 0) kmevent_iteminsert(kme, curid); /* ソート */
        }

        public UInt32 kmevent_gettimer(KMEVENT kme, UInt32 curid, ref UInt32 time)
        {
            UInt32 nextcount;
            nextcount = kme.item[curid!=0 ? curid : kme.item[0].next].count;
            if (nextcount==0) return 0;
            nextcount -= kme.item[0].count;
            if (time!=0) time = nextcount;
            return 1;
        }

        //public void kmevent_setevent(KMEVENT kme, UInt32 curid, dlgProc proc, object user)
        public void kmevent_setevent(KMEVENT kme, UInt32 curid, dlgProc proc, m_hes.HESHES user)
        {
            kme.item[curid].proc = proc;
            kme.item[curid].user = user;
        }

        /* 指定サイクル分実行 */
        public void kmevent_process(KMEVENT kme, UInt32 cycles)
        {
            UInt32 id;
            UInt32 nextcount;
            kme.item[0].count += cycles;
            if (kme.item[0].next == 0)
            {
                /* リストが空なら終わり */
                kme.item[0].count = 0;
                return;
            }
            nextcount = kme.item[kme.item[0].next].count;
            while (nextcount!=0 && kme.item[0].count >= nextcount)
            {
                /* イベント発生済フラグのリセット */
                for (id = kme.item[0].next; id!=0; id = kme.item[id].next)
                {
                    kme.item[id].sysflag &= 0xfc;// ~((byte)KMEVENT_FLAG.KMEVENT_FLAG_BREAKED + (byte)KMEVENT_FLAG.KMEVENT_FLAG_DISPATCHED);
                }
                /* nextcount分進行 */
                kme.item[0].count -= nextcount;
                for (id = kme.item[0].next; id!=0; id = kme.item[id].next)
                {
                    if (kme.item[id].count==0) continue;
                    kme.item[id].count -= nextcount;
                    if (kme.item[id].count!=0) continue;
                    /* イベント発生フラグのセット */
                    kme.item[id].sysflag |= (byte)KMEVENT_FLAG.KMEVENT_FLAG_BREAKED;
                }
                for (id = kme.item[0].next; id!=0; id = kme.item[id].next)
                {
                    /* イベント発生済フラグの確認 */
                    if ((kme.item[id].sysflag & (byte)KMEVENT_FLAG.KMEVENT_FLAG_DISPATCHED)!=0) continue;
                    kme.item[id].sysflag |=(byte)KMEVENT_FLAG.KMEVENT_FLAG_DISPATCHED;
                    /* イベント発生フラグの確認 */
                    if ((kme.item[id].sysflag & (byte)KMEVENT_FLAG.KMEVENT_FLAG_BREAKED)==0) continue;
                    /* 対象イベント起動 */
                    kme.item[id].proc(kme, id, kme.item[id].user);
                    /* 先頭から再走査 */
                    id = 0;
                }
                nextcount = kme.item[kme.item[0].next].count;
            }
        }

    }
}
