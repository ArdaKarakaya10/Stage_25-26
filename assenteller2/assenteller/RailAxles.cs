using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using assenteller;

namespace assenteller
{
    public class RailAxles
    {
        //private DatabaseHandler dbHandler = new DatabaseHandler();
        
        public List<data_wps> GetOldToNewList(List<data_wps> dataWPS)
        {
            List<data_wps> list_new_to_old = dataWPS;
            /*List<data_wps> list_old_to_new = new List<data_wps>();

            // Omkeren
            for (int i = list_new_to_old.Count - 1; i >= 0; i--)
            {
                list_old_to_new.Add(list_new_to_old[i]);
            }
            */
            return list_new_to_old;
        }
    }
}
