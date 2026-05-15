
using Carrot;
using UnityEngine;

public class QR_scan : MonoBehaviour
{
    [Header("Main Obj")]
    public App_Contacts app;

    [Header("QR Obj")]
    public Sprite icon_create_qr;
    public Sprite icon_read_qr;

    public void Show_QR_create_by_data(string data_str)
    {
        app.carrot.play_sound_click();
        app.carrot.show_loading();
        string url_create = "https://chart.googleapis.com/chart?chs=350x350&cht=qr&chl=" + data_str;
        this.app.carrot.get_img(url_create, Get_qr_image_done);
    }

    private void Get_qr_image_done(Texture2D tex)
    {
        app.carrot.hide_loading();
        app.play_sound(2);
        app.carrot.camera_pro.show_photoshop(tex,2f);
    }

    public void Create_Code()
    {
        string id_user = app.carrot.user.get_id_user_login();
        if (id_user == "")
        {
            this.app.carrot.user.show_login(() => Create_Code());
        }
        else
        {
            this.app.carrot.play_sound_click();
            string s_email=app.carrot.user.get_data_user_login("email");
            string s_phone=app.carrot.user.get_data_user_login("phone");

            Carrot_Box box_qr = this.app.carrot.Create_Box();
            box_qr.set_title(app.carrot.lang.Val("qr_tip", "QR"));
            box_qr.set_icon(this.icon_create_qr);

            if (s_phone != "")
            {
                Carrot_Box_Item item_qr_phone = box_qr.create_item("item_qr_phone");
                item_qr_phone.set_icon(this.icon_read_qr);
                item_qr_phone.set_title("Phone");
                item_qr_phone.set_tip(app.carrot.lang.Val("user_phone", "Phone number"));
                item_qr_phone.set_act(() => this.Show_QR_create_by_data(s_phone));
            }

            if (s_email != "")
            {
                Carrot_Box_Item item_qr_email = box_qr.create_item("item_qr_email");
                item_qr_email.set_icon(this.icon_read_qr);
                item_qr_email.set_title("Email");
                item_qr_email.set_tip(app.carrot.lang.Val("user_email", "Email (Email)"));
                item_qr_email.set_act(() => this.Show_QR_create_by_data(s_email));
            }
        }
    }

    public void Btn_create_code()
    {
        this.app.carrot.play_sound_click();
        this.Create_Code();
    }

}