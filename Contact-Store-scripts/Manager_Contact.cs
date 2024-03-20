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

    private Query_Order_Direction order_by = Query_Order_Direction.ASCENDING;
    private IList list_contacts;
    private bool is_read_cache = false;
    private Carrot_Box box_info = null;

    public void List()
    {
        app.carrot.clear_contain(app.area_body_main);
        app.Add_item_loading();

        if (app.carrot.is_offline()) this.is_read_cache = true;

        if (this.is_read_cache == false)
        {
            this.Get_data_from_server();
        }
        else
        {
            string s_list_data = PlayerPrefs.GetString("contacts_" + this.order_by.ToString() + "_" + app.carrot.lang.Get_key_lang(), "");
            if (s_list_data == "")
                this.Get_data_from_server();
            else
                this.get_data_from_cache(s_list_data);
        }
    }

    private void Get_data_from_server()
    {
        StructuredQuery q = new("user-" + app.carrot.lang.Get_key_lang());
        q.Add_select("name");
        q.Add_select("avatar");
        q.Add_select("lang");
        q.Add_select("email");
        q.Add_select("phone");
        q.Add_select("sex");
        q.Add_select("status_share");
        q.Add_where("status_share", Query_OP.EQUAL, "0");
        q.Add_order("name",this.order_by);
        q.Set_limit(60);
        app.carrot.server.Get_doc(q.ToJson(), Act_get_data_from_server_done, Act_get_data_from_server_fail);
    }

    private void Act_get_data_from_server_done(string s_data)
    {
        app.carrot.clear_contain(app.area_body_main);

        Fire_Collection fc = new(s_data);
        if (!fc.is_null)
        {
            this.list_contacts = (IList)Json.Deserialize("[]");

            for(int i=0;i<fc.fire_document.Length;i++)
            {
                IDictionary data_contact = fc.fire_document[i].Get_IDictionary();
                data_contact["user_id"] = data_contact["id"].ToString();
                data_contact["type_item"] = "contact";
                if (data_contact["type_item"] != null) data_contact.Remove("rates");
                if (data_contact["backup_contact"] != null) data_contact.Remove("backup_contact");
                this.list_contacts.Add(data_contact);
            }

            PlayerPrefs.SetString("contacts_" + this.order_by.ToString() + "_" + app.carrot.lang.Get_key_lang(), Carrot.Json.Serialize(this.list_contacts));
            this.is_read_cache = true;

            this.Add_item_title();
            this.show_list_data_contacts(this.list_contacts);
        }
        else
        {
            app.add_item_none();
        }
    }

    private void Act_get_data_from_server_fail(string s_error)
    {
        Debug.Log(s_error);
        app.carrot.clear_contain(app.area_body_main);
        Carrot_Box_Item item_error = app.add_item_title_list("Error");
        item_error.set_icon_white(app.carrot.icon_carrot_bug);
        item_error.set_tip("Operation failed, please try again!");
    }

    private void get_data_from_cache(string s_data_json)
    {
        app.carrot.clear_contain(app.area_body_main);

        this.list_contacts = (IList)Carrot.Json.Deserialize(s_data_json);
        this.Add_item_title();
        this.show_list_data_contacts(this.list_contacts);
    }

    private void Add_item_title()
    {
        Carrot_Box_Item item_title = app.add_item_title_list(this.app.carrot.lang.Val("contact", "Contact") +" (" + app.carrot.lang.Get_key_lang() + ")");
        item_title.set_tip(this.app.carrot.lang.Val("contact_list", "List of contacts in your country ") + "("+this.list_contacts.Count+" "+ this.app.carrot.lang.Val("contact", "Contact")+")");
        Carrot_Box_Btn_Item btn_sort_name = item_title.create_item();
        btn_sort_name.set_icon(this.icon_sort_name);
        btn_sort_name.set_icon_color(Color.white);
        btn_sort_name.set_color(app.carrot.color_highlight);
        btn_sort_name.set_act(() => this.Sort());
    }


    public void show_list_data_contacts(IList list_contacts)
    {
        app.scrollbar_on_top();
        foreach (IDictionary data_contact in list_contacts)
        {
            var id_contact = ""; if (data_contact["id"] != null) id_contact = data_contact["id"].ToString();
            var lang_contact = "en"; if (data_contact["lang"] != null) lang_contact = data_contact["lang"].ToString();
            string s_tip = "";

            Carrot_Box_Item item_contact = app.create_item_main();
            item_contact.on_load(app.carrot);
            item_contact.check_type();
            item_contact.set_icon(this.icon_contact);
            if (data_contact["name"] != null) item_contact.set_title(data_contact["name"].ToString());

            if (data_contact["phone"] != null)
            {
                var s_phone = data_contact["phone"].ToString();
                if (s_phone != "")
                {
                    Carrot_Box_Btn_Item btn_call = item_contact.create_item();
                    if (data_contact["sex"] != null)
                    {
                        if (data_contact["sex"].ToString() == "0")
                            btn_call.set_icon(app.icon_call_boy);
                        else
                            btn_call.set_icon(app.icon_call_girl);
                    }
                    else
                    {
                        btn_call.set_icon(app.icon_call_boy);
                    }
                    btn_call.set_color(app.carrot.color_highlight);
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
                    Carrot_Box_Btn_Item btn_email = item_contact.create_item();
                    btn_email.set_icon(app.carrot.icon_carrot_mail);
                    btn_email.set_color(app.carrot.color_highlight);
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
                    Carrot_Box_Btn_Item btn_save = item_contact.create_item();
                    btn_save.set_icon(this.icon_save);
                    btn_save.set_color(app.carrot.color_highlight);
                    btn_save.set_act(() => app.book_contact.add(data_contact));
                }

                if (type_item=="phonebook")
                {
                    Carrot_Box_Btn_Item btn_del = item_contact.create_item();
                    btn_del.set_icon(app.carrot.sp_icon_del_data);
                    btn_del.set_color(Color.red);
                    btn_del.set_act(() => app.book_contact.delete(int.Parse(data_contact["index"].ToString())));
                }
            }

            item_contact.set_act(() => this.view_info_contact(data_contact));
        }
        app.Update_items_color();
    }

    public void view_info_contact(IDictionary data)
    {
        var id_contact = ""; if (data["id"] != null) id_contact = data["id"].ToString();
        var lang_contact = "en"; if (data["lang"] != null) lang_contact = data["lang"].ToString();

        app.play_sound(0);
        app.carrot.ads.show_ads_Interstitial();
        if (this.box_info != null) this.box_info.close();
        this.box_info=app.carrot.user.Show_info_user_by_data(data);
        Carrot_Box_Btn_Panel panel_tool=box_info.create_panel_btn();

        var s_phone = "";
        if (data["phone"] != null)
        {
            s_phone = data["phone"].ToString();
            Carrot_Button_Item btn_call = panel_tool.create_btn("btn_call");
            if (data["sex"] != null)
            {
                if (data["sex"].ToString() == "0")
                    btn_call.set_icon_white(app.icon_call_boy);
                else
                    btn_call.set_icon_white(app.icon_call_girl);
            }
            else
            {
                btn_call.set_icon_white(app.icon_call_boy);
            }
            btn_call.set_label_color(Color.white);
            btn_call.set_label(this.app.carrot.lang.Val("call", "Call"));
            btn_call.set_bk_color(app.carrot.color_highlight);
            btn_call.set_act_click(() => this.Call(s_phone));
        }

        var s_mail = "";
        if (data["email"] != null)
        {
            s_mail = data["email"].ToString();
            if (s_mail != "")
            {
                Carrot_Button_Item btn_mail = panel_tool.create_btn("btn_mail");
                btn_mail.set_icon_white(app.carrot.icon_carrot_mail);
                btn_mail.set_label_color(Color.white);
                btn_mail.set_label(this.app.carrot.lang.Val("send_mail", "Send Mail"));
                btn_mail.set_bk_color(app.carrot.color_highlight);
                btn_mail.set_act_click(() => this.Mail(s_mail));
            }
        }

        Carrot_Button_Item btn_share = panel_tool.create_btn("btn_share");
        btn_share.set_icon_white(app.carrot.sp_icon_share);
        btn_share.set_label_color(Color.white);
        btn_share.set_label(this.app.carrot.lang.Val("share", "Share"));
        btn_share.set_bk_color(app.carrot.color_highlight);
        btn_share.set_act_click(() => this.Share(id_contact,lang_contact));

        Carrot_Box_Btn_Panel panel_qr = box_info.create_panel_btn();

        /*
        Carrot_Button_Item btn_qr_link = panel_qr.create_btn("btn_qr");
        btn_qr_link.set_icon_white(app.qr.icon_read_qr);
        btn_qr_link.set_label_color(Color.white);
        btn_qr_link.set_label("QR (Link)");
        btn_qr_link.set_bk_color(app.carrot.color_highlight);
        btn_qr_link.set_act_click(() => this.QR_Code_link_user(id_contact,lang_contact));
        */

        if (s_phone != "")
        {
            Carrot_Button_Item btn_qr_phone = panel_qr.create_btn("btn_qr_phone");
            btn_qr_phone.set_icon_white(app.qr.icon_read_qr);
            btn_qr_phone.set_label_color(Color.white);
            btn_qr_phone.set_label("QR (Phone)");
            btn_qr_phone.set_bk_color(app.carrot.color_highlight);
            btn_qr_phone.set_act_click(() => this.QR_Code_phone_number(s_phone));
        }

        if (s_mail != "")
        {
            Carrot_Button_Item btn_qr_mail = panel_qr.create_btn("btn_qr_mail");
            btn_qr_mail.set_icon_white(app.qr.icon_read_qr);
            btn_qr_mail.set_label_color(Color.white);
            btn_qr_mail.set_label("QR (Mail)");
            btn_qr_mail.set_bk_color(app.carrot.color_highlight);
            btn_qr_mail.set_act_click(() => this.QR_Code_phone_number(s_mail));
        }

        Carrot_Box_Btn_Panel panel_act = box_info.create_panel_btn();

        if (data["type_item"] != null)
        {
            string type_item = data["type_item"].ToString();
            if (type_item == "contact")
            {
                Carrot_Button_Item btn_save = panel_act.create_btn("btn_save");
                btn_save.set_icon_white(this.icon_save);
                btn_save.set_label_color(Color.white);
                btn_save.set_label(this.app.carrot.lang.Val("save_contact","Save Contact"));
                btn_save.set_bk_color(app.carrot.color_highlight);
                btn_save.set_act_click(() => app.book_contact.add(data));

                if (app.carrot.model_app == ModelApp.Develope)
                {
                    string s_id = data["id"].ToString();
                    string s_lang = this.app.carrot.lang.Get_key_lang();
                    Carrot_Button_Item btn_del_dev = panel_act.create_btn("btn_del_dev");
                    btn_del_dev.set_icon_white(this.app.carrot.sp_icon_del_data);
                    btn_del_dev.set_label_color(Color.white);
                    btn_del_dev.set_label("Delete (Dev)");
                    btn_del_dev.set_bk_color(app.carrot.color_highlight);
                    btn_del_dev.set_act_click(() =>Delete_contact(s_id,s_lang));
                }
            }

            if (type_item == "phonebook")
            {
                if (data["index"] != null)
                {
                    Carrot_Button_Item btn_del = panel_act.create_btn("btn_del");
                    btn_del.set_icon_white(app.carrot.sp_icon_del_data);
                    btn_del.set_label_color(Color.white);
                    btn_del.set_label(this.app.carrot.lang.Val("del_contact", "Delete contact"));
                    btn_del.set_bk_color(Color.red);
                    btn_del.set_act_click(() => app.book_contact.delete(int.Parse(data["index"].ToString())));

                    Carrot_Button_Item btn_edit= panel_act.create_btn("btn_edit");
                    btn_edit.set_icon_white(app.carrot.user.icon_user_edit);
                    btn_edit.set_label_color(Color.white);
                    btn_edit.set_label(this.app.carrot.lang.Val("edit", "Edit"));
                    btn_edit.set_bk_color(app.carrot.color_highlight);
                    btn_edit.set_act_click(() => app.book_contact.edit(int.Parse(data["index"].ToString())));
                }
            }
        }

        Carrot_Button_Item btn_close = panel_act.create_btn("btn_close");
        btn_close.set_icon_white(app.carrot.icon_carrot_cancel);
        btn_close.set_label_color(Color.white);
        btn_close.set_label(this.app.carrot.lang.Val("cancel", "Cancel"));
        btn_close.set_bk_color(app.carrot.color_highlight);
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
        string url_share = app.carrot.mainhost+"?p=phone_book&id="+id_contact+"&user_lang="+lang_contact;
        app.carrot.show_share(url_share, "Share this contacts with everyone");
    }

    private void QR_Code_link_user(string id_contact, string lang_contact)
    {
        string url_share = app.carrot.mainhost + "?p=phone_book&id=" + id_contact + "&user_lang=" + lang_contact;
        app.qr.Show_QR_create_by_data(url_share);
    }

    private void QR_Code_phone_number(string s_phone)
    {
        app.qr.Show_QR_create_by_data(s_phone);
    }

    private void Sort()
    {
        app.carrot.play_sound_click();
        if (order_by == Query_Order_Direction.ASCENDING)
            order_by = Query_Order_Direction.DESCENDING;
        else
            order_by = Query_Order_Direction.ASCENDING;

        this.List();
    }

    public IList get_list_contacts()
    {
        return this.list_contacts;
    }

    public Carrot.Carrot_Box get_box_info()
    {
        return this.box_info;
    }

    public void Delete_contact(string s_id,string s_lang)
    {
        Debug.Log("Delete Dev " + s_id+" success !!!");
        this.app.carrot.server.Delete_Doc("user-" + s_lang, s_id,Act_delete_contact_done,Act_delete_contact_fail);
    }

    public void Act_delete_contact_done(string s_data)
    {
        app.carrot.show_msg("Conact store", "Delete Success!", Msg_Icon.Success);
    }

    public void Act_delete_contact_fail(string s_error)
    {
        app.carrot.show_msg("Conact store", s_error, Msg_Icon.Error);
    }
}
