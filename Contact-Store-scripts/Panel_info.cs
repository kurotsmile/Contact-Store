using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_info : MonoBehaviour {

	public Text txt_title;
	public Text txt_tip;
	public Button btn_action;
	public Image icon;

	public int type;

	public void click(){
		if(this.type==0){
			GameObject.Find ("App_Contacts").GetComponent<Book_contact> ().change_model_list_contacts();
		}

		if(this.type==1){
			GameObject.Find ("App_Contacts").GetComponent<Book_contact> ().change_model_list_contacts();
		}

		if(this.type==3){
			GameObject.Find ("App_Contacts").GetComponent<Book_contact> ().import_my_contact();
		}
	}

	public void act(){
		if(this.type==0){
			GameObject.Find("App_Contacts").GetComponent<Book_contact>().change_model_list_contacts();
		}

		if(this.type==1){
			GameObject.Find ("App_Contacts").GetComponent<Book_contact> ().change_model_list_contacts();
		}

	}
}
