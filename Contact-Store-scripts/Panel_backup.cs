using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Panel_backup : MonoBehaviour
{
	public Sprite icon_backup;
	public GameObject prefab_backup_item;
	public GameObject prefab_view_backup_item;
	public GameObject prefab_contact_backup;
	public Transform area_list_backup;
	private App_Contacts cts;
	public void show()
	{
		this.gameObject.SetActive(true);
		this.cts = GameObject.Find("App_Contacts").GetComponent<App_Contacts>();
		this.show_list_backup();
	}

	private void show_list_backup()
    {
		this.cts.add_item_loading(this.area_list_backup);
		WWWForm frm_backup = this.cts.carrot.frm_act("list_backup");
		frm_backup.AddField("user_id", this.cts.carrot.get_id_user_login());
		frm_backup.AddField("user_lang", this.cts.carrot.get_lang_user_login());
		this.cts.carrot.send_hide(frm_backup, list_backup_handle);
	}

	private void list_backup_handle(string s_data)
	{
		GameObject.Find("App_Contacts").GetComponent<App_Contacts>().carrot.clear_contain(this.area_list_backup);
		IList list_backup = (IList)Carrot.Json.Deserialize(s_data);
		if (list_backup.Count > 0)
		{
			foreach (IDictionary i_backup in list_backup)
			{
				GameObject obj_item_backup = Instantiate(this.prefab_backup_item);
				obj_item_backup.transform.SetParent(this.area_list_backup);
				obj_item_backup.transform.localPosition = new Vector3(obj_item_backup.transform.localPosition.x, obj_item_backup.transform.localPosition.y, 0f);
				obj_item_backup.transform.localScale = new Vector3(1f, 1f, 1f);
				obj_item_backup.transform.localRotation = Quaternion.Euler(Vector3.zero);
				obj_item_backup.GetComponent<Panel_backup_item>().txt_name.text = i_backup["tip"].ToString();
				obj_item_backup.GetComponent<Panel_backup_item>().id = i_backup["id"].ToString();
				obj_item_backup.GetComponent<Panel_backup_item>().lang = i_backup["lang"].ToString();
			}
        }
        else
        {
			this.cts.add_none_info(this.area_list_backup);
        }
	}

	public void close()
    {
        this.gameObject.SetActive(false);
    }

    public void start_backup()
    {
		this.cts.carrot.send(this.cts.GetComponent<Book_contact>().frm_submit_backup_contact(), act_start_backup);
	}

	private void act_start_backup(string s_data)
    {
		this.cts.carrot.show_msg(PlayerPrefs.GetString("backup_title", "Back up contacts"), PlayerPrefs.GetString("backup_success", "Create backup of contacts successfully"));
		this.show_list_backup();
    }

	public void delete_item_backup(string id_backup,string lang_backup)
	{
		WWWForm frm_backup = GameObject.Find("App_Contacts").GetComponent<App_Contacts>().carrot.frm_act("delete_backup");
		frm_backup.AddField("id_backup", id_backup);
		frm_backup.AddField("lang_backup", lang_backup);
		GameObject.Find("App_Contacts").GetComponent<App_Contacts>().carrot.send(frm_backup, delete_backup);
	}

	private void delete_backup(string s_data)
	{
		this.cts.carrot.show_msg(PlayerPrefs.GetString("backup_title", "Back up contacts"), PlayerPrefs.GetString("backup_delete_success", "Delete backup successfully"));
		this.show_list_backup();
	}

	public void view_backup(string id_backup,string lang_backup)
    {
		WWWForm frm = GameObject.Find("App_Contacts").GetComponent<App_Contacts>().carrot.frm_act("view_backup");
		frm.AddField("id_backup",id_backup);
		frm.AddField("lang_backup", lang_backup);
		GameObject.Find("App_Contacts").GetComponent<App_Contacts>().carrot.send(frm, act_view_backup);
	}

	private void act_view_backup(string s_data)
    {
		IDictionary data = (IDictionary)Carrot.Json.Deserialize(s_data);
		this.cts.carrot.show_list_box(data["comment"].ToString(), this.icon_backup);
		IList list_contact = (IList)Carrot.Json.Deserialize(data["data"].ToString());
		for (int i = 0; i < list_contact.Count; i++) 
        {
			IDictionary data_backup = (IDictionary)list_contact[i];
			GameObject obj_item_backup = Instantiate(this.prefab_contact_backup);
			obj_item_backup.transform.SetParent(this.cts.carrot.area_body_box);
			obj_item_backup.transform.localPosition = new Vector3(obj_item_backup.transform.localPosition.x, obj_item_backup.transform.localPosition.y, 0f);
			obj_item_backup.transform.localScale = new Vector3(1f, 1f, 1f);
			obj_item_backup.transform.localRotation = Quaternion.Euler(Vector3.zero);
			obj_item_backup.GetComponent<item_contact_backup>().txt_name.text = data_backup["name"].ToString();
			string s_phone_contact="";
			if (data_backup["phone"] != null)
			{
				IList list_phone = (IList)data_backup["phone"];
				if(list_phone.Count>0)if(list_phone[0]!=null) s_phone_contact = list_phone[0].ToString();
			}

            if(s_phone_contact=="")
            {
				if (data_backup["email"] != null)
				{
					IList list_email = (IList)data_backup["email"];
					if(list_email.Count>0)if (list_email[0] != null) s_phone_contact = list_email[0].ToString();
				}
			}

			obj_item_backup.GetComponent<item_contact_backup>().txt_phone.text = s_phone_contact;
		}
    }

	public void export_backup(string id_backup, string lang_backup)
	{
		WWWForm frm = this.cts.carrot.frm_act("view_backup");
		frm.AddField("id_backup", id_backup);
		frm.AddField("lang_backup", lang_backup);
		this.cts.carrot.send(frm, act_export_backup);
	}

	private void act_export_backup(string s_data)
    {
		Debug.Log("Export Backup:" + s_data);
		IDictionary data = (IDictionary)Carrot.Json.Deserialize(s_data);
		IList list_contact = (IList)Carrot.Json.Deserialize(data["data"].ToString());
		for (int i = 0; i < list_contact.Count; i++)
		{
			IDictionary contact_info = (IDictionary)list_contact[i];
			Carrot.Contact_carrot c = new Carrot.Contact_carrot("backup");
			c.add_field("1", "name", "user_name",contact_info["name"].ToString(),"","");
			IList list_phone = (IList)contact_info["phone"];
			for(int y=0;y<list_phone.Count;y++) c.add_field("4", "sdt", "user_phone", list_phone[y].ToString(),"","");
			Debug.Log("Contact new:" + c.get_json());
			this.cts.GetComponent<Book_contact>().add_contact(c.user_id, c.get_json(), null);
		}
	}

}
