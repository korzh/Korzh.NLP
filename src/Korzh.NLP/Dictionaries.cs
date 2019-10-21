namespace Korzh.NLP
{
    public static class Dictionaries
    {
        private static readonly string _defaultEnglishStopWords =
            "a,able,about,above,across,after,afterwards,again,against,all,almost,alone,along,already,also,althought,always," +
            "am,among,amongst,amoungst,amount,an,and,another,any,anyhow,anyone,anything,anyway,anywhere,are,around,as,at," +
            "back,be,became,because,become,becomes,becoming,been,before,beforehand,behind,being,below,beside,besides,between," +
            "beyond,both,bottom,but,by,can,cannot,cant,co,con,could,couldnt,cry,describe,detail,dear,did,do,does,done,down,due," +
            "during,each,eight,either,eleven,else,elsewhere,etc,even,ever,every,everyone,everything,everywhere,few,fifteen,find" +
            "for,found,full,further,from,get,go,got,had,has,hasnt,have,he,her,hers,him,his,how,however,i,ie,if,in,indeed,into,is,it,its," +
            "just,least,let,like,likely,many,may,me,might,more,moreover,most,mostly,mucn,must,my,neither,never,no,nobody,nor,not,nothing,now," +
            "of,off,often,on,once,one,only,or,other,others,our,ours,own,rather,said,say,says,she,should,since,so,some," +
            "than,that,the,their,them,then,there,these,they,this,tis,to,too,twas,us,very,via" +
            "wants,was,we,were,what,when,where,which,while,who,whom,why,will,with,would,yet,you,your,yours,yourself,yourselves";

        private static readonly string _defaultRussianStopWords =
            "а,без,более,больше,будет,будто,бы,был,была,были,было,быть,в,вам,вас,вдруг,ведь,во,вот," +
            "впрочем,все,всегда,всего,всех,всю,вы,г,где,говорил,да,даже,два,для,до,другой,его,ее,ей,ему,если," +
            "есть,еще,ж,же,жизнь,за,зачем,здесь,и,из,из-за,или,им,иногда,их,к,кажеться,как,какая,какой,когда," +
            "конечно,которого,которые,кто,куда,ли,лучше,между,меня,мне,много,может,можно,мой,моя,мы,на,над,надо," +
            "наконец,нас,не,него,нее,ней,нельзя,нет,ни,нибудь,никогда,ним,них,ничего,но,ну,о,об,один,он,она,они,опять," +
            "от,перед,по,под,после,потом,потому,почти,при,про,раз,разве,с,сам,свое,свою,себе,сегодня,сейчас,сказал,сказала," +
            "сказать,со,совсем,так,такой,там,тебя,тем,теперь,то,тогда,того,тоже,только,том,три,тут,ты,у,уж,уже,хорошо," +
            "хоть,чего,человек,чем,через,что,чтоб,чтобы,чуть,эти,этого,этой,этом,этот,эту,я";

        public static string EnglishStopWords = _defaultEnglishStopWords;
        public static string RussianStopWords = _defaultRussianStopWords;    
    }
}
