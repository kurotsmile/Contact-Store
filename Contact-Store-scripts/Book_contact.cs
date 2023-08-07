using Carrot;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class Book_contact : MonoBehaviour
{
	private int leng = 0;
	[Header("Contacts Book Obj")]
	public Transform area_body_main;
	public Texture2D avatar_default;
	public GameObject menu_footer_edit;

	[Header("Icon contacts")]
	public Sprite sp_delete;
	public Sprite sp_call;
	public Sprite sp_back;
	public Sprite sp_icon_import;
	public Sprite sp_icon_add_phone;
	public Sprite sp_icon_update_phone;

	private bool is_call_model;
	private App_Contacts ct;
	private string edit_user_id="";
	private int edit_index_contact = -1;
	private bool is_edit_user_login = false;
	private bool is_import_contact = false;
	public void load_book_contact()
	{
		this.ct = this.GetComponent<App_Contacts>();
		this.leng = PlayerPrefs.GetInt("contact_leng", 0);
		this.check_call_model();
		this.menu_footer_edit.SetActive(false);
		if (PlayerPrefs.GetInt("is_import_contact", 0) == 0)
			this.is_import_contact = false;
		else
			this.is_import_contact = true;

	}

	private void check_call_model()
	{
		if (PlayerPrefs.GetInt("is_call_model", 0) == 0) this.is_call_model = true;
		else this.is_call_model = false;
	}

	public void change_model_list_contacts()
	{
		if (this.is_call_model) PlayerPrefs.SetInt("is_call_model", 1);
		else PlayerPrefs.SetInt("is_call_model", 0);
		this.check_call_model();
		this.show_list_book();
	}

	private bool check_ready_book(string id)
	{
		for (int i = 0; i < this.leng; i++)
		{
			if (PlayerPrefs.GetString("contact_data" + i, "") != "")
			{
				IDictionary data = (IDictionary)Carrot.Json.Deserialize(PlayerPrefs.GetString("contact_data" + i));
				if (data["user_id"].ToString() == id) return true;
			}
		}
		return false;
	}

	public void add_contact(string id, string str_data, byte[] avatar)
	{
		Debug.Log("Add contact:" + str_data);
		if (this.check_ready_book(id) == false)
		{
			PlayerPrefs.SetString("contact_data" + this.leng, str_data);
			if (avatar != null) this.ct.carrot.get_tool().save_file(id + ".png", avatar);
			this.leng++;
			PlayerPrefs.SetInt("contact_leng", this.leng);
			this.ct.carrot.show_msg(PlayerPrefs.GetString("app_title", "app_title"), PlayerPrefs.GetString("save_success", "save_success"), Carrot.Msg_Icon.Success);
		}
		else
		{
			this.ct.carrot.show_msg(PlayerPrefs.GetString("app_title", "app_title"), PlayerPrefs.GetString("save_fail", "save_fail"), Carrot.Msg_Icon.Error);
		}
	}

	public void update_contact(int index, string str_data, byte[] avatar)
	{
		PlayerPrefs.SetString("contact_data" + index, str_data);
		IDictionary data_contact = (IDictionary)Carrot.Json.Deserialize(str_data);
		if (avatar != null)
		{
			string user_id = data_contact["user_id"].ToString();
			this.ct.carrot.get_tool().save_file(user_id + ".png", avatar);
		}
		this.ct.carrot.show_msg(PlayerPrefs.GetString("app_title", "app_title"), PlayerPrefs.GetString("save_success", "save_success"), Carrot.Msg_Icon.Success);
	}

	public void show_list_book()
	{
		this.ct.StopAllCoroutines();
		this.ct.carrot.stop_all_act();
		this.ct.btn_add_contact.SetActive(true);
		this.ct.btn_add_account.SetActive(false);
		this.ct.img_btn_contact_home.color = this.ct.color_normal;
		this.ct.img_btn_contact_store.color = this.ct.color_sel;
		this.ct.img_search.sprite = this.ct.icon_search_contact;
		this.ct.button_search_option.SetActive(false);
		if(this.ct.carrot.is_online())
			this.ct.button_contact_backup.SetActive(true);
		else
			this.ct.button_contact_backup.SetActive(false);
		this.ct.button_edit_contact_user_login.SetActive(false);

		PlayerPrefs.SetInt("is_view_contact", 1);
		this.area_body_main.parent.gameObject.SetActive(true);
		this.area_body_main.gameObject.SetActive(true);

		this.ct.StopAllCoroutines();
		this.ct.carrot.clear_contain(this.area_body_main);

		GameObject offline_info = Instantiate(this.ct.prefab_tip_info);
		offline_info.transform.SetParent(this.area_body_main);
		offline_info.transform.localPosition = new Vector3(offline_info.transform.localPosition.x, offline_info.transform.localPosition.y, 0f);
		offline_info.transform.localScale = new Vector3(1f, 1f, 1f);
		offline_info.transform.localRotation = Quaternion.Euler(Vector3.zero);
		offline_info.GetComponent<Panel_info>().icon.color = Color.black;
		offline_info.GetComponent<Panel_info>().txt_title.text = PlayerPrefs.GetString("book_contact", "book_contact");
		offline_info.GetComponent<Panel_info>().txt_tip.text = PlayerPrefs.GetString("book_contact_tip1", "book_contact_tip1");
		if (this.is_call_model)
		{
			offline_info.GetComponent<Panel_info>().type = 0;
			offline_info.GetComponent<Panel_info>().btn_action.GetComponent<Image>().sprite = this.sp_delete;
		}
		else
		{
			offline_info.GetComponent<Panel_info>().type = 1;
			offline_info.GetComponent<Panel_info>().btn_action.GetComponent<Image>().sprite = this.sp_call;
		}

		for (int i = 0; i < this.leng; i++)
		{
			string s_data_contact = PlayerPrefs.GetString("contact_data" + i, "");
			if (s_data_contact != "")
			{
				GameObject book_item = Instantiate(this.ct.prefab_contact_main_item);
				book_item.transform.SetParent(this.area_body_main);
				book_item.transform.localPosition = new Vector3(book_item.transform.localPosition.x, book_item.transform.localPosition.y, 0f);
				book_item.transform.localScale = new Vector3(1f, 1f, 1f);
				book_item.transform.localRotation = Quaternion.Euler(Vector3.zero);
				book_item.GetComponent<Prefab_contact_item_main>().s_user_id = i.ToString();
				book_item.GetComponent<Prefab_contact_item_main>().type = 1;
				book_item.GetComponent<Prefab_contact_item_main>().carrot = this.ct.carrot;
				book_item.GetComponent<Prefab_contact_item_main>().set_data(i, s_data_contact, this.is_call_model);
			}
		}

		if (this.ct.carrot.is_online())
		{
			this.ct.add_tip_info(this.sp_back, PlayerPrefs.GetString("phonebook"), PlayerPrefs.GetString("book_contact_tip2"), 2);
			this.ct.add_tip_info(this.sp_icon_import, PlayerPrefs.GetString("book_data_import"), PlayerPrefs.GetString("book_data_import_tip"), 3);
			this.get_import_contact();
		}
		this.ct.check_link_deep_app();
	}

	public void delete_contact(int index)
	{
		this.act_delete_contact(index);
		this.ct.carrot.show_msg(PlayerPrefs.GetString("app_title", "app_title"), PlayerPrefs.GetString("delete_success", "delete_success"), Carrot.Msg_Icon.Success);
		this.show_list_book();
	}

	private void act_delete_contact(int index)
    {
		string s_data_contact = PlayerPrefs.GetString("contact_data" + index);
		if (s_data_contact != "")
		{
			IDictionary data = (IDictionary)Carrot.Json.Deserialize(s_data_contact);
			if (data != null)
			{
				if (data["user_id"] != null)
				{
					if (data["user_id"].ToString() != "")
						this.ct.carrot.get_tool().delete_file(data["user_id"].ToString() + ".png");
				}
			}
			PlayerPrefs.DeleteKey("contact_data" + index);
		}
	}

	public void delete_all_contact()
    {
		for(int i = 0; i < this.leng; i++) this.act_delete_contact(i);
    }

	public void import_my_contact()
	{
		this.is_import_contact = true;
		string s_link_get_contact;
        if (this.ct.carrot.user.get_id_user_login() != "")
			s_link_get_contact=this.ct.carrot.mainhost + "/getcontact?id_device=" + SystemInfo.deviceUniqueIdentifier + "&user_id=" + this.ct.carrot.user.get_id_user_login() + "&user_lang=" + this.ct.carrot.user.get_lang_user_login();
        else
			s_link_get_contact = this.ct.carrot.mainhost + "/getcontact?id_device=" + SystemInfo.deviceUniqueIdentifier+ "&user_lang="+PlayerPrefs.GetString("lang","en");
		PlayerPrefs.SetInt("is_import_contact", 1);
		Application.OpenURL(s_link_get_contact);
	}

	void OnApplicationFocus(bool hasFocus)
	{
		this.get_import_contact();
		if (this.ct != null) this.ct.check_link_deep_app();
	}

	private void get_import_contact()
    {
		if (this.is_import_contact)
		{
			WWWForm frm_get_import = this.ct.carrot.frm_act("get_import");
			//this.ct.carrot.send_hide(frm_get_import, this.act_get_import);
		}
	}

	private void act_get_import(string s_data)
	{
		IDictionary data_import = (IDictionary)Json.Deserialize(s_data);
		if (data_import["error"].ToString() == "0")
		{
			IList list_info = (IList)Json.Deserialize(data_import["data"].ToString());
			for(int i = 0; i < list_info.Count; i++)
            {
				Contact_carrot c = new Contact_carrot("phone");
				IDictionary data_contact = (IDictionary)list_info[i];
                if (data_contact["name"] != null)
                {
					IList list_name = (IList)data_contact["name"];
					for (int y = 0; y < list_name.Count; y++) {
						c.add_field("1", "name", "user_name",list_name[y].ToString(),"", "");
					}
				}

				if (data_contact["tel"] != null)
				{
					IList list_phone = (IList)data_contact["tel"];
					for (int y = 0; y < list_phone.Count; y++)
					{
						c.add_field("8", "sdt", "user_phone", list_phone[y].ToString(), "", "");
					}
				}

				if (data_contact["address"] != null)
				{
					IList list_address = (IList)data_contact["address"];
					for (int y = 0; y < list_address.Count; y++)
					{
						IDictionary data_address = (IDictionary)list_address[y];
						string s_address = "";
						if (data_address["dependentLocality"] != null) if (data_address["dependentLocality"].ToString().Trim() != "") s_address = s_address + " " + data_address["dependentLocality"].ToString();
						if (data_address["city"] != null) if (data_address["city"].ToString().Trim() != "") s_address = s_address+" "+data_address["city"].ToString();
						if (data_address["region"] != null) if (data_address["region"].ToString().Trim() != "") s_address = s_address + " " + data_address["region"].ToString();
						c.add_field("9", "address", "address", s_address.Trim(), "", "");
					}
				}

				if (data_contact["email"] != null)
				{
					IList list_email = (IList)data_contact["email"];
					for (int y = 0; y < list_email.Count; y++)
					{
						c.add_field("5", "mail", "mail", list_email[y].ToString(), "", "");
					}
				}

				this.add_contact(c.user_id, c.get_json(), null);
			}
			
			if (list_info.Count > 0) { 
				this.ct.carrot.show_msg(PlayerPrefs.GetString("book_data_import", "Contacts from the phone"), PlayerPrefs.GetString("book_data_import_success", "Get the phone contacts to the application successfully!"), Carrot.Msg_Icon.Success);
				this.ct.carrot.delay_function(2f, this.show_list_book);
			}
		}
		this.is_import_contact = false;
		PlayerPrefs.SetInt("is_import_contact",0);
		Debug.Log("Import:" + s_data);
	}

	public void add_new_contact(string s_new_phone)
	{
		this.ct.carrot.clear_contain(this.ct.area_body_main);
		GameObject offline_info = Instantiate(this.ct.prefab_tip_info);
		offline_info.transform.SetParent(this.area_body_main);
		offline_info.transform.localPosition = new Vector3(offline_info.transform.localPosition.x, offline_info.transform.localPosition.y, 0f);
		offline_info.transform.localScale = new Vector3(1f, 1f, 1f);
		offline_info.transform.localRotation = Quaternion.Euler(Vector3.zero);
		offline_info.GetComponent<Panel_info>().icon.sprite = this.sp_icon_add_phone;
		offline_info.GetComponent<Panel_info>().icon.color = Color.black;
		offline_info.GetComponent<Panel_info>().txt_title.text = PlayerPrefs.GetString("add_contact", "Add new contact");
		offline_info.GetComponent<Panel_info>().txt_tip.text = PlayerPrefs.GetString("add_contact_tip", "New contact created successfully!");
		offline_info.GetComponent<Panel_info>().btn_action.gameObject.SetActive(false);
		offline_info.GetComponent<Panel_info>().type=- 1;

		/*
		GameObject item_field_avatar = Instantiate(this.ct.carrot.item_user_edit_prefab);
		item_field_avatar.name = "field_avatar";
		item_field_avatar.transform.SetParent(this.area_body_main);
		item_field_avatar.transform.localPosition = new Vector3(item_field_avatar.transform.localPosition.x, item_field_avatar.transform.localPosition.y, 0f);
		item_field_avatar.transform.localScale = new Vector3(1f, 1f, 1f);
		item_field_avatar.transform.localRotation = Quaternion.Euler(Vector3.zero);
		item_field_avatar.GetComponent<Carrot_item_user_edit>().set_data("6", "");
		item_field_avatar.GetComponent<Carrot_item_user_edit>().s_name = "avatar";
		item_field_avatar.GetComponent<Carrot_item_user_edit>().txt_title.text = PlayerPrefs.GetString("avatar_user", "Avatar");
		*/

		this.add_field_edit("1", "", "name", "user_name", "Full name","","",false);
		this.add_field_edit("8", s_new_phone, "sdt", "user_phone", "Phone number","","",false);

		this.edit_index_contact = -1;
		this.edit_user_id = "";
		this.is_edit_user_login = false;

		this.ct.btn_add_contact.SetActive(false);
		this.menu_footer_edit.SetActive(true);
		//this.ct.carrot.data_avatar_user = null;
	}

	public void add_field_edit(string type_field,string val_field,string name_field,string title_field,string title_en_field,string val_select,string val_select_en,bool is_delete)
    {
		/*
		GameObject item_field_edit = Instantiate(this.ct.carrot.item_user_edit_prefab);
		item_field_edit.name = "field_add";
		item_field_edit.transform.SetParent(this.area_body_main);
		item_field_edit.transform.localPosition = new Vector3(item_field_edit.transform.localPosition.x, item_field_edit.transform.localPosition.y, 0f);
		item_field_edit.transform.localScale = new Vector3(1f, 1f, 1f);
		item_field_edit.transform.localRotation = Quaternion.Euler(Vector3.zero);
		item_field_edit.GetComponent<Carrot_item_user_edit>().set_data(type_field, val_field);
		item_field_edit.GetComponent<Carrot_item_user_edit>().s_name = name_field;
		item_field_edit.GetComponent<Carrot_item_user_edit>().txt_title.text = PlayerPrefs.GetString(title_field, title_en_field);
		item_field_edit.GetComponent<Carrot_item_user_edit>().button_delete.SetActive(is_delete);
	

		if (type_field == "2") item_field_edit.GetComponent<Carrot_item_user_edit>().set_data_select_down(val_select, val_select_en, val_field);
		*/
	}

	public void show_edit(int index,string s_data,bool is_edit_user_login_cur)
    {
		this.ct.carrot.clear_contain(this.area_body_main);
		IDictionary data = (IDictionary)Carrot.Json.Deserialize(s_data);
		IList list_info = (IList)data["list_info"];
		this.ct.carrot.close();
		//this.ct.carrot.data_avatar_user = null;

		this.is_edit_user_login = is_edit_user_login_cur;
		this.edit_index_contact = index;
		this.edit_user_id = data["user_id"].ToString();

		GameObject update_title= Instantiate(this.ct.prefab_tip_info);
		update_title.transform.SetParent(this.area_body_main);
		update_title.transform.localPosition = new Vector3(update_title.transform.localPosition.x, update_title.transform.localPosition.y, 0f);
		update_title.transform.localScale = new Vector3(1f, 1f, 1f);
		update_title.transform.localRotation = Quaternion.Euler(Vector3.zero);
		update_title.GetComponent<Panel_info>().icon.sprite = this.sp_icon_update_phone;
		update_title.GetComponent<Panel_info>().icon.color = Color.black;
		if(is_edit_user_login_cur)
			update_title.GetComponent<Panel_info>().txt_title.text = PlayerPrefs.GetString("acc_edit", "Update account information");
		else
			update_title.GetComponent<Panel_info>().txt_title.text = PlayerPrefs.GetString("update_contact", "Update contact");
		update_title.GetComponent<Panel_info>().txt_tip.text = PlayerPrefs.GetString("update_contact_tip", "Update and add contact information fields");
		update_title.GetComponent<Panel_info>().btn_action.gameObject.SetActive(false);
		update_title.GetComponent<Panel_info>().type = -1;

		/*
		GameObject item_field_avatar = Instantiate(this.ct.carrot.item_user_edit_prefab);
		item_field_avatar.name = "field_avatar";
		item_field_avatar.transform.SetParent(this.area_body_main);
		item_field_avatar.transform.localPosition = new Vector3(item_field_avatar.transform.localPosition.x, item_field_avatar.transform.localPosition.y, 0f);
		item_field_avatar.transform.localScale = new Vector3(1f, 1f, 1f);
		item_field_avatar.transform.localRotation = Quaternion.Euler(Vector3.zero);
		item_field_avatar.GetComponent<Carrot_item_user_edit>().set_data("6", "");
		item_field_avatar.GetComponent<Carrot_item_user_edit>().s_name = "avatar";
		item_field_avatar.GetComponent<Carrot_item_user_edit>().txt_title.text = PlayerPrefs.GetString("user_avatar", "Avatar");
        if (is_edit_user_login)
        {
			this.ct.carrot.load_file_img("avatar", item_field_avatar.GetComponent<Carrot_item_user_edit>().img_avatar);
		}
        else
        {
			this.ct.carrot.load_file_img(data["user_id"].ToString() + ".png", item_field_avatar.GetComponent<Carrot_item_user_edit>().img_avatar);
			if (this.ct.carrot.check_file_exist(data["user_id"].ToString() + ".png")) this.ct.carrot.data_avatar_user = item_field_avatar.GetComponent<Carrot_item_user_edit>().img_avatar.sprite.texture.EncodeToPNG();
		}
		*/

		for (int i = 0; i < list_info.Count; i++)
        {
			IDictionary data_item_info = (IDictionary)list_info[i];
			if (data_item_info["id_name"] != null)
			{
				if (data_item_info["id_name"].ToString() == "user_link") continue;
				if (this.is_edit_user_login == false)if (data_item_info["id_name"].ToString() == "status") continue;

				string val_sel = "";
				string val_sel_en = "";
				bool is_delete_field = false;
				if (data_item_info["val_update"]!=null) val_sel = Carrot.Json.Serialize(data_item_info["val_update"]);
				if (data_item_info["val_update_en"] != null) val_sel_en = Carrot.Json.Serialize(data_item_info["val_update_en"]);
				if(is_edit_user_login_cur)
					if (i > 7) is_delete_field = true; else is_delete_field = false;
				else
					if (i >= 2) is_delete_field = true; else is_delete_field = false;
				this.add_field_edit(data_item_info["type_update"].ToString(), data_item_info["val"].ToString(), data_item_info["id_name"].ToString(), data_item_info["title"].ToString(), data_item_info["title_en"].ToString(),val_sel,val_sel_en, is_delete_field);
			}
		}
		this.ct.btn_add_contact.SetActive(false);
		this.ct.btn_add_account.SetActive(false);
		this.menu_footer_edit.SetActive(true);
	}

	public void done_add_or_update_contact()
	{
		Contact_carrot c = new Contact_carrot("offline");
		 
		foreach (Transform child in this.area_body_main)
		{
			if (child.name == "field_add")
			{
				/*
				Carrot_item_user_edit item_field = child.GetComponent<Carrot_item_user_edit>();
				c.add_field(item_field.s_type,item_field.s_name, item_field.txt_title.text, item_field.get_val(), item_field.val_select, item_field.val_select_en);
				*/
			}
		}

		if (this.is_edit_user_login)
		{
			/*
			IDictionary data_user_login = (IDictionary)Carrot.Json.Deserialize(this.ct.carrot.s_data_user_login);
			c.user_id = this.ct.carrot.get_id_user_login();
			c.user_lang = this.ct.carrot.get_lang_user_login();
			WWWForm frm_update_contact = this.ct.carrot.frm_act("update_user_info");
			frm_update_contact.AddField("data", c.get_json());
			if(this.ct.carrot.data_avatar_user!=null) frm_update_contact.AddBinaryData("avatar", this.ct.carrot.data_avatar_user);
			this.ct.carrot.send(frm_update_contact, act_after_update_info_user_login);
			*/
		}
		else
		{
			if (this.edit_user_id != "")
			{
				c.user_id = this.edit_user_id;
				//this.update_contact(this.edit_index_contact, c.get_json(), this.ct.carrot.data_avatar_user);
				this.ct.carrot.show_msg(PlayerPrefs.GetString("app_title", "app_title"), PlayerPrefs.GetString("save_success", "save_success"), Msg_Icon.Success);
			}
			else
			{
				//this.add_contact(c.user_id, c.get_json(), this.ct.carrot.data_avatar_user);
				this.ct.carrot.show_msg(PlayerPrefs.GetString("add_contact", "Add new contact"), PlayerPrefs.GetString("add_contact_success", "New contact created successfully!"), Msg_Icon.Success);
			}
			this.menu_footer_edit.SetActive(false);
			this.show_list_book();
		}
	}

	private void act_after_update_info_user_login(string s_data)
    {
		IDictionary data_acc = (IDictionary)Carrot.Json.Deserialize(s_data);
		this.ct.carrot.show_msg(PlayerPrefs.GetString("acc_edit", "Update account information"),PlayerPrefs.GetString(data_acc["msg"].ToString(), data_acc["msg_en"].ToString()),Carrot.Msg_Icon.Success);
		if (data_acc["error"].ToString() == "0")
		{
			//this.ct.carrot.set_data_user_login(data_acc);
			this.menu_footer_edit.SetActive(false);
		}
	}

	public void cancel_edit_or_add()
    {
		this.show_list_book();
		this.menu_footer_edit.SetActive(false);
    }

	public WWWForm frm_submit_backup_contact()
	{
		string s_data_backup = "[";
		int length_contact = 0;
		for (int i = 0; i < this.leng; i++)
		{
			string s_data_contact = PlayerPrefs.GetString("contact_data" + i, "");
			if (s_data_contact != "")
			{
				IDictionary data = (IDictionary)Carrot.Json.Deserialize(s_data_contact);
				IDictionary contact = (IDictionary)data["contact"];
				if (contact== null)
				{
					Contact_backup cb = new Contact_backup((IList)data["list_info"]);
					s_data_backup = s_data_backup + cb.get_json() + ",";
					length_contact++;
				}
			}
		}

		s_data_backup = s_data_backup + "]";
		s_data_backup = s_data_backup.Replace(",]", "]");

		WWWForm frm_backup = this.ct.carrot.frm_act("backup");
		frm_backup.AddField("data", s_data_backup);
		frm_backup.AddField("length", length_contact);
		/*
		frm_backup.AddField("user_id", this.ct.carrot.get_id_user_login());
		frm_backup.AddField("user_lang", this.ct.carrot.get_lang_user_login());
		*/
		return frm_backup;
	}

	public void search(string s_key_search)
    {
		this.ct.StopAllCoroutines();
		this.ct.carrot.clear_contain(this.ct.area_body_main);
		Contact_carrot c;
		int count_contact_found=0;

		this.ct.add_tip_info(this.ct.icon_search_return, PlayerPrefs.GetString("search_return") + "(" + s_key_search + ")", PlayerPrefs.GetString("search_return_tip"),-1);

		for (int i = 0; i < this.leng; i++)
		{
			string s_data_contact = PlayerPrefs.GetString("contact_data" + i, "");
			if (s_data_contact != "")
			{
				c= new Contact_carrot("search");
				c.set_data_search(s_data_contact);
				if (c.check_name_or_phone(s_key_search))
				{
					count_contact_found++;
					GameObject book_item = Instantiate(this.ct.prefab_contact_main_item);
					book_item.transform.SetParent(this.area_body_main);
					book_item.transform.localPosition = new Vector3(book_item.transform.localPosition.x, book_item.transform.localPosition.y, 0f);
					book_item.transform.localScale = new Vector3(1f, 1f, 1f);
					book_item.transform.localRotation = Quaternion.Euler(Vector3.zero);
					book_item.GetComponent<Prefab_contact_item_main>().s_user_id = i.ToString();
					book_item.GetComponent<Prefab_contact_item_main>().type = 1;
					book_item.GetComponent<Prefab_contact_item_main>().carrot = this.ct.carrot;
					book_item.GetComponent<Prefab_contact_item_main>().set_data(i, s_data_contact, this.is_call_model);
				}
			}
		}

        if (count_contact_found == 0)
        {
			this.ct.add_none_info(this.area_body_main);
        }
	}

	public GameObject get_contact_by_phone(string s_phone)
    {
		Contact_carrot c;
		for (int i = 0; i < this.leng; i++)
		{
			string s_data_contact = PlayerPrefs.GetString("contact_data" + i, "");
			if (s_data_contact != "")
			{
				c = new Contact_carrot("search");
				c.set_data_search(s_data_contact);
				if (c.check_phone(s_phone))
				{
					GameObject book_item = Instantiate(this.ct.prefab_contact_main_item);
					book_item.transform.SetParent(this.area_body_main);
					book_item.transform.localPosition = new Vector3(book_item.transform.localPosition.x, book_item.transform.localPosition.y, 0f);
					book_item.transform.localScale = new Vector3(1f, 1f, 1f);
					book_item.transform.localRotation = Quaternion.Euler(Vector3.zero);
					book_item.GetComponent<Prefab_contact_item_main>().s_user_id = i.ToString();
					book_item.GetComponent<Prefab_contact_item_main>().type = 1;
					book_item.GetComponent<Prefab_contact_item_main>().carrot = this.ct.carrot;
					book_item.GetComponent<Prefab_contact_item_main>().set_data(i, s_data_contact, this.is_call_model);
					book_item.SetActive(false);
					return book_item;
				}
			}
		}
		return null;
	}

}

public class Contact_backup
{
	public string name;
	public string[] phone;
	public string[] email;
	public string sex;
	private IList list_info;

    public Contact_backup(IList list_info_contact)
    {
		this.list_info = list_info_contact;
		this.name = this.get_val_in_field("name");
		this.sex = this.get_val_in_field("sex");
		this.phone = this.get_val_arr_in_field("sdt");
	}

	private string get_val_in_field(string s_id_name)
	{
		for (int i = 0; i < this.list_info.Count; i++)
		{
			IDictionary data_item = (IDictionary)this.list_info[i];
			if (data_item["id_name"] != null) if (s_id_name == data_item["id_name"].ToString()) return data_item["val"].ToString();
		}
		return "";
	}

	private string[] get_val_arr_in_field(string s_id_name)
	{
		List<string> list_data = new List<string>();
		for (int i = 0; i < this.list_info.Count; i++)
		{
			IDictionary data_item = (IDictionary)this.list_info[i];
			if (data_item["id_name"] != null)
			{
				if (s_id_name == data_item["id_name"].ToString()) list_data.Add(data_item["val"].ToString());
			}
		}
		string[] arr_data = new string[list_data.Count];
		for(int i = 0; i < list_data.Count; i++)
        {
			arr_data[i] = list_data[i];
        }
		return arr_data;
	}

	public string get_json()
    {
		return JsonUtility.ToJson(this);
    }
}

namespace Carrot
{
	public class Contact_carrot
	{
		public string user_id;
		public string user_lang;
		public string avatar;
		public string avatar_full;
		public string type_contact = "";
		private List<string> list_data_field;

        public Contact_carrot(string type_contact)
        {
			this.list_data_field = new List<string>();
			this.user_id = System.Guid.NewGuid().ToString("N");
			this.user_lang = PlayerPrefs.GetString("lang");
			this.type_contact = type_contact;
		}

		private IList list_info;
		public void set_data_search(string s_data)
        {
			IDictionary data_contact = (IDictionary)Json.Deserialize(s_data);
			this.list_info = (IList)data_contact["list_info"];
		}

		public bool check_name_or_phone(string s_key_search)
        {
			bool is_contact_found = false;
			for(int i = 0; i < this.list_info.Count; i++)
            {
				IDictionary data_info = (IDictionary)this.list_info[i];
				if (data_info["id_name"] != null)
				{
					if (data_info["id_name"].ToString() == "name")
					{
						if (data_info["val"].ToString().ToLower().Contains(s_key_search.ToLower()))
						{
							is_contact_found = true;
							break;
						}
					}

					if (data_info["id_name"].ToString() == "sdt")
					{
						if (data_info["val"].ToString().Contains(s_key_search.ToLower()))
						{
							is_contact_found = true;
							break;
						}
					}

					if (data_info["id_name"].ToString() == "phone")
					{
						if (data_info["val"].ToString().Contains(s_key_search.ToLower()))
						{
							is_contact_found = true;
							break;
						}
					}

					if (data_info["id_name"].ToString() == "phone_home")
					{
						if (data_info["val"].ToString().Contains(s_key_search.ToLower()))
						{
							is_contact_found = true;
							break;
						}
					}
				}
            }
			return is_contact_found;
        }

		public bool check_phone(string s_phone)
		{
			bool is_contact_found = false;
			for (int i = 0; i < this.list_info.Count; i++)
			{
				IDictionary data_info = (IDictionary)this.list_info[i];
				if (data_info["id_name"] != null)
				{

					if (data_info["id_name"].ToString() == "sdt")
					{
						if (data_info["val"].ToString().Contains(s_phone.ToLower()))
						{
							is_contact_found = true;
							break;
						}
					}

					if (data_info["id_name"].ToString() == "phone")
					{
						if (data_info["val"].ToString().Contains(s_phone.ToLower()))
						{
							is_contact_found = true;
							break;
						}
					}

					if (data_info["id_name"].ToString() == "phone_home")
					{
						if (data_info["val"].ToString().Contains(s_phone.ToLower()))
						{
							is_contact_found = true;
							break;
						}
					}
				}
			}
			return is_contact_found;
		}

		public void add_field(string type,string id_name,string title,string val,string val_sel,string val_sel_en)
        {
			string s_field;
			if (val_sel_en!="")
				s_field= "{\"id_name\":\""+ id_name + "\",\"title\":\""+title+"\",\"title_en\":\""+ title + "\",\"val\":\"" + val + "\",\"type_update\":\""+ type + "\",\"val_update\":" + val_sel + ",\"val_update_en\":" + val_sel_en + "}";
			else
				s_field = "{\"id_name\":\"" + id_name + "\",\"title\":\"" + title + "\",\"title_en\":\"" + title + "\",\"val\":\"" + val + "\",\"type_update\":\"" + type + "\"}";
			list_data_field.Add(s_field);
		}

		public string get_json()
        {
			string s_data_field = "";
			for(int i = 0; i < this.list_data_field.Count; i++)
            {
				s_data_field = s_data_field + this.list_data_field[i] + ",";
			}
			s_data_field="{\"list_info\":[" + s_data_field + "],\"user_id\":\""+this.user_id+ "\",\"user_lang\":\"" + this.user_lang + "\",\"type\":\"" + this.type_contact + "\"}";
			s_data_field = s_data_field.Replace(",]", "]");
			return s_data_field;
		}
	}
}