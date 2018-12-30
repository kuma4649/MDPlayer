namespace MDPlayer.Driver.MUCOM88.ver1_0
{
    public class errmsg
    {

        //ORG	08800H
        //08B00H ﾏﾃﾞﾅﾗ OK

        public static string[] ERRORTB = new string[] {
             SYNTAX
            , ILLEG
            , NEXTWF
            , FORWNE
            , V_OVER
            , O_OVER
            , NOTDIV
            , NOTRJP
            , NENGPR
            , DEFRCH
            , MAXLOP
            , NOTFND
            , OVFLOW
            , DEFRMD
            , MEMEND
            , NOTMAC
            , ENDMAC
        };

        public static string SYNTAX = "ﾌﾞﾝﾎﾟｳ ﾆ ｱﾔﾏﾘ ｶﾞ ｱﾘﾏｽ";
        public static string ILLEG = "ﾊﾟﾗﾒｰﾀﾉ ｱﾀｲ ｶﾞ ｲｼﾞｮｳﾃﾞｽ";
        public static string NEXTWF = " ]  ﾉ ｶｽﾞ ｶﾞ ｵｵｽｷﾞﾏｽ";
        public static string FORWNE = " [  ﾉ ｶｽﾞ ｶﾞ ｵｵｽｷﾞﾏｽ";
        public static string V_OVER = "ｵﾝｼｮｸ ﾉ ｶｽﾞ ｶﾞ ｵｵｽｷﾞﾏｽ";
        public static string O_OVER = "ｵｸﾀｰﾌﾞ ｶﾞ ｷﾃｲﾊﾝｲ ｦ ｺｴﾃﾏｽ";
        public static string NOTDIV = "ﾘｽﾞﾑ ｶﾞ ｸﾛｯｸ ﾉ ﾁｦ ｺｴﾃﾏｽ";
        public static string NOTRJP = "[ ] ﾅｲ ﾆ / ﾊ ﾋﾄﾂﾀﾞｹﾃﾞｽ";
        public static string NENGPR = "ﾊﾟﾗﾒｰﾀ ｶﾞ ﾀﾘﾏｾﾝ";
        public static string DEFRCH = "ｺﾉﾁｬﾝﾈﾙ ﾃﾞﾊ ﾂｶｴﾅｲ ｺﾏﾝﾄﾞｶﾞｱﾘﾏｽ";
        public static string MAXLOP = "[ ] ﾉ ﾈｽﾄﾊ 16ｶｲ ﾏﾃﾞﾃﾞｽ";
        public static string NOTFND = "ｵﾝｼｮｸ ﾃﾞｰﾀ ｶﾞ ﾗｲﾌﾞﾗﾘ ﾆ ｿﾝｻﾞｲｼﾏｾﾝ";
        public static string OVFLOW = "ｶｳﾝﾀｰ ｵｰﾊﾞｰﾌﾛｰ";
        public static string DEFRMD = "ﾓｰﾄﾞ ｴﾗｰ";
        public static string MEMEND = "ｵﾌﾞｼﾞｪｸﾄ ﾘｮｳｲｷ ｦ ｺｴﾏｼﾀ";
        public static string NOTMAC = "ﾃｲｷﾞｼﾃﾅｲ ﾏｸﾛﾅﾝﾊﾞｰｶﾞｱﾘﾏｽ";
        public static string ENDMAC = "ﾏｸﾛｴﾝﾄﾞｺｰﾄﾞ ｶﾞ ｱﾘﾏｾﾝ";

    }
}
