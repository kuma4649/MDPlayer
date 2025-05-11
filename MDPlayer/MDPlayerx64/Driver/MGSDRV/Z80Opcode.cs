using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDPlayer.Driver.MGSDRV
{
    //参考
    //https://baltazarstudios.com/webshare/A-Z80/Z80-Opcode-Tables.pdf
    //http://z80-heaven.wikidot.com/

    public static class Z80Opcode
    {
        private static string[] opcodeTable = [
"nop ",      "ld bc,xx ", "ld (bc),a ", "inc bc ",
"inc b ",    "dec b ",    "ld b,x ",    "rlca ",
"ex af,af' ","add hl,bc ","ld a,(bc) ", "dec bc ",
"inc c ",    "dec c ",    "ld c,x ",    "rrca ",

"djnz x ",   "ld de,xx ", "ld (de),a ", "inc de ",
"inc d ",    "dec d ",    "ld d,x ",    "rla ",
"jr x ",     "add hl,de ","ld a,(de) ", "dec de ",
"inc e ",    "dec e ",    "ld e,x ",    "rra ",

"jr nz,x ",  "ld hl,xx ", "ld (xx),hl ","inc hl ",
"inc h ",    "dec h ",    "ld h,x ",    "daa ",
"jr z,x ",   "add hl,hl ","ld hl,(xx) ","dec hl ",
"inc l ",    "dec l ",    "ld l,x ",    "cpl ",

"jr nc,x ",  "ld sp,xx ", "ld (xx),a ", "inc sp ",
"inc (hl) ", "dec (hl) ", "ld (hl),x ", "scf ",
"jr c,x ",   "add hl,sp ","ld a,(xx) ", "dec sp ",
"inc a ",    "dec a ",    "ld a,x ",    "ccf ",

"ld b,b ",   "ld b,c ",   "ld b,d ",    "ld b,e ",
"ld b,h ",   "ld b,l ",   "ld b,(hl) ", "ld b,a ",
"ld c,b ",   "ld c,c ",   "ld c,d ",    "ld c,e ",
"ld c,h ",   "ld c,l ",   "ld c,(hl) ", "ld c,a ",

"ld d,b ",      "ld d,c ",      "ld d,d ",      "ld d,e ",
"ld d,h ",      "ld d,l ",      "ld d,(hl) ",   "ld d,a ",
"ld e,b ",      "ld e,c ",      "ld e,d ",      "ld e,e ",
"ld e,h ",      "ld e,l ",      "ld e,(hl) ",   "ld e,a ",

"ld h,b ",      "ld h,c ",      "ld h,d ",      "ld h,e ",
"ld h,h ",      "ld h,l ",      "ld h,(hl) ",   "ld h,a ",
"ld l,b ",      "ld l,c ",      "ld l,d ",      "ld l,e ",
"ld l,h ",      "ld l,l ",      "ld l,(hl) ",   "ld l,a ",

"ld (hl),b ",   "ld (hl),c ",   "ld (hl),d ",   "ld (hl),e ",
"ld (hl),h ",   "ld (hl),l ",   "halt ",        "ld (hl),a ",
"ld a,b ",      "ld a,c ",      "ld a,d ",      "ld a,e ",
"ld a,h ",      "ld a,l ",      "ld a,(hl) ",   "ld a,a ",

"add a,b ",     "add a,c ",     "add a,d ",     "add a,e ",
"add a,h ",     "add a,l ",     "add a,(hl) ",  "add a,a ",
"adc a,b ",     "adc a,c ",     "adc a,d ",     "adc a,e ",
"adc a,h ",     "adc a,l ",     "adc a,(hl) ",  "adc a,a ",

"sub b ",       "sub c ",       "sub d ",       "sub e ",
"sub h ",       "sub l ",       "sub (hl) ",    "sub a ",
"sbc a,b ",     "sbc a,c ",     "sbc a,d ",     "sbc a,e ",
"sbc a,h ",     "sbc a,l ",     "sbc a,(hl) ",  "sbc a,a ",

"and b ",       "and c ",       "and d ",       "and e ",
"and h ",       "and l ",       "and (hl) ",    "and a ",
"xor b ",       "xor c ",       "xor d ",       "xor e ",
"xor h ",       "xor l ",       "xor (hl) ",    "xor a ",

"or b ",        "or c ",        "or d ",        "or e ",
"or h ",        "or l ",        "or (hl) ",     "or a ",
"cp b ",        "cp c ",        "cp d ",        "cp e ",
"cp h ",        "cp l ",        "cp (hl) ",     "cp a ",

"ret nz ",      "pop bc ",      "jp nz,xx ",    "jp xx ",
"call nz,xx ",  "push bc ",     "add a,x ",     "rst 00h ",
"ret z ",       "ret ",         "jp z,xx ",     "CB",
"call z,xx ",   "call xx ",     "adc a,x ",     "rst 08h ",

"ret nc ",      "pop de ",      "jp nc,xx ",    "out (x),a ",
"call nc,xx ",  "push de ",     "sub x ",       "rst 10h ",
"ret c ",       "exx ",         "jp c,xx ",     "in a,(x) ",
"call c,xx ",   "DD (IX)",      "sbc a,x ",     "rst 18h ",

"ret po ",      "pop hl ",      "jp po,xx ",    "ex (sp),hl ",
"call po,xx ",  "push hl ",     "and x ",       "rst 20h ",
"ret pe ",      "jp (hl) ",     "jp pe,xx ",    "ex de,hl ",
"call pe,xx ",  "ED",           "xor x ",       "rst 28h ",

"ret p ",       "pop af ",      "jp p,xx ",     "di ",
"call p,xx ",   "push af ",     "or x ",        "rst 30h ",
"ret m ",       "ld sp,hl ",    "jp m,xx ",     "ei ",
"call m,xx ",   "FD (IY)",      "cp x ",        "rst 38h"
            ];

        private static string[] opcodeTableED = [
"","","","","","","","","","","","","","","","",

"","","","","","","","","","","","","","","","",

"","","","","","","","","","","","","","","","",

"","","","","","","","","","","","","","","","",

"in b,(c) ",    "out (c),b ",   "sbc hl,bc ",   "ld (nn),bc ",
"neg ",         "retn ",        "im 0 ",        "ld i,a ",
"in c,(c) ",    "out (c),c ",   "adc hl,bc ",   "ld bc,(nn) ",
"neg ",         "reti ",        "im 0 ",        "ld r,a ",

"in d,(c) ",    "out (c),d ",   "sbc hl,de ",   "ld (nn),de ",
"neg ",         "retn ",        "im 1 ",        "ld a,i ",
"in e,(c) ",    "out (c),e ",   "adc hl,de ",   "ld de,(nn) ",
"neg ",         "retn ",        "im 2 ",        "ld a,r ",

"in h,(c) ",    "out (c),h ",   "sbc hl,hl ",   "ld (nn),hl ",
"neg ",         "retn ",        "im 0 ",        "rrd ",
"in l,(c) ",    "out (c),l ",   "adc hl,hl ",   "ld hl,(nn) ",
"neg ",         "retn ",        "im 0 ",        "rld ",

"in (c) ",      "out (c),0",    "sbc hl,sp ",   "ld (nn),sp ",
"neg ",         "retn ",        "im 1 ",        "",
"in a,(c) ",    "out (c),a ",   "adc hl,sp ",   "ld sp,(nn) ",
"neg ",         "retn ",        "im 2 ",        "",

"","","","","","","","","","","","","","","","",

"","","","","","","","","","","","","","","","",

"ldi",          "cpi",          "ini",          "outi",
"","","","",
"ldd",          "cpd",          "ind",          "outd",
"","","","",

"ldir",         "cpir",         "inir",         "otir",
"","","","",
"lddr",         "cpdr",         "indr",         "outdr",
"","","","",

"","","","","","","","","","","","","","","","",
"","","","","","","","","","","","","","","","",
"","","","","","","","","","","","","","","","",
"","","","","","","","","","","","","","","",""
            ];

        private static string[] opcodeTableCB = [
"rlc b ",       "rlc c ",       "rlc d ",       "rlc e ",
"rlc h ",       "rlc l ",       "rlc (hl) ",    "rlc a ",
"rrc b ",       "rrc c ",       "rrc d ",       "rrc e ",
"rrc h ",       "rrc l ",       "rrc (hl) ",    "rrc a ",

"rl b ",        "rl c ",        "rl d ",        "rl e ",
"rl h ",        "rl l ",        "rl (hl) ",     "rl a ",
"rr b ",        "rr c ",        "rr d ",        "rr e ",
"rr h ",        "rr l ",        "rr (hl) ",     "rr a ",

"sla b ",       "sla c ",       "sla d ",       "sla e ",
"sla h ",       "sla l ",       "sla (hl) ",    "sla a ",
"sra b ",       "sra c ",       "sra d ",       "sra e ",
"sra h ",       "sra l ",       "sra (hl) ",    "sra a ",

"sll b ",       "sll c ",       "sll d ",       "sll e ",
"sll h ",       "sll l ",       "sll (hl) ",    "sll a ",
"srl b ",       "srl c ",       "srl d ",       "srl e ",
"srl h ",       "srl l ",       "srl (hl) ",    "srl a ",

"bit 0,b ",     "bit 0,c ",     "bit 0,d ",     "bit 0,e ",
"bit 0,h ",     "bit 0,l ",     "bit 0,(hl) ",  "bit 0,a ",
"bit 1,b ",     "bit 1,c ",     "bit 1,d ",     "bit 1,e ",
"bit 1,h ",     "bit 1,l ",     "bit 1,(hl) ",  "bit 1,a ",

"bit 2,b ",     "bit 2,c ",     "bit 2,d ",     "bit 2,e ",
"bit 2,h ",     "bit 2,l ",     "bit 2,(hl) ",  "bit 2,a ",
"bit 3,b ",     "bit 3,c ",     "bit 3,d ",     "bit 3,e ",
"bit 3,h ",     "bit 3,l ",     "bit 3,(hl) ",  "bit 3,a ",

"bit 4,b ",     "bit 4,c ",     "bit 4,d ",     "bit 4,e ",
"bit 4,h ",     "bit 4,l ",     "bit 4,(hl) ",  "bit 4,a ",
"bit 5,b ",     "bit 5,c ",     "bit 5,d ",     "bit 5,e ",
"bit 5,h ",     "bit 5,l ",     "bit 5,(hl) ",  "bit 5,a ",

"bit 6,b ",     "bit 6,c ",     "bit 6,d ",     "bit 6,e ",
"bit 6,h ",     "bit 6,l ",     "bit 6,(hl) ",  "bit 6,a ",
"bit 7,b ",     "bit 7,c ",     "bit 7,d ",     "bit 7,e ",
"bit 7,h ",     "bit 7,l ",     "bit 7,(hl) ",  "bit 7,a ",

"res 0,b ",     "res 0,c ",     "res 0,d ",     "res 0,e ",
"res 0,h ",     "res 0,l ",     "res 0,(hl) ",  "res 0,a ",
"res 1,b ",     "res 1,c ",     "res 1,d ",     "res 1,e ",
"res 1,h ",     "res 1,l ",     "res 1,(hl) ",  "res 1,a ",

"res 2,b ",     "res 2,c ",     "res 2,d ",     "res 2,e ",
"res 2,h ",     "res 2,l ",     "res 2,(hl) ",  "res 2,a ",
"res 3,b ",     "res 3,c ",     "res 3,d ",     "res 3,e ",
"res 3,h ",     "res 3,l ",     "res 3,(hl) ",  "res 3,a ",

"res 4,b ",     "res 4,c ",     "res 4,d ",     "res 4,e ",
"res 4,h ",     "res 4,l ",     "res 4,(hl) ",  "res 4,a ",
"res 5,b ",     "res 5,c ",     "res 5,d ",     "res 5,e ",
"res 5,h ",     "res 5,l ",     "res 5,(hl) ",  "res 5,a ",

"res 6,b ",     "res 6,c ",     "res 6,d ",     "res 6,e ",
"res 6,h ",     "res 6,l ",     "res 6,(hl) ",  "res 6,a ",
"res 7,b ",     "res 7,c ",     "res 7,d ",     "res 7,e ",
"res 7,h ",     "res 7,l ",     "res 7,(hl) ",  "res 7,a ",

"set 0,b ",     "set 0,c ",     "set 0,d ",     "set 0,e ",
"set 0,h ",     "set 0,l ",     "set 0,(hl) ",  "set 0,a ",
"set 1,b ",     "set 1,c ",     "set 1,d ",     "set 1,e ",
"set 1,h ",     "set 1,l ",     "set 1,(hl) ",  "set 1,a ",

"set 2,b ",     "set 2,c ",     "set 2,d ",     "set 2,e ",
"set 2,h ",     "set 2,l ",     "set 2,(hl) ",  "set 2,a ",
"set 3,b ",     "set 3,c ",     "set 3,d ",     "set 3,e ",
"set 3,h ",     "set 3,l ",     "set 3,(hl) ",  "set 3,a ",

"set 4,b ",     "set 4,c ",     "set 4,d ",     "set 4,e ",
"set 4,h ",     "set 4,l ",     "set 4,(hl) ",  "set 4,a ",
"set 5,b ",     "set 5,c ",     "set 5,d ",     "set 5,e ",
"set 5,h ",     "set 5,l ",     "set 5,(hl) ",  "set 5,a ",

"set 6,b ",     "set 6,c ",     "set 6,d ",     "set 6,e ",
"set 6,h ",     "set 6,l ",     "set 6,(hl) ",  "set 6,a ",
"set 7,b ",     "set 7,c ",     "set 7,d ",     "set 7,e ",
"set 7,h ",     "set 7,l ",     "set 7,(hl) ",  "set 7,a"
            ];

        private static string[] opcodeTableDD = [
"","","","", "","","","", "","add ix,bc","","", "","","","",

"","","","", "","","","", "","add ix,de","","", "","","","",

"",             "ld ix,xx",     "ld (xx),ix",   "inc ix",
"inc ixh",      "dec ixh",      "ld ixh,x",     "",
"",             "add ix,ix",    "ld ix,(xx)",   "dec ix",
"inc ixl",      "dec ixl",      "ld ixl,x",     "",

"","","","",
"inc (ix+d)",   "dec (ix+d)",   "ld (ix+d),x",  "",
"",             "add ix,sp",    "",             "",
"","","","",

"","","","",
"ld b,ixh",     "ld b,ixl",     "ld b,(ix+d)",  "",
"","","","",
"ld c,ixh",     "ld c,ixl",     "ld c,(ix+d)",  "",

"","","","",
"ld d,ixh",     "ld d,ixl",     "ld d,(ix+d)",  "",
"","","","",
"ld e,ixh",     "ld e,ixl",     "ld e,(ix+d)",  "",

"ld ixh,b",     "ld ixh,c",     "ld ixh,d",     "ld ixh,e",
"ld ixh,ixh",   "ld ixh,l",     "ld h,(ix+d)",  "ld ixh,a",
"ld ixl,b",     "ld ixl,c",     "ld ixl,d",     "ld ixl,e",
"ld ixl,ixh",   "ld ixl,l",     "ld l,(ix+d)",  "ld ixl,a",

"ld (ix+d),b ", "ld (ix+d),c ", "ld (ix+d),d ", "ld (ix+d),e",
"ld (ix+d),h ", "ld (ix+d),l",  "ld (ix+d),a",  "",
"",             "",             "",             "",
"ld a,ixh",     "ld a,ixl",     "ld a,(ix+d)",  "",

"","","","",
"add a,ixh",    "add a,ixl",    "add a,(ix+d)", "",
"","","","",
"adc a,ixh",    "adc a,ixl",    "adc a,(ix+d)", "",

"","","","",
"sub ixh",      "sub ixl",      "sub (ix+d)",   "",
"","","","",
"sbc a,ixh",    "sbc a,ixl",    "sbc a,(ix+d)", "",

"","","","",
"and ixh",      "and ixl",      "and (ix+d)",   "",
"","","","",
"xor ixh",      "xor ixl",      "xor (ix+d)",   "",

"","","","",
"or ixh",       "or ixl",       "or (ix+d)",    "",
"","","","",
"cp ixh",       "cp ixl",       "cp (ix+d)",    "",

"","","","","","","","","","","","","","","","",

"","","","","","","","","","","","","","","","",

"",             "pop ix",       "",             "ex (sp),ix",
"",             "push ix",      "",             "",
"",             "jp (ix)",      "",             "",
"","","","",

"","","","",
"","","","",
"",             "ld sp,ix",     "",             "",
"","","",""
            ];

        private static string[] opcodeTableFD = [
"","","","", "","","","", "","add iy,bc","","", "","","","",

"","","","", "","","","", "","add iy,de","","", "","","","",

"",             "ld iy,xx",     "ld (xx),iy",   "inc iy",
"inc iyh",      "dec iyh",      "ld iyh,x",     "",
"",             "add iy,iy",    "ld iy,(xx)",   "dec iy",
"inc iyl",      "dec iyl",      "ld iyl,x",     "",

"","","","",
"inc (iy+d)",   "dec (iy+d)",   "ld (iy+d),x",  "",
"",             "add iy,sp",    "",             "",
"","","","",

"","","","",
"ld b,iyh",     "ld b,iyl",     "ld b,(iy+d)",  "",
"","","","",
"ld c,iyh",     "ld c,iyl",     "ld c,(iy+d)",  "",

"","","","",
"ld d,iyh",     "ld d,iyl",     "ld d,(iy+d)",  "",
"","","","",
"ld e,iyh",     "ld e,iyl",     "ld e,(iy+d)",  "",

"ld iyh,b",     "ld iyh,c",     "ld iyh,d",     "ld iyh,e",
"ld iyh,iyh",   "ld iyh,l",     "ld h,(iy+d)",  "ld iyh,a",
"ld iyl,b",     "ld iyl,c",     "ld iyl,d",     "ld iyl,e",
"ld iyl,iyh",   "ld iyl,l",     "ld l,(iy+d)",  "ld iyl,a",

"ld (iy+d),b ", "ld (iy+d),c ", "ld (iy+d),d ", "ld (iy+d),e",
"ld (iy+d),h ", "ld (iy+d),l",  "ld (iy+d),a",  "",
"",             "",             "",             "",
"ld a,iyh",     "ld a,iyl",     "ld a,(iy+d)",  "",

"","","","",
"add a,iyh",    "add a,iyl",    "add a,(iy+d)", "",
"","","","",
"adc a,iyh",    "adc a,iyl",    "adc a,(iy+d)", "",

"","","","",
"sub iyh",      "sub iyl",      "sub (iy+d)",   "",
"","","","",
"sbc a,iyh",    "sbc a,iyl",    "sbc a,(iy+d)", "",

"","","","",
"and iyh",      "and iyl",      "and (iy+d)",   "",
"","","","",
"xor iyh",      "xor iyl",      "xor (iy+d)",   "",

"","","","",
"or iyh",       "or iyl",       "or (iy+d)",    "",
"","","","",
"cp iyh",       "cp iyl",       "cp (iy+d)",    "",

"","","","","","","","","","","","","","","","",

"","","","","","","","","","","","","","","","",

"",             "pop iy",       "",             "ex (sp),iy",
"",             "push iy",      "",             "",
"",             "jp (iy)",      "",             "",
"","","","",

"","","","",
"","","","",
"",             "ld sp,iy",     "",             "",
"","","",""
            ];

        private static string[] opcodeTableDDCB = [
"rlc (ix+d),b",         "rlc (ix+d),c",         "rlc (ix+d),d ",        "rlc (ix+d),e",
"rlc (ix+d),h",         "rlc (ix+d),l",         "rlc (ix+d)",           "rlc (ix+d),a",
"rrc (ix+d),b",         "rrc (ix+d),c",         "rrc (ix+d),d ",        "rrc (ix+d),e",
"rrc (ix+d),h",         "rrc (ix+d),l",         "rrc (ix+d)",           "rrc (ix+d),a",

"rl (ix+d),b",          "rl (ix+d),c",          "rl (ix+d),d",          "rl (ix+d),e",
"rl (ix+d),h",          "rl (ix+d),l",          "rl (ix+d)",            "rl (ix+d),a",
"rr (ix+d),b",          "rr (ix+d),c",          "rr (ix+d),d",          "rr (ix+d),e",
"rr (ix+d),h",          "rr (ix+d),l",          "rr (ix+d)",            "rr (ix+d),a",

"sla (ix+d),b",         "sla (ix+d),c",         "sla (ix+d),d",         "sla (ix+d),e",
"sla (ix+d),h",         "sla (ix+d),l",         "sla (ix+d)",           "sla (ix+d),a",
"sra (ix+d),b",         "sra (ix+d),c",         "sra (ix+d),d",         "sra (ix+d),e",
"sra (ix+d),h",         "sra (ix+d),l",         "sra (ix+d)",           "sra (ix+d),a",

"sll (ix+d),b",         "sll (ix+d),c",         "sll (ix+d),d",         "sll (ix+d),e",
"sll (ix+d),h",         "sll (ix+d),l",         "sll (ix+d)",           "sll (ix+d),a",
"srl (ix+d),b",         "srl (ix+d),c",         "srl (ix+d),d",         "srl (ix+d),e",
"srl (ix+d),h",         "srl (ix+d),l",         "srl (ix+d)",           "srl (ix+d),a",

"bit 0,(ix+d)",         "bit 0,(ix+d)",         "bit 0,(ix+d)",         "bit 0,(ix+d)",
"bit 0,(ix+d)",         "bit 0,(ix+d)",         "bit 0,(ix+d)",         "bit 0,(ix+d)",
"bit 1,(ix+d)",         "bit 1,(ix+d)",         "bit 1,(ix+d)",         "bit 1,(ix+d)",
"bit 1,(ix+d)",         "bit 1,(ix+d)",         "bit 1,(ix+d)",         "bit 1,(ix+d)",

"bit 2,(ix+d)",         "bit 2,(ix+d)",         "bit 2,(ix+d)",         "bit 2,(ix+d)",
"bit 2,(ix+d)",         "bit 2,(ix+d)",         "bit 2,(ix+d)",         "bit 2,(ix+d)",
"bit 3,(ix+d)",         "bit 3,(ix+d)",         "bit 3,(ix+d)",         "bit 3,(ix+d)",
"bit 3,(ix+d)",         "bit 3,(ix+d)",         "bit 3,(ix+d)",         "bit 3,(ix+d)",

"bit 4,(ix+d)",         "bit 4,(ix+d)",         "bit 4,(ix+d)",         "bit 4,(ix+d)",
"bit 4,(ix+d)",         "bit 4,(ix+d)",         "bit 4,(ix+d)",         "bit 4,(ix+d)",
"bit 5,(ix+d)",         "bit 5,(ix+d)",         "bit 5,(ix+d)",         "bit 5,(ix+d)",
"bit 5,(ix+d)",         "bit 5,(ix+d)",         "bit 5,(ix+d)",         "bit 5,(ix+d)",

"bit 6,(ix+d)",         "bit 6,(ix+d)",         "bit 6,(ix+d)",         "bit 6,(ix+d)",
"bit 6,(ix+d)",         "bit 6,(ix+d)",         "bit 6,(ix+d)",         "bit 6,(ix+d)",
"bit 7,(ix+d)",         "bit 7,(ix+d)",         "bit 7,(ix+d)",         "bit 7,(ix+d)",
"bit 7,(ix+d)",         "bit 7,(ix+d)",         "bit 7,(ix+d)",         "bit 7,(ix+d)",

"res 0,(ix+d),b",       "res 0,(ix+d),c",       "res 0,(ix+d),d",       "res 0,(ix+d),e",
"res 0,(ix+d),h",       "res 0,(ix+d),l",       "res 0,(ix+d)",         "res 0,(ix+d),a",
"res 1,(ix+d),b",       "res 1,(ix+d),c",       "res 1,(ix+d),d",       "res 1,(ix+d),e",
"res 1,(ix+d),h ",      "res 1,(ix+d),l",       "res 1,(ix+d)",         "res 1,(ix+d),a",

"res 2,(ix+d),b",       "res 2,(ix+d),c",       "res 2,(ix+d),d",       "res 2,(ix+d),e",
"res 2,(ix+d),h",       "res 2,(ix+d),l",       "res 2,(ix+d)",         "res 2,(ix+d),a",
"res 3,(ix+d),b",       "res 3,(ix+d),c",       "res 3,(ix+d),d",       "res 3,(ix+d),e",
"res 3,(ix+d),h",       "res 3,(ix+d),l",       "res 3,(ix+d)",         "res 3,(ix+d),a",

"res 4,(ix+d),b",       "res 4,(ix+d),c",       "res 4,(ix+d),d",       "res 4,(ix+d),e",
"res 4,(ix+d),h",       "res 4,(ix+d),l",       "res 4,(ix+d)",         "res 4,(ix+d),a",
"res 5,(ix+d),b",       "res 5,(ix+d),c",       "res 5,(ix+d),d ",      "res 5,(ix+d),e",
"res 5,(ix+d),h",       "res 5,(ix+d),l",       "res 5,(ix+d)",         "res 5,(ix+d),a",

"res 6,(ix+d),b",       "res 6,(ix+d),c",       "res 6,(ix+d),d",       "res 6,(ix+d),e",
"res 6,(ix+d),h",       "res 6,(ix+d),l",       "res 6,(ix+d)",         "res 6,(ix+d),a",
"res 7,(ix+d),b",       "res 7,(ix+d),c",       "res 7,(ix+d),d",       "res 7,(ix+d),e",
"res 7,(ix+d),h",       "res 7,(ix+d),l",       "res 7,(ix+d)",         "res 7,(ix+d),a",

"set 0,(ix+d),b",       "set 0,(ix+d),c",       "set 0,(ix+d),d",       "set 0,(ix+d),e",
"set 0,(ix+d),h",       "set 0,(ix+d),l",       "set 0,(ix+d)",         "set 0,(ix+d),a",
"set 1,(ix+d),b",       "set 1,(ix+d),c",       "set 1,(ix+d),d",       "set 1,(ix+d),e",
"set 1,(ix+d),h",       "set 1,(ix+d),l",       "set 1,(ix+d)",         "set 1,(ix+d),a",

"set 2,(ix+d),b",       "set 2,(ix+d),c",       "set 2,(ix+d),d",       "set 2,(ix+d),e",
"set 2,(ix+d),h",       "set 2,(ix+d),l",       "set 2,(ix+d)",         "set 2,(ix+d),a",
"set 3,(ix+d),b",       "set 3,(ix+d),c",       "set 3,(ix+d),d",       "set 3,(ix+d),e",
"set 3,(ix+d),h",       "set 3,(ix+d),l",       "set 3,(ix+d)",         "set 3,(ix+d),a",

"set 4,(ix+d),b",       "set 4,(ix+d),c",       "set 4,(ix+d),d",       "set 4,(ix+d),e",
"set 4,(ix+d),h",       "set 4,(ix+d),l",       "set 4,(ix+d)",         "set 4,(ix+d),a",
"set 5,(ix+d),b",       "set 5,(ix+d),c",       "set 5,(ix+d),d",       "set 5,(ix+d),e",
"set 5,(ix+d),h",       "set 5,(ix+d),l",       "set 5,(ix+d)",         "set 5,(ix+d),a",

"set 6,(ix+d),b",       "set 6,(ix+d),c",       "set 6,(ix+d),d",       "set 6,(ix+d),e",
"set 6,(ix+d),h",       "set 6,(ix+d),l",       "set 6,(hl)",           "set 6,(ix+d),a",
"set 7,(ix+d),b",       "set 7,(ix+d),c",       "set 7,(ix+d),d",       "set 7,(ix+d),e",
"set 7,(ix+d),h",       "set 7,(ix+d),l",       "set 7,(hl)",           "set 7,(ix+d),a",
            ];

        private static string[] opcodeTableFDCB = [
"rlc (iy+d),b",         "rlc (iy+d),c",         "rlc (iy+d),d ",        "rlc (iy+d),e",
"rlc (iy+d),h",         "rlc (iy+d),l",         "rlc (iy+d)",           "rlc (iy+d),a",
"rrc (iy+d),b",         "rrc (iy+d),c",         "rrc (iy+d),d ",        "rrc (iy+d),e",
"rrc (iy+d),h",         "rrc (iy+d),l",         "rrc (iy+d)",           "rrc (iy+d),a",

"rl (iy+d),b",          "rl (iy+d),c",          "rl (iy+d),d",          "rl (iy+d),e",
"rl (iy+d),h",          "rl (iy+d),l",          "rl (iy+d)",            "rl (iy+d),a",
"rr (iy+d),b",          "rr (iy+d),c",          "rr (iy+d),d",          "rr (iy+d),e",
"rr (iy+d),h",          "rr (iy+d),l",          "rr (iy+d)",            "rr (iy+d),a",

"sla (iy+d),b",         "sla (iy+d),c",         "sla (iy+d),d",         "sla (iy+d),e",
"sla (iy+d),h",         "sla (iy+d),l",         "sla (iy+d)",           "sla (iy+d),a",
"sra (iy+d),b",         "sra (iy+d),c",         "sra (iy+d),d",         "sra (iy+d),e",
"sra (iy+d),h",         "sra (iy+d),l",         "sra (iy+d)",           "sra (iy+d),a",

"sll (iy+d),b",         "sll (iy+d),c",         "sll (iy+d),d",         "sll (iy+d),e",
"sll (iy+d),h",         "sll (iy+d),l",         "sll (iy+d)",           "sll (iy+d),a",
"srl (iy+d),b",         "srl (iy+d),c",         "srl (iy+d),d",         "srl (iy+d),e",
"srl (iy+d),h",         "srl (iy+d),l",         "srl (iy+d)",           "srl (iy+d),a",

"bit 0,(iy+d)",         "bit 0,(iy+d)",         "bit 0,(iy+d)",         "bit 0,(iy+d)",
"bit 0,(iy+d)",         "bit 0,(iy+d)",         "bit 0,(iy+d)",         "bit 0,(iy+d)",
"bit 1,(iy+d)",         "bit 1,(iy+d)",         "bit 1,(iy+d)",         "bit 1,(iy+d)",
"bit 1,(iy+d)",         "bit 1,(iy+d)",         "bit 1,(iy+d)",         "bit 1,(iy+d)",

"bit 2,(iy+d)",         "bit 2,(iy+d)",         "bit 2,(iy+d)",         "bit 2,(iy+d)",
"bit 2,(iy+d)",         "bit 2,(iy+d)",         "bit 2,(iy+d)",         "bit 2,(iy+d)",
"bit 3,(iy+d)",         "bit 3,(iy+d)",         "bit 3,(iy+d)",         "bit 3,(iy+d)",
"bit 3,(iy+d)",         "bit 3,(iy+d)",         "bit 3,(iy+d)",         "bit 3,(iy+d)",

"bit 4,(iy+d)",         "bit 4,(iy+d)",         "bit 4,(iy+d)",         "bit 4,(iy+d)",
"bit 4,(iy+d)",         "bit 4,(iy+d)",         "bit 4,(iy+d)",         "bit 4,(iy+d)",
"bit 5,(iy+d)",         "bit 5,(iy+d)",         "bit 5,(iy+d)",         "bit 5,(iy+d)",
"bit 5,(iy+d)",         "bit 5,(iy+d)",         "bit 5,(iy+d)",         "bit 5,(iy+d)",

"bit 6,(iy+d)",         "bit 6,(iy+d)",         "bit 6,(iy+d)",         "bit 6,(iy+d)",
"bit 6,(iy+d)",         "bit 6,(iy+d)",         "bit 6,(iy+d)",         "bit 6,(iy+d)",
"bit 7,(iy+d)",         "bit 7,(iy+d)",         "bit 7,(iy+d)",         "bit 7,(iy+d)",
"bit 7,(iy+d)",         "bit 7,(iy+d)",         "bit 7,(iy+d)",         "bit 7,(iy+d)",

"res 0,(iy+d),b",       "res 0,(iy+d),c",       "res 0,(iy+d),d",       "res 0,(iy+d),e",
"res 0,(iy+d),h",       "res 0,(iy+d),l",       "res 0,(iy+d)",         "res 0,(iy+d),a",
"res 1,(iy+d),b",       "res 1,(iy+d),c",       "res 1,(iy+d),d",       "res 1,(iy+d),e",
"res 1,(iy+d),h ",      "res 1,(iy+d),l",       "res 1,(iy+d)",         "res 1,(iy+d),a",

"res 2,(iy+d),b",       "res 2,(iy+d),c",       "res 2,(iy+d),d",       "res 2,(iy+d),e",
"res 2,(iy+d),h",       "res 2,(iy+d),l",       "res 2,(iy+d)",         "res 2,(iy+d),a",
"res 3,(iy+d),b",       "res 3,(iy+d),c",       "res 3,(iy+d),d",       "res 3,(iy+d),e",
"res 3,(iy+d),h",       "res 3,(iy+d),l",       "res 3,(iy+d)",         "res 3,(iy+d),a",

"res 4,(iy+d),b",       "res 4,(iy+d),c",       "res 4,(iy+d),d",       "res 4,(iy+d),e",
"res 4,(iy+d),h",       "res 4,(iy+d),l",       "res 4,(iy+d)",         "res 4,(iy+d),a",
"res 5,(iy+d),b",       "res 5,(iy+d),c",       "res 5,(iy+d),d ",      "res 5,(iy+d),e",
"res 5,(iy+d),h",       "res 5,(iy+d),l",       "res 5,(iy+d)",         "res 5,(iy+d),a",

"res 6,(iy+d),b",       "res 6,(iy+d),c",       "res 6,(iy+d),d",       "res 6,(iy+d),e",
"res 6,(iy+d),h",       "res 6,(iy+d),l",       "res 6,(iy+d)",         "res 6,(iy+d),a",
"res 7,(iy+d),b",       "res 7,(iy+d),c",       "res 7,(iy+d),d",       "res 7,(iy+d),e",
"res 7,(iy+d),h",       "res 7,(iy+d),l",       "res 7,(iy+d)",         "res 7,(iy+d),a",

"set 0,(iy+d),b",       "set 0,(iy+d),c",       "set 0,(iy+d),d",       "set 0,(iy+d),e",
"set 0,(iy+d),h",       "set 0,(iy+d),l",       "set 0,(iy+d)",         "set 0,(iy+d),a",
"set 1,(iy+d),b",       "set 1,(iy+d),c",       "set 1,(iy+d),d",       "set 1,(iy+d),e",
"set 1,(iy+d),h",       "set 1,(iy+d),l",       "set 1,(iy+d)",         "set 1,(iy+d),a",

"set 2,(iy+d),b",       "set 2,(iy+d),c",       "set 2,(iy+d),d",       "set 2,(iy+d),e",
"set 2,(iy+d),h",       "set 2,(iy+d),l",       "set 2,(iy+d)",         "set 2,(iy+d),a",
"set 3,(iy+d),b",       "set 3,(iy+d),c",       "set 3,(iy+d),d",       "set 3,(iy+d),e",
"set 3,(iy+d),h",       "set 3,(iy+d),l",       "set 3,(iy+d)",         "set 3,(iy+d),a",

"set 4,(iy+d),b",       "set 4,(iy+d),c",       "set 4,(iy+d),d",       "set 4,(iy+d),e",
"set 4,(iy+d),h",       "set 4,(iy+d),l",       "set 4,(iy+d)",         "set 4,(iy+d),a",
"set 5,(iy+d),b",       "set 5,(iy+d),c",       "set 5,(iy+d),d",       "set 5,(iy+d),e",
"set 5,(iy+d),h",       "set 5,(iy+d),l",       "set 5,(iy+d)",         "set 5,(iy+d),a",

"set 6,(iy+d),b",       "set 6,(iy+d),c",       "set 6,(iy+d),d",       "set 6,(iy+d),e",
"set 6,(iy+d),h",       "set 6,(iy+d),l",       "set 6,(hl)",           "set 6,(iy+d),a",
"set 7,(iy+d),b",       "set 7,(iy+d),c",       "set 7,(iy+d),d",       "set 7,(iy+d),e",
"set 7,(iy+d),h",       "set 7,(iy+d),l",       "set 7,(hl)",           "set 7,(iy+d),a",
            ];

        public static string GetNimo(byte n1, byte n2 = 0, byte n3 = 0)
        {
            if (n1 == 0xed)
                return opcodeTableED[n2];

            if (n1 == 0xcb)
                return opcodeTableCB[n2];

            if (n1 == 0xdd)
            {
                if (n2 != 0xcb) return opcodeTableDD[n2];
                else return opcodeTableDDCB[n3];
            }

            if (n1 == 0xfd)
            {
                if (n2 != 0xcb) return opcodeTableFD[n2];
                else return opcodeTableFDCB[n3];
            }

            return opcodeTable[n1];
        }
    }
}
