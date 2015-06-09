// <copyright file="RegisterUI.cs" company="Wadii Bellamine">
// Copyright (c) 2015 All Rights Reserved
// </copyright>
// <author>Wadii Bellamine, Wahid Bouakline</author>
// <date>2/25/2015 8:37 AM </date>

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Network;
using Network.Packets;

public class RegisterUI : MonoBehaviour {

	EventSystem system;
	
	GameObject messageBox;
	
	public Button registerButton;
	public Color errorColor;
	public int minPwLength;
	public char[] invalidUserNameChars;
	public char[] invalidPasswordChars;
	public char[] invalidEmailChars;
	
	private string username;
	private string email;
	private string password;
	
	private bool userNameValid;
	private bool passwordValid;
	private bool passwordMatchValid;
	private bool emailValid;
	
	// Use this for initialization
	void Start () {
		system = EventSystem.current;
		registerButton.interactable = false;
		userNameValid = false;
		passwordValid = false;
		passwordMatchValid = false;
		emailValid = false;
		
		UserManager.Instance.registerCallback(UserOperation.REGISTER, (object obj) =>
		{
			RegisterReply packet = (RegisterReply)obj;
			
			RegisterResponse registrationResponse = (RegisterResponse)packet.response;
			
			switch(registrationResponse)
			{
				case RegisterResponse.SUCCESS:
				case RegisterResponse.ERROR:
				case RegisterResponse.EMAIL_INVALID:
				default:
					break;
			}
			
			Debug.Log ("Nigga you have registered with this return code " + packet.response);
		});
	}
	
	// Update is called once per frame
	void Update () {
		Utils.handleTab( system );
		
		if ( userNameValid &&  passwordValid && passwordMatchValid && emailValid )
			registerButton.interactable = true;
		else
			registerButton.interactable = false;
	}
	
	public void backBtn() {
		Application.LoadLevel("loginScene");
	}
	
	public void registerBtn(){
		NetworkManager.Send(new RegisterPacket(username, email, password));
	}
	
	public void validateField(ref bool fieldValid, bool valid, InputField input) 
	{
		ColorBlock colors = input.colors;
		
		if( valid ) {
			colors.highlightedColor = colors.normalColor;
		}
		else {
			colors.highlightedColor = errorColor;
		}
		
		fieldValid = valid;
		
		input.colors = colors;
	}
	
	public void validateUsername()
	{
		InputField input = system.currentSelectedGameObject.GetComponent<InputField>();
		username = input.text;
		validateField(ref userNameValid, username.IndexOfAny( invalidUserNameChars ) == -1, input);
	}
	
	public void validatePassword(){
		InputField input = system.currentSelectedGameObject.GetComponent<InputField>();
		password = input.text;
		validateField(ref passwordValid, password.IndexOfAny( invalidPasswordChars ) == -1, input);
	}
	
	public void validatePasswordFinish(){
		InputField input = system.currentSelectedGameObject.GetComponent<InputField>();
		password = input.text;
		validateField(ref passwordValid, password.Length >= minPwLength, input);
	}
	
	public void validatePasswordMatch(){
		InputField input = system.currentSelectedGameObject.GetComponent<InputField>();
		string text = input.text;
		validateField(ref passwordMatchValid, text == password, input);
	}
	
	public void validateEmail(){
		InputField input = system.currentSelectedGameObject.GetComponent<InputField>();
		email = input.text;
		validateField(ref emailValid, Utils.isEmail(email), input);
	}
}
