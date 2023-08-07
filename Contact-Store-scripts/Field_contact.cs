using Firebase.Extensions;
using System.Collections;
using UnityEngine;

public class Field_contact : MonoBehaviour
{
    public Sprite icon_fields;
    private Carrot.Carrot carrot;
    private bool is_save_field_offline=false;
    public void load_field()
    {
        this.carrot = this.GetComponent<App_Contacts>().carrot;
        this.is_save_field_offline = true;
    }

    public void show_list_field()
    {
        if(this.carrot.is_online())
            this.show_list_filed_online();
        else
            this.show_list_filed_offline();
    }

    private void show_list_filed_online()
    {
        this.carrot.show_search(act_done_search,PlayerPrefs.GetString("add_field_other", "More information"));
    }

    private void act_done_search()
    {
        this.carrot.db.Collection("user-" + this.carrot.lang.get_key_lang()).WhereEqualTo("name","").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {

        });
    }

    private void show_list_filed_offline()
    {
        this.carrot.Create_Box(PlayerPrefs.GetString("add_field_other", "More information"), this.icon_fields);
        string s_data_field = PlayerPrefs.GetString("filed_data");
        Debug.Log("Fields:" + s_data_field);
        this.show_list_field_by_data(s_data_field);
    }

    private void show_search_field()
    {
        this.carrot.show_search(act_done_search, PlayerPrefs.GetString("search_field_tip", "Enter the field information you want to add (e.g. tiktok,facebook,gmail...)"));
    }

    private void act_done_search(string s_data)
    {
        this.carrot.hide_loading();
        Debug.Log("search:" + s_data);
        this.show_list_field_by_data(s_data);
    }

    private void act_show_list_field(string s_data)
    {
        Debug.Log(s_data);
        PlayerPrefs.SetString("filed_data", s_data);
        this.show_list_field_by_data(s_data);
        if(this.is_save_field_offline)
        if(this.carrot.is_online())this.carrot.delay_function(1.5f, save_filed_offline);
    }

    private void show_list_field_by_data(string s_data)
    {
        Carrot.Carrot_Box list_box_field = this.carrot.Create_Box();
        list_box_field.set_title("List Field");
        list_box_field.set_icon(this.icon_fields);
        IList list_field = (IList)Carrot.Json.Deserialize(s_data);
        for (int i = 0; i < list_field.Count; i++)
        {
            IDictionary data_field = (IDictionary)list_field[i];
            Carrot.Carrot_Box_Item item_box_field = list_box_field.create_item("item_box_field");
            item_box_field.txt_name.text= data_field["name"].ToString();
            item_box_field.txt_tip.text= data_field["tip"].ToString();

            string s_arr_key = Carrot.Json.Serialize(data_field["arr_key"]);
            string s_arr_val = Carrot.Json.Serialize(data_field["arr_val"]);
        }
    }

    private void save_filed_offline()
    {
        this.is_save_field_offline = false;
    }

    public void add_info_by_field_contact(Item_field field_item)
    {
        if (this.carrot.get_tool().check_file_exist("field_" + field_item.s_name_id + ".png")) this.carrot.get_tool().save_file("field_"+field_item.s_name_id+".png",field_item.icon.sprite.texture.EncodeToPNG());
        this.GetComponent<Book_contact>().add_field_edit(field_item.s_type, field_item.s_val, field_item.s_name_id, field_item.txt_name.text, field_item.txt_name.text, field_item.s_val_sel, field_item.s_val_sel_en, true);
        this.carrot.close();
    }

}
