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

    public void list()
    {
        //this.app.carrot.clear_contain(this.app.area_body_main);
        this.app.add_item_loading(this.app.area_body_main);

        Query ContactQuery = this.app.carrot.db.Collection("user-" + this.app.carrot.lang.get_key_lang()).WhereEqualTo("status_share", "0");
        ContactQuery = ContactQuery.WhereNotEqualTo("phone", "");
        ContactQuery.Limit(60).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot QDocs = task.Result;
            if (task.IsCompleted)
            {
                if (QDocs.Count > 0)
                {
                    this.app.carrot.clear_contain(this.app.area_body_main);

                    foreach (DocumentSnapshot doc in QDocs.Documents)
                    {
                        IDictionary data_contact = doc.ToDictionary();
                        data_contact["id"] = doc.Id;

                        var id_contact = ""; if(data_contact["id"]!=null) id_contact=data_contact["id"].ToString();
                        var lang_contact = "en"; if (data_contact["lang"] != null) lang_contact = data_contact["lang"].ToString();

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
                        if (data_contact["phone"] != null) item_contact.set_tip(data_contact["phone"].ToString());

                        if (data_contact["phone"] != null)
                        {
                            Carrot.Carrot_Box_Btn_Item btn_call=item_contact.create_item();
                            btn_call.set_icon(this.icon_call);
                            btn_call.set_color(this.app.carrot.color_highlight);
                        }

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
}
