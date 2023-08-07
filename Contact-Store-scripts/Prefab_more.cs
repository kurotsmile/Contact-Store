using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Prefab_more : MonoBehaviour {
	public Text txt_title;
	public Text txt_tip;
	public void click(){
		GameObject.Find ("App_Contacts").GetComponent<App_Contacts> ().show_list_contact_home ();
	}
	
	public void search_option()
    {
		GameObject.Find("App_Contacts").GetComponent<App_Contacts>().btn_show_search();
	}
}
