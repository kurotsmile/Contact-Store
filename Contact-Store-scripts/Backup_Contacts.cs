using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[FirestoreData]
public struct Backup_contact_data
{
    [FirestoreProperty]
    public string id { get; set; }
    [FirestoreProperty]
    public int length {get; set;}
    [FirestoreProperty]
    public string date { get; set; }
    [FirestoreProperty]
    public IList contacts { get; set; }
}

public class Backup_Contacts : MonoBehaviour
{
    [Header("Obj Main")]
    public App_Contacts app;

    [Header("Obj Backup")]
    public Sprite icon_backup;
    public Sprite icon_download;

    private Carrot.Carrot_Window_Msg msg;

    public void show()
    {
        
        this.app.play_sound(0);
        if (this.app.carrot.user.get_id_user_login() == "")
        {
            this.app.carrot.show_msg(PlayerPrefs.GetString("backup", "Backup"), PlayerPrefs.GetString("backup_no_login", "You need to log in to your account to backup your contacts"), Carrot.Msg_Icon.Alert);
            this.app.carrot.delay_function(2f, this.show_login_for_backup);
        }
        else
        {
            this.list();
        }

    }

    private void show_login_for_backup()
    {
        this.app.carrot.user.show_login(this.list);
    }

    public void list()
    {
        this.app.carrot.clear_contain(this.app.area_body_main);
        this.app.add_item_loading();
        DocumentReference userRef = this.app.carrot.db.Collection("user-" + this.app.carrot.user.get_lang_user_login()).Document(this.app.carrot.user.get_id_user_login());
        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                this.app.carrot.hide_loading();
                this.app.carrot.clear_contain(this.app.area_body_main);
                IDictionary data_user = (IDictionary)snapshot.ToDictionary();
                IList list_backup;
                if (data_user["backup_contact"] != null)
                    list_backup = (IList)data_user["backup_contact"];
                else
                    list_backup = (IList)Carrot.Json.Deserialize("[]");

                if (list_backup.Count > 0)
                {
                    Carrot.Carrot_Box_Item item_title = this.app.add_item_title_list(PlayerPrefs.GetString("backup","Backup"));
                    item_title.set_icon(this.app.carrot.icon_carrot_all_category);
                    item_title.set_tip(PlayerPrefs.GetString("backup_list", "list of your backups"));
                    item_title.set_act(()=>this.list());

                    for (int i = 0; i < list_backup.Count; i++)
                    { 
                        IDictionary data_backup = (IDictionary)list_backup[i];
                        Carrot.Carrot_Box_Item item_backup = this.app.create_item_main();
                        item_backup.set_icon(this.app.carrot.icon_carrot_database);
                        item_backup.set_title(data_backup["date"].ToString());
                        item_backup.set_tip(data_backup["length"].ToString()+" "+PlayerPrefs.GetString("contact","Contact"));

                        Carrot.Carrot_Box_Btn_Item btn_download=item_backup.create_item();
                        btn_download.set_icon(this.icon_download);
                        btn_download.set_color(this.app.carrot.color_highlight);
                        Destroy(btn_download.GetComponent<Button>());

                        IList list_contact =(IList) data_backup["contacts"];
                        item_backup.set_act(() => this.download(list_contact));
                    }
                    this.add_item_create_new();
                }
                else
                {
                    this.app.add_item_none();
                    this.add_item_create_new();
                }
            }
            else
            {
                this.app.carrot.show_msg(PlayerPrefs.GetString("app_title", "World contact book"), "the operation has not been performed because of some server error, please try again later", Carrot.Msg_Icon.Error);
            }
        });
    }

    private void add_item_create_new()
    {
        Carrot.Carrot_Box_Item item_create = this.app.add_item_title_list(PlayerPrefs.GetString("create_backup","Create a new backup"));
        item_create.set_icon(this.app.carrot.icon_carrot_add);
        item_create.set_tip(PlayerPrefs.GetString("create_backup_tip","Start backing up your contacts in your app"));
        item_create.set_act(() => this.backup());
    }

    public void download(IList contacts)
    {
        this.app.play_sound(0);
        if (this.msg != null) this.msg.close();
        this.msg=this.app.carrot.show_msg(PlayerPrefs.GetString("backup", "Backup"),PlayerPrefs.GetString("backup_sync", "Do you want to sync your contacts with this backup?"),this.download_yes,this.download_no);
    }

    private void download_yes()
    {
        if (this.msg != null) this.msg.close();
    }

    private void download_no()
    {
        if (this.msg != null) this.msg.close();
    }

    public void backup()
    {
        this.app.carrot.show_loading();
        DocumentReference userRef = this.app.carrot.db.Collection("user-" + this.app.carrot.user.get_lang_user_login()).Document(this.app.carrot.user.get_id_user_login());
        userRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                this.app.carrot.hide_loading();
                IDictionary data_user = (IDictionary)snapshot.ToDictionary();
                IList list_backup;
                if (data_user["backup_contact"] != null)
                    list_backup=(IList)data_user["backup_contact"];
                else
                    list_backup=(IList) Carrot.Json.Deserialize("[]");

                IList list_contacts = this.app.book_contact.get_list_data_backup();
                Backup_contact_data backup_data_item = new Backup_contact_data();
                backup_data_item.id = "backup"+this.app.carrot.generateID();
                backup_data_item.date= DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
                backup_data_item.length = list_contacts.Count;
                backup_data_item.contacts = list_contacts;

                list_backup.Add(backup_data_item);

                Debug.Log("Count list contacts "+this.app.carrot.user.get_id_user_login()+" - "+this.app.carrot.user.get_lang_user_login()+":" + list_backup.Count);
                Dictionary<string, object> UpdateData = new Dictionary<string, object> { { "backup_contact", list_backup } };
                userRef.UpdateAsync(UpdateData);
            }
            else
            {
                this.app.carrot.show_msg(PlayerPrefs.GetString("app_title", "World contact book"), "the operation has not been performed because of some server error, please try again later", Carrot.Msg_Icon.Error);
            }
        });
    }
}
