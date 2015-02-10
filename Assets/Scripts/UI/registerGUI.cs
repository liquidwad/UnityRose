using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Network;
using Network.Packets;

public class registerGUI : MonoBehaviour {

	public Button registerBtn;
	
	public InputField usernameInput;
	
	public InputField passwordInput;
	
	public InputField emailInput;
	
	public const string MatchEmailPattern =
		@"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
			+ @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
			+ @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
			+ @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
			
	public static bool IsEmail(string email)
	{
		if (email != null) 
			return Regex.IsMatch(email, MatchEmailPattern);
		else 
			return false;
	}
	
	private bool validateRegister(string username, string email, string password) 
	{
		return (username != string.Empty && username.Length >= 5) &&
				(email != string.Empty && IsEmail(email)) &&
				(password != string.Empty && password.Length >= 6);
	}
	
	// Use this for initialization
	void Start () {
	
		registerBtn.onClick.AddListener(delegate {
			string username = usernameInput.text;
			string email = emailInput.text;
			string password = passwordInput.text;
			
			if(validateRegister(username, email, password)) 
			{
				NetworkManager.Send(new RegisterPacket(username, email, password));
			}
		});
	}
}
