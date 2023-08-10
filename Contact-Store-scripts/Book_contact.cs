using System;
using System.Collections;
using UnityEngine;

public class Book_contact : MonoBehaviour
{
	[Header("Obj Main")]
	public App_Contacts app;
	
	[Header("Contacts Book Obj")]
	private int length = 0;
	public Texture2D avatar_default;

	[Header("Icon contacts")]
	public Sprite sp_delete;
	public Sprite sp_call;
	public Sprite sp_back;
	public Sprite sp_icon_import;
	public Sprite sp_icon_add_phone;
	public Sprite sp_icon_update_phone;

	private int index_del = -1;
	private Carrot.Carrot_Window_Msg msg;

	public void load_book_contact()
	{
		this.length = PlayerPrefs.GetInt("contact_length", 0);
	}

	public void show()
    {
		this.app.carrot.clear_contain(this.app.area_body_main);
		this.app.add_item_loading();
		this.app.carrot.delay_function(0.5f, this.list);
	}

	private void list()
    {
		this.app.carrot.clear_contain(this.app.area_body_main);
		if (this.length > 0)
		{
			Carrot.Carrot_Box_Item item_title = this.app.add_item_title_list("Book_Contacts");
			item_title.set_icon_white(this.sp_icon_import);

			IList list_contact_book = (IList)Carrot.Json.Deserialize("[]");
			for (int i = this.length - 1; i >= 0; i--)
			{
				string s_data = PlayerPrefs.GetString("contact_" + i, "");
				if (s_data != "")
				{
					IDictionary data_contact = (IDictionary)Carrot.Json.Deserialize(s_data);
					data_contact["type_item"] = "phonebook";
					data_contact["index"] = i;
					list_contact_book.Add(data_contact);
				}

			}
			this.app.manager_contact.show_list_data_contacts(list_contact_book);
		}
		else
		{
			this.app.add_item_none();
		}
	}

	public void add(IDictionary data)
    {
		this.app.play_sound(0);
		PlayerPrefs.SetString("contact_" + this.length, Carrot.Json.Serialize(data));
		this.length++;
		PlayerPrefs.SetInt("contact_length", this.length);
		this.app.carrot.show_msg("Contact Store", "Contact Archive Successful!",Carrot.Msg_Icon.Success);
    }

	public void delete(int index)
    {
		this.index_del = index;
		if (this.msg != null) this.msg.close();
		this.msg = this.app.carrot.show_msg("Delete", "Are you sure you want to remove this item?", act_yes_delete, act_no_delete);
    }

	private void act_yes_delete()
    {
		if (this.msg != null) this.msg.close();
		PlayerPrefs.DeleteKey("contact_" + this.index_del);
		this.list();
		this.app.play_sound(0);
	}

	private void act_no_delete()
    {
		if (this.msg != null) this.msg.close();
		this.app.play_sound(0);
	}

	public GameObject get_contact_by_phone(string s_dial_txt)
    {
		return null;
    }

	public IList get_list_data_backup()
    {
		IList list_data = (IList)Carrot.Json.Deserialize("[]");
		if (this.length > 0)
		{
			for (int i = this.length - 1; i >= 0; i--)
			{
				string s_data = PlayerPrefs.GetString("contact_" + i, "");
				if (s_data != "")
				{
					IDictionary data_contact = (IDictionary)Carrot.Json.Deserialize(s_data);
					data_contact["type_item"] = "phonebook";
					data_contact["index"] = i;
					list_data.Add(data_contact);
				}

			}
		}
		return list_data;
	}
}