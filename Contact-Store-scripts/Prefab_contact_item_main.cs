using Carrot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Prefab_contact_item_main : MonoBehaviour {
	public Text txt_name;
	public Text txt_address;
	public Text txt_phone;
	public Image img_avatar;
	public string s_user_id;
	public string s_user_lang;
	public Image img_btn_call;
	public GameObject btn_sms;
	public GameObject btn_del;
	public Image img_icon_contact;
	public int type=0;
	private IList list_info;
	public Carrot.Carrot carrot;
	public GameObject prefab_contacts_act;
	private string s_data;
	private int index;
	public void click(){
		if (this.type == 0) {
			GameObject.Find ("App_Contacts").GetComponent<App_Contacts> ().view_contact (this.s_user_id,this.s_user_lang);
		}

		if(this.type==1){
			/*
			GameObject.Find("App_Contacts").GetComponent<App_Contacts>().StopAllCoroutines();
			this.carrot.stop_all_act();
			this.carrot.show_list_box(this.txt_name.text, this.img_avatar.sprite);
			this.carrot.img_box_icon.color = Color.white;
			for(int i = 0; i < this.list_info.Count; i++)
            {
				IDictionary data_info =(IDictionary) this.list_info[i];
				if (data_info["val"].ToString() == "") continue;
				GameObject item_info=Instantiate(this.carrot.item_user_info_prefab);
				item_info.transform.SetParent(this.carrot.area_body_box);
				item_info.transform.localPosition = new Vector3(item_info.transform.localPosition.x, item_info.transform.localPosition.y, 0f);
				item_info.transform.localScale = new Vector3(1f, 1f, 1f);
				item_info.transform.localRotation = Quaternion.Euler(Vector3.zero);
				item_info.GetComponent<Carrot_item_user_info>().txt_value.text = data_info["val"].ToString();
				item_info.GetComponent<Carrot_item_user_info>().txt_title.text = PlayerPrefs.GetString(data_info["title"].ToString(), data_info["title_en"].ToString());
				item_info.GetComponent<Carrot_item_user_info>().type= data_info["type_update"].ToString();
				if (data_info["val_update"] != null)
					item_info.GetComponent<Carrot_item_user_info>().set_val_en(Json.Serialize(data_info["val_update_en"]), data_info["val"].ToString());
				else
					item_info.GetComponent<Carrot_item_user_info>().txt_value.text = data_info["val"].ToString();
				if (data_info["id_name"] != null)
				{
					string s_id_name_field = data_info["id_name"].ToString();
					if (s_id_name_field == "sdt") s_id_name_field = "phone";
					if (s_id_name_field == "user_link") s_id_name_field = "web";
					this.carrot.load_file_img("field_" + s_id_name_field + ".png", item_info.GetComponent<Carrot_item_user_info>().img_icon);
				}
			}

			GameObject item_act_contact = Instantiate(this.prefab_contacts_act);
			item_act_contact.transform.SetParent(this.carrot.area_body_box);
			item_act_contact.transform.localPosition = new Vector3(item_act_contact.transform.localPosition.x, item_act_contact.transform.localPosition.y, 0f);
			item_act_contact.transform.localScale = new Vector3(1f, 1f, 1f);
			item_act_contact.transform.localRotation = Quaternion.Euler(Vector3.zero);
			item_act_contact.GetComponent<Item_contacts_act>().set_data_by_contact(this.index,s_data);
			*/
		}

		if (this.type == 2) {
			GameObject.Find ("App_Contacts").GetComponent<App_Contacts> ().view_contact_import(int.Parse(this.s_user_id));
		}
	}

	public void call(){
		Application.OpenURL ("tel://" + this.txt_phone.text);
		Debug.Log ("Call:" + this.txt_phone.text);
	}

	public void sms(){
		Application.OpenURL ("sms://" + this.txt_phone.text);
		Debug.Log ("sms:" + this.txt_phone.text);
	}

	public void set_data(int index,string s_data,bool is_call_model)
    {
		IDictionary data_contact = (IDictionary)Carrot.Json.Deserialize(s_data);
		this.list_info = (IList)data_contact["list_info"];
		this.s_user_id = data_contact["user_id"].ToString();
		//this.carrot.load_file_img(this.s_user_id + ".png", this.img_avatar,60);
		this.txt_name.text = this.get_val_in_field("name");
		this.txt_address.text = this.get_val_in_field("address");
		this.txt_phone.text = this.get_val_in_field("sdt");
		this.type = 1;
		this.index = index;
		this.s_data = s_data;
		if (is_call_model)
		{
			this.btn_del.SetActive(false);
			this.btn_sms.SetActive(true);
			this.img_btn_call.gameObject.SetActive(true);
        }
        else
        {
			this.btn_del.SetActive(true);
			this.btn_sms.SetActive(false);
			this.img_btn_call.gameObject.SetActive(false);
		}
	}

	private string get_val_in_field(string s_id_name)
	{
		for (int i = 0; i < this.list_info.Count; i++)
		{
			IDictionary data_item = (IDictionary)this.list_info[i];
			if (s_id_name == data_item["id_name"].ToString()) return data_item["val"].ToString();
		}
		return "";
	}

	public void btn_delete()
	{
		GameObject.Find("App_Contacts").GetComponent<Book_contact>().delete_contact(this.index);
	}
}
