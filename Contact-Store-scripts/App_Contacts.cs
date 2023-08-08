using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Extensions;
using Firebase.Firestore;

public class App_Contacts : MonoBehaviour
{
    [Header("Obj Main")]
    public Carrot.Carrot carrot;
    public Manager_Contact manager_contact;
    public Search_Contacts search;

    [Header("Obj App")]
    public GameObject panel_backup;
    public GameObject panel_search;
    public GameObject panel_call;
    public QR_scan qr;
    public Transform area_body_main;


    [Header("Template Prefab")]
    public GameObject prefab_contact_main_item;
    public GameObject prefab_row_waitting;
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
            this.btn_show_list_lang();
        else
        {
            this.manager_contact.list();
            this.check_link_deep_app();
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
            this.carrot.delay_function(2f, this.carrot.show_login);
        }
        else
            this.panel_backup.GetComponent<Panel_backup>().show();
        this.play_sound(0);
    }

    public void btn_list_contact_home()
    {
        this.play_sound(0);
        this.manager_contact.list();
    }


    public void view_contact(string s_user_id,string s_user_lang)
    {
        this.carrot.user.show_user_by_id(s_user_id, s_user_lang, after_view_contact);
        this.carrot.ads.show_ads_Interstitial();
    }

    private void after_view_contact(string s_data)
    {

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

    public void add_item_loading_or_screen_loading(Transform area_body_add)
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
        this.manager_contact.list();
    }

    public void btn_show_list_app_other()
    {
        this.play_sound(0);
        this.carrot.show_list_carrot_app();
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
        this.search.search_contact(this.inp_search.text.Trim());
    }

    public void btn_search_option()
    {

    }


    public Carrot.Carrot_Box_Item add_item_title_list(string s_title)
    {
        GameObject obj_title_list = Instantiate(this.prefab_contact_main_item);
        obj_title_list.transform.SetParent(this.area_body_main);
        obj_title_list.transform.localPosition = new Vector3(obj_title_list.transform.localPosition.x, obj_title_list.transform.localPosition.y, 0f);
        obj_title_list.transform.localScale = new Vector3(1f, 1f, 1f);
        obj_title_list.transform.localRotation = Quaternion.Euler(Vector3.zero);

        Carrot.Carrot_Box_Item item_title = obj_title_list.GetComponent<Carrot.Carrot_Box_Item>();
        item_title.set_icon_white(this.carrot.icon_carrot_all_category);
        item_title.txt_name.color = Color.white;
        item_title.txt_tip.color = Color.white;
        item_title.on_load(this.carrot);
        item_title.check_type();
        item_title.set_title(s_title);
        item_title.GetComponent<Image>().color = this.carrot.color_highlight;

        return item_title;
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
