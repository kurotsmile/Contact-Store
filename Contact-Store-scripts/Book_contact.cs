﻿using System;
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

	public void load_book_contact()
	{
		this.length = PlayerPrefs.GetInt("contact_length", 0);
	}

	public void show()
    {
		this.app.carrot.clear_contain(this.app.area_body_main);
		if (this.length > 0)
        {
			this.app.add_item_title_list("Book_Contacts");

			IList list_contact_book = (IList)Carrot.Json.Deserialize("[]");
			for (int i = this.length - 1; i >= 0; i--)
			{
				string s_data = PlayerPrefs.GetString("contact_" + i, "");
				if (s_data != "")
				{
					IDictionary data_contact = (IDictionary)Carrot.Json.Deserialize(s_data);
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
		PlayerPrefs.SetString("contact_" + this.length, Carrot.Json.Serialize(data));
		this.length++;
		PlayerPrefs.SetInt("contact_length", this.length);
		this.app.carrot.show_msg("Contact Store", "Contact Archive Successful!",Carrot.Msg_Icon.Success);
    }

	public GameObject get_contact_by_phone(string s_dial_txt)
    {
		return null;
    }
}