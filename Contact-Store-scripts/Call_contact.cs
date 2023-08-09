using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Call_contact : MonoBehaviour
{
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
            this.StopAllCoroutines();
            GameObject item_found = this.bc.get_contact_by_phone(s_dial_txt);
            if (item_found != null)
            {
                /*
                Prefab_contact_item_main data_obj = item_found.GetComponent<Prefab_contact_item_main>();
                this.txt_contact_name_found.text = data_obj.txt_name.text;
                this.txt_contact_phone_found.text = data_obj.txt_phone.text;
                this.img_contact_found.sprite = data_obj.img_avatar.sprite;
                this.button_add_contact.SetActive(false);
                this.panel_info_contact_found.SetActive(true);
                this.panel_info_contact_found.GetComponent<Button>().onClick.RemoveAllListeners();
                this.panel_info_contact_found.GetComponent<Button>().onClick.AddListener(data_obj.click);
                */
            }
            else
            {
                this.button_add_contact.SetActive(true);
                this.panel_info_contact_found.SetActive(false);

                if (s_dial_txt.Length < 20 && this.ct.carrot.is_online()) StartCoroutine(get_info_by_number_phone());
            }
        }
        this.bc.GetComponent<App_Contacts>().play_sound(1);
    }

    private IEnumerator get_info_by_number_phone()
    {
        yield return new WaitForSeconds(2f);
        WWWForm frm = this.ct.carrot.frm_act("get_info_phone");
        frm.AddField("phone", this.s_dial_txt);
       // this.ct.carrot.send_hide(frm, act_get_info_by_phone);
        this.img_loading.SetActive(true);
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
        this.ct.view_contact(this.user_id_contact,this.user_lang_contact);
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
}
