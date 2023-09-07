using Carrot;
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

	[Header("Contact Book Field")]
	private Carrot.Carrot_Box_Item item_field_name;
    private Carrot.Carrot_Box_Item item_field_phone;
    private Carrot.Carrot_Box_Item item_field_email;
    private Carrot.Carrot_Box_Item item_field_sex;

    private int index_del = -1;
	private Carrot.Carrot_Window_Msg msg;
	private Carrot.Carrot_Box box;

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
			Carrot.Carrot_Box_Item item_title = this.app.add_item_title_list(PlayerPrefs.GetString("phonebook", "Phonebook"));
			item_title.set_icon_white(this.sp_icon_import);
			item_title.set_tip(PlayerPrefs.GetString("phonebook_list", "List of saved contacts"));

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
		this.create(data);
		this.app.play_sound(0);
		this.app.carrot.show_msg("Contact Store", "Contact Archive Successful!", Carrot.Msg_Icon.Success);
	}

	public void create(IDictionary data)
	{
		PlayerPrefs.SetString("contact_" + this.length, Carrot.Json.Serialize(data));
		this.length++;
		PlayerPrefs.SetInt("contact_length", this.length);
	}

	public void delete(int index)
	{
		this.index_del = index;
		if (this.msg != null) this.msg.close();
		this.msg = this.app.carrot.show_msg("Delete", "Are you sure you want to remove this item?", act_yes_delete, act_no_delete);
	}

	private void act_yes_delete()
	{
		if (this.app.manager_contact.get_box_info() != null) this.app.manager_contact.get_box_info().close();
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

	public void delete_all()
	{
		for (int i = 0; i < this.length; i++) PlayerPrefs.DeleteKey("contact_" + i);
		PlayerPrefs.DeleteKey("contact_length");
		this.length = 0;
	}

	public void Create_New_BookContact(){
		this.box=this.app.carrot.Create_Box();
		box.set_icon(this.sp_icon_add_phone);
		box.set_title("Add New Contact Book");

        this.item_field_name=box.create_item("item_name");
        this.item_field_name.set_icon(this.app.carrot.user.icon_user_info);
        this.item_field_name.set_title("Full name");
        this.item_field_name.set_tip("Enter the contact's full name");
        this.item_field_name.set_type(Carrot.Box_Item_Type.box_value_input);
        this.item_field_name.check_type();

		this.item_field_phone = box.create_item("item_phone");
        this.item_field_phone.set_icon(this.app.carrot.user.icon_user_info);
        this.item_field_phone.set_title("Phone");
        this.item_field_phone.set_tip("Enter Phone Number");
        this.item_field_phone.set_type(Carrot.Box_Item_Type.box_value_input);
        this.item_field_phone.check_type();

        this.item_field_email = box.create_item("item_email");
        this.item_field_email.set_icon(this.app.carrot.icon_carrot_mail);
        this.item_field_email.set_title("Email");
        this.item_field_email.set_tip("Enter Email Addreess");
        this.item_field_email.set_type(Carrot.Box_Item_Type.box_value_input);
        this.item_field_email.check_type();

        this.item_field_sex = box.create_item("item_sex");
        this.item_field_sex.set_icon(this.app.carrot.icon_carrot_mail);
        this.item_field_sex.set_title("Sex");
        this.item_field_sex.set_tip("Select Your Sex");
        this.item_field_sex.set_type(Carrot.Box_Item_Type.box_value_input);
        this.item_field_sex.check_type();

        Carrot.Carrot_Box_Btn_Panel panel=box.create_panel_btn();
		Carrot.Carrot_Button_Item btn_done=panel.create_btn("btn_done");
		btn_done.set_label("Done");
		btn_done.set_icon_white(this.app.carrot.icon_carrot_done);
		btn_done.set_bk_color(this.app.carrot.color_highlight);
		btn_done.set_act_click(() => this.create_contact_done());

		Carrot.Carrot_Button_Item btn_cancel = panel.create_btn("btn_cancel");
        btn_cancel.set_label("Cancel");
        btn_cancel.set_icon_white(this.app.carrot.icon_carrot_cancel);
		btn_cancel.set_bk_color(this.app.carrot.color_highlight);
		btn_cancel.set_act_click(()=>box.close());
	}

	private void create_contact_done()
    {
		IDictionary contact_data = (IDictionary) Json.Deserialize("{}");
        contact_data["name"]= this.item_field_name.get_val();
        contact_data["phone"] = this.item_field_phone.get_val();
        contact_data["email"] = this.item_field_email.get_val();
        contact_data["sex"] = this.item_field_sex.get_val();
        contact_data["type"] = "phonebook";
        contact_data["index"] = this.length;

        this.create(contact_data);
		this.box.close();

        this.app.carrot.show_msg("Contact Store", "Contact Archive Successful!", Carrot.Msg_Icon.Success);
    }

}