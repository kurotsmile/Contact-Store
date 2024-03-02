using Carrot;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Backup_Contacts : MonoBehaviour
{
    [Header("Obj Main")]
    public App_Contacts app;

    [Header("Obj Backup")]
    public Sprite icon_backup;
    public Sprite icon_download;

    private Carrot_Window_Msg msg;
    private IList list_contact_download;
    private int index_delete = -1;
    private IList list_backup;

    public void Show()
    {
        this.app.play_sound(0);
        if (this.app.carrot.user.get_id_user_login() == "")
        {
            this.app.carrot.show_msg(PlayerPrefs.GetString("backup", "Backup"), PlayerPrefs.GetString("backup_no_login", "You need to log in to your account to backup your contacts"), Carrot.Msg_Icon.Alert);
            this.app.carrot.delay_function(2f, this.Show_login_for_backup);
        }
        else
        {
            this.app.carrot.clear_contain(this.app.area_body_main);

            Carrot_Box_Item item_backup= app.create_item_main();
            item_backup.set_title(PlayerPrefs.GetString("create_backup", "Create a new backup"));
            item_backup.set_tip(PlayerPrefs.GetString("create_backup_tip", "Start backing up your contacts in your app"));
            item_backup.set_icon(this.icon_backup);
            item_backup.set_act(() => this.Backup());

            Carrot_Box_Item item_syn= app.create_item_main();
            item_syn.set_title(PlayerPrefs.GetString("backup_list", "list of your backups"));
            item_syn.set_tip(PlayerPrefs.GetString("backup_list", "list of your backups"));
            item_syn.set_icon(this.icon_download);
            item_syn.set_act(() => this.List());
        }
    }

    private void Show_login_for_backup()
    {
        this.app.carrot.user.show_login(this.Show);
    }

    public void List()
    {
        this.app.carrot.clear_contain(this.app.area_body_main);
        this.app.Add_item_loading();

        StructuredQuery q = new(app.carrot.Carrotstore_AppId);
        q.Add_where("user_id",Query_OP.EQUAL, app.carrot.user.get_id_user_login());
        app.carrot.server.Get_doc(q.ToJson(), Act_list_done, Act_list_fail);
    }

    private void Act_list_done(string s_data)
    {
        Fire_Collection fc = new(s_data);
        if (!fc.is_null)
        {
            this.app.carrot.hide_loading();
            this.app.carrot.clear_contain(this.app.area_body_main);
            IDictionary data_user = fc.fire_document[0].Get_IDictionary();

            if (data_user["backup_contact"] != null)
                list_backup = (IList)data_user["backup_contact"];
            else
                list_backup = (IList)Carrot.Json.Deserialize("[]");

            if (list_backup.Count > 0)
            {
                Carrot_Box_Item item_title = this.app.add_item_title_list(PlayerPrefs.GetString("backup", "Backup"));
                item_title.set_icon(this.app.carrot.icon_carrot_all_category);
                item_title.set_tip(PlayerPrefs.GetString("backup_list", "list of your backups"));
                item_title.set_act(() => this.List());

                for (int i = 0; i < list_backup.Count; i++)
                {
                    var index_item = i;
                    IDictionary data_backup = (IDictionary)list_backup[i];
                    Carrot_Box_Item item_backup = this.app.create_item_main();
                    item_backup.set_icon(this.app.carrot.icon_carrot_database);
                    item_backup.set_title(data_backup["date"].ToString());
                    item_backup.set_tip(data_backup["length"].ToString() + " " + PlayerPrefs.GetString("contact", "Contact"));

                    Carrot_Box_Btn_Item btn_download = item_backup.create_item();
                    btn_download.set_icon(this.icon_download);
                    btn_download.set_color(this.app.carrot.color_highlight);
                    Destroy(btn_download.GetComponent<Button>());

                    Carrot_Box_Btn_Item btn_del = item_backup.create_item();
                    btn_del.set_icon(this.app.carrot.sp_icon_del_data);
                    btn_del.set_color(Color.red);
                    btn_del.set_act(() => this.delete_backup(index_item));

                    IList list_contact = (IList)data_backup["contacts"];
                    item_backup.set_act(() => this.download(list_contact));
                }
                this.add_item_create_new();
            }
        }
        else
        {
            this.app.add_item_none();
            this.add_item_create_new();
        }
    }

    private void Act_list_fail(string s_error)
    {
        this.app.carrot.show_msg(PlayerPrefs.GetString("app_title", "World contact book"), "the operation has not been performed because of some server error, please try again later", Carrot.Msg_Icon.Error);
    }

    private void add_item_create_new()
    {
        Carrot_Box_Item item_create = this.app.add_item_title_list(PlayerPrefs.GetString("create_backup","Create a new backup"));
        item_create.set_icon(this.app.carrot.icon_carrot_add);
        item_create.set_tip(PlayerPrefs.GetString("create_backup_tip","Start backing up your contacts in your app"));
        item_create.set_act(() => this.Backup());
    }

    public void download(IList contacts)
    {
        this.list_contact_download = contacts;
        this.app.play_sound(0);
        if (this.msg != null) this.msg.close();
        this.msg=this.app.carrot.show_msg(PlayerPrefs.GetString("backup", "Backup"),PlayerPrefs.GetString("backup_sync", "Do you want to sync your contacts with this backup?"),this.download_yes,this.download_no);
    }

    private void download_yes()
    {
        this.app.book_contact.delete_all();
        foreach(IDictionary contact in this.list_contact_download)
        {
            this.app.book_contact.create(contact);
        }
        if (this.msg != null) this.msg.close();
        this.app.book_contact.show();
        this.msg = this.app.carrot.show_msg(PlayerPrefs.GetString("backup", "Backup"), PlayerPrefs.GetString("backup_success", "Download the backup and sync successfully!"),Carrot.Msg_Icon.Success);
    }

    private void download_no()
    {
        if (this.msg != null) this.msg.close();
    }

    private void delete_backup(int index_del)
    {
        this.index_delete = index_del;
        this.msg = this.app.carrot.show_msg("Delete", "Are you sure you want to remove this item?", act_yes_delete, act_no_delete);
    }

    private void act_yes_delete()
    {
        if (this.msg != null) this.msg.close();
        this.app.play_sound(0);

        list_backup.RemoveAt(this.index_delete);
        Dictionary<string, object> UpdateData = new Dictionary<string, object> { { "backup_contact", list_backup } };
        this.List();
    }

    private void act_no_delete()
    {
        if (this.msg != null) this.msg.close();
        this.app.play_sound(0);
    }

    public void Backup()
    {
        if (app.carrot.user.get_id_user_login() == "")
        {
            this.Show_login_for_backup();
            return;
        }

        IList list_contacts = this.app.book_contact.get_list_data_backup();
        if (list_contacts.Count == 0)
        {
            this.app.carrot.show_msg(PlayerPrefs.GetString("app_title", "World contact book"), PlayerPrefs.GetString("list_none_tip", "There are no items in the list"), Msg_Icon.Alert);
            return;
        }

        this.app.carrot.show_loading();
        string id_backup= "backup" + this.app.carrot.generateID()+UnityEngine.Random.Range(0,20);
        
        IDictionary backup_data = (IDictionary)Json.Deserialize("{}");
        backup_data["id"] = 
        backup_data["date"] = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
        backup_data["length"] = list_contacts.Count;
        backup_data["reports"] = list_contacts;
        backup_data["user_id"] = this.app.carrot.user.get_id_user_login();
        backup_data["user_lang"] = this.app.carrot.user.get_lang_user_login();

        string s_json = app.carrot.server.Convert_IDictionary_to_json(backup_data);
        app.carrot.server.Add_Document_To_Collection(app.carrot.Carrotstore_AppId, id_backup,s_json, Backup_data_done, Backup_fail);
    }

    private void Backup_fail(string s_error)
    {
        app.carrot.hide_loading();
        this.app.carrot.show_msg(PlayerPrefs.GetString("app_title", "World contact book"), "the operation has not been performed because of some server error, please try again later", Carrot.Msg_Icon.Error);
    }

    private void Backup_data_done(string s_data)
    {
        app.carrot.hide_loading();
        this.app.carrot.show_msg(PlayerPrefs.GetString("backup", "Backup"), PlayerPrefs.GetString("backup_success", "Download the backup and sync successfully!"), Msg_Icon.Success);
    }
}
