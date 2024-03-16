using Carrot;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum Search_Contacts_Type {search_name_or_phone,search_name,search_phone}
public class Search_Contacts : MonoBehaviour
{
    [Header("Obj Main")]
    public App_Contacts app;

    [Header("Obj Search")]
    public Sprite icon_search_return;

    private Carrot_Box box_search_advanced = null;
    private Carrot_Box_Item item_name;
    private Carrot_Box_Item item_phone;
    private Carrot_Box_Item item_sex;

    private string s_key_search_temp = "";
    private Search_Contacts_Type type_search = Search_Contacts_Type.search_name_or_phone;

    public void search_contact(string s_key)
    {
        s_key = s_key.Trim();
        this.type_search = Search_Contacts_Type.search_name_or_phone;
        if (s_key != "")
        {
            app.carrot.ads.show_ads_Interstitial();
            app.carrot.show_loading();
            app.inp_search.text = "";
            this.s_key_search_temp = s_key;

            if (app.carrot.is_offline())
                Search_name_or_phone_offline(s_key);
            else
                Search_name_online(s_key);
        }
        else
        {
            this.app.carrot.show_msg(PlayerPrefs.GetString("search","Search"), PlayerPrefs.GetString("search_key_error","Search keywords cannot be empty"), Msg_Icon.Alert);
        }
    }

    private void Search_name_or_phone_offline(string s_key)
    {
        app.carrot.show_loading();
        app.call.panel_call.SetActive(false);
        IList list = this.app.manager_contact.get_list_contacts();
        IList list_search = (IList)Json.Deserialize("[]");

        foreach (IDictionary contact in list)
        {
            bool is_add = false;
            if (contact["name"] != null)
            {
                string s_name = contact["name"].ToString();
                if (s_name.ToLower().Contains(s_key.ToLower()))
                {
                    list_search.Add(contact);
                    is_add = true;
                }
            }

            if (is_add == false)
            {
                if (contact["phone"] != null)
                {
                    string s_phone = contact["phone"].ToString();
                    if (s_phone.Contains(s_key)) list_search.Add(contact);
                }
            }
        }
        this.app.inp_search.text = "";
        this.app.carrot.clear_contain(this.app.area_body_main);

        Carrot_Box_Item item_search_result = this.app.add_item_title_list("Search Results");
        item_search_result.set_icon(this.icon_search_return);
        item_search_result.set_tip("Contacts found:" + list_search.Count);

        this.app.manager_contact.show_list_data_contacts(list_search);
    }

    private void Search_name_online(string s_key)
    {
        this.type_search = Search_Contacts_Type.search_name;
        StructuredQuery q = new("user-" + this.app.carrot.lang.get_key_lang());
        q.Add_where("status_share", Query_OP.EQUAL, "0");
        q.Add_where("name", Query_OP.EQUAL, s_key);
        q.Set_limit(20);
        app.carrot.server.Get_doc(q.ToJson(), Act_advanced_search_done, Act_advanced_search_fail);
    }

    public void Search_phone_number(string s_phone)
    {
        this.type_search = Search_Contacts_Type.search_phone;
        StructuredQuery q = new("user-" + this.app.carrot.lang.get_key_lang());
        q.Add_where("status_share", Query_OP.EQUAL, "0");
        q.Add_where("phone", Query_OP.EQUAL, s_phone);
        q.Set_limit(20);
        app.carrot.server.Get_doc(q.ToJson(), Act_advanced_search_done, Act_advanced_search_fail);
    }

    public void Search_email(string s_phone)
    {
        StructuredQuery q = new("user-" + this.app.carrot.lang.get_key_lang());
        q.Add_where("status_share", Query_OP.EQUAL, "0");
        q.Add_where("email", Query_OP.EQUAL, s_phone);
        q.Set_limit(20);
        app.carrot.server.Get_doc(q.ToJson(), Act_advanced_search_done, Act_advanced_search_fail);
    }

    public void advanced_search()
    {
        if (this.box_search_advanced != null) this.box_search_advanced.close();
        this.box_search_advanced = this.app.carrot.Create_Box("advanced_search");
        box_search_advanced.set_title(PlayerPrefs.GetString("advanced_search","Advanced Search"));
        box_search_advanced.set_icon(this.app.icon_search_contact);

        item_name=box_search_advanced.create_item("item_name");
        item_name.set_icon(this.app.carrot.user.icon_user_name);
        item_name.set_type(Carrot.Box_Item_Type.box_value_input);
        item_name.check_type();
        item_name.set_title(PlayerPrefs.GetString("user_name","Name"));
        item_name.set_tip("Search by name");

        item_phone = box_search_advanced.create_item("item_phone");
        item_phone.set_icon(this.app.carrot.icon_carrot_phone);
        item_phone.set_type(Carrot.Box_Item_Type.box_number_input);
        item_phone.check_type();
        item_phone.set_title(PlayerPrefs.GetString("user_phone","Phone"));
        item_phone.set_tip("Search by phone number");

        item_sex = box_search_advanced.create_item("item_sex");
        item_sex.set_icon(this.app.carrot.icon_carrot_sex);
        item_sex.set_type(Carrot.Box_Item_Type.box_value_dropdown);
        item_sex.check_type();
        item_sex.set_title(PlayerPrefs.GetString("user_sex","gender"));
        item_sex.set_tip("Search by gender");
        item_sex.dropdown_val.ClearOptions();
        item_sex.dropdown_val.options.Add(new Dropdown.OptionData() { text = PlayerPrefs.GetString("user_sex_no", "No select") });
        item_sex.dropdown_val.options.Add(new Dropdown.OptionData() { text = PlayerPrefs.GetString("user_sex_boy", "Male") });
        item_sex.dropdown_val.options.Add(new Dropdown.OptionData() { text = PlayerPrefs.GetString("user_sex_girl", "Female") });

        Carrot_Box_Btn_Panel panel=this.box_search_advanced.create_panel_btn();
        Carrot_Button_Item btn_done=panel.create_btn("btn_done");
        btn_done.set_icon_white(this.app.carrot.icon_carrot_done);
        btn_done.set_bk_color(this.app.carrot.color_highlight);
        btn_done.set_label_color(Color.white);
        btn_done.set_label(PlayerPrefs.GetString("done","Done"));
        btn_done.set_act_click(() => this.Act_advanced_search());

        Carrot_Button_Item btn_cancel = panel.create_btn("btn_cancel");
        btn_cancel.set_icon_white(this.app.carrot.icon_carrot_cancel);
        btn_cancel.set_bk_color(this.app.carrot.color_highlight);
        btn_cancel.set_label_color(Color.white);
        btn_cancel.set_label(PlayerPrefs.GetString("cancel","Cancel"));
        btn_cancel.set_act_click(() => this.box_search_advanced.close());
    }

    private void Act_advanced_search()
    {
        string s_name = item_name.get_val();
        string s_phone = item_phone.get_val();
        string s_sex = item_sex.get_val();

        if (item_name.get_val() == "" && item_phone.get_val() == "")
        {
            this.app.carrot.show_msg(PlayerPrefs.GetString("advanced_search", "Advanced Search"), PlayerPrefs.GetString("search_key_error", "Search keywords cannot be empty"), Msg_Icon.Alert);
            return;
        }

        StructuredQuery q = new("user-" + this.app.carrot.lang.get_key_lang());
        q.Add_where("status_share", Query_OP.EQUAL, "0");
        if (s_name != "") q.Add_where("name",Query_OP.EQUAL,s_name);
        if (s_phone != "") q.Add_where("phone",Query_OP.EQUAL,s_phone);
        if (s_sex != "0")
        {
            if (s_sex == "1") q.Add_where("sex", Query_OP.EQUAL, "0");
            if (s_sex == "2") q.Add_where("sex", Query_OP.EQUAL, "1");
        }
        q.Set_limit(20);
        app.carrot.server.Get_doc(q.ToJson(), Act_advanced_search_done, Act_advanced_search_fail);
    }

    private void Act_advanced_search_done(string s_data)
    {
        app.carrot.hide_loading();
        app.call.panel_call.SetActive(false);
        app.deviceOrientation.Stop_check();

        Fire_Collection fc = new(s_data);

        if (!fc.is_null)
        {
            if (this.box_search_advanced != null) this.box_search_advanced.close();
            this.app.carrot.clear_contain(this.app.area_body_main);
            Carrot_Box_Item item_search_result = this.app.add_item_title_list(this.s_key_search_temp);
            item_search_result.set_icon(this.icon_search_return);
            item_search_result.set_tip(fc.fire_document.Length.ToString());

            IList list_search = (IList)Json.Deserialize("[]");
            for (int i = 0; i < fc.fire_document.Length; i++)
            {
                IDictionary data_contact = fc.fire_document[i].Get_IDictionary();
                data_contact["user_id"] = data_contact["id"].ToString();
                data_contact["type_item"] = "contact";
                list_search.Add(data_contact);
            }
            this.app.manager_contact.show_list_data_contacts(list_search);
        }
        else
        {
            if (this.type_search == Search_Contacts_Type.search_name)
            {
                app.carrot.hide_loading();
                this.Search_phone_number(this.s_key_search_temp);
            }
            else
            {
                this.app.carrot.show_msg(PlayerPrefs.GetString("advanced_search", "Advanced Search"), PlayerPrefs.GetString("list_none_tip", "There are no items in the list"), Msg_Icon.Alert);
            }
            
        }
    }

    private void Act_advanced_search_fail(string s_error)
    {
        app.carrot.hide_loading();
        app.call.panel_call.SetActive(false);
        app.deviceOrientation.Stop_check();

        if (this.box_search_advanced != null) this.box_search_advanced.close();
        this.app.carrot.show_msg(PlayerPrefs.GetString("advanced_search", "Advanced Search"),s_error, Msg_Icon.Error);
    }
}
