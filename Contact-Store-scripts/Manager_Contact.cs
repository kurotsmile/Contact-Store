using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using UnityEngine;

public class Manager_Contact : MonoBehaviour
{
    [Header("Obj main")]
    public App_Contacts app;

    [Header("Icon")]
    public Sprite icon_contact;
    public Sprite icon_save;
    public Sprite icon_sort_name;

    private string s_type_order = "name";
    private IList list_contacts;
    private bool is_read_cache = false;

    public void list()
    {
        this.app.carrot.clear_contain(this.app.area_body_main);
        this.app.add_item_loading();

        if (this.app.carrot.is_offline()) this.is_read_cache = true;

        if (this.is_read_cache == false)
        {
            this.get_data_from_sever();
        }
        else
        {
            string s_list_data = PlayerPrefs.GetString("contacts_" + this.s_type_order + "_" + this.app.carrot.lang.get_key_lang(), "");
            if (s_list_data == "")
                this.get_data_from_sever();
            else
                this.get_data_from_cache(s_list_data);
        }
    }

    private void get_data_from_sever()
    {
        Query ContactQuery = this.app.carrot.db.Collection("user-" + this.app.carrot.lang.get_key_lang());
        ContactQuery = ContactQuery.WhereEqualTo("status_share", "0");
        ContactQuery = ContactQuery.WhereNotEqualTo(this.s_type_order, "");
        ContactQuery = ContactQuery.OrderByDescending(this.s_type_order);
        ContactQuery.Limit(60).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot QDocs = task.Result;
            if (task.IsCompleted)
            {
                this.app.carrot.clear_contain(this.app.area_body_main);
                if (QDocs.Count > 0)
                {
                    this.list_contacts = (IList)Carrot.Json.Deserialize("[]");

                    foreach (DocumentSnapshot doc in QDocs.Documents)
                    {
                        IDictionary data_contact = doc.ToDictionary();
                        data_contact["id"] = doc.Id;
                        data_contact["type_item"] = "user";
                        this.list_contacts.Add(data_contact);
                    }

                    PlayerPrefs.SetString("contacts_" + this.s_type_order + "_" + this.app.carrot.lang.get_key_lang(), Carrot.Json.Serialize(this.list_contacts));
                    this.is_read_cache = true;

                    this.add_item_title();
                    this.show_list_data_contacts(this.list_contacts);
                }
                else
                {
                    this.app.add_item_none();
                }
            }

            if (task.IsFaulted)
            {
                this.app.carrot.clear_contain(this.app.area_body_main);
                Carrot.Carrot_Box_Item item_error = this.app.add_item_title_list("Error");
                item_error.set_icon_white(this.app.carrot.icon_carrot_bug);
                item_error.set_tip("Operation failed, please try again!");
            }
        });
    }

    private void get_data_from_cache(string s_data_json)
    {
        this.app.carrot.clear_contain(this.app.area_body_main);

        this.list_contacts = (IList)Carrot.Json.Deserialize(s_data_json);
        this.add_item_title();
        this.show_list_data_contacts(this.list_contacts);
    }

    private void add_item_title()
    {
        Carrot.Carrot_Box_Item item_title = this.app.add_item_title_list("List Contact country(" + this.app.carrot.lang.get_key_lang() + ")");
        item_title.set_tip("Number of items listed:" + this.list_contacts.Count);
        Carrot.Carrot_Box_Btn_Item btn_sort_name = item_title.create_item();
        btn_sort_name.set_icon(this.icon_sort_name);
        btn_sort_name.set_icon_color(Color.white);
        btn_sort_name.set_color(this.app.carrot.color_highlight);
        btn_sort_name.set_act(() => this.sort());
    }


    public void show_list_data_contacts(IList list_contacts)
    {
        foreach (IDictionary data_contact in list_contacts)
        {
            var id_contact = ""; if (data_contact["id"] != null) id_contact = data_contact["id"].ToString();
            var lang_contact = "en"; if (data_contact["lang"] != null) lang_contact = data_contact["lang"].ToString();
            string s_tip = "";

            GameObject obj_contact_item = Instantiate(this.app.prefab_contact_main_item);
            obj_contact_item.transform.SetParent(this.app.area_body_main);
            obj_contact_item.transform.localPosition = new Vector3(0, 0, 0);
            obj_contact_item.transform.localScale = new Vector3(1f, 1f, 1f);
            obj_contact_item.transform.localRotation = Quaternion.Euler(Vector3.zero);

            Carrot.Carrot_Box_Item item_contact = obj_contact_item.GetComponent<Carrot.Carrot_Box_Item>();
            item_contact.on_load(this.app.carrot);
            item_contact.check_type();
            item_contact.set_icon(this.icon_contact);
            if (data_contact["name"] != null) item_contact.set_title(data_contact["name"].ToString());

            if (data_contact["phone"] != null)
            {
                var s_phone = data_contact["phone"].ToString();
                Carrot.Carrot_Box_Btn_Item btn_call = item_contact.create_item();
                if (data_contact["sex"] != null)
                {
                    if (data_contact["sex"].ToString() == "0")
                        btn_call.set_icon(this.app.icon_call_boy);
                    else
                        btn_call.set_icon(this.app.icon_call_girl);
                }
                else
                {
                    btn_call.set_icon(this.app.icon_call_boy);
                }
                btn_call.set_color(this.app.carrot.color_highlight);
                btn_call.set_act(() => this.call(s_phone));
                s_tip = data_contact["phone"].ToString();
            }


            if (s_tip == "") if (data_contact["email"] != null) s_tip = data_contact["email"].ToString();

            if (data_contact["email"] != null)
            {
                var s_email = data_contact["email"].ToString();
                Carrot.Carrot_Box_Btn_Item btn_email = item_contact.create_item();
                btn_email.set_icon(this.app.carrot.icon_carrot_mail);
                btn_email.set_color(this.app.carrot.color_highlight);
                btn_email.set_act(() => this.mail(s_email));
            }

            item_contact.set_tip(s_tip);

            if (data_contact["type_item"] != null)
            {
                string type_item = data_contact["type_item"].ToString();

                if(type_item== "user")
                {
                    Carrot.Carrot_Box_Btn_Item btn_save = item_contact.create_item();
                    btn_save.set_icon(this.icon_save);
                    btn_save.set_color(this.app.carrot.color_highlight);
                    btn_save.set_act(() => this.app.book_contact.add(data_contact));
                }

                if (type_item == "contact")
                {
                    Carrot.Carrot_Box_Btn_Item btn_del = item_contact.create_item();
                    btn_del.set_icon(this.app.carrot.sp_icon_del_data);
                    btn_del.set_color(Color.red);
                    btn_del.set_act(() => this.app.book_contact.delete(int.Parse(data_contact["index"].ToString())));
                }

            }
            item_contact.set_act(() => this.app.carrot.user.show_user_by_id(id_contact, lang_contact));
        }
    }

    private void call(string s_phone)
    {
        Application.OpenURL("tel://" + s_phone);
    }

    private void mail(string s_mail)
    {
        Application.OpenURL("mailto:" + s_mail);
    }

    private void sort()
    {
        if (this.s_type_order == "name")
            this.s_type_order = "phone";
        else
            this.s_type_order = "name";
        this.list();
    }

    public IList get_list_contacts()
    {
        return this.list_contacts;
    }
}
