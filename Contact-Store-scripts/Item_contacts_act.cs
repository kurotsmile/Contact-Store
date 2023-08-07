using Carrot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_contacts_act : MonoBehaviour
{
    private string s_data;
    private string s_phone;
    private string s_mail;
    private string s_link_contacts;
    private string s_user_id;
    private string s_user_lang;
    private int index_contacts;
    private Texture2D data_avatar;
    private IList list_info;
    public GameObject prefab_contacts_avatar;
    public GameObject button_save;
    public GameObject button_delete;
    public GameObject button_send_mail;
    public GameObject button_edit;
    public GameObject button_link;
    public GameObject button_vcf;

    private void hide_all_button()
    {
        this.button_delete.SetActive(false);
        this.button_save.SetActive(false);
        this.button_send_mail.SetActive(false);
        this.button_edit.SetActive(false);
        this.button_link.SetActive(false);
        this.button_vcf.SetActive(false);
    }

    public void set_data_by_account(string s_data)
    {
        this.hide_all_button();
        IDictionary data_contact =(IDictionary)Carrot.Json.Deserialize(s_data);
        this.s_data = s_data;
        this.list_info =(IList)data_contact["list_info"];
        this.s_user_id = data_contact["user_id"].ToString();
        this.s_phone=this.get_val_in_field("sdt");
        this.s_link_contacts=this.get_val_in_field("user_link");
        this.s_mail = this.get_val_in_field("email");
        if(data_contact["user_lang"]!=null)this.s_user_lang= data_contact["user_lang"].ToString();
        if (this.s_mail != "") this.button_send_mail.SetActive(true);
        if (this.s_link_contacts != "") this.button_link.SetActive(true);
        this.button_save.SetActive(true);
        this.button_vcf.SetActive(true);
        if (GameObject.Find("App_Contacts").GetComponent<App_Contacts>().carrot.is_online())
        {
            if (data_contact["avatar_full"] != null)
            if (data_contact["avatar_full"].ToString() != "") GameObject.Find("App_Contacts").GetComponent<App_Contacts>().carrot.get_img(data_contact["avatar_full"].ToString(), load_contacts_avatar);
        }
    }

    public void set_data_by_contact(int index,string s_data)
    {
        this.index_contacts = index;
        this.set_data_by_account(s_data);
        this.button_delete.SetActive(true);
        this.button_save.SetActive(false);
        this.button_edit.SetActive(true);
        this.button_vcf.SetActive(false);
    }

    private void load_contacts_avatar(Texture2D data_img)
    {
        /*
        GameObject item_info_avatar = Instantiate(this.prefab_contacts_avatar);
        item_info_avatar.transform.SetParent(GameObject.Find("App_Contacts").GetComponent<App_Contacts>().carrot.area_body_box);
        item_info_avatar.transform.localPosition = new Vector3(item_info_avatar.transform.localPosition.x, item_info_avatar.transform.localPosition.y, 0f);
        item_info_avatar.transform.localScale = new Vector3(1f, 1f, 1f);
        item_info_avatar.transform.localRotation = Quaternion.Euler(Vector3.zero);
        item_info_avatar.transform.SetAsFirstSibling();
        this.data_avatar = data_img;
        data_img = GameObject.Find("App_Contacts").GetComponent<App_Contacts>().carrot.ResampleAndCrop(data_img, 200, 60);
        item_info_avatar.GetComponent<Image>().sprite = GameObject.Find("App_Contacts").GetComponent<App_Contacts>().carrot.Texture2DtoSprite(data_img);
        item_info_avatar.GetComponent<Button>().onClick.AddListener(view_img_avatar);
        */
    }

    private void view_img_avatar()
    {
        GameObject.Find("App_Contacts").GetComponent<App_Contacts>().carrot.camera_pro.show_list_img(null);
    }

    public void btn_call()
    {
        Application.OpenURL("tel://"+this.s_phone);
        Debug.Log("tel://" + this.s_phone);
    }

    public void btn_sms()
    {
        Application.OpenURL("sms://"+this.s_phone);
        Debug.Log("sms://" + this.s_phone);
    }

    public void btn_save()
    {
        this.button_save.SetActive(false);
        GameObject.Find("App_Contacts").GetComponent<Book_contact>().add_contact(this.s_user_id, this.s_data, this.data_avatar.EncodeToPNG());
    }

    public void btn_qr()
    {
        if(this.s_link_contacts!="")
            GameObject.Find("App_Contacts").GetComponent<App_Contacts>().qr.show_QR_create_by_data(this.s_link_contacts,true);
        else
            GameObject.Find("App_Contacts").GetComponent<App_Contacts>().qr.show_QR_create_by_data(this.s_phone,true);

        GameObject.Find("App_Contacts").GetComponent<App_Contacts>().carrot.close();
    }

    public void btn_delete()
    {
        GameObject.Find("App_Contacts").GetComponent<App_Contacts>().carrot.close();
        GameObject.Find("App_Contacts").GetComponent<Book_contact>().delete_contact(this.index_contacts);
    }

    public void btn_link()
    {
        Application.OpenURL(this.s_link_contacts);
    }

    public void btn_send_mail()
    {
        Application.OpenURL("mailto:"+this.s_mail);
    }

    public void btn_edit()
    {
        GameObject.Find("App_Contacts").GetComponent<Book_contact>().show_edit(this.index_contacts,this.s_data,false);
    }

    public void btn_download_vcf()
    {
        //Application.OpenURL(GameObject.Find("App_Contacts").GetComponent<App_Contacts>().carrot.get_url_host()+"/download_vcf.php?id_user="+this.s_user_id+"&lang="+this.s_user_lang);
    }

    private string get_val_in_field(string s_id_name)
    {
        for(int i = 0; i < this.list_info.Count; i++)
        {
            IDictionary data_item =(IDictionary)this.list_info[i];
            if(data_item["id_name"]!=null)if (s_id_name == data_item["id_name"].ToString()) return data_item["val"].ToString();
        }
        return "";
    }
}