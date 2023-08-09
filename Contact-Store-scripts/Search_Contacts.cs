using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Search_Contacts : MonoBehaviour
{
    [Header("Obj Main")]
    public App_Contacts app;

    [Header("Obj Search")]
    public Sprite icon_search_return;

    private Carrot.Carrot_Box box_search_advanced = null;

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
            this.app.inp_search.text = "";
            this.app.carrot.clear_contain(this.app.area_body_main);

            Carrot.Carrot_Box_Item item_search_result=this.app.add_item_title_list("Search Results");
            item_search_result.set_icon(this.icon_search_return);
            item_search_result.set_tip("Contacts found:" + list_search.Count);
            this.app.manager_contact.show_list_data_contacts(list_search);
        }
        else
        {
            this.app.carrot.show_msg("Contact Search", "Search keywords cannot be empty", Carrot.Msg_Icon.Alert);
        }
    }

    public void advanced_search()
    {
        if (this.box_search_advanced != null) this.box_search_advanced.close();
        this.box_search_advanced = this.app.carrot.Create_Box("advanced_search");
        box_search_advanced.set_title("Advanced Search");
        box_search_advanced.set_icon(this.app.icon_search_contact);

        Carrot.Carrot_Box_Item item_name=box_search_advanced.create_item("item_name");
        item_name.set_icon(this.app.carrot.user.icon_user_name);
        item_name.set_type(Carrot.Box_Item_Type.box_value_input);
        item_name.check_type();
        item_name.set_title("Name");
        item_name.set_tip("Search by name");

        Carrot.Carrot_Box_Item item_phone = box_search_advanced.create_item("item_phone");
        item_phone.set_icon(this.app.carrot.icon_carrot_phone);
        item_phone.set_type(Carrot.Box_Item_Type.box_number_input);
        item_phone.check_type();
        item_phone.set_title("Phone");
        item_phone.set_tip("Search by phone number");

        Carrot.Carrot_Box_Item item_sex = box_search_advanced.create_item("item_sex");
        item_sex.set_icon(this.app.carrot.icon_carrot_sex);
        item_sex.set_type(Carrot.Box_Item_Type.box_value_dropdown);
        item_sex.check_type();
        item_sex.set_title("gender");
        item_sex.set_tip("Search by gender");
        item_sex.dropdown_val.ClearOptions();
        item_sex.dropdown_val.options.Add(new Dropdown.OptionData() { text = PlayerPrefs.GetString("user_sex_no", "No select") });
        item_sex.dropdown_val.options.Add(new Dropdown.OptionData() { text = PlayerPrefs.GetString("user_sex_boy", "Male") });
        item_sex.dropdown_val.options.Add(new Dropdown.OptionData() { text = PlayerPrefs.GetString("user_sex_girl", "Female") });

        Carrot.Carrot_Box_Btn_Panel panel=this.box_search_advanced.create_panel_btn();
        Carrot.Carrot_Button_Item btn_done=panel.create_btn("btn_done");
        btn_done.set_icon_white(this.app.carrot.icon_carrot_done);
        btn_done.set_bk_color(this.app.carrot.color_highlight);
        btn_done.set_label_color(Color.white);
        btn_done.set_label("Done");
        btn_done.set_act_click(() => this.act_advanced_search());

        Carrot.Carrot_Button_Item btn_cancel = panel.create_btn("btn_cancel");
        btn_cancel.set_icon_white(this.app.carrot.icon_carrot_cancel);
        btn_cancel.set_bk_color(this.app.carrot.color_highlight);
        btn_cancel.set_label_color(Color.white);
        btn_cancel.set_label("Cancel");
        btn_cancel.set_act_click(() => this.box_search_advanced.close());
    }

    private void act_advanced_search()
    {
        if(this.box_search_advanced!=null) this.box_search_advanced.close();
    }
}
