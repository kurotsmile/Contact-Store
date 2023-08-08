using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Extensions;
using Firebase.Firestore;

public class App_Contacts : MonoBehaviour
{
    [Header("Obj Main")]
    public Carrot.Carrot carrot;

    [Header("Obj App")]
    public GameObject panel_backup;
    public GameObject panel_search;
    public GameObject panel_call;
    public QR_scan qr;
    public Transform area_body_main;

    [Header("Template Prefab")]
    public GameObject prefab_contact_main_item;
    public GameObject prefab_contact_more;
    public GameObject prefab_tip_info;
    public GameObject prefab_row_waitting;
    public GameObject prefab_contact_act;
    public GameObject prefab_item_none;

    [Header("Search Obj")]
    public InputField inp_search_name;
    public InputField inp_search_phone;
    public InputField inp_search_address;
    public Dropdown dropdow_search_sex;

    [Header("Icon Obj")]
    public Sprite icon_call_girl;
    public Sprite icon_call_boy;
    public Sprite icon_search_return;
    public Sprite icon_contact_public;
    public Sprite icon_search_user;
    public Sprite icon_search_contact;
    public Sprite icon_setting_on_sound;
    public Sprite icon_setting_off_sound;

    [Header("Other Object")]
    public Image img_avatar_account;
    public Image img_btn_login;
    public Sprite image_btn_login_add;
    public InputField inp_search;
    public Image img_btn_contact_home;
    public Image img_btn_contact_store;
    public Image img_search;
    public GameObject button_search_option;
    public GameObject button_edit_contact_user_login;
    public GameObject button_contact_backup;
    public GameObject btn_add_account;
    public GameObject btn_add_contact;
    public AudioSource[] sound;

    public Color32 color_sel;
    public Color32 color_normal;

    [Header("Setting")]
    public Image img_setting_audio;
    public GameObject panel_setting_remove_ads;
    private bool is_check_user_go_backup = false;
    private string link_deep_app;

    void Start()
    {
        this.link_deep_app = Application.absoluteURL;
        this.panel_backup.SetActive(false);
        this.panel_search.SetActive(false);
        this.panel_call.SetActive(false);

        this.carrot.Load_Carrot(check_exit_app);
        this.GetComponent<Book_contact>().load_book_contact();
        this.GetComponent<Field_contact>().load_field();
    }

    public void check_link_deep_app()
    {
        if (this.link_deep_app.Trim() != "")
        {
            if (this.carrot.is_online())
            {
                if (this.link_deep_app.Contains("show"))
                {
                    string data_link = this.link_deep_app.Replace("contactstore://show/", "");
                    string[] paramet_user = data_link.Split('/');
                    this.view_contact(paramet_user[0], paramet_user[1]);
                    this.link_deep_app = "";
                }
            }

            if (this.link_deep_app.Contains("tel"))
            {
                string data_link = this.link_deep_app.Replace("contactstore://tel/", "");
                this.panel_call.GetComponent<Call_contact>().show_by_phone_number(data_link);
                this.link_deep_app = "";
            }
        }
    }

    public void load_app_online()
    {
        if (PlayerPrefs.GetString("lang") == "")
        {
            this.btn_show_list_lang();
        }
        else
        {
            if (PlayerPrefs.GetInt("is_view_contact", 0) == 0)
                this.show_list_contact_home();
            else
            {
                this.add_item_loading_or_screen_loading(this.area_body_main);
                this.carrot.delay_function(0.5f, this.GetComponent<Book_contact>().show_list_book);
            }
        }
    }

    public void load_app_offline()
    {
        this.add_item_loading_or_screen_loading(this.area_body_main);
        this.carrot.delay_function(0.5f, this.GetComponent<Book_contact>().show_list_book);
    }

    private void check_exit_app()
    {
        this.play_sound(0);

        if (this.panel_search.activeInHierarchy)
        {
            this.panel_search.SetActive(false);
            this.carrot.set_no_check_exit_app();
        }
        else if (this.panel_call.activeInHierarchy)
        {
            this.panel_call.SetActive(false);
            this.carrot.set_no_check_exit_app();
        }
        else if (this.qr.gameObject.activeInHierarchy)
        {
            this.qr.close();
            this.carrot.set_no_check_exit_app();
        }
    }

    public void show_backup()
    {
        if (this.carrot.user.get_id_user_login() == "")
        {
            this.carrot.show_msg(PlayerPrefs.GetString("backup", "Backup"), PlayerPrefs.GetString("backup_no_login", "You need to log in to your account to backup your contacts"), Carrot.Msg_Icon.Alert);
            is_check_user_go_backup = true;
            this.carrot.delay_function(2f, this.carrot.show_login);
        }
        else
            this.panel_backup.GetComponent<Panel_backup>().show();
        this.play_sound(0);
    }

    public void show_list_contact_home()
    {
        this.StopAllCoroutines();
        this.carrot.stop_all_act();
        this.add_item_loading_or_screen_loading(this.area_body_main);

        Query ContactQuery = this.carrot.db.Collection("user-" + this.carrot.lang.get_key_lang()).WhereEqualTo("status_share", "0");
        ContactQuery = ContactQuery.WhereNotEqualTo("phone","");
        ContactQuery.Limit(60).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot QDocs=task.Result;
            if (task.IsCompleted)
            {
                if (QDocs.Count > 0)
                {
                    this.carrot.clear_contain(this.area_body_main);

                    foreach (DocumentSnapshot doc in QDocs.Documents)
                    {
                        IDictionary data_contact = doc.ToDictionary();

                        GameObject obj_contact_item = Instantiate(this.prefab_contact_main_item);
                        obj_contact_item.transform.SetParent(this.area_body_main);
                        obj_contact_item.transform.localPosition = new Vector3(0, 0, 0);
                        obj_contact_item.transform.localScale = new Vector3(1f, 1f, 1f);
                        obj_contact_item.transform.localRotation = Quaternion.Euler(Vector3.zero);

                        Prefab_contact_item_main contact_obj = obj_contact_item.GetComponent<Prefab_contact_item_main>();
                        if(data_contact["name"]!=null) contact_obj.txt_name.text = data_contact["name"].ToString();
                        if(data_contact["phone"]!=null) contact_obj.txt_phone.text = data_contact["phone"].ToString();
                    }
                }
            }
        });
    }

    private void act_get_list_contact_home(string s_data)
    {
        string type_view;
        this.carrot.hide_loading();
        this.carrot.clear_contain(this.area_body_main);
        this.panel_search.SetActive(false);
        this.panel_call.SetActive(false);
        IDictionary data_contacts= (IDictionary)Carrot.Json.Deserialize(s_data);
        IList list_contact = (IList)data_contacts["list_contacts"];
        type_view = data_contacts["type_view"].ToString();
        this.area_body_main.gameObject.SetActive(false);
        this.area_body_main.parent.gameObject.SetActive(false);

        this.img_search.sprite = this.icon_search_user;
        this.img_btn_contact_home.color = this.color_sel;
        this.img_btn_contact_store.color = this.color_normal;
        this.btn_add_account.SetActive(true);
        this.btn_add_contact.SetActive(false);
        this.button_contact_backup.SetActive(false);
        this.button_edit_contact_user_login.SetActive(true);
        this.button_search_option.SetActive(true);
        this.GetComponent<Book_contact>().menu_footer_edit.SetActive(false);

        this.area_body_main.gameObject.SetActive(true);
        this.area_body_main.parent.gameObject.SetActive(true);

        if (type_view == "search") this.add_tip_info(this.icon_search_return, PlayerPrefs.GetString("search_return")+"("+list_contact.Count+")", PlayerPrefs.GetString("search_return_tip"),0);
        if (list_contact.Count > 0)
        {
            if (type_view == "nomal") this.add_prefab_more();
            foreach (IDictionary app in list_contact)
            {
                GameObject item_app_other;

                item_app_other = Instantiate(this.prefab_contact_main_item);
                item_app_other.transform.SetParent(this.area_body_main);

                item_app_other.transform.localPosition = new Vector3(item_app_other.transform.localPosition.x, item_app_other.transform.localPosition.y, 0f);
                item_app_other.transform.localScale = new Vector3(1f, 1f, 1f);
                item_app_other.transform.localRotation = Quaternion.Euler(Vector3.zero);
                item_app_other.GetComponent<Prefab_contact_item_main>().type = 0;
                item_app_other.GetComponent<Prefab_contact_item_main>().txt_name.text = app["name"].ToString();
                if (item_app_other.GetComponent<Prefab_contact_item_main>().btn_del != null) item_app_other.GetComponent<Prefab_contact_item_main>().btn_del.SetActive(false);

                item_app_other.GetComponent<Prefab_contact_item_main>().txt_address.text = app["address"].ToString();
                item_app_other.GetComponent<Prefab_contact_item_main>().btn_sms.SetActive(false);
                if (app["sex"].ToString() == "0")
                    item_app_other.GetComponent<Prefab_contact_item_main>().img_btn_call.sprite = this.icon_call_boy;
                else
                    item_app_other.GetComponent<Prefab_contact_item_main>().img_btn_call.sprite = this.icon_call_girl;

                item_app_other.GetComponent<Prefab_contact_item_main>().txt_phone.text = app["phone"].ToString();
                item_app_other.GetComponent<Prefab_contact_item_main>().s_user_id = app["id"].ToString();
                item_app_other.GetComponent<Prefab_contact_item_main>().s_user_lang = app["lang"].ToString();
                item_app_other.GetComponent<Prefab_contact_item_main>().img_icon_contact.sprite = this.icon_contact_public;
                if (app["avatar"].ToString() != "") this.carrot.get_img(app["avatar"].ToString(), item_app_other.GetComponent<Prefab_contact_item_main>().img_avatar);
            }
        }
        else
        {
            this.add_none_info(this.area_body_main);
        }

        if (type_view == "nomal") this.add_prefab_more();
        if (type_view == "search") this.add_tip_info(this.GetComponent<Book_contact>().sp_back, PlayerPrefs.GetString("phonebook"), PlayerPrefs.GetString("book_contact_tip2"),2);

        if (this.inp_search.text.Trim() != "") this.inp_search.text = "";

        this.check_link_deep_app();
    }

    private void add_prefab_more()
    {
        GameObject item_more;
        item_more = Instantiate(this.prefab_contact_more);
        item_more.transform.SetParent(this.area_body_main);

        item_more.transform.localPosition = new Vector3(item_more.transform.localPosition.x, item_more.transform.localPosition.y, 0f);
        item_more.transform.localScale = new Vector3(1f, 1f, 1f);
        item_more.transform.localRotation = Quaternion.Euler(Vector3.zero);
        item_more.GetComponent<Prefab_more>().txt_title.text = PlayerPrefs.GetString("more", "See more");
        item_more.GetComponent<Prefab_more>().txt_tip.text = PlayerPrefs.GetString("more_tip", "Click here to see 20 more contacts");
    }

    public void view_contact(string s_user_id,string s_user_lang)
    {
        this.carrot.user.show_user_by_id(s_user_id, s_user_lang, after_view_contact);
        this.carrot.ads.show_ads_Interstitial();
    }

    private void after_view_contact(string s_data)
    {
        GameObject item_act_contact = Instantiate(this.prefab_contact_act);
        //item_act_contact.transform.SetParent(this.carrot.area_body_box);
        item_act_contact.transform.localPosition = new Vector3(item_act_contact.transform.localPosition.x, item_act_contact.transform.localPosition.y, 0f);
        item_act_contact.transform.localScale = new Vector3(1f, 1f, 1f);
        item_act_contact.transform.localRotation = Quaternion.Euler(Vector3.zero);
        item_act_contact.GetComponent<Item_contacts_act>().set_data_by_account(s_data);
    }

    public void view_contact_import(int index)
    {
        this.carrot.ads.show_ads_Interstitial();
        this.play_sound(0);
    }

    public void btn_setting()
    {
        this.carrot.Create_Setting();
    }

    public void delete_backup(string sid, string lang)
    {
       this.panel_backup.GetComponent<Panel_backup>().delete_item_backup(sid, lang);
    }

    public void add_item_loading(Transform area_body)
    {
        GameObject item_waiting = Instantiate(this.prefab_row_waitting);
        item_waiting.transform.SetParent(area_body);
        item_waiting.transform.localPosition = new Vector3(item_waiting.transform.localPosition.x, item_waiting.transform.localPosition.y, 0f);
        item_waiting.transform.localScale = new Vector3(1f, 1f, 1f);
        item_waiting.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    private void add_item_loading_or_screen_loading(Transform area_body_add)
    {
        if (area_body_add.childCount > 6)
            this.carrot.show_loading();
        else
            this.add_item_loading(area_body_add);
    }

    public void btn_show_user_login()
    {
        this.carrot.show_login();
    }

    public void btn_show_list_lang()
    {
        this.play_sound(0);
        this.carrot.show_list_lang(act_after_load_lang);
    }

    private void act_after_load_lang(string s_data)
    {
        this.show_list_contact_home();
    }

    public void btn_show_list_app_other()
    {
        this.play_sound(0);
        this.carrot.show_list_carrot_app();
    }

    public void btn_buy_product(int index)
    {
        this.play_sound(0);
        this.carrot.buy_product(index);
    }

    public void btn_restore_product()
    {
        this.play_sound(0);
        this.carrot.show_loading();
        this.carrot.restore_product();
    }

    public void btn_delete_all_data()
    {
        this.play_sound(0);
        this.carrot.show_loading();
        this.GetComponent<Book_contact>().delete_all_contact();
        this.carrot.delete_all_data();
        this.carrot.delay_function(3.5f, this.btn_show_list_lang);
    }

    public void btn_share_app()
    {
        this.play_sound(0);
        this.carrot.show_share();
    }

    public void btn_account()
    {
        this.carrot.show_login();
    }

    public void btn_show_search()
    {
        this.play_sound(0);
        if (this.carrot.is_online())
        {
            this.panel_search.SetActive(true);
            this.dropdow_search_sex.options.Clear();
            this.dropdow_search_sex.options.Add(new Dropdown.OptionData() { text = PlayerPrefs.GetString("user_sex_no", "No select") });
            this.dropdow_search_sex.options.Add(new Dropdown.OptionData() { text = PlayerPrefs.GetString("user_sex_boy", "Male") });
            this.dropdow_search_sex.options.Add(new Dropdown.OptionData() { text = PlayerPrefs.GetString("user_sex_girl", "Female") });
        }
    }

    public void btn_close_search()
    {
        this.play_sound(0);
        this.panel_search.SetActive(false);
    }

    public void btn_search_contact()
    {
        if (this.inp_search.text.Trim() != "")
        {
            if (PlayerPrefs.GetInt("is_view_contact",0)==0)
            {
                WWWForm frm = this.carrot.frm_act("get_list_contacts");
                frm.AddField("search", this.inp_search.text);
               // this.carrot.send_hide(frm, act_get_list_contact_home);
            }
            else
            {
                this.GetComponent<Book_contact>().search(this.inp_search.text);
                this.inp_search.text = "";
            }
        }
    }

    public void btn_search_option()
    {
        WWWForm frm_seach = this.carrot.frm_act("get_list_contacts");
        frm_seach.AddField("search","search_option");
        frm_seach.AddField("search_name", this.inp_search_name.text);
        frm_seach.AddField("search_address", this.inp_search_address.text);
        frm_seach.AddField("search_sex", this.dropdow_search_sex.value);
        frm_seach.AddField("search_phone", this.inp_search_phone.text);
       // this.carrot.send(frm_seach,this.act_get_list_contact_home);
    }

    public void add_tip_info(Sprite icon_tip, string s_name, string s_tip,int type_act)
    {
        GameObject Item_info_tip = Instantiate(this.prefab_tip_info);
        Item_info_tip.transform.SetParent(this.area_body_main);
        Item_info_tip.transform.localPosition = new Vector3(Item_info_tip.transform.localPosition.x, Item_info_tip.transform.localPosition.y, 0f);
        Item_info_tip.transform.localScale = new Vector3(1f, 1f, 1f);
        Item_info_tip.transform.localRotation = Quaternion.Euler(Vector3.zero);
        Item_info_tip.GetComponent<Panel_info>().btn_action.gameObject.SetActive(false);
        Item_info_tip.GetComponent<Panel_info>().type = type_act;
        Item_info_tip.GetComponent<Panel_info>().txt_title.text = s_name;
        Item_info_tip.GetComponent<Panel_info>().txt_tip.text = s_tip;
        Item_info_tip.GetComponent<Panel_info>().icon.sprite = icon_tip;
        Item_info_tip.GetComponent<Panel_info>().icon.color = Color.black;
    }

    public void add_none_info(Transform area_body_add)
    {
        GameObject Item_info_none = Instantiate(this.prefab_item_none);
        Item_info_none.transform.SetParent(area_body_add);
        Item_info_none.transform.localPosition = new Vector3(Item_info_none.transform.localPosition.x, Item_info_none.transform.localPosition.y, 0f);
        Item_info_none.transform.localScale = new Vector3(1f, 1f, 1f);
        Item_info_none.transform.localRotation = Quaternion.Euler(Vector3.zero);
        Item_info_none.GetComponent<Panel_info>().txt_title.text = PlayerPrefs.GetString("info_none","None");
        Item_info_none.GetComponent<Panel_info>().txt_tip.text = PlayerPrefs.GetString("info_none_tip", "There are no related entries");
        Item_info_none.GetComponent<Panel_info>().icon.color = Color.black;
    }

    public void btn_show_call()
    {
        this.play_sound(0);
        this.panel_call.SetActive(true);
        this.panel_call.GetComponent<Call_contact>().show();
    }

    public void btn_close_call()
    {
        this.play_sound(0);
        this.carrot.stop_all_act();
        this.panel_call.GetComponent<Call_contact>().StopAllCoroutines();
        this.panel_call.SetActive(false);
    }

    public void play_sound(int index)
    {
        if (this.carrot.get_status_sound()) this.sound[index].Play();
    }
}
