using System.Collections.Specialized;
using System.Web;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class QR_scan : MonoBehaviour
{
    [Header("Main Obj")]
    public App_Contacts app;

    [Header("QR Obj")]
    public GameObject panel_scan_qr;
    public CodeWriter codeWtr;
    public CodeWriter.CodeType codetype;
    public Image previewImg_Read; 
    public Texture2D targetTex;
    public CodeReader reader;
    public PreviewController preview;
    public Text btn_create_qr_txt;
    public Image btn_create_qr_icon;
    public Sprite icon_create_qr;
    public Sprite icon_read_qr;

    public GameObject panel_data_qr;
    public Text txt_data_qr;

    bool camera_flip = false;

    public void On_load()
    {
        this.panel_scan_qr.SetActive(false);
        this.panel_data_qr.SetActive(false);
        CodeWriter.onCodeEncodeFinished += GetCodeImage;
        CodeReader.OnCodeFinished += GetDataFromReader;
    }

    public void GetCodeImage(Texture2D tex)
    {
        app.play_sound(2);
        app.carrot.camera_pro.show_photoshop(tex);
        this.StartReader();
    }

    public void Show_QR_scan()
    {
        this.app.play_sound(0);
        this.panel_scan_qr.SetActive(true);
        this.panel_data_qr.SetActive(false);
        this.StartReader();
    }

    public void Show_QR_create_by_data(string data_str,bool is_link)
    {
        this.app.play_sound(0);
        if(is_link)
            codeWtr.CreateQRCode_Url(data_str,codetype);
        else
            codeWtr.CreateQRCode_PhoneNumber(data_str, codetype);
    }

    public void Close()
    {
        this.app.play_sound(0);
        this.panel_data_qr.SetActive(false);
        this.panel_scan_qr.SetActive(false);
        this.StopReader();
    }


    public void ReadCode()
    {
       reader.ReadCode(targetTex);
    }

    public void StartReader()
    {
        if (reader != null)
        {
            reader.StartWork();
        }
    }

    public void StopReader()
    {
        if (reader != null)
        {
            reader.StopWork();
        }
    }

    public void GetDataFromReader(string dataStr)
    {
        this.app.play_sound(2);

        if (IsUrl(dataStr))
        {
            NameValueCollection parameters = GetUrlParameters(dataStr);
            if (parameters["user_lang"] != null) app.View_contact_by_id(parameters["id"].ToString(), parameters["user_lang"].ToString());
            this.Close();
        }else if (IsEmail(dataStr))
        {
            this.app.search.Search_email(dataStr);
            this.Close();
        }
        else if (IsPhoneNumber(dataStr))
        {
            this.app.search.Search_phone_number(dataStr);
            this.Close();
        }
        else
        {
            this.panel_data_qr.SetActive(true);
            this.txt_data_qr.text = dataStr;
        }
    }


    public void Btn_camera_flip()
    {
        if (this.camera_flip)
        {
            this.preview.frontCamera(false);
            this.preview.rearCamera(true);
            this.camera_flip = false;
        }
        else
        {
            this.camera_flip = true;
            this.preview.frontCamera(true);
            this.preview.rearCamera(false);
        }
        this.app.play_sound(0);
    }

    public void Create_Code()
    {
        if (codeWtr != null)
        {
            string id_user = app.carrot.user.get_id_user_login();
            if (id_user == "")
            {
                this.app.carrot.user.show_login(()=>Create_Code());
            }
            else
            {
                this.StopReader();
                string s_user_link = app.carrot.mainhost + "/?p=phone_book&id=" + app.carrot.user.get_id_user_login() + "&user_lang=" + app.carrot.user.get_lang_user_login();
                codeWtr.CreateCode(codetype, s_user_link);
            }
        }
    }

    public void Btn_create_code()
    {
        this.app.carrot.play_sound_click();
        this.Create_Code();
    }

    public void Btb_delete_data_qr()
    {
        this.app.play_sound(0);
        this.panel_data_qr.SetActive(false);
    }


    public void Click_goto_data_link()
    {
        this.app.play_sound(0);
        Application.OpenURL(this.txt_data_qr.text);
    }

    static NameValueCollection GetUrlParameters(string url)
    {
        Uri uri = new(url);
        string queryString = uri.Query;
        NameValueCollection parameters = HttpUtility.ParseQueryString(queryString);
        return parameters;
    }

    static bool IsPhoneNumber(string input)
    {
        Regex phoneNumberRegex = new(@"^\(\d{3}\) \d{3}-\d{4}$");
        return phoneNumberRegex.IsMatch(input);
    }

    static bool IsEmail(string input)
    {
        Regex emailRegex = new(@"^\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b$");
        return emailRegex.IsMatch(input);
    }

    static bool IsUrl(string input)
    {
        Regex urlRegex = new(@"^(http|https):\/\/([\w-]+(\.[\w-]+)+)([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?$");
        return urlRegex.IsMatch(input);
    }
}
