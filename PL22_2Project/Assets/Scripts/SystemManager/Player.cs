using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	public static Player player;
	private Transform thisTransform;

	/// <summary>
	/// 通常時、左右移動までに必要な時間
	/// </summary>
	private float normalHorizontalSpeed = 0.3f;

	/// <summary>
	/// 通常時、上下移動までに必要な時間
	/// </summary>
	private float normalVerticalSpeed = 0.3f;


	/// <summary>
	/// ハサミ時、左右移動までに必要な時間
	/// </summary>
	private float cuttingHorizontalSpeed = 1f;

	/// <summary>
	/// ハサミ時、上下移動までに必要な時間
	/// </summary>
	private float cuttingVerticalSpeed = 1f;

	private float moveTimer;
	private const float moveRange = 0.5f;

	private bool isCuttingMode;

	public Slider moveSlider;

	public GameObject line;

	// Start is called before the first frame update
	void Start() {
		thisTransform = transform;
	}

	// Update is called once per frame
	void Update() {
		float speed = 9999999;

		if (Input.GetKey(KeyCode.Space)) {
			isCuttingMode = true;
		} else {
			isCuttingMode = false;
		}

		if (isCuttingMode == false) {
			if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
				moveTimer += Time.deltaTime;
				speed = normalHorizontalSpeed;
				if(moveTimer > normalHorizontalSpeed && SystemManager.RIGHT_MAX >= thisTransform.position.x + moveRange) {
					thisTransform.position += Vector3.right * moveRange;
					moveTimer = 0;
				}
			} else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
				moveTimer += Time.deltaTime;
				speed = normalHorizontalSpeed;
				if(moveTimer > normalHorizontalSpeed && SystemManager.LEFT_MAX <= thisTransform.position.x - moveRange) {
					thisTransform.position += Vector3.left * moveRange;
					moveTimer = 0;
				}
			} else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
				moveTimer += Time.deltaTime;
				speed = normalVerticalSpeed;
				if (moveTimer > normalVerticalSpeed && SystemManager.UP_MAX >= thisTransform.position.z + moveRange) {
					thisTransform.position += Vector3.forward * moveRange;
					moveTimer = 0;
				}
			} else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
				moveTimer += Time.deltaTime;
				speed = normalVerticalSpeed;
				if (moveTimer > normalVerticalSpeed && SystemManager.DOWN_MAX <= thisTransform.position.z - moveRange) {
					thisTransform.position += Vector3.back * moveRange;
					moveTimer = 0;
				}
			}
		} else {
			if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
				moveTimer += Time.deltaTime;
				speed = cuttingHorizontalSpeed;
				if (moveTimer > cuttingHorizontalSpeed && SystemManager.RIGHT_MAX >= thisTransform.position.x + moveRange) {
					var newLine = Instantiate(line, new Vector3(thisTransform.position.x + moveRange / 2, -0.5f, thisTransform.position.z), Quaternion.identity, SystemManager.instance.transform.GetChild(0));
					newLine.transform.localScale = new Vector3(moveRange,0.05f,0.05f);
					thisTransform.position += Vector3.right * moveRange;
					moveTimer = 0;
				}
			} else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
				moveTimer += Time.deltaTime;
				speed = cuttingHorizontalSpeed;
				if (moveTimer > cuttingHorizontalSpeed && SystemManager.LEFT_MAX <= thisTransform.position.x - moveRange) {
					var newLine = Instantiate(line, new Vector3(thisTransform.position.x - moveRange / 2, -0.5f, thisTransform.position.z), Quaternion.identity, SystemManager.instance.transform.GetChild(0));
					newLine.transform.localScale = new Vector3(moveRange, 0.05f, 0.05f);
					thisTransform.position += Vector3.left * moveRange;
					moveTimer = 0;
				}
			} else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
				moveTimer += Time.deltaTime;
				speed = cuttingVerticalSpeed;
				if (moveTimer > cuttingVerticalSpeed && SystemManager.UP_MAX >= thisTransform.position.z + moveRange) {
					var newLine = Instantiate(line, new Vector3(thisTransform.position.x, -0.5f, thisTransform.position.z + moveRange / 2), Quaternion.identity, SystemManager.instance.transform.GetChild(0));
					newLine.transform.localScale = new Vector3(moveRange, 0.05f, 0.05f);
					newLine.transform.Rotate(new Vector3(0, 90, 0));
					thisTransform.position += Vector3.forward * moveRange;
					moveTimer = 0;
				}
			} else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
				moveTimer += Time.deltaTime;
				speed = cuttingVerticalSpeed;
				if (moveTimer > cuttingVerticalSpeed && SystemManager.DOWN_MAX <= thisTransform.position.z - moveRange) {
					var newLine = Instantiate(line, new Vector3(thisTransform.position.x, -0.5f, thisTransform.position.z - moveRange / 2), Quaternion.identity, SystemManager.instance.transform.GetChild(0));
					newLine.transform.localScale = new Vector3(moveRange, 0.05f, 0.05f);
					newLine.transform.Rotate(new Vector3(0, 90, 0));
					thisTransform.position += Vector3.back * moveRange;
					moveTimer = 0;
				}
			}
		}

		if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
			moveTimer = 0;
		}

		moveSlider.value = moveTimer / speed;
	}
}
