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
    public CodeWriter codeWtr;
    public CodeWriter.CodeType codetype;
    public RawImage previewImg_wirite; 
    public Texture2D targetTex;
    public CodeReader reader;
    public PreviewController preview;
    string str;
    private bool is_show_create = false;
    public Button btn_create_qr;
    public Button btn_flip_camera;
    public Text btn_create_qr_txt;
    public Image btn_create_qr_icon;
    public Sprite icon_create_qr;
    public Sprite icon_read_qr;

    public GameObject panel_data_qr;
    public Text txt_data_qr;

    bool camera_flip = false;
    public void Start()
    {
        this.panel_data_qr.SetActive(false);
        CodeWriter.onCodeEncodeFinished += GetCodeImage;
        CodeReader.OnCodeFinished += getDataFromReader;
    }

    public void GetCodeImage(Texture2D tex)
    {
        targetTex = tex;
        RectTransform component = this.previewImg_wirite.rectTransform;
        float y = component.sizeDelta.x * (float)tex.height / (float)tex.width;
        component.sizeDelta = new Vector2(component.sizeDelta.x, y);
        previewImg_wirite.texture = targetTex;
    }

    public void show_QR_scan()
    {
        this.app.play_sound(0);
        this.gameObject.SetActive(true);
        this.StartReader();
        this.is_show_create = false;
        this.check_show_btn_create_qr();
    }

    public void show_QR_create_by_data(string data_str,bool is_link)
    {
        this.app.play_sound(0);
        this.gameObject.SetActive(true);
        if(is_link)
            codeWtr.CreateQRCode_Url(data_str,codetype);
        else
            codeWtr.CreateQRCode_PhoneNumber(data_str, codetype);
        this.is_show_create = true;
        this.check_show_btn_create_qr();
    }

    public void close()
    {
        this.panel_data_qr.SetActive(false);
        this.StopReader();
        this.gameObject.SetActive(false);
    }


    public void readCode()
    {
        str = reader.ReadCode(targetTex);
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

    public void getDataFromReader(string dataStr)
    {
        this.app.play_sound(2);

        if (IsUrl(dataStr))
        {
            NameValueCollection parameters = GetUrlParameters(dataStr);
            if (parameters["user_lang"] != null) app.View_contact_by_id(parameters["id"].ToString(), parameters["user_lang"].ToString());
            this.close();
        }else if (IsEmail(dataStr))
        {
            this.app.search.Search_email(dataStr);
            this.close();
        }
        else if (IsPhoneNumber(dataStr))
        {
            this.app.search.Search_phone_number(dataStr);
            this.close();
        }
        else
        {
            this.panel_data_qr.SetActive(true);
            this.txt_data_qr.text = dataStr;
        }
    }


    public void btn_camera_flip()
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

    public void create_Code()
    {
        if (codeWtr != null)
        {
            string id_user = app.carrot.user.get_id_user_login();
            if (id_user != "")
            {
                this.StopReader();
                string s_user_link = app.carrot.mainhost + "/?p=phone_book&id=" + app.carrot.user.get_id_user_login() + "&user_lang=" + app.carrot.user.get_lang_user_login();
                codeWtr.CreateCode(codetype, s_user_link);
            }
        }
    }

    public void Change_create_and_read()
    {
        this.panel_data_qr.SetActive(false);
        this.app.play_sound(2);
        if (this.is_show_create)
        {
            this.readCode();
            this.StartReader();
            this.is_show_create = false;
        }
        else
        {
            this.is_show_create = true;
            this.create_Code();
        }
        this.check_show_btn_create_qr();
    }


    public void check_show_btn_create_qr()
    {
        if (this.is_show_create)
        {
            this.btn_create_qr_icon.sprite = this.icon_read_qr;
            this.btn_create_qr_txt.text = PlayerPrefs.GetString("qr_scan", "QR Scan");
            this.btn_flip_camera.gameObject.SetActive(false);
        }
        else
        {
            this.btn_create_qr_icon.sprite = this.icon_create_qr;
            this.btn_create_qr_txt.text = PlayerPrefs.GetString("qr_create", "QR Create");
            this.btn_flip_camera.gameObject.SetActive(true);
        }
    }

    public void delete_data_qr()
    {
        this.panel_data_qr.SetActive(false);
    }


    public void click_goto_data_link()
    {
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
        Regex phoneNumberRegex = new Regex(@"^\(\d{3}\) \d{3}-\d{4}$");
        return phoneNumberRegex.IsMatch(input);
    }

    static bool IsEmail(string input)
    {
        Regex emailRegex = new Regex(@"^\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b$");
        return emailRegex.IsMatch(input);
    }

    static bool IsUrl(string input)
    {
        Regex urlRegex = new Regex(@"^(http|https):\/\/([\w-]+(\.[\w-]+)+)([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?$");
        return urlRegex.IsMatch(input);
    }
}
