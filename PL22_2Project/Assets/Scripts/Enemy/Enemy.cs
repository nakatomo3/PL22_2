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

	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {
		
	}

	private void OnCollisionStay(Collision collision) {
		if(collision.gameObject.GetComponent<Rigidbody>().useGravity == true) {

		}
	}
}