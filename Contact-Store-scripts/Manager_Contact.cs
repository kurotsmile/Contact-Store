using Carrot;
using System.Collections;
using UnityEngine;

public class Manager_Contact : MonoBehaviour
{
    [Header("Obj main")]
    public App_Contacts app;

    [Header("Icon")]
    public Sprite icon_contact;
    public Sprite icon_save;
    public Sprite icon_sort_name;

    private string s_type_order = "name";
    private IList list_contacts;
    private bool is_read_cache = false;
    private Carrot_Box box_info = null;

    public void list()
    {
        this.app.carrot.clear_contain(this.app.area_body_main);
        this.app.add_item_loading();

        if (this.app.carrot.is_offline()) this.is_read_cache = true;

        if (this.is_read_cache == false)
        {
            this.Get_data_from_server();
        }
        else
        {
            string s_list_data = PlayerPrefs.GetString("contacts_" + this.s_type_order + "_" + this.app.carrot.lang.get_key_lang(), "");
            if (s_list_data == "")
                this.Get_data_from_server();
            else
                this.get_data_from_cache(s_list_data);
        }
    }

    private void Get_data_from_server()
    {
        StructuredQuery q = new("user-" + this.app.carrot.lang.get_key_lang());
        q.Add_where("status_share", Query_OP.EQUAL, "0");
        q.Set_limit(60);
        app.carrot.server.Get_doc(q.ToJson(), Act_get_data_from_server_done, Act_get_data_from_server_fail);
    }

    private void Act_get_data_from_server_done(string s_data)
    {
        this.app.carrot.clear_contain(this.app.area_body_main);

        Fire_Collection fc = new(s_data);
        if (!fc.is_null)
        {
            this.list_contacts = (IList)Carrot.Json.Deserialize("[]");

            for(int i=0;i<fc.fire_document.Length;i++)
            {
                IDictionary data_contact = fc.fire_document[i].Get_IDictionary();
                data_contact["user_id"] = data_contact["id"].ToString();
                data_contact["id"] = data_contact["id"].ToString();
                data_contact["type_item"] = "contact";
                if (data_contact["type_item"] != null) data_contact.Remove("rates");
                if (data_contact["backup_contact"] != null) data_contact.Remove("backup_contact");
                this.list_contacts.Add(data_contact);
            }

            PlayerPrefs.SetString("contacts_" + this.s_type_order + "_" + this.app.carrot.lang.get_key_lang(), Carrot.Json.Serialize(this.list_contacts));
            this.is_read_cache = true;

            this.Add_item_title();
            this.show_list_data_contacts(this.list_contacts);
        }
        else
        {
            this.app.add_item_none();
        }
    }

    private void Act_get_data_from_server_fail(string s_error)
    {
        this.app.carrot.clear_contain(this.app.area_body_main);
        Carrot.Carrot_Box_Item item_error = this.app.add_item_title_list("Error");
        item_error.set_icon_white(this.app.carrot.icon_carrot_bug);
        item_error.set_tip("Operation failed, please try again!");
    }

    private void get_data_from_cache(string s_data_json)
    {
        this.app.carrot.clear_contain(this.app.area_body_main);

        this.list_contacts = (IList)Carrot.Json.Deserialize(s_data_json);
        this.Add_item_title();
        this.show_list_data_contacts(this.list_contacts);
    }

    private void Add_item_title()
    {
        Carrot.Carrot_Box_Item item_title = this.app.add_item_title_list(PlayerPrefs.GetString("contact", "Contact") +" (" + this.app.carrot.lang.get_key_lang() + ")");
        item_title.set_tip(PlayerPrefs.GetString("contact_list", "List of contacts in your country ") + "("+this.list_contacts.Count+" "+ PlayerPrefs.GetString("contact", "Contact")+")");
        Carrot.Carrot_Box_Btn_Item btn_sort_name = item_title.create_item();
        btn_sort_name.set_icon(this.icon_sort_name);
        btn_sort_name.set_icon_color(Color.white);
        btn_sort_name.set_color(this.app.carrot.color_highlight);
        btn_sort_name.set_act(() => this.Sort());
    }


    public void show_list_data_contacts(IList list_contacts)
    {
        this.app.scrollbar_on_top();
        foreach (IDictionary data_contact in list_contacts)
        {
            var id_contact = ""; if (data_contact["id"] != null) id_contact = data_contact["id"].ToString();
            var lang_contact = "en"; if (data_contact["lang"] != null) lang_contact = data_contact["lang"].ToString();
            string s_tip = "";

            GameObject obj_contact_item = Instantiate(this.app.prefab_contact_main_item);
            obj_contact_item.transform.SetParent(this.app.area_body_main);
            obj_contact_item.transform.localPosition = new Vector3(0, 0, 0);
            obj_contact_item.transform.localScale = new Vector3(1f, 1f, 1f);
            obj_contact_item.transform.localRotation = Quaternion.Euler(Vector3.zero);

            Carrot.Carrot_Box_Item item_contact = obj_contact_item.GetComponent<Carrot.Carrot_Box_Item>();
            item_contact.on_load(this.app.carrot);
            item_contact.check_type();
            item_contact.set_icon(this.icon_contact);
            if (data_contact["name"] != null) item_contact.set_title(data_contact["name"].ToString());

            if (data_contact["phone"] != null)
            {
                var s_phone = data_contact["phone"].ToString();
                if (s_phone != "")
                {
                    Carrot.Carrot_Box_Btn_Item btn_call = item_contact.create_item();
                    if (data_contact["sex"] != null)
                    {
                        if (data_contact["sex"].ToString() == "0")
                            btn_call.set_icon(this.app.icon_call_boy);
                        else
                            btn_call.set_icon(this.app.icon_call_girl);
                    }
                    else
                    {
                        btn_call.set_icon(this.app.icon_call_boy);
                    }
                    btn_call.set_color(this.app.carrot.color_highlight);
                    btn_call.set_act(() => this.Call(s_phone));
                    s_tip = data_contact["phone"].ToString();
                }
            }


            if (s_tip == "") if (data_contact["email"] != null) s_tip = data_contact["email"].ToString();

            if (data_contact["email"] != null)
            {
                var s_email = data_contact["email"].ToString();
                if (s_email != "")
                {
                    Carrot.Carrot_Box_Btn_Item btn_email = item_contact.create_item();
                    btn_email.set_icon(this.app.carrot.icon_carrot_mail);
                    btn_email.set_color(this.app.carrot.color_highlight);
                    btn_email.set_act(() => this.Mail(s_email));
                }
                else
                {
                    s_tip = "Incognito";
                }
            }

            item_contact.set_tip(s_tip);

            if (data_contact["type_item"] != null)
            {
                string type_item = data_contact["type_item"].ToString();

                if(type_item=="contact")
                {
                    Carrot.Carrot_Box_Btn_Item btn_save = item_contact.create_item();
                    btn_save.set_icon(this.icon_save);
                    btn_save.set_color(this.app.carrot.color_highlight);
                    btn_save.set_act(() => this.app.book_contact.add(data_contact));
                }

                if (type_item=="phonebook")
                {
                    Carrot.Carrot_Box_Btn_Item btn_del = item_contact.create_item();
                    btn_del.set_icon(this.app.carrot.sp_icon_del_data);
                    btn_del.set_color(Color.red);
                    btn_del.set_act(() => this.app.book_contact.delete(int.Parse(data_contact["index"].ToString())));
                }
            }

            item_contact.set_act(() => this.view_info_contact(data_contact));
        }
    }

    public void view_info_contact(IDictionary data)
    {
        var id_contact = ""; if (data["id"] != null) id_contact = data["id"].ToString();
        var lang_contact = "en"; if (data["lang"] != null) lang_contact = data["lang"].ToString();

        this.app.play_sound(0);
        this.app.carrot.ads.show_ads_Interstitial();
        if (this.box_info != null) this.box_info.close();
        this.box_info=this.app.carrot.user.Show_info_user_by_data(data);
        Carrot.Carrot_Box_Btn_Panel panel_tool=box_info.create_panel_btn();

        if (data["phone"] != null)
        {
            var s_phone = data["phone"].ToString();
            Carrot.Carrot_Button_Item btn_call = panel_tool.create_btn("btn_call");
            if (data["sex"] != null)
            {
                if (data["sex"].ToString() == "0")
                    btn_call.set_icon_white(this.app.icon_call_boy);
                else
                    btn_call.set_icon_white(this.app.icon_call_girl);
            }
            else
            {
                btn_call.set_icon_white(this.app.icon_call_boy);
            }
            btn_call.set_label_color(Color.white);
            btn_call.set_label(PlayerPrefs.GetString("call", "Call"));
            btn_call.set_bk_color(this.app.carrot.color_highlight);
            btn_call.set_act_click(() => this.Call(s_phone));
        }

        if (data["email"] != null)
        {
            var s_mail = data["email"].ToString();
            if (s_mail != "")
            {
                Carrot.Carrot_Button_Item btn_mail = panel_tool.create_btn("btn_mail");
                btn_mail.set_icon_white(this.app.carrot.icon_carrot_mail);
                btn_mail.set_label_color(Color.white);
                btn_mail.set_label(PlayerPrefs.GetString("send_mail", "Send Mail"));
                btn_mail.set_bk_color(this.app.carrot.color_highlight);
                btn_mail.set_act_click(() => this.Mail(s_mail));
            }
        }

        Carrot.Carrot_Button_Item btn_share = panel_tool.create_btn("btn_share");
        btn_share.set_icon_white(this.app.carrot.sp_icon_share);
        btn_share.set_label_color(Color.white);
        btn_share.set_label(PlayerPrefs.GetString("share", "Share"));
        btn_share.set_bk_color(this.app.carrot.color_highlight);
        btn_share.set_act_click(() => this.Share(id_contact,lang_contact));


        Carrot.Carrot_Box_Btn_Panel panel_act = box_info.create_panel_btn();

        if (data["type_item"] != null)
        {
            string type_item = data["type_item"].ToString();
            if (type_item == "contact")
            {
                Carrot.Carrot_Button_Item btn_save = panel_act.create_btn("btn_save");
                btn_save.set_icon_white(this.icon_save);
                btn_save.set_label_color(Color.white);
                btn_save.set_label(PlayerPrefs.GetString("save_contact","Save Contact"));
                btn_save.set_bk_color(this.app.carrot.color_highlight);
                btn_save.set_act_click(() => this.app.book_contact.add(data));
            }

            if (type_item == "phonebook")
            {
                if (data["index"] != null)
                {
                    Carrot.Carrot_Button_Item btn_del = panel_act.create_btn("btn_del");
                    btn_del.set_icon_white(this.app.carrot.sp_icon_del_data);
                    btn_del.set_label_color(Color.white);
                    btn_del.set_label(PlayerPrefs.GetString("del_contact", "Delete contact"));
                    btn_del.set_bk_color(Color.red);
                    btn_del.set_act_click(() => this.app.book_contact.delete(int.Parse(data["index"].ToString())));

                    Carrot.Carrot_Button_Item btn_edit= panel_act.create_btn("btn_edit");
                    btn_edit.set_icon_white(this.app.carrot.user.icon_user_edit);
                    btn_edit.set_label_color(Color.white);
                    btn_edit.set_label(PlayerPrefs.GetString("edit", "Edit"));
                    btn_edit.set_bk_color(this.app.carrot.color_highlight);
                    btn_edit.set_act_click(() => this.app.book_contact.edit(int.Parse(data["index"].ToString())));
                }
            }
        }

        Carrot.Carrot_Button_Item btn_close = panel_act.create_btn("btn_close");
        btn_close.set_icon_white(this.app.carrot.icon_carrot_cancel);
        btn_close.set_label_color(Color.white);
        btn_close.set_label(PlayerPrefs.GetString("cancel", "Cancel"));
        btn_close.set_bk_color(this.app.carrot.color_highlight);
        btn_close.set_act_click(() => box_info.close());
    }

    private void Call(string s_phone)
    {
        Application.OpenURL("tel://" + s_phone);
    }

    private void Mail(string s_mail)
    {
        Application.OpenURL("mailto:" + s_mail);
    }

    private void Share(string id_contact,string lang_contact)
    {
        string url_share = this.app.carrot.mainhost+"?p=phone_book&id="+id_contact+"&user_lang="+lang_contact;
        this.app.carrot.show_share(url_share, "Share this contacts with everyone");
    }

    private void Sort()
    {
        if (this.s_type_order == "name")
            this.s_type_order = "phone";
        else
            this.s_type_order = "name";
        this.list();
    }

    public IList get_list_contacts()
    {
        return this.list_contacts;
    }

    public Carrot.Carrot_Box get_box_info()
    {
        return this.box_info;
    }
}
