using UnityEngine;
using WebSocketSharp;

public class WebSocketClient : MonoBehaviour 
{
	private WebSocket ws;

	void OnGUI() {
		if (GUILayout.Button("Connect")) {
			this.ws = new WebSocket("ws://127.0.0.1:8080");
			this.ws.OnMessage += (object sender, MessageEventArgs e) => {
				print (e.Data);
			};
			this.ws.Connect ();
		}

		if (GUILayout.Button("Send")) {
			this.ws.Send (System.DateTime.Now.ToString ());
		}

		if (GUILayout.Button("Close")) {
			this.ws.Close ();
		}
	}
}