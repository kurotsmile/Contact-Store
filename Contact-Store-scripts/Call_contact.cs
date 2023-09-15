using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Call_contact : MonoBehaviour
{
    [Header("Main Obj")]
    public App_Contacts app;
    private string s_dial_txt;

    [Header("Call Obj")]
    public Text txt_phone_call;
    public GameObject button_call;
    public GameObject button_sms;
    public GameObject button_add_contact;
    public GameObject button_del;
    private Book_contact bc;
    public GameObject img_loading;

    [Header("Contact info found")]
    public GameObject panel_info_contact_found;
    public Image img_contact_found;
    public Text txt_contact_name_found;
    public Text txt_contact_phone_found;
    private string user_id_contact;
    private string user_lang_contact;


    private App_Contacts ct;
    public void show()
    {
        this.ct = GameObject.Find("App_Contacts").GetComponent<App_Contacts>();
        this.bc = GameObject.Find("App_Contacts").GetComponent<Book_contact>();
        this.btn_delete();
    }

    public void show_by_phone_number(string s_phone_numer)
    {
        this.gameObject.SetActive(true);
        this.btn_dial(s_phone_numer);
    }

    public void btn_dial(string s_dial)
    {
        this.s_dial_txt = this.s_dial_txt + s_dial;
        this.txt_phone_call.text = this.s_dial_txt;
        this.button_add_contact.SetActive(true);
        this.button_call.SetActive(true);
        this.button_sms.SetActive(true);
        this.button_del.SetActive(true);
        this.img_loading.SetActive(false);
        if (this.s_dial_txt.Length > 3)
        {
            IDictionary item_found = this.bc.get_contact_by_phone(s_dial_txt);
            if (item_found != null)
            {
                this.button_add_contact.SetActive(false);
                this.panel_info_contact_found.SetActive(true);
                if (item_found["name"] != null) this.txt_contact_name_found.text = item_found["name"].ToString();
                if (item_found["phone"] != null) this.txt_contact_phone_found.text = item_found["phone"].ToString();
                Sprite sp_avatar = this.app.carrot.get_tool().get_sprite_to_playerPrefs("avatar_user_" + item_found["id"]);
                if (sp_avatar != null) this.img_contact_found.sprite = sp_avatar;
                this.panel_info_contact_found.GetComponent<Button>().onClick.RemoveAllListeners();
                this.panel_info_contact_found.GetComponent<Button>().onClick.AddListener(()=>this.app.manager_contact.view_info_contact(item_found));
            }
            else
            {
                this.button_add_contact.SetActive(true);
                this.panel_info_contact_found.SetActive(false);

                if (s_dial_txt.Length < 20 && this.ct.carrot.is_online())
                {
                    this.get_info_by_number_phone(this.s_dial_txt);
                }
            }
        }
        this.bc.GetComponent<App_Contacts>().play_sound(1);
    }

    private void get_info_by_number_phone(string s_phone)
    {
        this.img_loading.SetActive(true);

        Query ContactQuery = this.app.carrot.db.Collection("user-" + this.app.carrot.lang.get_key_lang());
        ContactQuery = ContactQuery.WhereEqualTo("phone", s_phone);
        ContactQuery = ContactQuery.Limit(60);
        ContactQuery.Limit(1).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot QDocs = task.Result;
            if (task.IsCompleted)
            {
                if (QDocs.Count > 0)
                {
                    this.img_loading.SetActive(false);
                    foreach (DocumentSnapshot doc in QDocs.Documents)
                    {
                        IDictionary data_contact = doc.ToDictionary();
                        data_contact["user_id"] = doc.Id;
                        data_contact["id"] = doc.Id;
                        data_contact["type_item"] = "contact";
                        if (data_contact["type_item"] != null) data_contact.Remove("rates");
                        if (data_contact["backup_contact"] != null) data_contact.Remove("backup_contact");

                        this.button_add_contact.SetActive(false);
                        this.panel_info_contact_found.SetActive(true);
                        if (data_contact["name"] != null) this.txt_contact_name_found.text = data_contact["name"].ToString();
                        if (data_contact["phone"] != null) this.txt_contact_phone_found.text = data_contact["phone"].ToString();
                        Sprite sp_avatar = this.app.carrot.get_tool().get_sprite_to_playerPrefs("avatar_user_" + data_contact["id"]);
                        if (sp_avatar != null) this.img_contact_found.sprite = sp_avatar;
                        else this.app.carrot.get_img_and_save_playerPrefs(data_contact["avatar"].ToString(), this.img_contact_found, "avatar_user_" + data_contact["id"]);
                        this.panel_info_contact_found.GetComponent<Button>().onClick.RemoveAllListeners();
                        this.panel_info_contact_found.GetComponent<Button>().onClick.AddListener(() => this.app.manager_contact.view_info_contact(data_contact));
                    }
                }
            }

            if (task.IsFaulted)
            {
                this.img_loading.SetActive(false);
            }
        });
    }

    private void act_get_info_by_phone(string data)
    {
        Debug.Log(data);
        this.img_loading.SetActive(false);
        IDictionary data_info = (IDictionary)Carrot.Json.Deserialize(data);
        if (data_info["error"].ToString() == "0")
        {
            this.panel_info_contact_found.SetActive(true);
            this.txt_contact_name_found.text = data_info["name"].ToString();
            this.txt_contact_phone_found.text = data_info["phone"].ToString();
            this.user_id_contact= data_info["user_id"].ToString();
            this.user_lang_contact= data_info["user_lang"].ToString();
            this.ct.carrot.get_img(data_info["avatar"].ToString(),this.img_contact_found);
            this.panel_info_contact_found.GetComponent<Button>().onClick.RemoveAllListeners();
            this.panel_info_contact_found.GetComponent<Button>().onClick.AddListener(view_contact_info);
        }
    }

    public void view_contact_info()
    {
        this.ct.carrot.user.show_user_by_id(this.user_id_contact,this.user_lang_contact);
    }

    public void btn_call()
    {
        this.bc.GetComponent<App_Contacts>().play_sound(0);
        Application.OpenURL("tel:" +this.s_dial_txt);
    }

    public void btn_sms()
    {
        this.bc.GetComponent<App_Contacts>().play_sound(0);
        Application.OpenURL("sms:" + this.s_dial_txt);
    }

    public void btn_delete()
    {
        this.StopAllCoroutines();
        this.txt_phone_call.text = PlayerPrefs.GetString("call_tip");
        this.s_dial_txt = "";
        this.button_add_contact.SetActive(false);
        this.button_call.SetActive(false);
        this.button_sms.SetActive(false);
        this.button_del.SetActive(false);
        this.panel_info_contact_found.SetActive(false);
        this.img_loading.SetActive(false);
    }

    public void btn_add_contacts()
    {
        this.app.play_sound(0);
        this.app.book_contact.Create_New_BookContact_By_Phone_Number(this.s_dial_txt);
    }
}
