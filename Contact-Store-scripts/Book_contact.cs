using System;
using UnityEngine;

public class Book_contact : MonoBehaviour
{
	[Header("Obj Main")]
	public App_Contacts app;
	
	[Header("Contacts Book Obj")]
	private int leng = 0;
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
		this.leng = PlayerPrefs.GetInt("contact_leng", 0);
	}

	public void show()
    {

    }

	public GameObject get_contact_by_phone(string s_dial_txt)
    {
		return null;
    }
}