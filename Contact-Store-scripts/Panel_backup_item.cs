using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_backup_item : MonoBehaviour
{
    public Text txt_name;
    public string id;
    public string lang;
    public void click()
    {
      GameObject.Find("App_Contacts").GetComponent<App_Contacts>().panel_backup.GetComponent<Panel_backup>().view_backup(id,lang);
    }

    public void delete()
    {
        GameObject.Find("App_Contacts").GetComponent<App_Contacts>().delete_backup(this.id,this.lang);
    }

    public void export()
    {
        GameObject.Find("App_Contacts").GetComponent<App_Contacts>().panel_backup.GetComponent<Panel_backup>().export_backup(id, lang);
    }
}
