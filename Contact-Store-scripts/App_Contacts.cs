using Carrot;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class App_Contacts : MonoBehaviour
{
    [Header("Obj Main")]
    public Carrot.Carrot carrot;
    public Manager_Menu menu;
    public Manager_Contact manager_contact;
    public Book_contact book_contact;
    public Backup_Contacts backup_Contacts;
    public Search_Contacts search;

    [Header("Obj App")]
    public GameObject panel_call;
    public QR_scan qr;
    public Transform area_body_main;

    [Header("Template Prefab")]
    public GameObject prefab_contact_main_item;
    public GameObject prefab_row_waitting;

    [Header("Icon Obj")]
    public Sprite icon_call_girl;
    public Sprite icon_call_boy;
    public Sprite icon_contact_public;
    public Sprite icon_search_user;
    public Sprite icon_search_contact;
    public Sprite icon_sad;

    [Header("Other Object")]
    public Image img_avatar_account;
    public Image img_btn_login;
    public Sprite image_btn_login_add;
    public InputField inp_search;
    public Image img_btn_contact_home;
    public Image img_btn_contact_store;
    public Image img_search;
    public Color32 color_sel;
    public Color32 color_normal;
    public ScrollRect scroll_rect_main;

    [Header("Sounds")]
    public AudioSource[] sound;

    private string link_deep_app;

    void Start()
    {
        this.link_deep_app = Application.absoluteURL;
        Application.deepLinkActivated += OnDeepLinkActivated;

        this.panel_call.SetActive(false);

        this.carrot.Load_Carrot(Check_exit_app);
        this.book_contact.Load_book_contact();
    }

    [ContextMenu("Test Link")]
    public void Test_link()
    {
        this.OnDeepLinkActivated("contactstore://show/user9dd108c1acca4f8e8fefa8dec39d5a9d/vi");
        //this.onDeepLinkActivated("contactstore://tel/0978651577");
    }

    private void OnDeepLinkActivated(string url)
    {
        this.link_deep_app = url;
        if (this.carrot != null) this.carrot.delay_function(1f, this.Check_link_deep_app);
    }

    public void Check_link_deep_app()
    {
        if (this.link_deep_app.Trim() != "")
        {
            if (this.carrot.is_online())
            {
                if (this.link_deep_app.Contains("show"))
                {
                    string data_link = this.link_deep_app.Replace("contactstore://show/", "");
                    string[] paramet_user = data_link.Split('/');
                    this.View_contact_by_id(paramet_user[0], paramet_user[1]);
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

    public void Load_app_online()
    {
        if (PlayerPrefs.GetString("lang") == "")
            this.btn_show_list_lang();
        else
        {
            this.menu.load();
            this.Check_link_deep_app();
        }  
    }

    public void Load_app_offline()
    {
        this.book_contact.show();
    }

    private void Check_exit_app()
    {
        if (this.panel_call.activeInHierarchy)
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

    public void btn_setting()
    {
        this.carrot.Create_Setting();
    }

    public void Add_item_loading()
    {
        GameObject item_waiting = Instantiate(this.prefab_row_waitting);
        item_waiting.transform.SetParent(this.area_body_main);
        item_waiting.transform.localPosition = new Vector3(item_waiting.transform.localPosition.x, item_waiting.transform.localPosition.y, 0f);
        item_waiting.transform.localScale = new Vector3(1f, 1f, 1f);
        item_waiting.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    public void btn_show_user_login()
    {
        this.carrot.show_login();
    }

    public void btn_show_list_lang()
    {
        this.play_sound(0);
        this.carrot.show_list_lang(Act_after_load_lang);
    }

    private void Act_after_load_lang(string s_data)
    {
        this.manager_contact.List();
    }

    public void btn_account()
    {
        this.carrot.show_login();
    }

    public void btn_search_contact()
    {
        this.search.search_contact(this.inp_search.text.Trim());
    }

    public Carrot_Box_Item create_item_main()
    {
        GameObject obj_item_main = Instantiate(this.prefab_contact_main_item);
        obj_item_main.transform.SetParent(this.area_body_main);
        obj_item_main.transform.localPosition = new Vector3(obj_item_main.transform.localPosition.x, obj_item_main.transform.localPosition.y, 0f);
        obj_item_main.transform.localScale = new Vector3(1f, 1f, 1f);
        obj_item_main.transform.localRotation = Quaternion.Euler(Vector3.zero);

        Carrot_Box_Item item_title = obj_item_main.GetComponent<Carrot_Box_Item>();
        item_title.on_load(this.carrot);
        item_title.check_type();
        return item_title;
    }

    public Carrot_Box_Item add_item_title_list(string s_title)
    {
        Carrot_Box_Item item_title = this.create_item_main();
        item_title.set_icon_white(this.carrot.icon_carrot_all_category);
        item_title.txt_name.color = Color.white;
        item_title.txt_tip.color = Color.white;
        item_title.on_load(this.carrot);
        item_title.check_type();
        item_title.set_title(s_title);
        item_title.GetComponent<Image>().color = this.carrot.color_highlight;

        return item_title;
    }

    public Carrot_Box_Item add_item_none()
    {
        Carrot_Box_Item item_none = this.create_item_main();
        item_none.set_icon_white(this.icon_sad);
        item_none.txt_name.color = Color.white;
        item_none.txt_tip.color = Color.white;
        item_none.on_load(this.carrot);
        item_none.check_type();
        item_none.GetComponent<Image>().color = this.carrot.color_highlight;
        item_none.set_title(PlayerPrefs.GetString("list_none","List is empty"));
        item_none.set_tip(PlayerPrefs.GetString("list_none_tip", "There are no items in the list"));
        return item_none;
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

    public void scrollbar_on_top()
    {
        this.scroll_rect_main.verticalNormalizedPosition = 1f;
    }

    public void View_contact_by_id(string s_id,string s_lang)
    {
        this.carrot.show_loading();
        carrot.server.Get_doc_by_path("user-" + s_lang, s_id, Act_get_contact_by_id_done, Act_get_contact_by_id_fail);
    }

    private void Act_get_contact_by_id_done(string s_data)
    {
        this.carrot.hide_loading();
        Fire_Document fd = new(s_data);
        IDictionary data_contact = fd.Get_IDictionary();
        data_contact["user_id"] = data_contact["id"].ToString();
        data_contact["type_item"] = "contact";
        this.manager_contact.view_info_contact(data_contact);
    }

    private void Act_get_contact_by_id_fail(string s_error)
    {
        this.carrot.hide_loading();
        this.carrot.show_msg(PlayerPrefs.GetString("app_title", "World contact book"), PlayerPrefs.GetString("list_none_tip", "There are no items in the list"), Msg_Icon.Alert);
    }
}
