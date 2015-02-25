// <copyright file="BoneNode.cs" company="Wadii Bellamine">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Wadii Bellamine, Wahid Bouakline</author>
// <date>2/25/2015 8:37 AM </date>

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Network;
using Network.Packets;

public class LoginUI : MonoBehaviour {

	EventSystem system;
	
	private string username;
	
	private string password;
	
	public GameObject MessageBox;
	
	//problably have to create a UIManager
	public Queue<Action> funcQueue;
	
	// Use this for initialization
	void Start () {
		system = EventSystem.current;
		
		funcQueue = new Queue<Action>();
		
		UserManager.Instance.registerCallback(UserOperation.LOGIN, (object obj) =>
		{
			funcQueue.Enqueue(() => {
				LoginReply packet = (LoginReply)obj;
				
				GameObject msgbox = Instantiate<GameObject>(MessageBox);
				
				LoginStatus loginStatus = (LoginStatus)packet.response;
				
				string loginmessage = string.Empty;
				
				switch(loginStatus)
				{
					case LoginStatus.VALID:
						loginmessage = "Login has been succesful";
						break;
					case LoginStatus.NOT_EXIST:
						loginmessage = "This user doesn't exist";
						break;
					case LoginStatus.ERROR:
						loginmessage = "There has been an error";
						break;
					default:
					break;
				}
				
				UI.MessageBox.Show(msgbox, loginmessage, "LoginReply", ()=> {
					DestroyObject(msgbox);
				});
			});
		});
	}
	
	// Update is called once per frame
	void Update () 
	{
		Utils.handleTab( system );
		
		while(funcQueue.Count > 0) 
		{
			funcQueue.Dequeue().Invoke();
			
			
		}
	}
	
	public void saveUsername() {
		InputField input = system.currentSelectedGameObject.GetComponent<InputField>();
		username = input.text;
	}
	
	public void savePassword() {
		InputField input = system.currentSelectedGameObject.GetComponent<InputField>();
		password = input.text;
	}
	
	public void loginBtn(){
		NetworkManager.Send(new LoginPacket(username, password));
	}
	
	public void registerBtn(){
		Application.LoadLevel("registrationScene");
	}
}
