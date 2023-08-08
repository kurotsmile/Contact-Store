using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Contact : MonoBehaviour
{
    [Header("Obj main")]
    public App_Contacts app;

    [Header("Icon")]
    public Sprite icon_contact;
    public Sprite icon_call;
    public Sprite icon_save;
    public Sprite icon_sort_name;

    private string s_type_order = "name";

    public void list()
    {
        this.app.carrot.clear_contain(this.app.area_body_main);
        this.app.add_item_loading(this.app.area_body_main);

        Query ContactQuery = this.app.carrot.db.Collection("user-" + this.app.carrot.lang.get_key_lang());
        ContactQuery = ContactQuery.WhereEqualTo("status_share", "0");
        ContactQuery = ContactQuery.WhereNotEqualTo(this.s_type_order, "");
        ContactQuery = ContactQuery.OrderByDescending(this.s_type_order);
        ContactQuery.Limit(60).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot QDocs = task.Result;
            if (task.IsCompleted)
            {
                if (QDocs.Count > 0)
                {
                    this.app.carrot.clear_contain(this.app.area_body_main);

                    Carrot.Carrot_Box_Item item_title=this.app.add_item_title_list("List Contact country(" + this.app.carrot.lang.get_key_lang() + ")");
                    item_title.set_tip("Number of items listed:" + QDocs.Count);
                    Carrot.Carrot_Box_Btn_Item btn_sort_name=item_title.create_item();
                    btn_sort_name.set_icon(this.icon_sort_name);
                    btn_sort_name.set_icon_color(Color.white);
                    btn_sort_name.set_color(this.app.carrot.color_highlight);
                    btn_sort_name.set_act(() => this.sort());

                    foreach (DocumentSnapshot doc in QDocs.Documents)
                    {
                        IDictionary data_contact = doc.ToDictionary();
                        data_contact["id"] = doc.Id;

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
                            btn_call.set_icon(this.icon_call);
                            btn_call.set_color(this.app.carrot.color_highlight);
                            btn_call.set_act(() => this.call(s_phone));
                            s_tip= data_contact["phone"].ToString();
                        }
                        else
                        {
                            if(data_contact["email"]!=null) s_tip = data_contact["email"].ToString();
                        }

                        if (data_contact["email"] != null)
                        {
                            var s_email = data_contact["email"].ToString();
                            Carrot.Carrot_Box_Btn_Item btn_email = item_contact.create_item();
                            btn_email.set_icon(this.app.carrot.icon_carrot_mail);
                            btn_email.set_color(this.app.carrot.color_highlight);
                            btn_email.set_act(() => this.mail(s_email));
                        }

                        item_contact.set_tip(s_tip);

                        Carrot.Carrot_Box_Btn_Item btn_save = item_contact.create_item();
                        btn_save.set_icon(this.icon_save);
                        btn_save.set_color(this.app.carrot.color_highlight);

                        item_contact.set_act(()=>this.app.carrot.user.show_user_by_id(id_contact, lang_contact));
                    }
                }
                else
                {
                    this.app.carrot.show_msg("Contact Store", "No list Contacts", Carrot.Msg_Icon.Alert);
                }
            }
        });
    }

    private void call(string s_phone)
    {
        Application.OpenURL("tel:"+s_phone);
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
}
