using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_field : MonoBehaviour
{
    public Image icon;
    public Text txt_name;
    public Text txt_tip;
    public string s_name_id;
    public string s_type;
    public string s_val;
    public string s_val_sel;
    public string s_val_sel_en;
    public void click()
    {
        GameObject.Find("App_Contacts").GetComponent<Field_contact>().add_info_by_field_contact(this);
    }
}
