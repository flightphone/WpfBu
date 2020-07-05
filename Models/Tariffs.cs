using System;
using System.Collections.Generic;
using System.Text;

namespace WpfBu.Models
{
    class Tariffs : Finder
    {
        public override void start(object o)
        {
            SQLParams = new Dictionary<string, object>()
            {
                { "@AP_IATA", "VKO" },
                { "@AL_UTG", "<ВСЕ>" },
                { "@DateFlt", 1 }
            };
            base.start(o);
        }
    }


    public class Flights : Finder
    {

        public Flights()
        {
            SQLText = "select   *, '' PR_Status, 0 M from v_FlightCardsFormat where FC_Date between @DateStart and @DateFinish order by FC_Date";
            SQLParams = new Dictionary<string, object>()
            {
                { "@DateStart", new DateTime(2017, 3, 1) },
                { "@DateFinish", new DateTime(2017, 3, 15) }

            };
        }

    }

}
