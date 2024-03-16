using Carrot;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Call_contact : MonoBehaviour
{
    [Header("Main Obj")]
    public App_Contacts app;
    private string s_dial_txt;

    [Header("Call Obj")]
    public GameObject panel_call;
    public Text txt_phone_call;
    public GameObject button_call;
    public GameObject button_sms;
    public GameObject button_add_contact;
    public GameObject button_del;
    public GameObject img_loading;

    [Header("Contact info found")]
    public GameObject panel_info_contact_found;
    public Image img_contact_found;
    public Text txt_contact_name_found;
    public Text txt_contact_phone_found;
    private string user_id_contact;
    private string user_lang_contact;

    [Header("UI Call")]
    public Transform tr_arean_landscape_left;
    public Transform tr_arean_landscape_right;
    public Transform tr_arean_body_protrait;

    public Transform obj_panel_btn_bottom;
    public Transform obj_panel_info_contact;
    public Transform obj_panel_dialing;
    public Transform obj_panel_tip_call;

    public void show()
    {
        this.app.deviceOrientation.Start_check();
        this.app.Check_scene_size();
        this.panel_call.SetActive(true);
        this.Act_delete_all_obj_btn();
    }

    public void show_by_phone_number(string s_phone_numer)
    {
        this.app.deviceOrientation.Start_check();
        this.panel_call.SetActive(true);
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
            IDictionary item_found = this.app.book_contact.get_contact_by_phone(s_dial_txt);
            if (item_found != null)
            {
                this.app.play_sound(2);
                this.app.carrot.play_vibrate();
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

                if (this.s_dial_txt.Length > 8)
                {
                    if (s_dial_txt.Length < 20 && this.app.carrot.is_online())
                    {
                        this.Get_info_by_number_phone(this.s_dial_txt);
                    }
                }
            }
        }
        this.app.play_sound(1);
    }

    private void Get_info_by_number_phone(string s_phone)
    {
        this.img_loading.SetActive(true);
        StructuredQuery q = new("user-" + this.app.carrot.lang.get_key_lang());
        q.Add_where("status_share", Query_OP.EQUAL, "0");
        q.Add_where("phone", Query_OP.EQUAL, s_phone);
        q.Set_limit(1);
        app.carrot.server.Get_doc(q.ToJson(), Act_Get_info_by_number_phone_done, Act_Get_info_by_number_phone_fail);
    }

    private void Act_Get_info_by_number_phone_done(string s_data)
    {
        Fire_Collection fc = new(s_data);
        if (!fc.is_null)
        {
            this.app.play_sound(2);
            this.app.carrot.play_vibrate();
            this.img_loading.SetActive(false);
            for (int i = 0; i < fc.fire_document.Length; i++)
            {
                IDictionary data_contact = fc.fire_document[i].Get_IDictionary();
                data_contact["user_id"] = data_contact["id"].ToString();
                data_contact["id"] = data_contact["id"].ToString();
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
        else
        {
            this.img_loading.SetActive(false);
        }
    }

    private void Act_Get_info_by_number_phone_fail(string s_error)
    {
        this.img_loading.SetActive(false);
    }

    public void view_contact_info()
    {
        this.app.carrot.user.show_user_by_id(this.user_id_contact,this.user_lang_contact);
    }

    public void btn_call()
    {
        this.app.play_sound(0);
        Application.OpenURL("tel:" +this.s_dial_txt);
    }

    public void btn_sms()
    {
        this.app.play_sound(0);
        Application.OpenURL("sms:" + this.s_dial_txt);
    }

    public void Act_delete_all_obj_btn()
    {
        this.txt_phone_call.text = PlayerPrefs.GetString("call_tip", "Let's start dialing the contact number");
        this.s_dial_txt = "";
        this.button_add_contact.SetActive(false);
        this.button_call.SetActive(false);
        this.button_sms.SetActive(false);
        this.button_del.SetActive(false);
        this.panel_info_contact_found.SetActive(false);
        this.img_loading.SetActive(false);
    }

    public void btn_delete()
    {
        app.carrot.play_sound_click();
        app.carrot.play_vibrate();
        this.Act_delete_all_obj_btn();
    }

    public void btn_add_contacts()
    {
        this.app.play_sound(0);
        this.app.book_contact.Create_New_BookContact_By_Phone_Number(this.s_dial_txt);
    }

    public void Show_UI_for_landscape()
    {
        this.obj_panel_dialing.GetComponent<GridLayoutGroup>().cellSize = new Vector2(50f, 50f);
        this.obj_panel_dialing.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 300);
        this.obj_panel_tip_call.SetParent(this.tr_arean_landscape_left);
        this.obj_panel_info_contact.SetParent(this.tr_arean_landscape_left);
        this.obj_panel_dialing.SetParent(this.tr_arean_landscape_right);
        this.obj_panel_btn_bottom.SetParent(this.tr_arean_landscape_left);
    }

    public void Show_UI_for_portrait()
    {
        this.obj_panel_dialing.GetComponent<GridLayoutGroup>().cellSize = new Vector2(80f, 80f);
        this.obj_panel_dialing.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 380);
        this.obj_panel_tip_call.SetParent(this.tr_arean_body_protrait);
        this.obj_panel_info_contact.SetParent(this.tr_arean_body_protrait);
        this.obj_panel_dialing.SetParent(this.tr_arean_body_protrait);
        this.obj_panel_btn_bottom.SetParent(this.tr_arean_body_protrait);
    }

    public void Hide()
    {
        this.app.deviceOrientation.Stop_check();
        this.StopAllCoroutines();
        this.panel_call.SetActive(false);
    }

    public void Check_scene_size()
    {
        if (app.deviceOrientation.Get_status_portrait())
            Show_UI_for_portrait();
        else
            Show_UI_for_landscape();
    }
}
