using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public struct AStar {
		public XY thisXY;
		public int actualScore;
		public int estimatedScore;
		public XY parentXY;
		public bool canMove;
		public AStarState state;
	}

	public struct XY {
		public int x;
		public int y;
		public XY(int x, int y) {
			this.x = x; this.y = y;
		}
	}

	public enum AStarState {
		None,
		Open,
		Close
	}

	public List<List<AStar>> stage = new List<List<AStar>>();

	private XY targetXY;
	private List<XY> root = new List<XY>();
	private Queue<XY> rootQueue = new Queue<XY>();

	private Queue<XY> openQueue = new Queue<XY>();

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

	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {


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
	}
	
}