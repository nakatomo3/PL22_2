using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SystemManager : MonoBehaviour {

	public static SystemManager instance;

	public static float LEFT_MAX = -5;
	public static float RIGHT_MAX = 5;
	public static float UP_MAX = 5;
	public static float DOWN_MAX = -5;

	/// <summary>
	/// 敵の出現時間のリスト
	/// </summary>
	List<float> enemyTimer;

	/// <summary>
	/// 敵の出現場所(x軸のみ)のリスト
	/// </summary>
	List<float> enemyPosX;

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

	private void Awake() {
		instance = this;
	}

	// Start is called before the first frame update
	void Start() {
		enemyTimer = new List<float>();
		enemyPosX = new List<float>();
		enemyType = new List<EnemyType>();

		if(ReadCSV("CSVs/"+PlayerPrefs.GetInt("stageNum",0)) == false) {
			StopUnity();
		}

		if(enemyTimer.Count <= 0) {
			Debug.LogError("登録されているEnemyの数が0です");
			StopUnity();
		}
	}

	// Update is called once per frame
	void Update() {
		gameTimer += Time.deltaTime;
		CreateEnemy();

	}

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
				enemyType.Add((EnemyType)int.Parse(values[2]));
			}
		} catch {
			Debug.LogError("CSVの読み込みに失敗しました。\nよくあるミス：,を行末に入れている、必要な情報を入れていない、一行目を消している");
			return false;
		}
		return true;
	}

	private void CreateEnemy() {
		if(enemyCreateCounter < enemyTimer.Count && enemyTimer[enemyCreateCounter] < gameTimer) {
			Instantiate(enemyObjects[(int)enemyType[enemyCreateCounter]], new Vector3(enemyPosX[enemyCreateCounter], 0, UP_MAX - 2), Quaternion.identity, transform);
			enemyCreateCounter++;
			enemyCounter++;
		}
	}

	private void StopUnity() {
		#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
		#endif
	}
}