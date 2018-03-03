using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour {

	public GameObject topPipe;
	public GameObject btmPipe;

	public GameObject t;
	public GameObject b;

	public float top;
	public float bottom;
	public float x;
	public float w;
	public float speed;

	// Use this for initialization
	void Start () {
		float diffY = ScreenUtils.GetWorldScreenSize ().y / 2f;

		// How big is the empty space
		float spacing = Random.Range (3f, ScreenUtils.GetWorldScreenSize ().y / 2f);

		// Where is the center of the empty space
		float centery = Random.Range(spacing,ScreenUtils.GetWorldScreenSize ().y-spacing);

		this.top = centery - spacing / 2;
		this.bottom = ScreenUtils.GetWorldScreenSize ().y - (centery + spacing / 2);

		this.x = ScreenUtils.GetWorldScreenSize ().x / 2f;

		this.w = 1f;
		this.speed = 0.16f;

		Vector3 topPos = topPipe.transform.localPosition;
		Vector3 btmPos = btmPipe.transform.localPosition;
		Vector3 topSc = topPipe.transform.localScale;
		Vector3 btmSc = btmPipe.transform.localScale;

		topPos.y = diffY-(this.top/2f);
		topSc.y = this.top;

		btmPos.y = -diffY+(this.bottom/2f);
		btmSc.y = this.bottom;

		topPipe.transform.localPosition = topPos;
		topPipe.transform.localScale = topSc;

		btmPipe.transform.localPosition = btmPos;
		btmPipe.transform.localScale = btmSc;

		Vector3 pos = gameObject.transform.localPosition;
		pos.y = 0f;
		pos.z = 0f;
		gameObject.transform.localPosition = pos;

		UpdatePosition ();

		/* make sure top and bottom position
		topPos.x = 0.6f;
		topPos.y = diffY-this.top;
		btmPos.x = 0.6f;
		btmPos.y = -diffY+this.bottom;
		t.transform.localPosition = topPos;
		b.transform.localPosition = btmPos;
		*/
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Move() {
		this.x -= this.speed;
		UpdatePosition ();
	}

	private void UpdatePosition() {
		Vector3 pos = gameObject.transform.localPosition;
		pos.x = this.x;
		gameObject.transform.localPosition = pos;
	}

	public bool Hits(Bird bird) {
		float diffY = ScreenUtils.GetWorldScreenSize ().y / 2f;

		float top = (diffY - (this.top));

		if ((bird.y + (bird.r/2f)) > top || (bird.y - (bird.r/2f)) < -diffY+(this.bottom)) {
			if (bird.x > this.x && bird.x < this.x + (this.w/2f)) {
				return true;
			}
		}
		return false;
	}

	public bool OffScreen() {
		if (this.x < -this.w-(ScreenUtils.GetWorldScreenSize ().x/2f)) {
			return true;
		} else {
			return false;
		}
	}
}

/*
function Pipe() {

  // How big is the empty space
  var spacing = random(120, height / 2);
  // Where is th center of the empty space
  var centery = random(spacing, height - spacing);

  // Top and bottom of pipe
  this.top = centery - spacing / 2;
  this.bottom = height - (centery + spacing / 2);
  // Starts at the edge
  this.x = width;
  // Width of pipe
  this.w = 50;
  // How fast
  this.speed = 4;

  // Did this pipe hit a bird?
  this.hits = function(bird) {
    if ((bird.y - bird.r) < this.top || (bird.y + bird.r) > (height - this.bottom)) {
      if (bird.x > this.x && bird.x < this.x + this.w) {
        return true;
      }
    }
    return false;
  }

  // Draw the pipe
  this.show = function() {
    stroke(255);
    fill(200);
    rect(this.x, 0, this.w, this.top);
    rect(this.x, height - this.bottom, this.w, this.bottom);
  }

  // Update the pipe
  this.update = function() {
    this.x -= this.speed;
  }

  // Has it moved offscreen?
  this.offscreen = function() {
    if (this.x < -this.w) {
      return true;
    } else {
      return false;
    }
  }
}
*/
