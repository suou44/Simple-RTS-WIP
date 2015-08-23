using UnityEngine;
using System.Collections;
using RTS;

public class HUD : MonoBehaviour {

	public GUISkin resourceSkin, ordersSkin, selectBoxSkin;

	public Texture2D activeCursor;
	public Texture2D Spear, leftCursor, rightCursor, upCursor, downCursor;
	public Texture2D[] moveCursors, attackCursors, harvestCursors;
	public GUISkin mouseCursorSkin;

	public Texture2D Castle;

	private CursorState activeCursorState;
	private int currentFrame = 0;

	private Player player;

	private const int ORDERS_BAR_HEIGHT = 248, RESOURCE_BAR_HEIGHT = 28;

	/*** Game Engine Methods ***/

	void Start () {
		player = transform.root.GetComponent<Player>();
		ResourceManager.StoreSelectBoxItems (selectBoxSkin);
		SetCursorState (CursorState.Select);
	}

	void OnGUI () {
		//we only want to draw a GUI for human players
		if(player) {
			DrawOrdersBar();
			DrawResourceBar();
			DrawMouseCursor ();
		}
	}

	/*** Private Worker Methods ***/

	private void DrawOrdersBar() {
		GUI.skin = ordersSkin;
		GUI.BeginGroup(new Rect(0,Screen.height - ORDERS_BAR_HEIGHT,Screen.width,ORDERS_BAR_HEIGHT));
		GUI.Box(new Rect(0,0,Screen.width,ORDERS_BAR_HEIGHT),"");
		string selectionName = "";
		if (player.SelectedObject) {
			selectionName = player.SelectedObject.objectName;
		}
		if (!selectionName.Equals("")) {
			GUI.Label (new Rect (0, 20, 100, 15), selectionName);
		}

		GUI.EndGroup();
	}

	private void DrawResourceBar() {
		GUI.skin = resourceSkin;
		GUI.BeginGroup(new Rect(0,0,Screen.width,RESOURCE_BAR_HEIGHT));
		GUI.Box(new Rect(0,0,Screen.width,RESOURCE_BAR_HEIGHT),"");
		GUI.EndGroup();
	}

	public bool MouseInBounds() {
		//Screen coordinates start in the lower-left corner of the screen
		//not the top-left of the screen like the drawing coordinates do
		Vector3 mousePos = Input.mousePosition;
		bool insideWidth = mousePos.x >= 0 && mousePos.x <= Screen.width;
		bool insideHeight = mousePos.y >= ORDERS_BAR_HEIGHT && mousePos.y <= Screen.height - RESOURCE_BAR_HEIGHT;
		return insideWidth && insideHeight;
	}

	public Rect GetPlayingArea(){
		return new Rect (0, RESOURCE_BAR_HEIGHT, Screen.width, Screen.height - RESOURCE_BAR_HEIGHT);
	}

	private void DrawMouseCursor(){

		bool mouseOverHud = !MouseInBounds () && activeCursorState != CursorState.PanRight && activeCursorState != CursorState.PanUp;
		if (mouseOverHud) {
			Cursor.visible = true;
		} else {
			Cursor.visible = false;
			GUI.skin = mouseCursorSkin;
			GUI.BeginGroup (new Rect (0, 0, Screen.width, Screen.height));
			UpdateCursorAnimation ();
			Rect cursorPosition = GetCursorDrawPosition ();
			GUI.Label (cursorPosition, activeCursor);
			GUI.EndGroup ();
		}
	}

	private void UpdateCursorAnimation(){
		//sequence animation for cursor (based on more than one image for the cursor)
		//change once per second, loops through asrray of images
		if(activeCursorState == CursorState.Move){
			currentFrame = (int)Time.time % moveCursors.Length;
			activeCursor = moveCursors[currentFrame];
		}else if(activeCursorState == CursorState.Attack){
			currentFrame = (int)Time.time % attackCursors.Length;
			activeCursor = attackCursors[currentFrame];
		}else if(activeCursorState == CursorState.Harvest){
			currentFrame = (int)Time.time % harvestCursors.Length;
			activeCursor = harvestCursors[currentFrame];
		}
	}

	private Rect GetCursorDrawPosition(){
		//set base position for custom cursor image
		float leftPos = Input.mousePosition.x;
		float topPos = Screen.height - Input.mousePosition.y; //screen draw coords are inverted
		//adjust position base on the type of cursor being shown
		if (activeCursorState == CursorState.PanRight)
			leftPos = Screen.width - activeCursor.width;
		else if (activeCursorState == CursorState.PanDown)
			topPos = Screen.height - activeCursor.height;
		else if (activeCursorState == CursorState.Move || activeCursorState == CursorState.Select || activeCursorState == CursorState.Harvest) {
			topPos -= activeCursor.height / 2;
			leftPos -= activeCursor.width / 2;
		}
		return new Rect (leftPos, topPos, activeCursor.width, activeCursor.height);
	}

	public void SetCursorState(CursorState newState){
		activeCursorState = newState;
		switch (newState) {
		case CursorState.Select:
			activeCursor = Spear;
			break;
		case CursorState.Attack:
			currentFrame = (int)Time.time % attackCursors.Length;
			activeCursor = attackCursors [currentFrame];
			break;
		case CursorState.Harvest:
			currentFrame = (int)Time.time % harvestCursors.Length;
			activeCursor = harvestCursors [currentFrame];
			break;
		case CursorState.Move:
			currentFrame = (int)Time.time % moveCursors.Length;
			activeCursor = moveCursors [currentFrame];
			break;
		case CursorState.PanLeft:
			activeCursor = leftCursor;
			break;
		case CursorState.PanRight:
			activeCursor = rightCursor;
			break;
		case CursorState.PanUp:
			activeCursor = upCursor;
			break;
		case CursorState.PanDown:
			activeCursor = downCursor;
			break;
		default:
			break;
		}
	}
}
