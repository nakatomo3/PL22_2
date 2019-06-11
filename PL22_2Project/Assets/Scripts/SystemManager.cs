using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	/// 敵の出現場所のリスト
	/// </summary>
	List<Vector3> enemyPosition;

	/// <summary>
	/// 敵のタイプ
	/// </summary>
	List<EnemyType> enemyType;

	enum EnemyType {
		NORMAL = 0,
		last
	};

	public GameObject[] enemyObjects;

	private void Awake() {
		instance = this;
	}

	// Start is called before the first frame update
	void Start() {
		enemyObjects = new GameObject[(int)EnemyType.last];
		ReadCSV();
		CreateEnemy();
	}

	// Update is called once per frame
	void Update() {

	}

	private void ReadCSV() {

	}

	private void CreateEnemy() {
		
	}
}