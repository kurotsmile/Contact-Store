using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field_contact : MonoBehaviour
{
    public Sprite icon_fields;
    public GameObject prefab_item_field;
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
        this.carrot.show_list_box_search(PlayerPrefs.GetString("add_field_other", "More information"), this.icon_fields, show_search_field);
        WWWForm frm = this.GetComponent<App_Contacts>().carrot.frm_act("list_field");
        this.GetComponent<App_Contacts>().carrot.send(frm, act_show_list_field);
    }

    private void show_list_filed_offline()
    {
        this.carrot.show_list_box(PlayerPrefs.GetString("add_field_other", "More information"), this.icon_fields);
        string s_data_field = PlayerPrefs.GetString("filed_data");
        Debug.Log("Fields:" + s_data_field);
        this.show_list_field_by_data(s_data_field);
    }

    private void show_search_field()
    {
        WWWForm frm_search = this.carrot.frm_act("search_field");
        this.carrot.show_search(frm_search,act_done_search, PlayerPrefs.GetString("search_field_tip", "Enter the field information you want to add (e.g. tiktok,facebook,gmail...)"));
    }

    private void act_done_search(string s_data)
    {
        this.carrot.hide_search();
        Debug.Log("search:" + s_data);
        this.carrot.clear_contain(this.carrot.area_body_box);
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
        IList list_field = (IList)Carrot.Json.Deserialize(s_data);
        for (int i = 0; i < list_field.Count; i++)
        {
            IDictionary data_field = (IDictionary)list_field[i];
            GameObject item_field = Instantiate(this.prefab_item_field);
            item_field.transform.SetParent(this.carrot.area_body_box);
            item_field.transform.localPosition = new Vector3(item_field.transform.localPosition.x, item_field.transform.localPosition.y, 0f);
            item_field.transform.localScale = new Vector3(1f, 1f, 1f);
            item_field.transform.localRotation = Quaternion.Euler(Vector3.zero);
            item_field.GetComponent<Item_field>().txt_name.text = data_field["name"].ToString();
            item_field.GetComponent<Item_field>().txt_tip.text = data_field["tip"].ToString();
            item_field.GetComponent<Item_field>().s_name_id = data_field["id"].ToString();
            item_field.GetComponent<Item_field>().s_type = data_field["type"].ToString();
            item_field.GetComponent<Item_field>().s_val = data_field["val"].ToString();
            string s_arr_key = Carrot.Json.Serialize(data_field["arr_key"]);
            string s_arr_val = Carrot.Json.Serialize(data_field["arr_val"]);
            item_field.GetComponent<Item_field>().s_val_sel = s_arr_key;
            item_field.GetComponent<Item_field>().s_val_sel_en = s_arr_val;
            if (this.is_save_field_offline)
                this.carrot.get_img(data_field["icon"].ToString(), item_field.GetComponent<Item_field>().icon);
            else
                this.carrot.load_file_img("field_"+data_field["id"].ToString()+".png", item_field.GetComponent<Item_field>().icon);
        }
    }

    private void save_filed_offline()
    {
        foreach(Transform item_field in this.carrot.area_body_box)
        {
            Item_field data_field = item_field.GetComponent<Item_field>();
            this.carrot.save_file("field_" + data_field.s_name_id+".png", data_field.icon.sprite.texture.EncodeToPNG());
        }
        this.is_save_field_offline = false;
    }

    public void add_info_by_field_contact(Item_field field_item)
    {
        if (this.carrot.check_file_exist("field_" + field_item.s_name_id + ".png")) this.carrot.save_file("field_"+field_item.s_name_id+".png",field_item.icon.sprite.texture.EncodeToPNG());
        this.GetComponent<Book_contact>().add_field_edit(field_item.s_type, field_item.s_val, field_item.s_name_id, field_item.txt_name.text, field_item.txt_name.text, field_item.s_val_sel, field_item.s_val_sel_en, true);
        this.carrot.close();
    }

}
