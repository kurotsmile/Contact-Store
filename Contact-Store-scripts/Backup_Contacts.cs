using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backup_Contacts : MonoBehaviour
{
    [Header("Obj Main")]
    public App_Contacts app;

    [Header("Obj Backup")]
    public Sprite icon_backup;

    public void show()
    {
        this.app.play_sound(0);
        if (this.app.carrot.user.get_id_user_login() == "")
        {
            this.app.carrot.show_msg(PlayerPrefs.GetString("backup", "Backup"), PlayerPrefs.GetString("backup_no_login", "You need to log in to your account to backup your contacts"), Carrot.Msg_Icon.Alert);
            this.app.carrot.delay_function(2f, this.app.carrot.show_login);
        }
        else
        {
            
        }
    }

    public void list()
    {

    }

    public void backup()
    {
        this.app.carrot.db.Collection("user-" + this.app.carrot.user.get_lang_user_login()).Document(this.app.carrot.user.get_id_user_login()).GetSnapshotAsync();
    }
}
