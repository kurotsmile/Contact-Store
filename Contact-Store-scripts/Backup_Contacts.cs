using UnityEngine;

public class Backup_Contacts : MonoBehaviour
{
    [Header("Obj Main")]
    public App_Contacts app;

    [Header("Obj Backup")]
    public Sprite icon_backup;

    public void show()
    {
        this.app.carrot.clear_contain(this.app.area_body_main);

        this.app.play_sound(0);
        if (this.app.carrot.user.get_id_user_login() == "")
        {
            this.app.carrot.show_msg(PlayerPrefs.GetString("backup", "Backup"), PlayerPrefs.GetString("backup_no_login", "You need to log in to your account to backup your contacts"), Carrot.Msg_Icon.Alert);
            this.app.carrot.delay_function(2f, this.app.carrot.show_login);
        }
        else
        {
            Carrot.Carrot_Box_Item item_backup=this.app.add_item_title_list("Backup");
            item_backup.set_icon(this.icon_backup);
            item_backup.set_tip("List of backed up items");
        }

        Carrot.Carrot_Box_Item item_create = this.app.add_item_title_list("Create a new backup");
        item_create.set_icon(this.app.carrot.icon_carrot_add);
        item_create.set_tip("Start backing up your contacts in your app");
    }

    public void list()
    {

    }

    public void backup()
    {
        this.app.carrot.db.Collection("user-" + this.app.carrot.user.get_lang_user_login()).Document(this.app.carrot.user.get_id_user_login()).GetSnapshotAsync();
    }
}
