using UnityEngine;
using System.Collections;

//<Irreversable Formulae> <禁逆公式>

public class PlayerInteractions : MonoBehaviour {

	bool gameOver;

	// starlight gameobjects
	GameObject[] starGO;

	// screen fading in and fading out settings:
	float fadeSpeed = 1.5f;

	public Material skybox1; // day
	public Material skybox2; // night

	public GameObject title1; // the title appears during the openup animation phase

	// dialogs (bool)
	bool[] dialogBools = new bool[250];
	//bool dialog001;
	//bool dialog002;

	public AudioClip openupBGM;

	// story index:
	// - for each period of the story, there are several indices to serve as boolean values to turn
	// - specific reactions on and off.
	bool[] storyIndex = new bool[99];
	// index 0: 1.openup animation 
	// index 1: noodle talks with Zhong
	// index 2: purple gold task with mayor
	// openup animation waypoints:
	public GameObject waypoint1; // carbon crystal 1
	public GameObject waypoint2; // copper crystal
	public GameObject waypoint3; // hydra crystal
	public GameObject waypoint4; // iron crystal
	public GameObject waypoint5; // lead crystal
	public GameObject waypoint6; // mercury crystal
	int waypntState; // indicates which waypoint is being executed
	Vector3 nextWaypnt1; // the direction from player to waypoint1
	Vector3 nextWaypnt2; // the direction from waypoint1 to waypoint2
	Vector3 nextWaypnt3; // the direction from waypoint2 to waypoint3
	Vector3 nextWaypnt4; // the direction from waypoint3 to waypoint4
	Vector3 nextWaypnt5; // the direction from waypoint4 to waypoint5
	Vector3 nextWaypnt6; // the direction from waypoint5 to waypoint6
	// the initial minimal distance between a Vector3 and another Vector3 for calibrating waypoints
	float minDist1; // player and waypoint1
	float minDist2; // waypoint1 and 2
	float minDist3; // waypoint2 and 3
	float minDist4; // waypoint3 and 4
	float minDist5; // waypoint4 and 5
	float minDist6; // waypoint 5 and 6

	// Locomotion parameter settings
	float walkSpeed = 5.0f;
	float rotationSpeed = 100.0f;
	Vector3 moveDir;

	Animator animator;
	public GameObject camera; // get the camera for player
	Vector3 oriCamPos; // the original position of camera
	Quaternion oriCamRot; // the original rotation of camera
	public Transform battleCameraSphere; // used to denote where the camera should be at battle
	Animator enemyAnimator; // the animator of the enemy's

	// For the enemy we're currently fighting against:
	GameObject fightingEnemy; // the enemy that we are fighting against now
	int enemyHealth; // the current health of the enemy
	// no max health limit needed?
	int enemyAttack; // the attack ability of the enemy, while the really harm an enemy can draw is calculated
	// upon enemyAttack, playerDefense
	int enemyDefense;
	int harmToPlayer; // the real harm the enemy draws to the player
	int harmToEnemy; // the real harm the player draws to the enemy

	public Texture2D mtrlImage;

	bool drAIsClicked; // if the "Dr.A" button is clicked
	bool mtrlIsClicked; // if the "Materials" button is clicked
	bool stsIsClicked; // if the "status" button is clicked
	bool fmlIsClicked; // if the "Formulae" button is clicked

	bool onBattle; // if the GUI of the battle should be shown
	bool showBattleEnd; // shows how much exp has the player gained blablabla
	bool closeBattleEnd;
	bool hasFlipedCoin;
	bool sbePlusOnce; // show battle end (gold chloride + rochelle salt) plus once

	bool zcnPlusOnce; // zhong's cool noodle plus once

	bool showFoundItem; // show what item has the player just found
	string foundItemStr; // the string name of the found item

	// Status:
	int attack = 5;
	int defense = 5;
	int health = 20;
	int maxHealth = 20;
	int level = 1;
	int exp = 0;
	// the amount of exp needed for achieving each level:
	int[] expNeeded = new int[100]; //(0-99)
	//int currExpIndex = 1;
	string weapon = "Knife";
	int expPlus = 0; // the exp gained after each battle

	const int BOX_X = 50; // the left upper x of "Dr.A's pocket lab"
	const int BOX_Y = 70; // the left upper y of "Dr.A"'s pocket lab'
	const int GRID_WIDTH = 65; // the width of a grid inside "Dr.a's pocket lab"
	const int GRID_HEIGHT = 30; // the height of a grid inside "Dr.a's pocket lab"
	int numOfHydra = 0; // the number of HydraCrystals
	int numOfCarbon = 0; // the number of CarbonCrystals
	int numOfLead = 0; // the number of LeadCrystals (Pb)
	int numOfTin = 0; // the number of TinCrystals (Sn)
	int numOfIron = 0; // the number of IronCrystals (Fe)
	int numOfCopper = 0; // the number of CopperCrystals (Cu)
	int numOfMercury = 0; // the number of MercuryCrystals (Hg)
	int numOfSilver = 0; // the number of SilverCrystals (Ag)
	int numOfPurpleGold = 0; // the number of purple gold
	int numOfGoldChloride = 0; // the number of gold chloride
	int numOfRochelleSalt = 0; // the number of rochelle salt
	int numOfCoolNoodle = 0; // the number of cool noodles made by Zhong
	int numOfStarlight = 0; // the number of starlight

	int[] mtrlPageIndex = {1, 2, 3, 4, 5}; // the index to control the pages in the "materials" section
	int[] fmlPageIndex = {1,2,3,4,5,6,7,8,9,10}; // the index to control the pages in the "formulae" section
	int currMtrlPage; // the current page of materials
	int currFmlPage; // the current page of formulae
	bool mtrlPage1;
	bool mtrlPage2;
	bool mtrlPage3;
	bool mtrlPage4;
	bool mtrlPage5;

	bool leadClicked;

	string op1; // operand 1
	string op2; // operand 2
	string re; // result

	string triumphStr = "Successful!";
	string getItemStr1 = "No Item Found";
	string getItemStr2 = "No Item Found";

	void Awake() {
		animator = GetComponent<Animator>();

		// at the beginning of the game, turn on storyIndex 0
		//storyIndex[0] = true;

		// set the texture so that it is the size of the screen
		guiTexture.pixelInset = new Rect(0, 0, Screen.width, Screen.height);
	}

	void Start () {

		starGO = GameObject.FindGameObjectsWithTag("Starlight"); // assign the starGO array
		// set the starlight to false here for now
		foreach(GameObject sgo in starGO) {
			sgo.SetActive(false);
		}

		drAIsClicked = false;
		mtrlIsClicked = false;
		stsIsClicked = false;
		fmlIsClicked = false;

		currMtrlPage = mtrlPageIndex[0];
		currFmlPage = fmlPageIndex[0];

		leadClicked = false;

		onBattle = false;
		showBattleEnd = false;
		showFoundItem = false;

		// assign expNeeded for each level at the beginning of runtime:
		expNeeded[0] = 0;
		for(int i = 1; i < 100; i++) {
			// max level: 100
			expNeeded[i] = expNeeded[i-1] + i*15;
		}

		// this gets the camera pos and rot at the beginning of the game
		//oriCamPos = camera.transform.position;
		//oriCamRot = camera.transform.rotation;

		// start following waypoint 1 during the Openup Animation phase
		//FollowWaypoints();

		// directions to follow the waypoints:
		nextWaypnt1 = new Vector3(waypoint1.transform.position.x - this.transform.position.x, 
			0, waypoint1.transform.position.z - this.transform.position.z);
		nextWaypnt2 = new Vector3(waypoint2.transform.position.x - waypoint1.transform.position.x, 
			0, waypoint2.transform.position.z - waypoint1.transform.position.z);
		nextWaypnt3 = new Vector3(waypoint3.transform.position.x - waypoint2.transform.position.x, 
			0, waypoint3.transform.position.z - waypoint2.transform.position.z);
		nextWaypnt4 = new Vector3(waypoint4.transform.position.x - waypoint3.transform.position.x, 
			0, waypoint4.transform.position.z - waypoint3.transform.position.z);
		nextWaypnt5 = new Vector3(waypoint5.transform.position.x - waypoint4.transform.position.x, 
			0, waypoint5.transform.position.z - waypoint4.transform.position.z);
		nextWaypnt6 = new Vector3(waypoint6.transform.position.x - waypoint5.transform.position.x, 
			0, waypoint6.transform.position.z - waypoint5.transform.position.z);

		// calculate the minimal distance (initial distance) between player and waypoints:
		//minDist1 = Vector3.Distance(this.transform.position, waypoint1.transform.position);
		minDist2 = Vector3.Distance(waypoint1.transform.position, waypoint2.transform.position) + 5;
		minDist3 = Vector3.Distance(waypoint2.transform.position, waypoint3.transform.position) + 5;
		minDist4 = Vector3.Distance(waypoint3.transform.position, waypoint4.transform.position) + 5; // for calibration
		//minDist5 = Vector3.Distance(waypoint4.transform.position, waypoint5.transform.position);
		//minDist6 = Vector3.Distance(waypoint5.transform.position, waypoint6.transform.position);

		sbePlusOnce = false;
	}

	void Update () {

		if(gameOver) {
			Application.LoadLevel("GameOver en");
		}

		maxHealth = 20 * level;

		// Locomotion controls
		if(!storyIndex[0]) { // if it's showing openup animation, disable the custom locomotion
			if(!onBattle) {
				if(Input.GetAxis("Vertical") != 0) {
					animator.SetBool("Walk", true);
				} else {
					animator.SetBool("Walk", false);
				}
				
				float translation = Input.GetAxis("Vertical") * walkSpeed;
				float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
				translation *= Time.deltaTime;
				rotation *= Time.deltaTime;
				transform.Translate(0, 0, translation);
				transform.Rotate(0, rotation, 0);			
			} else { // if on battle
				// player's position is fixed
				animator.SetBool("Walk", false); // let player remain idle or sth other than walking
				// let the animator status be "preparing for battle"
			}			
		} // end if(!storyIndex[0]) 
		else if(storyIndex[0]) { // when it's showing openup animation
		// 6 waypoints, 6 finite states:
			//int stepCounter;
			
			// follow those waypoints one by one
			
			//FollowWaypoints();
				
			/*
			if(waypntState == 2) {
				nextWaypnt = new Vector3(waypoint2.transform.position.x - this.transform.position.x, 
					0, waypoint2.transform.position.z - this.transform.position.z);
				this.transform.LookAt(waypoint2.transform);
				stepCounter = 0; // used to count the steps
				while(stepCounter < 10 && waypoint2 != null) {
					this.transform.position += 0.05f * nextWaypnt * Time.deltaTime;
					stepCounter++;
				}
				waypntState = 3;
			}
			*/
			FollowWaypoints();

		} // end if(storyIndex[0])

		if(!storyIndex[0]) {
			// if it's not OpenUp phase
			// disable all the titles(cubes)
			GameObject[] titleGO;
			titleGO = GameObject.FindGameObjectsWithTag("Title");
			foreach(GameObject gogo in titleGO) {
				gogo.SetActive(false);
			}
		}

		// in storyIndex[3] -- finding purple gold
		if(storyIndex[3]) {
			if(numOfPurpleGold >= 1) {
				// having found purple gold
				storyIndex[4] = true;
				dialogBools[15] = true;
				storyIndex[3] = false;
			}
		} // end storyindex 3

		if(storyIndex[6]) {

			// set the starlight to true from here
			GameObject[] slGO;
			slGO = GameObject.FindGameObjectsWithTag("Starlight");
			foreach(GameObject go in slGO) {
				go.SetActive(true);
			}

			storyIndex[5] = false;
			dialogBools[32] = true;
			// Moon Goddess entry
			FadeOut();
			RenderSettings.skybox = skybox2;
			FadeIn();
			//storyIndex[6] = false;
		} // end storyIndex 6

		// only show the starlight in storyIndex7
		if(storyIndex[7]) {
			foreach(GameObject gogogo in starGO) {
				if(gogogo != null) gogogo.SetActive(true);
			}
		} /*else if(!storyIndex[7]) {
			GameObject[] slGO2;
			slGO2 = GameObject.FindGameObjectsWithTag("Starlight");
			foreach(GameObject go in slGO2) {
				go.SetActive(false);
			}
		} */

	} // end Update

	void OnCollisionEnter(Collision collision) {
		if(collision.gameObject.tag == "Carbon") {
			//Instantiate(carbon_ps, this.transform.position, this.transform.rotation);
			AlertFoundItem("Carbon Crystal");
			numOfCarbon++;
			Destroy(collision.gameObject);
		}
		if(collision.gameObject.tag == "Hydra") {
			AlertFoundItem("Hydra Crystal");
			numOfHydra++;
			Destroy(collision.gameObject);
		}
		if(collision.gameObject.tag == "Pb") {
			AlertFoundItem("Lead Crystal");
			numOfLead++;
			Destroy(collision.gameObject);
		}
		if(collision.gameObject.tag == "Sn") {
			AlertFoundItem("Tin Crystal");
			numOfTin++;
			Destroy(collision.gameObject);
		}
		if(collision.gameObject.tag == "Fe") {
			AlertFoundItem("Iron Crystal");
			numOfIron++;
			Destroy(collision.gameObject);
		}
		if(collision.gameObject.tag == "Cu") {
			AlertFoundItem("Copper Crystal");
			numOfCopper++;
			Destroy(collision.gameObject);
		}
		if(collision.gameObject.tag == "Hg") {
			AlertFoundItem("Mercury Crystal");
			numOfMercury++;
			Destroy(collision.gameObject);
		}
		if(collision.gameObject.tag == "Ag") {
			AlertFoundItem("Silver Crystal");
			numOfSilver++;
			Destroy(collision.gameObject);
		}
		if(collision.gameObject.tag == "PurpleGold") {
			AlertFoundItem("Purple Gold");
			numOfPurpleGold++;
			Destroy(collision.gameObject);
		}
		if(collision.gameObject.tag == "RochelleSalt") {
			AlertFoundItem("Rochelle Salt");
			numOfRochelleSalt++;
			Destroy(collision.gameObject);
		}
		if(collision.gameObject.tag == "AuCl") {
			AlertFoundItem("Gold Chloride");
			numOfGoldChloride++;
			Destroy(collision.gameObject);
		}
		if(collision.gameObject.tag == "Starlight") {
			AlertFoundItem("Starlight");
			numOfStarlight++;
			Destroy(collision.gameObject);
		}

		if(collision.gameObject.tag == "StoryIndex8") {
			// the cube trigger on the bridge: A will start saying sth and the dawn breaks
			if(storyIndex[7]) {
				// this happends only in storyIndex 7
				dialogBools[35] = true;
			}
		}

		if(collision.gameObject.tag == "Zhong") {
			AlertFoundItem("Zhong");
			// freeze players' locomotion while chatting
			//if(Input.GetKeyDown(KeyCode.Space)) {
				// 
				if(storyIndex[1]) {
					dialogBools[3] = true;
				}
				
				this.transform.LookAt(collision.gameObject.transform); // look at the character while chatting
				collision.gameObject.transform.LookAt(this.transform); // look at the player while chatting

				if(storyIndex[5]) {
					// go to eat noodles at Zhong's place
					dialogBools[24] = true;
				}
			//}
		}
		if(collision.gameObject.tag == "Mayor") {
			AlertFoundItem("Village Chief");

			if(storyIndex[2]) {
				dialogBools[7] = true;
			}
			this.transform.LookAt(collision.gameObject.transform); // look at the mayor while chatting
			collision.gameObject.transform.LookAt(this.transform); // look at the player while chatting

			if(storyIndex[4]) {
				dialogBools[17] = true;
			}
		}

		if(collision.gameObject.tag == "Enemy") {
			AlertFoundItem("Battle");
			// triggers a battle
			fightingEnemy = collision.gameObject; // assign the currently colliding enemy to the fighting enemy
			Battle();
		}
		if(collision.gameObject.tag == "Troll") {
			AlertFoundItem("Battle");
			// triggers a battle with a troll
			expPlus = 8;
			fightingEnemy = collision.gameObject; // is this line really useful?
			// there should be a dictionary thingie to provide enemyHealth, enemyAttack, and enemyDefense
			enemyAnimator = fightingEnemy.GetComponent<Animator>();
			enemyHealth = 9;
			enemyAttack = 6;
			enemyDefense = 1;
			// there should be formulae to calculate the real harm for player-to-enemy, and enemy-to-player
			// formula: realHarm = attack - defense*50% > 0
			harmToEnemy = attack - (int)(enemyDefense * 0.5f); // harm per round
			if(harmToEnemy < 0) harmToEnemy = 0;
			harmToPlayer = enemyAttack - (int)(defense * 0.5f); // harm per round
			if(harmToPlayer < 0) harmToPlayer = 0;
			Battle();
		}
 	} // end OnCollisionEnter

 	void AlertFoundItem(string itemName) {
 		// displays a GUI box telling what item has the player just found
 		showFoundItem = true;
 		foundItemStr = itemName;
 		//WaitForItemDisplay();
 	}

 	void DetermineEnemyCategory() {
 		// used to determine which kind the enemy is, and give out the related health, attack ... of the enemy
 		// I think it's better to use different tags directly on collision
 	} // end DetermineEnemyCategory

 	void Battle() {
 		// triggers a battle

 		onBattle = true;
 		BattleCamera();
 	} // end Battle

 	void BattleEnd() {
 		// at the end of a battle:
 		//showBattleEnd = true;
 	}

	void OnGUI() {
		/* icon + texture
		public Texture2D icon;
        GUI.Button (new Rect (10,10,100,20), new GUIContent ("Click me", icon, "This is the tooltip"));
        GUI.Label (new Rect (10,40,100,20), GUI.tooltip);
    	*/

    	/* text field
    	private string textFieldString = "text field";
    	textFieldString = GUI.TextField(new Rect(25, 25, 100, 30), textFieldString);
    	*/

    	/* text area
    	private string textAreaString = "text area";
    	textAreaString = GUI.TextArea (new Rect (25, 25, 100, 30), textAreaString);
    	*/

    	/* toggle
    	private bool toggleBool = true;
    	toggleBool = GUI.Toggle (new Rect (25, 25, 100, 30), toggleBool, "Toggle");
    	*/

    	/* tool bar
    	private int toolbarInt = 0;
    	private string[] toolbarStrings = {"Toolbar1", "Toolbar2", "Toolbar3"};
    	toolbarInt = GUI.Toolbar (new Rect (25, 25, 250, 30), toolbarInt, toolbarStrings);
    	*/

		GUI.backgroundColor = Color.black;

		    int ALabLX = BOX_X; // the left corner X of the box of A's pocket lab
		    int ALabUY = BOX_Y; // the upper corner Y of the box of A's pocket lab
		    int ALabRX = BOX_X + Screen.width - 100; // the right corner X of the box of A's lab
		    int ALabLY = BOX_Y + Screen.height - 100; // the lower Y of the box of A's lab
		    int ALabWidth = Screen.width - 100;
		    int ALabHeight = Screen.height - 100;

		    int submenuWidth = ALabWidth/6; // the submenus mean "status","materials","system","formulae"
		    int submenuHeight = ALabHeight/6; 

		    int slot1 = ALabHeight/5; // the height of slots between submenus

			int subcontentLX = ALabLX + ALabWidth/4;
			int subcontentUY = ALabUY + ALabHeight/6;
			int subcontentWidth = ALabWidth/3; // the width of each line on subcontent
			int subcontentHeight = ALabHeight/8; // the height of each line on subcontent
		
			int slot2 = ALabHeight/8; // the height of the slots between each line from the subcontent
		// if Dr.A is clicked:
		if(drAIsClicked) {


	        GUI.Box(new Rect(ALabLX, ALabUY, ALabWidth, ALabHeight), "Dr.A's Pocket Lab");
		    if(GUI.Button(new Rect(ALabLX + ALabWidth/50, ALabUY + ALabHeight/8, submenuWidth, submenuHeight), "Status")) {
		    	stsIsClicked = true;
		    	mtrlIsClicked = false;
		    	fmlIsClicked = false;
		    }

		    //GUI.Button(new Rect(55, 140, 65, 30), "Materials");	
		    if(GUI.Button(new Rect(ALabLX + ALabWidth/50, ALabUY + ALabHeight/8 + slot1, submenuWidth, submenuHeight), "Materials")) {
		    	mtrlIsClicked = true;
		    	stsIsClicked = false;
		    	fmlIsClicked = false;
		    }		
		    if(GUI.Button(new Rect(ALabLX + ALabWidth/50, ALabUY + ALabHeight/8 + 2*slot1, submenuWidth, submenuHeight), "System")) {
		    	// save, load, settings and exit
		    }
		    if(GUI.Button(new Rect(ALabLX + ALabWidth/50, ALabUY + ALabHeight/8 + 3*slot1, submenuWidth, submenuHeight), "Formulae")) {
		    	// change "belongings" to sth like "solid properties", sounds like ancient
		    	// new English name: formulae
		    	fmlIsClicked = true;
		    	mtrlIsClicked = false;
		    	stsIsClicked = false;
		    }
		}

		// if Status is clicked
		if(stsIsClicked) {
			// show the status of the player
			GUI.Label(new Rect(subcontentLX, subcontentUY, subcontentWidth, subcontentHeight), "Health " + health + "/" + maxHealth);
			GUI.Label(new Rect(subcontentLX, subcontentUY + slot2, subcontentWidth, subcontentHeight), "Attack " + attack);
			GUI.Label(new Rect(subcontentLX, subcontentUY + 2*slot2, subcontentWidth, subcontentHeight), "Defend " + defense);
			GUI.Label(new Rect(subcontentLX, subcontentUY + 3*slot2, subcontentWidth, subcontentHeight), "Level " + level);	
			GUI.Label(new Rect(subcontentLX, subcontentUY + 4*slot2, subcontentWidth, subcontentHeight), "Experience " + exp + "/" + expNeeded[level]);
		}

		// if Materials is clicked
		if(mtrlIsClicked) {
			// interaction/explosion bar:
			GUI.Box(new Rect(ALabLX, ALabUY - subcontentHeight/2, ALabWidth, subcontentHeight/2), op1+" + "+op2+" = "+re);
			// put 2 buttons on the 2 sides of this equation
			if(GUI.Button(new Rect(ALabLX + ALabWidth/15, ALabUY - subcontentHeight/2, ALabWidth/6, subcontentHeight/2), "Clear")) {
				//
				op1 = null;
				op2 = null;
			}
			if(GUI.Button(new Rect(ALabLX + 4*ALabWidth/5, ALabUY - subcontentHeight/2, ALabWidth/6, subcontentHeight/2), "React")) {
				// asset op1 and op2 are not null
				//if( (op1 != "" && op1 != null && op2 != "" && op2　!= null) {
				if(op1 != null && op2 != null) {
					// there is a list of feasible reactions
					//1. purple gold = gold chloride + rochelle salt
					if( (op1 == "Rochelle Salt" && op2 == "Gold Chloride") || 
						(op1 == "Gold Chloride" && op2 == "Rochelle Salt") ) {
						re = "Purple Gold";
						numOfPurpleGold++;
						ResetOp1Op2();
						AlertFoundItem("Purple Gold");
					}
				} // else no reactions or alert texts				
			}

			// show the materials and their properties: hydracrystals, carboncrystals...
			if(GUI.Button(new Rect(ALabLX + ALabWidth/2, ALabUY + ALabHeight - subcontentHeight, ALabWidth/10, subcontentHeight), "#")){
				// asset op1 and op2 are not null
				//if( (op1 != "" && op1 != null && op2 != "" && op2　!= null) {
				if(op1 != null && op2 != null) {
					// there is a list of feasible reactions
					//1. purple gold = gold chloride + rochelle salt
					if( (op1 == "Rochelle Salt" && op2 == "Gold Chloride") || 
						(op1 == "Gold Chloride" && op2 == "Rochelle Salt") ) {
						re = "Purple Gold";
						numOfPurpleGold++;
						ResetOp1Op2();
						AlertFoundItem("Purple Gold");
					}
				} // else no reactions or alert texts
			}
			if(GUI.Button(new Rect(ALabLX + ALabWidth/2 + ALabWidth/10, ALabUY + ALabHeight - subcontentHeight, ALabWidth/10, subcontentHeight), ">")){
				if(currMtrlPage < 5) currMtrlPage++;
			}
			if(GUI.Button(new Rect(ALabLX + ALabWidth/2 - ALabWidth/10, ALabUY + ALabHeight - subcontentHeight, ALabWidth/10, subcontentHeight), "<")){
				if(currMtrlPage > 1) currMtrlPage--;
			}

			// page 1
			if(currMtrlPage == 1) {
				// -column1
				//GUI.Button(new Rect(BOX_X+GRID_WIDTH+30, BOX_Y+30, 150, 20), 
					//new GUIContent("水晶体 *" + numOfHydra, "Hydra crystals are atomic water."));
				//GUI.RepeatButton(new Rect(BOX_X+GRID_WIDTH+30, BOX_Y+60, 150, 20), "炭晶体 *" + numOfCarbon);
				if(GUI.Button(new Rect(ALabLX+ALabWidth/4, ALabUY+ALabHeight/8, subcontentWidth, subcontentHeight), "Hydra Crystal *" + numOfHydra)){
					numOfHydra = GoToInteraction(numOfHydra, "Water");
				}
				if(GUI.Button(new Rect(ALabLX+ALabWidth/4, ALabUY+ALabHeight/8+ALabHeight/7, subcontentWidth, subcontentHeight), "Carbon Crystal *" + numOfCarbon)){
					numOfCarbon = GoToInteraction(numOfCarbon, "Carbon");
				}
				if(GUI.Button(new Rect(ALabLX+ALabWidth/4, ALabUY+ALabHeight/8+2*ALabHeight/7, subcontentWidth, subcontentHeight), "Lead Crystal *" + numOfLead)){
					numOfLead = GoToInteraction(numOfLead, "Lead");
				}
				if(GUI.Button(new Rect(ALabLX+ALabWidth/4, ALabUY+ALabHeight/8+3*ALabHeight/7, subcontentWidth, subcontentHeight), "Tin Crystal *" + numOfTin)){
					numOfTin = GoToInteraction(numOfTin, "Tin");
				}			
				// -column2
				if(GUI.Button(new Rect(ALabLX+3*ALabWidth/5, ALabUY+ALabHeight/8, subcontentWidth, subcontentHeight), "Mercury Crystal *" + numOfMercury)){
					numOfMercury = GoToInteraction(numOfMercury, "Mercury");
				}
				if(GUI.Button(new Rect(ALabLX+3*ALabWidth/5, ALabUY+ALabHeight/8+ALabHeight/7, subcontentWidth, subcontentHeight), "Silver Crystal *" + numOfSilver)){
					numOfSilver = GoToInteraction(numOfSilver, "Silver");
				}
				if(GUI.Button(new Rect(ALabLX+3*ALabWidth/5, ALabUY+ALabHeight/8+2*ALabHeight/7, subcontentWidth, subcontentHeight), "Purple Gold *" + numOfPurpleGold)){
					numOfPurpleGold = GoToInteraction(numOfPurpleGold, "Purple Gold");
				}
				if(GUI.Button(new Rect(ALabLX+3*ALabWidth/5, ALabUY+ALabHeight/8+3*ALabHeight/7, subcontentWidth, subcontentHeight), "Gold Chloride *" + numOfGoldChloride)){
					numOfGoldChloride = GoToInteraction(numOfGoldChloride, "Gold Chloride");
				}
			}

			// page 2
			if(currMtrlPage == 2) {
				if(GUI.Button(new Rect(ALabLX+ALabWidth/4, ALabUY+ALabHeight/8, subcontentWidth, subcontentHeight), "Copper Crystal *" + numOfCopper)){
					numOfCopper = GoToInteraction(numOfCopper, "Copper");
				}
				if(GUI.Button(new Rect(ALabLX+ALabWidth/4, ALabUY+ALabHeight/8+ALabHeight/7, subcontentWidth, subcontentHeight), "Iron Crystal *" + numOfIron)){
					numOfIron = GoToInteraction(numOfIron, "Iron");
				}
				if(GUI.Button(new Rect(ALabLX+ALabWidth/4, ALabUY+ALabHeight/8+2*ALabHeight/7, subcontentWidth, subcontentHeight), "Rochelle Salt *" + numOfRochelleSalt)){
					numOfRochelleSalt = GoToInteraction(numOfRochelleSalt, "Rochelle Salt");
				}	
				if(GUI.Button(new Rect(ALabLX+ALabWidth/4, ALabUY+ALabHeight/8+3*ALabHeight/7, subcontentWidth, subcontentHeight), "Zhong's Noodles *" + numOfCoolNoodle)) {
					numOfCoolNoodle = GoToInteraction(numOfCoolNoodle, "Zhong's Noodles");
				}
				// - column 2
				if(GUI.Button(new Rect(ALabLX+3*ALabWidth/5, ALabUY+ALabHeight/8, subcontentWidth, subcontentHeight), "Condensed Starlight *" + numOfStarlight)){
					numOfStarlight = GoToInteraction(numOfStarlight, "Condensed Starlight");
				}				
			}

			// page index
			GUI.Label(new Rect(ALabLX+ALabWidth-ALabWidth/9, ALabUY+ALabHeight-subcontentHeight, ALabWidth/10, subcontentHeight), "Page "+currMtrlPage);
			
		}

		// if formulae is clicked
		if(fmlIsClicked) {
			// formulae~
			//GUI.Label(new Rect(BOX_X+10, BOX_Y+10, 200, 30), "罗谢尔盐 + 氯化金 = 紫金");

			if(GUI.Button(new Rect(ALabLX + ALabWidth/2 + ALabWidth/10, ALabUY + ALabHeight - subcontentHeight, ALabWidth/10, subcontentHeight), ">")){
				if(currFmlPage < 10) currFmlPage++;
			}
			if(GUI.Button(new Rect(ALabLX + ALabWidth/2 - ALabWidth/10, ALabUY + ALabHeight - subcontentHeight, ALabWidth/10, subcontentHeight), "<")){
				if(currFmlPage > 1) currFmlPage--;
			}

			if(currFmlPage == 1) GUI.Label(new Rect(ALabLX+ALabWidth/4, ALabUY+ALabHeight/8+ALabHeight/7, 2*subcontentWidth, subcontentHeight), 
				"Rochelle Salt + Gold Chloride = Purple Gold");
			if(currFmlPage == 2) GUI.Label(new Rect(ALabLX+ALabWidth/4, ALabUY+ALabHeight/8+ALabHeight/7, 2*subcontentWidth, subcontentHeight), 
				"Glo-wormes + Water = Luminous Water");
			if(currFmlPage == 3) GUI.Label(new Rect(ALabLX+ALabWidth/4, ALabUY+ALabHeight/8+ALabHeight/7, 2*subcontentWidth, subcontentHeight), 
				"Rose Quartz + Water = Oil of Rose Quartz");
			if(currFmlPage == 4) GUI.Label(new Rect(ALabLX+ALabWidth/4, ALabUY+ALabHeight/8+ALabHeight/7, 2*subcontentWidth, subcontentHeight), 
				"Condensed Starlight + Rain/Snow/Hail/Dew = Celestial Water");
			if(currFmlPage == 5) GUI.Label(new Rect(ALabLX+ALabWidth/4, ALabUY+ALabHeight/8+ALabHeight/7, 2*subcontentWidth, subcontentHeight), 
				"Earth of Water + Sunlight = Universal Gur");
			if(currFmlPage == 6) GUI.Label(new Rect(ALabLX+ALabWidth/4, ALabUY+ALabHeight/8+ALabHeight/7, 2*subcontentWidth, subcontentHeight), 
				"Curacao Blue + Liqueurs d'amandes = Rhine Love Song");

			GUI.Label(new Rect(ALabLX+ALabWidth-ALabWidth/9, ALabUY+ALabHeight-subcontentHeight, ALabWidth/10, subcontentHeight), "Page "+currFmlPage);
		}

		//drAIsClicked = GUI.Button(new Rect(0, 0, 50, 50), "Dr.A");
		if(GUI.Button(new Rect(0, 0, 50, 50), "Dr.A")) {
			if(drAIsClicked) {
				drAIsClicked = false;
				mtrlIsClicked = false;
				fmlIsClicked = false;
				stsIsClicked = false;
			} else if(!drAIsClicked) {
				drAIsClicked = true;
			}
		} 

		// On Battle:
		if(onBattle) {
			if(fightingEnemy == null) {
				RestoreCamera();
				onBattle = false;
			}

			GUI.Box(new Rect(Screen.width/3, 3*Screen.height/4, Screen.width/3, Screen.height/4),"");
			if(GUI.Button(new Rect(Screen.width/3, 3*Screen.height/4, Screen.width/6, Screen.height/8), "Attack")) {
				// play attack animation
				//ResetDefendAnim();
				StartCoroutine( PlayOneShot("Attack", animator) );
				//animator.SetBool("Attack", true);
				// draw harm for this round:
				if(fightingEnemy != null) {
					if(enemyHealth > 0) {
						StartCoroutine( PlayOneShot("T_hit", enemyAnimator) );
						enemyHealth -= harmToEnemy;
					} else if(enemyHealth <= 0) {
						StartCoroutine( PlayOneShot("T_die", enemyAnimator) );
						RestoreCamera();
						Destroy(fightingEnemy, 2.0f);
						onBattle = false;
						showBattleEnd = true;
						sbePlusOnce = false;
						//hasFlipedCoin = false;
					} 					
				} else {
					// if fightingEnemy == null
					RestoreCamera();
					onBattle = false;
				}

			}
			if(GUI.Button(new Rect(Screen.width/2, 3*Screen.height/4, Screen.width/6, Screen.height/8), "Defend")) {
				//animator.SetBool("Defend", true); // this lasts until other buttons have been clicked
				StartCoroutine( PlayOneShot("Defend", animator) );
			}
			GUI.Button(new Rect(Screen.width/3, 7*Screen.height/8, Screen.width/6, Screen.height/8), "Medicine");
			if(GUI.Button(new Rect(Screen.width/2, 7*Screen.height/8, Screen.width/6, Screen.height/8), "Flight")) {
				// this can be used to enforce leaving this battle
				RestoreCamera();
				onBattle = false;
			}


			GUI.Box(new Rect(2*Screen.width/3, 9*Screen.height/10, Screen.width/3, Screen.height/10), 
				"Dr.A   Health " + health + "/" + maxHealth);


			// determine when the enemy dies
			/*
			if(enemyHealth < 0) {
				onBattle = false;
				//BattleEnd();
				// destroy the enemy
				if(enemyAnimator != null) {
					//StartCoroutine( PlayOneShot("T_die", enemyAnimator) );
					Destroy(fightingEnemy, 1.0f);
				}
				//RestoreCamera();
			}
			*/
		}

		// simplified Show Battle End
		if(showBattleEnd) {
			// flip the coin
			//bool hasFlipedCoin == false;

			if(storyIndex[3] && !sbePlusOnce) {
				// the player is looking for gold chloride or rochelle salt
					getItemStr1 = "Gold Chloride";
					numOfGoldChloride++;
					getItemStr2 = "Rochelle Salt";
					numOfRochelleSalt++;
					sbePlusOnce = true;
			//} else {
				// other story indices
				//getItemStr = "无战利品";
				//hasFlipedCoin = false;
			}
			if(GUI.Button(new Rect(Screen.width/3, Screen.height/3, Screen.width/3, Screen.height/3), 
				triumphStr + "\n" + getItemStr1 + "\n" + getItemStr2)) {
				//
				//closeBattleEnd = true;
				showBattleEnd = false;
			}
			
		}
		//if(closeBattleEnd) showBattleEnd = false;

		// show battle end - HAVE SEVERE PROBLEMS!!!
		/*
		if(showBattleEnd) {
			GUI.Box(new Rect(Screen.width/2-50, Screen.height/2-50, 100, 100), "战斗胜利");
			GUI.Label(new Rect(Screen.width/2-40, Screen.height/2 - 20, 100, 20), 
				"经验值 "+exp+"+"+expPlus+"/"+expNeeded[level]);
			exp += expPlus;
			if(exp >= expNeeded[level]) {
				GUI.Label(new Rect(Screen.width/2-40, Screen.height/2, 100, 20), "等级 "+level+"+1");
				level++;
			} else {
				GUI.Label(new Rect(Screen.width/2-40, Screen.height/2, 100, 20), "等级 " + level);
			}
			if(GUI.Button(new Rect(Screen.width/2-20, Screen.height/2+25, 40, 20), "确定")){
				showBattleEnd = false;
			}
		}
		*/
		// show what item has the player just found
		if(showFoundItem) {
			GUI.Box(new Rect(Screen.width/2-50, 10, 100, 20), foundItemStr);
		}

		// show what new item has just been created:
		//if(showCreatedItem) {
			// can be replaced by the upper "showFoundItem"
		//}

		// show dialog 001
		string dialog001Str = "Dr.A: Just reviewed a little bit, \nthe competition in the capital should not be\na problem now.";
		if(dialogBools[0]) {
			if(GUI.Button(DrawDialogRect(), dialog001Str)) {
				//dialog001 = false;
				dialogBools[1] = true;
				//GUI.Button(new Rect(50, Screen.height-180, Screen.width-100, 75), dialog001Str);
			}
		}
		dialog001Str = "Dr.A: So hungry... it's right before the village, \n why don't I go to Zhong's for some noodles?";
		if(dialogBools[1]) {
			dialogBools[0] = false;
			storyIndex[1] = true;
			if(GUI.Button(DrawDialogRect(), dialog001Str)) {
				storyIndex[0] = false;
				dialogBools[1] = false;
				//dialog001Str = "有点饿了，正好走到村子口了，去找钟吃凉面好了。";
				//GUI.Button(new Rect(50, Screen.height-180, Screen.width-100, 75), dialog001Str);
			}
		}
		string dialogStr = "Dr.A: Zhong, have you made any noodles today?";
		if(dialogBools[3] && storyIndex[1]) {
			if(GUI.Button(DrawDialogRect(), dialogStr)) {
				//dialogBools[3] = false;
				dialogBools[4] = true;
			}
		}
		dialogStr = "Zhong: I got up late today.. but I'm making them\nright now. The chief is looking for you,\ngood thing you're back early today";
		if(dialogBools[4] && storyIndex[1]) {
			dialogBools[3] = false;
			if(GUI.Button(DrawDialogRect(), dialogStr)) {
				dialogBools[5] = true;
			}
		}
		dialogStr = "Dr.A: The chief's been looking for me? \n I'd better go and ask him.";
		if(dialogBools[5] && storyIndex[1]) {
			dialogBools[4] = false;

			if(GUI.Button(DrawDialogRect(), dialogStr)) {
				dialogBools[6] = true; // for each block of dialogs, indices should be continuous,
				// there should be one slot of index between each block, for example:
				// dialog0-1 dialog3-5 dialog7-?....
			}
		}
		if(dialogBools[6]) {
			storyIndex[1] = false; // end the noodles talk with Zhong
			storyIndex[2] = true; // start the purple gold talk with mayor
			dialogBools[6] = false;
		}

		dialogStr = "Chief: A, you've finished practicing early today.\n This is great, I need your favor actually.";
		if(dialogBools[7] && storyIndex[2]) {
			//
			if(GUI.Button(DrawDialogRect(), dialogStr)) {
				dialogBools[8] = true;
			}
		}

		dialogStr = "Dr.A: If I kept my usual schedule today,\nwouldn't you wait until midnight at my door?\nBut nothing can trouble me, \nplease tell me your majesty.";
		if(dialogBools[8] && storyIndex[2]) {
			dialogBools[7] = false;
			if(GUI.Button(DrawDialogRect(), dialogStr)) {
				dialogBools[9] = true;
			}
		}

		dialogStr = "Chief: Here's the thing... I need \nyou to find some materials,\n then make purple gold with them.";
		if(dialogBools[9] && storyIndex[2]) {
			dialogBools[8] = false;
			if(GUI.Button(DrawDialogRect(), dialogStr)) {
				dialogBools[10] = true;
			}
		}

		dialogStr = "Dr.A: Oh really? I've picked up\n some materials on my way home.\nWhat is the formula for purple gold?";
		if(dialogBools[10] && storyIndex[2]) {
			dialogBools[9] = false;
			if(GUI.Button(DrawDialogRect(), dialogStr)) {
				dialogBools[11] = true;
			}
		}

		dialogStr = "Chief: Here's the formula: \nGold Chloride + Rochelle Salt,\n You can collect the materials or \nfight the trolls to get them. \nI think you'll find a lot under the sidehill.";
		if(dialogBools[11] && storyIndex[2]) {
			dialogBools[10] = false;
			if(GUI.Button(DrawDialogRect(), dialogStr)) {
				dialogBools[12] = true;
			}
		}

		dialogStr = "Dr.A: Fine, I've written it on my formulae notebook.\nI'm going right now, have faith in me!";
		if(dialogBools[12] && storyIndex[2]) {
			dialogBools[11] = false;
			if(GUI.Button(DrawDialogRect(), dialogStr)) {
				//dialogBools[13] = true;
				dialogBools[13] = true;
			}
		}
		if(dialogBools[13]) {
			storyIndex[2] = false;
			storyIndex[3] = true; // open for finding crystals and fighting trolls
			dialogBools[13] = false;
		}


		//____________storyIndex[4]__________________
		if(storyIndex[4]) {
			dialogStr = "Dr.A: I got purple gold! He'd be amazed \nhow fast that is once more.";
			if(dialogBools[15]) {
				if(GUI.Button(DrawDialogRect(), dialogStr)) {
					dialogBools[15] = false;
				}
			}
		}

		if(dialogBools[17]) {
			// A goes to tell mayor that he has got purple gold
			dialogStr = "Dr.A: I got it nailed, there you go.";
			numOfPurpleGold--;
			if(GUI.Button(DrawDialogRect(), dialogStr)) dialogBools[18] = true;
		}
		if(dialogBools[18]) {
			dialogBools[17] = false;
			dialogStr = "Chief: Thank you so much.... \nZhong just made noodles.\nGo and get your share.";
			if(GUI.Button(DrawDialogRect(), dialogStr)) dialogBools[19] = true;
		}
		if(dialogBools[19]) {
			dialogBools[18] = false;
			dialogStr = "Dr.A: It's not that big a favor, see you later.";
			if(GUI.Button(DrawDialogRect(), dialogStr)) dialogBools[20] = true;
		}
		if(dialogBools[20]) {
			dialogBools[19] = false;
			dialogStr = "Chief: Right, you're going to the capital Dainon tomorrow.\nWe're going to say goodbye tomorrow morning.";
			if(GUI.Button(DrawDialogRect(), dialogStr)) dialogBools[21] = true;
		}
		if(dialogBools[21]) {
			dialogBools[20] = false;
			dialogStr = "Dr.A: Alright, see you tomorrow morning then.";
			if(GUI.Button(DrawDialogRect(), dialogStr)) dialogBools[22] = true;
		}
		if(dialogBools[22]) {
			dialogBools[21] = false;
			storyIndex[5] = true;
			storyIndex[4] = false;
		}

		// __________________ storyIndex 5_____________________________________________
		if(dialogBools[24]) {
			dialogStr = "Zhong: The noodles are ready, A, come and eat.";
			if(GUI.Button(DrawDialogRect(), dialogStr)) dialogBools[25] = true;
		}
		if(dialogBools[25]) {
			dialogStr = "Dr.A: Awesome, every time I eat your noodles,\nI don't feel exhausted at all.";
			dialogBools[24] = false;
			if(GUI.Button(DrawDialogRect(), dialogStr)) dialogBools[26] = true;
		}
		if(dialogBools[26]) {
			dialogStr = "Zhong: Well, too bad I can't go to Dainon with you,\nbut I made one more share, you can \ntake it tomorrow.";
			dialogBools[25] = false;
			if(GUI.Button(DrawDialogRect(), dialogStr)) dialogBools[27] = true;
		}
		if(dialogBools[27]) {
			dialogStr = "Dr.A: That'll be extremely great!";
			dialogBools[26] = false;
			if(GUI.Button(DrawDialogRect(), dialogStr)) dialogBools[28] = true;
		}
		if(dialogBools[28]) {
			dialogStr = "Zhong: It's no big deal, \nyou'd better go home and rest after,\nwe'll be seeing you off tomorrow morning.";
			dialogBools[27] = false;
			if(!zcnPlusOnce) {
				numOfCoolNoodle++;
				zcnPlusOnce = true;
			}
			if(GUI.Button(DrawDialogRect(), dialogStr)) dialogBools[29] = true;
		}
		if(dialogBools[29]) {
			dialogStr = "Dr.A: OK fine, I'll see you beside \nthe wooden bridge tomorrow.";
			dialogBools[28] = false;
			if(GUI.Button(DrawDialogRect(), dialogStr)) dialogBools[30] = true;
		}
		if(dialogBools[30]) {
			dialogBools[29] = false;
			// enable fading in and out
			storyIndex[6] = true; // enter Moon Goddess story (I)
			//RenderSettings.skybox = skybox2;

			dialogBools[30] = false;
		}

		if(dialogBools[32]) {
			//dialogBools[32] = true;
			// hide the two jumping boys
			GameObject[] twoBoysGO;
			twoBoysGO = GameObject.FindGameObjectsWithTag("normalnpc");
			foreach(GameObject tbgo in twoBoysGO) {
				tbgo.SetActive(false);
			}
			// hide Zhong and Mayor
			GameObject zhongGO = GameObject.FindWithTag("Zhong");
			if(zhongGO != null) zhongGO.SetActive(false);
			GameObject mayorGO = GameObject.FindWithTag("Mayor");
			if(mayorGO != null) mayorGO.SetActive(false);
			dialogStr = "Dr.A: I'm too excited to sleep...\nWhy don't I go along the bridge? \nI can collect some starlight along the way.";
			storyIndex[6] = false;
			storyIndex[7] = true;
			if(GUI.Button(DrawDialogRect(), dialogStr)) dialogBools[32] = false;
		}

		// story index 7
		if(dialogBools[33]) {
			dialogStr = "Dr.A: ";
			dialogBools[32] = false;
			if(GUI.Button(DrawDialogRect(), dialogStr)) dialogBools[33] = false;
		}

		// story index 8
		if(dialogBools[35]) {
			dialogStr = "Dr.A: I'd really like to see the world \noutside the Rabbit Village....";
			RenderSettings.skybox = skybox1;
			if(GUI.Button(DrawDialogRect(), dialogStr)) dialogBools[36] = true;
		}
		if(dialogBools[36]) {
			dialogBools[35] = false;
			dialogStr = "Dr.A: Eh? It's morning already. \nThey should be on their way coming.\nGoodbye, Rabbit Village.";
			if(GUI.Button(DrawDialogRect(), dialogStr)) gameOver = true;//dialogBools[37] = false;
		}
		/*
		if(dialogBools[37]) {
			dialogBools[36] = false;
			dialogStr = "A博士： 咦？！已经天亮了么！";
			if(GUI.Button(DrawDialogRect(), dialogStr)) dialogBools[38] = true;
		}
		if(dialogBools[38]) {
			dialogBools[37] = false;
			dialogStr = "村长： 是啊，你也该出发了。 \n 我们兔子村都很为你感到自豪哦，加油小A！ \n 我们都等着你的好消息。";
			if(GUI.Button(DrawDialogRect(), dialogStr)) dialogBools[39] = true;
		}
		if(dialogBools[39]) {
			dialogBools[38] = false;
			dialogStr = "钟： 小A，你一直都很向往外面的生活， \n 外面很危险，你要努力适应啊！";
			if(GUI.Button(DrawDialogRect(), dialogStr)) dialogBools[40] = true;
		}
		if(dialogBools[40]) {
			dialogBools[39] = false;
			dialogStr = "A博士： 好的，大家，我要启程了！\n 我会给你们写信的！";
			if(GUI.Button(DrawDialogRect(), dialogStr)) dialogBools[41] = true;
		}
		*/

	} // end OnGUI

	public IEnumerator PlayOneShot(string paramName, Animator _animator) {
		_animator.SetBool(paramName, true);
		yield return null;
		_animator.SetBool(paramName, false);
	} // end PlayOneShot

	public IEnumerator WaitForItemDisplay() {
		// let the process wait for some seconds
		showFoundItem = true;
		yield return null;
		showFoundItem = false;
	} // end Wait

	void ResetDefendAnim() {
		animator.SetBool("Defend", false);
	}

	void RestoreCamera() {
		// restore the position and rotation for the camera
		camera.transform.position = oriCamPos;
		camera.transform.rotation = oriCamRot;
	} // end RestoreCamera

	void BattleCamera() {
		// store the current camera position and rotation
		oriCamPos = camera.transform.position;
 		oriCamRot = camera.transform.rotation;
		// set the camera for battle situations
		camera.transform.position = battleCameraSphere.transform.position;
		camera.transform.LookAt(this.transform);
	} // end BattleCamera

	int GoToInteraction(int numOfElement, string shownNameOfElement) {
		// put the selected element to the interaction with others

		// prerequisite: avoid "both op1 and op2 are full"
		if(op1 != null && op2!= null) return numOfElement;

		if(numOfElement >= 1) {
			if(op1 == "" || op1 == null) op1 = shownNameOfElement;
			else op2 = shownNameOfElement;	
			numOfElement--;		
		}
		return numOfElement;
	} // end GoToInteraction

	void ResetOp1Op2() {
		// reset op1 and op2 after a successful or unsuccessful interaction
		op1 = null;
		op2 = null;
		re = null;
	} // end ResetOp1Op2

	void FollowWaypoints() {

		// play BGM
		//audio.PlayOneShot(openupBGM, 0.7F);

		// being executed in Start rather than Update?
		// maybe because it's in Start, so Time.deltaTime is not working? - so no start?
		animator.SetBool("Walk", true);
		// switches:
		bool open1 = true;
		bool open2 = false;
		bool open3 = false;
		bool open4 = false;
		bool open5 = false;
		bool open6 = false;
		//Vector3 nextWaypnt;
		// for nextWaypnt is decreasing as the player approaches his goal, the speed would drop drastically
	
		// "LookAt" has an issue, when he approaches a certain cube, he still looks at it, it's not prominent
		// if there's a big distance between them, but this makes the player looks up severely wrong when
		// they are fairly near each other.
		// the LookAt should be calibrated (with erasing y's influence)
		// LookAt's y should be equal to player's y
		// ...... y's been solved, and he's no pitching to weird directions any more, BUT
		// he gets used to turn right about 30 degrees just before he's about to grab the cubs
		// --- solution: forbid yawing if distance between the player and the crystal is less than.. 10?
		// --- solution2: make the player do a grabing animation (or similar) to sweep this flaw under the rug

		// the 5th crystal isn't going as planned to be.
		// solution: create a minDist for each crystal from the player, the minDist should be smaller and smaller,
		// once the minDist is becoming larger and larger again, destroy the crystal immediately
		
			// 1. waypoint1:
			if(waypoint1 != null && open1) {
				/*
				float tempDist1 = Vector3.Distance(this.transform.position, waypoint1.transform.position);
				if((int)tempDist1 <= (int)minDist1) {
					minDist1 = tempDist1;
				} else {
					Destroy(waypoint1);
				}
				*/
				// should make nextWaypnt constant
				//nextWaypnt = new Vector3(waypoint1.transform.position.x - this.transform.position.x, 
				//	0, waypoint1.transform.position.z - this.transform.position.z);
				Transform lookAtTrans = waypoint1.transform;
				lookAtTrans.position = new Vector3(lookAtTrans.position.x, this.transform.position.y,
							lookAtTrans.position.z);
				this.transform.LookAt(lookAtTrans);
				int stepCounter = 0; // used to count the steps
				//while(stepCounter < 10 && waypoint1 != null) {
					this.transform.position += 0.2f * nextWaypnt1 * Time.deltaTime;
					stepCounter++;
				//}		
				open1 = false; // I don't think this line is necessary, but just for logical seamlessness...
				//open2 = true;		
			}
			if(waypoint1 == null && waypoint2 != null) {
				open2 = true;
			}

			// 2. waypoint2:
			if(waypoint2 != null && open2) {
				
				float tempDist2 = Vector3.Distance(this.transform.position, waypoint2.transform.position);
				if((int)tempDist2 <= (int)minDist2) {
					minDist2 = tempDist2;
				} else {
					Destroy(waypoint2);
				}
				
				//nextWaypnt = new Vector3(waypoint2.transform.position.x - this.transform.position.x, 
				//	0, waypoint2.transform.position.z - this.transform.position.z);
				Transform lookAtTrans = waypoint2.transform;
				lookAtTrans.position = new Vector3(lookAtTrans.position.x, this.transform.position.y,
							lookAtTrans.position.z);
				this.transform.LookAt(waypoint2.transform);
				int stepCounter = 0; // used to count the steps
				while(stepCounter < 10 && waypoint2 != null) {
					this.transform.position += 0.008f * nextWaypnt2 * Time.deltaTime;
					stepCounter++;
				}		
				open2 = false; // I don't think this line is necessary, but just for logical seamlessness...
				//open3 = true;		
			}
			if(waypoint2 == null && waypoint3 != null) open3 = true;

			// 3. waypoint3:
			if(waypoint3 != null && open3) {
				
				float tempDist3 = Vector3.Distance(this.transform.position, waypoint3.transform.position);
				if((int)tempDist3 <= (int)minDist3) {
					minDist3 = tempDist3;
				} else {
					Destroy(waypoint3);
				}				
				
				Transform lookAtTrans = waypoint3.transform;
				lookAtTrans.position = new Vector3(lookAtTrans.position.x, this.transform.position.y,
							lookAtTrans.position.z);
				this.transform.LookAt(waypoint3.transform);
				int stepCounter = 0;
				while(stepCounter < 10 && waypoint3 != null) {
					this.transform.position += 0.007f * nextWaypnt3 * Time.deltaTime;
					stepCounter++;
				}
				open3 = false;
			}
			if(waypoint3 == null && waypoint4 != null) open4 = true;

			// 4. waypoint4:
			if(waypoint4 != null && open4) {
				
				float tempDist4 = Vector3.Distance(this.transform.position, waypoint4.transform.position);
				if((int)tempDist4 <= (int)minDist4) {
					minDist4 = tempDist4;
				} else {
					Destroy(waypoint4);
				}	
				
				Transform lookAtTrans = waypoint4.transform;
				lookAtTrans.position = new Vector3(lookAtTrans.position.x, this.transform.position.y,
							lookAtTrans.position.z);
				this.transform.LookAt(waypoint4.transform);
				int stepCounter = 0;
				while(stepCounter < 10 && waypoint4 != null) {
					this.transform.position += 0.01f * nextWaypnt4 * Time.deltaTime;
					stepCounter++;
				}
				open4 = false;
			}
			if(waypoint4 == null && waypoint5 != null) open5 = true;

			if(open5) {
				// kill the music 
				//audio.Stop();
				// set the animation back to idle
				animator.SetBool("Walk", false);
				Destroy(title1);
				// show dialogs
				dialogBools[0] = true;
			}

			/*
			// 5. waypoint5:
			if(waypoint5 != null && open5) {

				float tempDist5 = Vector3.Distance(waypoint4.transform.position, waypoint5.transform.position);
				if((int)tempDist5 <= (int)minDist5) {
					minDist5 = tempDist5;
				} else {
					Destroy(waypoint5);
				}	

				Transform lookAtTrans = waypoint5.transform;
				lookAtTrans.position = new Vector3(lookAtTrans.position.x, this.transform.position.y,
							lookAtTrans.position.z);
				this.transform.LookAt(waypoint5.transform);
				int stepCounter = 0;
				while(stepCounter < 10 && waypoint5 != null) {
					this.transform.position += 0.01f * nextWaypnt5 * Time.deltaTime;
					stepCounter++;
				}
				open5 = false;
			}
			if(waypoint5 == null && waypoint6 != null) open6 = true;

			// 6. waypoint6:
			if(waypoint6 != null && open6) {

				float tempDist6 = Vector3.Distance(waypoint5.transform.position, waypoint6.transform.position);
				if((int)tempDist6 <= (int)minDist6) {
					minDist6 = tempDist6;
				} else {
					Destroy(waypoint6);
				}	

				Transform lookAtTrans = waypoint6.transform;
				lookAtTrans.position = new Vector3(lookAtTrans.position.x, this.transform.position.y,
							lookAtTrans.position.z);
				this.transform.LookAt(waypoint6.transform);
				int stepCounter = 0;
				while(stepCounter < 10 && waypoint6 != null) {
					this.transform.position += 0.01f * nextWaypnt6 * Time.deltaTime;
					stepCounter++;
				}
				open6 = false;
			}
			*/
			//if(waypoint5 == null && waypoint6 != null) open6 = true;
		
			// after he reaches waypoint1, turn open1 off and open2 on
			//open1 = false;
			//open2 = true;	
		

		
			// 2. waypoint2:
			/*
			nextWaypnt = new Vector3(waypoint2.transform.position.x - this.transform.position.x,
				0, waypoint2.transform.position.z - this.transform.position.z);
			this.transform.LookAt(waypoint2.transform);
			stepCounter = 0;
			while(stepCounter < 10 && waypoint2 != null) {
				this.transform.position += 1.0f * nextWaypnt * Time.deltaTime;
				stepCounter++;
			}
			*/			
			// after he reaches waypoint2, turn open2 off and open3 on
			//open2 = false;
			//open3 = true;
		


	} // end FollowWaypoints

	Rect DrawDialogRect() {
		Rect dialogRect = new Rect(BOX_X, 3*Screen.height/4, 2*Screen.width/3, Screen.height/4);

		return dialogRect;
	} // end DrawDialogRect

	int numPlusOne(int numOfWhatever) {
		// plus one to number of whatever, just once!!!!
		// used for ShowBattleEnd only
		sbePlusOnce = false;
		return numOfWhatever + 1;
	} // end numPlusOne

	// Screen fading in
	void FadeToClear() {
		// Lerp the colour of the texture between itself and transparent
		guiTexture.color = Color.Lerp(guiTexture.color, Color.clear, fadeSpeed * Time.deltaTime);
	}
	void FadeIn() {
		// fade the texture to clear.
		FadeToClear();
		// if the texture is almost clear
		if(guiTexture.color.a <= 0.05f) {
			// set the color to clear and disable the guiTexture
			guiTexture.color = Color.clear;
			guiTexture.enabled = false;
		}
	}

	// Screen fading out
	void FadeToBlack() {
		// Lerp the color of the texture between itself and black
		guiTexture.color = Color.Lerp(guiTexture.color, Color.black, fadeSpeed * Time.deltaTime);
	}
	void FadeOut() {
		// make sure the texture is enabled
		guiTexture.enabled = true;
		// start fading towards black
		FadeToBlack();
		// if the screen is almost black
		if(guiTexture.color.a >= 0.95f) {
			// stop fading out
		}
	}

} // end PlayerInteractions
