using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;

public class SystemManager : MonoBehaviour {

	public static SystemManager instance;

	public static float LEFT_MAX = -5;
	public static float RIGHT_MAX = 5;
	public static float UP_MAX = 5;
	public static float DOWN_MAX = -5;

	public int score;
	public Text scoreText;

	public GameObject stageSquare;
	public List<List<Rigidbody>> squareRigidbody;

	/// <summary>
	/// 敵の出現時間のリスト
	/// </summary>
	List<float> enemyTimer;

	/// <summary>
	/// 敵の出現場所(x軸のみ)のリスト
	/// </summary>
	List<float> enemyPosX;
	List<float> enemyPosY;

	/// <summary>
	/// 敵のタイプ
	/// </summary>
	List<EnemyType> enemyType;

	enum EnemyType {
		NORMAL = 0,
		last
	};

	public GameObject[] enemyObjects;

	private int enemyCreateCounter = 0;
	private int enemyCounter = 0;

	private float gameTimer = 0;

	private List<List<bool>> isHorizontalLine;
	private List<List<bool>> isVerticalLine;

	private List<List<bool>> isHole;

	[HideInInspector]
	public int HP = 3;
	public Text hpText;

	private void Awake() {
		instance = this;
	}

	void Start() {
		enemyTimer = new List<float>();
		enemyPosX = new List<float>();
		enemyPosY = new List<float>();
		enemyType = new List<EnemyType>();

		isHorizontalLine = new List<List<bool>>();
		isVerticalLine = new List<List<bool>>();
		isHole = new List<List<bool>>();

		squareRigidbody = new List<List<Rigidbody>>();

		if(ReadCSV("CSVs/"+PlayerPrefs.GetInt("stageNum",0)) == false) {
			StopUnity();
		}

		if(enemyTimer.Count <= 0) {
			Debug.LogError("登録されているEnemyの数が0です");
			StopUnity();
		}

		for(int i = 0; i <= (RIGHT_MAX - LEFT_MAX) /Player.moveRange; i++) {
			isHorizontalLine.Add(new List<bool>());
			isVerticalLine.Add(new List<bool>());
			isHole.Add(new List<bool>());
			for (int j = 0; j <= (UP_MAX - DOWN_MAX) / Player.moveRange; j++) {
				isHorizontalLine[i].Add(false);
				isVerticalLine[i].Add(false);
				isHole[i].Add(false);
			}
			isVerticalLine[i].Add(false);
		}

		CreateStage();
	}

	// Update is called once per frame
	void Update() {
		gameTimer += Time.deltaTime;
		CreateEnemy();

		scoreText.text = "Score:" + score.ToString();
		hpText.text = "HP:"+HP.ToString();
	}

	/// <summary>
	/// CSVを読み込んで、敵の情報を格納していく
	/// </summary>
	/// <param name="FileName">ファイルの名前</param>
	/// <returns></returns>
	private bool ReadCSV(string FileName) {
		var csv = Resources.Load(FileName) as TextAsset;
		if(csv == null) {
			Debug.LogError(FileName + "は存在しません。Resources/CSVsの中にあるか確認してください");
			return false;
		}
		try {
			var csvReader = new StringReader(csv.text);
			csvReader.ReadLine();
			while (csvReader.Peek() > -1) {
				var values = csvReader.ReadLine().Split(',');
				enemyTimer.Add(float.Parse(values[0]));
				enemyPosX.Add(float.Parse(values[1]));
				enemyPosY.Add(float.Parse(values[2]));
				enemyType.Add((EnemyType)int.Parse(values[3]));
			}
		} catch {
			Debug.LogError("CSVの読み込みに失敗しました。\nよくあるミス：,を行末に入れている、必要な情報を入れていない、一行目を消している");
			return false;
		}
		return true;
	}

	/// <summary>
	/// 現在の行を確認して、敵を生成する。
	/// </summary>
	private void CreateEnemy() {
		if(enemyCreateCounter < enemyTimer.Count && enemyTimer[enemyCreateCounter] < gameTimer) {
			Instantiate(enemyObjects[(int)enemyType[enemyCreateCounter]], new Vector3(enemyPosX[enemyCreateCounter], 0, enemyPosY[enemyCreateCounter]), Quaternion.identity, transform);
			enemyCreateCounter++;
			enemyCounter++;
		}
	}

	/// <summary>
	/// UnityEditorを終了させる
	/// </summary>
	private void StopUnity() {
		#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
		#endif
	}

	/// <summary>
	/// Lineデータを変更する
	/// </summary>
	/// <param name="x"></param>
	/// <param name="z"></param>
	public void SetLine(float x, float z) {
		int X, Y = 0;
		bool isHorizontal;
		if (x % Player.moveRange == 0) {
			isHorizontal = false;
		} else {
			isHorizontal = true;
		}

		if (isHorizontal == true) {
			X = Mathf.RoundToInt((x - LEFT_MAX - Player.moveRange / 2) / Player.moveRange);
			Y = Mathf.RoundToInt((z - DOWN_MAX) / Player.moveRange);
			isHorizontalLine[X][Y] = true;
		} else {
			X = Mathf.RoundToInt((x - LEFT_MAX) / Player.moveRange);
			Y = Mathf.RoundToInt((z - DOWN_MAX - Player.moveRange / 2) / Player.moveRange);
			isVerticalLine[X][Y] = true;
		}

		//Debug.Log("X:" + X + "Y:" + Y);

		CheckisHole(X, Y, isHorizontal);
	}

	/// <summary>
	/// 穴が成立したかチェックする
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public bool CheckisHole(float x, float y, bool isHorizontal) {
		//線が一周して戻ってきたら穴

		Queue<Vector2> checkQueue = new Queue<Vector2>();
		List<List<bool>> isConnect = new List<List<bool>>();
		List<List<Vector2>> parent = new List<List<Vector2>>();
		for (int i = 0; i < isVerticalLine.Count; i++) {
			isConnect.Add(new List<bool>());
			parent.Add(new List<Vector2>());
			for (int j = 0; j < isVerticalLine.Count; j++) {
				isConnect[i].Add(false);
				parent[i].Add(new Vector2(-1, -1));
			}
		}

		int count = 0;
		checkQueue.Enqueue(new Vector2(x, y));

		bool isContinue = true;
		while (isContinue == true) {
			//広げてキューリストに追加する
			Vector2 dequeue = checkQueue.Dequeue();

			//Debug.Log("left" + (dequeue == parent[(int)dequeue.x - 1][(int)dequeue.y]));
			//Debug.Log("right" + (dequeue == parent[(int)dequeue.x + 1][(int)dequeue.y]));
			//Debug.Log("up" + (dequeue == parent[(int)dequeue.x][(int)dequeue.y + 1]));
			//Debug.Log("down" + (dequeue == parent[(int)dequeue.x][(int)dequeue.y - 1]));

			if (dequeue.x > 0 && isHorizontalLine[(int)dequeue.x - 1][(int)dequeue.y] == true && parent[(int)dequeue.x][(int)dequeue.y] != dequeue + Vector2.left) {
				if (isConnect[(int)dequeue.x - 1][(int)dequeue.y] == true) {
					isContinue = false;
				} else {
					isConnect[(int)dequeue.x - 1][(int)dequeue.y] = true;
					parent[(int)dequeue.x - 1][(int)dequeue.y] = dequeue;
					checkQueue.Enqueue(new Vector2(dequeue.x - 1, dequeue.y));
				}
			}

			if (dequeue.x < (RIGHT_MAX - LEFT_MAX) / Player.moveRange && isHorizontalLine[(int)dequeue.x][(int)dequeue.y] == true && parent[(int)dequeue.x][(int)dequeue.y] != dequeue + Vector2.right) {
				if (isConnect[(int)dequeue.x + 1][(int)dequeue.y]) {
					isContinue = false;
				} else {
					isConnect[(int)dequeue.x + 1][(int)dequeue.y] = true;
					parent[(int)dequeue.x + 1][(int)dequeue.y] = dequeue;
					checkQueue.Enqueue(new Vector2(dequeue.x + 1, dequeue.y));
				}
			}

			if (dequeue.y > 0 && isVerticalLine[(int)dequeue.x][(int)dequeue.y - 1] == true && parent[(int)dequeue.x][(int)dequeue.y] != dequeue + Vector2.down) {
				if (isConnect[(int)dequeue.x][(int)dequeue.y - 1]) {
					isContinue = false;
				} else {
					isConnect[(int)dequeue.x][(int)dequeue.y - 1] = true;
					parent[(int)dequeue.x][(int)dequeue.y - 1] = dequeue;
					checkQueue.Enqueue(new Vector2(dequeue.x, dequeue.y - 1));
				}
			}
			if(dequeue.y < (UP_MAX - DOWN_MAX) / Player.moveRange && isVerticalLine[(int)dequeue.x][(int)dequeue.y] == true && parent[(int)dequeue.x][(int)dequeue.y] != dequeue + Vector2.up) {
				if (isConnect[(int)dequeue.x][(int)dequeue.y + 1]) {
					isContinue = false;
				} else {
					isConnect[(int)dequeue.x][(int)dequeue.y + 1] = true;
					parent[(int)dequeue.x][(int)dequeue.y + 1] = dequeue;
					checkQueue.Enqueue(new Vector2(dequeue.x, dequeue.y + 1));
				}
			}

			//Debug.Log(count+":"+dequeue+":"+isContinue);
			count++;

			if (checkQueue.Count <= 0 || count >= 1000) {
				//見つからなかった
				return false;
			}

			if (isContinue == false) {
				//Debug.Log("穴状態となっている個所が見つかりました");
				break;
			}
		}

		//isConnectで一番外側の線の内側を穴にして、それと繋がっている部分をすべて穴にする
		//int firstHoleX = -1, firstHoleY = -1;
		//for(int i = 0; i < isHorizontalLine.Count; i++) {
		//	if (firstHoleX != -1) {
		//		break;
		//	}
		//	for (int j = 0; j < isHorizontalLine.Count; j++) {
		//		if(isConnect[i][j] == true) {
		//			firstHoleX = i;
		//			firstHoleY = j;
		//			//squareRigidbody[firstHoleX][firstHoleY-1].useGravity = true;
		//			break;
		//		}
		//	}
		//}
		

		//bool isContinueHole = true;
		//int holeCounter = 0;
		//Queue<Vector2> holeQueue = new Queue<Vector2>();
		//holeQueue.Enqueue(new Vector2(x,y));

		//while (isContinueHole) {
		//	Vector2 dequeue = holeQueue.Dequeue();
		//	squareRigidbody[(int)dequeue.x][(int)dequeue.y].useGravity = true;
		//	if (dequeue.x > 0 && isVerticalLine[(int)dequeue.x][(int)dequeue.y] == false) {
		//		squareRigidbody[(int)dequeue.x - 1][(int)dequeue.y].useGravity = true;
		//		holeQueue.Enqueue(new Vector2(dequeue.x - 1, dequeue.y));
		//	}
		//	if (dequeue.x < isHorizontalLine.Count && isVerticalLine[(int)dequeue.x + 1][(int)dequeue.y] == false) {
		//		squareRigidbody[(int)dequeue.x + 1][(int)dequeue.y].useGravity = true;
		//		holeQueue.Enqueue(new Vector2(dequeue.x + 1, dequeue.y));
		//	}
		//	if (dequeue.y > 0 && isHorizontalLine[(int)dequeue.x][(int)dequeue.y] == false) {
		//		squareRigidbody[(int)dequeue.x][(int)dequeue.y-1].useGravity = true;
		//		holeQueue.Enqueue(new Vector2(dequeue.x, dequeue.y-1));
		//	}
		//	if (dequeue.y < isVerticalLine.Count && isHorizontalLine[(int)dequeue.x][(int)dequeue.y+1] == false) {
		//		squareRigidbody[(int)dequeue.x][(int)dequeue.y+1].useGravity = true;
		//		holeQueue.Enqueue(new Vector2(dequeue.x, dequeue.y+1));
		//	}

		//	holeCounter++;
		//	if(holeCounter > 1000 || holeQueue.Count <= 0) {
		//		break;
		//	}
		//}

		List<List<bool>> isHole = new List<List<bool>>();
		for(int i = 0; i < squareRigidbody.Count; i++) {
			isHole.Add(new List<bool>());
			for(int j = 0; j < squareRigidbody[i].Count; j++) {
				isHole[i].Add(true);
			}
		}

		Queue<Vector2> holeQueue = new Queue<Vector2>();
		holeQueue.Enqueue(new Vector2(squareRigidbody.Count-1, squareRigidbody[0].Count-1));
		int holeCount = 0;
		while (holeCount < 1000) {


			Vector2 dequeue = holeQueue.Dequeue();

			if (dequeue.x > 0 && isVerticalLine[(int)dequeue.x][(int)dequeue.y] == false && isHole[(int)dequeue.x - 1][(int)dequeue.y] == true) {
				isHole[(int)dequeue.x - 1][(int)dequeue.y] = false;
				holeQueue.Enqueue(new Vector2(dequeue.x - 1, dequeue.y));
			}
			if (dequeue.x < isVerticalLine.Count-1 && isVerticalLine[(int)dequeue.x + 1][(int)dequeue.y] == false && isHole[(int)dequeue.x + 1][(int)dequeue.y] == true) {
				isHole[(int)dequeue.x + 1][(int)dequeue.y] = false;
				holeQueue.Enqueue(new Vector2(dequeue.x + 1, dequeue.y));
			}
			if (dequeue.y > 0 && isHorizontalLine[(int)dequeue.x][(int)dequeue.y] == false && isHole[(int)dequeue.x][(int)dequeue.y - 1] == true) {
				isHole[(int)dequeue.x][(int)dequeue.y-1] = false;
				holeQueue.Enqueue(new Vector2(dequeue.x, dequeue.y - 1));
			}
			if (dequeue.y < isVerticalLine.Count-1 && isHorizontalLine[(int)dequeue.x][(int)dequeue.y + 1] == false && isHole[(int)dequeue.x][(int)dequeue.y + 1] == true) {
				isHole[(int)dequeue.x][(int)dequeue.y+1] = false;
				holeQueue.Enqueue(new Vector2(dequeue.x, dequeue.y + 1));
			}
			holeCount++;
			if (holeQueue.Count <= 0) {
				break;
			}
		}
		for(int i = 0; i < isHole.Count; i++) {
			for(int j = 0; j < isHole[i].Count; j++) {
				if(isHole[i][j] == true) {
					squareRigidbody[i][j].useGravity = true;
				}
			}
		}

		return true;
	}

	private bool CheckisGameOver() {
		return false;
	}

	private void CreateStage() {
		for (int i = 0; i < isVerticalLine.Count; i++) {
			squareRigidbody.Add(new List<Rigidbody>());
			for (int j = 0; j < isVerticalLine.Count; j++) {
				var newSquare = Instantiate(stageSquare, new Vector3(i * Player.moveRange + LEFT_MAX + Player.moveRange/2, -0.5f, j * Player.moveRange + DOWN_MAX + Player.moveRange / 2), Quaternion.identity, transform.GetChild(1));
				newSquare.transform.localScale = new Vector3(Player.moveRange,0.01f, Player.moveRange);
				squareRigidbody[i].Add(newSquare.GetComponent<Rigidbody>());
			}
		}
	}
}