using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UI;
using Network;
using Network.Packets;

public class LoginGUI : MonoBehaviour {
	
	public Button loginBtn;
	
	public Button registerBtn;
	
	public Button backBtn;
	
	public InputField usernameInput;
	
	public InputField passwordInput;
	
	public Canvas registerCanvas;
	
	private bool validateLogin(string username, string password) {
		return (username != string.Empty && username.Length >= 5) && 
				(password != string.Empty && password.Length >= 6);
	}
	
	public void Start() {
	
		loginBtn.onClick.AddListener(delegate {
			Debug.Log("Login button clicked");
			
			string username = usernameInput.text;
			string password = passwordInput.text;
			
			if(this.validateLogin(username, password)) {
				NetworkManager.Send(new LoginPacket(username, password));
			}
		});	
		
		registerBtn.onClick.AddListener(delegate {	
			Debug.Log("Register");
			this.GetComponent<Canvas>().enabled = false;
			registerCanvas.enabled = true;
		});	
		
		backBtn.onClick.AddListener(delegate {
			Debug.Log("Back");
			this.GetComponent<Canvas>().enabled = true;
			registerCanvas.enabled = false;
		});
		
		// Add packet received delegates
		
		UserManager.Instance.registerCallback((int)UserOperation.LOGIN, (object loginreply) => 
		{
			LoginReply packet = (LoginReply)loginreply;
			//lOGIN
			LoginStatus response = (LoginStatus) packet.response;
			
			/*MessageBox msgBox = gameObject.GetComponent<Camera>().GetComponent<MessageBox>();
			if( response == LoginStatus.VALID ) {
				MessageBox.Show("Login valid: Going to char select...", "Login Valid", delegate{});
			}
			else {	
				MessageBox.Show("Login failed: try again", "Login failed", delegate{});
			}*/
		});
	}
}
