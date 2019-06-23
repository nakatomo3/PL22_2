using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

	private float attackTimer = 0;
	[HideInInspector]
	public static float attackInterval = 20f;

	public Rigidbody rigidbody;

	private enum Direction {
		Up,
		Right,
		Left,
		Down,
		Fall
	}
	private Direction lastDirection = Direction.Right;
	public GameObject Side, Fall, Down;
	private bool isFall = false;
	public Slider slider;

	// Start is called before the first frame update
	void Start() {
	}

	// Update is called once per frame
	void Update() {
		slider.value = attackTimer/attackInterval;

		switch (lastDirection) {
			case Direction.Down:
				Side.SetActive(false);
				Fall.SetActive(false);
				Down.SetActive(true);
				break;
			case Direction.Right:
				Side.SetActive(true);
				Fall.SetActive(false);
				Down.SetActive(false);
				break;
			case Direction.Left:
				Side.SetActive(true);
				Fall.SetActive(false);
				Down.SetActive(false);
				break;
			case Direction.Fall:
				Side.SetActive(false);
				Fall.SetActive(true);
				Down.SetActive(false);
				break;
		}

		RaycastHit hit;
		if (!Physics.Raycast(transform.position,Vector3.down, out hit, 0.9f)) {
			isFall = true;
			rigidbody.useGravity = true;
		}
		if(isFall == true) {
			lastDirection = Direction.Fall;
		}

		if(transform.position.y < -5) {
			SystemManager.instance.score += 1000;
			Destroy(gameObject);
		}

		attackTimer += Time.deltaTime;
		if(attackTimer >= attackInterval) {
			SystemManager.instance.HP--;
			Destroy(gameObject);
		}

		//Move();
	}

}