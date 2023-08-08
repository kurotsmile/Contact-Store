using System.Collections;
using UnityEngine;

public class Search_Contacts : MonoBehaviour
{
    [Header("Obj Main")]
    public App_Contacts app;

    public void search_contact(string s_key)
    {
        if (s_key != "")
        {
            IList list = this.app.manager_contact.get_list_contacts();
            IList list_search = (IList) Carrot.Json.Deserialize("[]");

            foreach (IDictionary contact in list)
            {
                bool is_add = false;
                if (contact["name"]!=null)
                {
                    string s_name = contact["name"].ToString();
                    if (s_name.Contains(s_key))
                    {
                        list_search.Add(contact);
                        is_add = true;
                    }
                }

                if (is_add == false)
                {
                    if (contact["phone"]!=null)
                    {
                        string s_phone = contact["phone"].ToString();
                        if (s_phone.Contains(s_key)) list_search.Add(contact);
                    }
                }
            }

            this.app.carrot.clear_contain(this.app.area_body_main);
            this.app.manager_contact.show_list_data_contacts(list_search);
        }
        else
        {
            this.app.carrot.show_msg("Contact Search", "Search keywords cannot be empty", Carrot.Msg_Icon.Alert);
        }
    }
}
