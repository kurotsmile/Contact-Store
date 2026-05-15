using UnityEngine;
using UnityEngine.UI;

public class Manager_Menu : MonoBehaviour
{
    [Header("Obj Main")]
    public App_Contacts app;

    [Header("Menu Footer and Extension Btn")]
    public Image[] img_menu_footer_bk;
    public GameObject[] btn_extension_add;

    [Header("Color")]
    public Color32 color_sel;
    public Color32 color_normal;

    private int index_cur_func = -1;

    public void load()
    {
        this.index_cur_func = PlayerPrefs.GetInt("index_cur_func",0);
        this.select_menu(this.index_cur_func);
    }

    private void select_menu(int index)
    {
        this.index_cur_func = index;

        for (int i = 0; i < this.img_menu_footer_bk.Length; i++)
        {
            this.img_menu_footer_bk[i].color = this.color_normal;
            this.btn_extension_add[i].gameObject.SetActive(false);
        }

        this.img_menu_footer_bk[this.index_cur_func].color = this.color_sel;
        this.btn_extension_add[this.index_cur_func].SetActive(true);

        if(this.index_cur_func!=3)this.app.call.panel_call.SetActive(false);
        if (this.index_cur_func == 0) this.app.manager_contact.List();
        if (this.index_cur_func == 1) this.app.book_contact.show();
        if (this.index_cur_func == 2) this.app.backup_Contacts.Show();
        if (this.index_cur_func == 3) this.app.btn_show_call();
    }

    public void click_menu(int index)
    {
        this.app.play_sound(0);
        PlayerPrefs.SetInt("index_cur_func", index);
        this.select_menu(index);
        this.app.carrot.ads.show_ads_Interstitial();
    }
}
