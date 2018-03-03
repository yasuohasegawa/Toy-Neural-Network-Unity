using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour {

	public float x;
	public float y;
	public float r;

	public float gravity;
	public float lift;
	public float velocity;

	public NeuralNetwork brain;

	public int score;
	public float fitness;

	// Use this for initialization
	void Start () {
		
	}

	public void Init(NeuralNetwork _brain) {
		// position and size of bird
		this.x = -(ScreenUtils.GetWorldScreenSize ().y / 2f)+1f;
		this.y = ScreenUtils.GetWorldScreenSize ().y / 2f;
		this.r = 0.5f;

		// Gravity, lift and velocity
		this.gravity = 0.4f;
		this.lift = 1.3f;
		this.velocity = 0f;


		// Is this a copy of another Bird or a new one?
		// The Neural Network is the bird's "brain"
		if (_brain is NeuralNetwork) {
			this.brain = _brain.copy();
			this.brain.mutate();
		} else {
			this.brain = new NeuralNetwork(4, 16, 2);
		}

		// Score is how many frames it's been alive
		this.score = 0;

		// Fitness is normalized version of score
		this.fitness = 0f;

		Vector3 pos = gameObject.transform.localPosition;
		pos.x = this.x;
		pos.y = this.y;
		gameObject.transform.localPosition = pos;
	}

	// Update is called once per frame
	void Update () {
		
	}

	public Bird copy() {
		Init (this.brain);
		return this;
	}

	public void think(List<GameObject> pipes) {
		// First find the closest pipe
		Pipe closest = null;
		float record = Mathf.Infinity;
		for (var i = 0; i < pipes.Count; i++) {
			Pipe p = pipes [i].GetComponent<Pipe> ();
			float diff = p.x - this.x;
			if (diff > 0 && diff < record) {
				record = diff;
				closest = pipes[i].GetComponent<Pipe>();
			}
		}

		if (closest != null) {
			// Now create the inputs to the neural network
			List<float> inputs = new List<float>();
			// x position of closest pipe
			float diffY = ScreenUtils.GetWorldScreenSize ().y / 2f;

			float i0 = map(closest.x, this.x, ScreenUtils.GetWorldScreenSize ().x/2f, -1f, 1f);
			inputs.Add (i0);
			// top of closest pipe opening
			float i1 = map(diffY-closest.top, -ScreenUtils.GetWorldScreenSize ().y/2f, ScreenUtils.GetWorldScreenSize ().y/2f, -1f, 1f);
			inputs.Add (i1);
			// bottom of closest pipe opening
			float i2 = map(-diffY+closest.bottom, -ScreenUtils.GetWorldScreenSize ().y/2f, ScreenUtils.GetWorldScreenSize ().y/2f, -1f, 1f);
			inputs.Add (i2);
			// bird's y position
			float i3 = map(this.y, -ScreenUtils.GetWorldScreenSize ().y/2f, ScreenUtils.GetWorldScreenSize ().y, -1f, 1f);
			inputs.Add (i3);
			// Get the outputs from the network
			List<float> action = this.brain.feedforward(inputs);
			// Decide to jump or not!

			if (action[1] > action[0]) {
				this.up();
			}
		}

	}

	public void up() {
		this.velocity += this.lift;
	}

	public void move() {
		this.velocity -= this.gravity;
		this.velocity *= 0.4f;
		this.y += this.velocity;

		// Keep it stuck to top or bottom
		float height  = -ScreenUtils.GetWorldScreenSize ().y/2f;
		if (this.y < height) {
			this.y = height;
			this.velocity = 0f;
		}
		if (this.y > 5f) {
			this.y = 5f;
			this.velocity = 0f;
		}

		Vector3 pos = gameObject.transform.localPosition;
		pos.y = this.y;
		gameObject.transform.localPosition = pos;

		// Every frame it is alive increases the score
		this.score++;
	}


	public float map(float x, float in_min, float in_max, float out_min, float out_max) {
		return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
	}
}


/*
// Daniel Shiffman
// Nature of Code: Intelligence and Learning
// https://github.com/shiffman/NOC-S17-2-Intelligence-Learning

// This flappy bird implementation is adapted from:
// https://youtu.be/cXgA1d_E-jY&

function Bird(brain) {
	// position and size of bird
	this.x = 64;
	this.y = height / 2;
	this.r = 12;

	// Gravity, lift and velocity
	this.gravity = 0.4;
	this.lift = -12;
	this.velocity = 0;

	// Is this a copy of another Bird or a new one?
	// The Neural Network is the bird's "brain"
	if (brain instanceof NeuralNetwork) {
		this.brain = brain.copy();
		this.brain.mutate();
	} else {
		this.brain = new NeuralNetwork(4, 16, 2);
	}

	// Score is how many frames it's been alive
	this.score = 0;
	// Fitness is normalized version of score
	this.fitness = 0;

	// Create a copy of this bird
	this.copy = function() {
		return new Bird(this.brain);
	}

		// Display the bird
		this.show = function() {
		fill(255, 100);
		stroke(255);
		ellipse(this.x, this.y, this.r * 2, this.r * 2);
	}

		// This is the key function now that decides
		// if it should jump or not jump!
		this.think = function(pipes) {
		// First find the closest pipe
		var closest = null;
		var record = Infinity;
		for (var i = 0; i < pipes.length; i++) {
			var diff = pipes[i].x - this.x;
			if (diff > 0 && diff < record) {
				record = diff;
				closest = pipes[i];
			}
		}

		if (closest != null) {
			// Now create the inputs to the neural network
			var inputs = [];
			// x position of closest pipe
			inputs[0] = map(closest.x, this.x, width, -1, 1);
			// top of closest pipe opening
			inputs[1] = map(closest.top, 0, height, -1, 1);
			// bottom of closest pipe opening
			inputs[2] = map(closest.bottom, 0, height, -1, 1);
			// bird's y position
			inputs[3] = map(this.y, 0, height, -1, 1);
			// Get the outputs from the network
			var action = this.brain.feedforward(inputs);
			// Decide to jump or not!
			console.log(action);
			if (action[1] > action[0]) {
				this.up();
			}
		}
	}

		// Jump up
		this.up = function() {
		this.velocity += this.lift;
	}

		// Update bird's position based on velocity, gravity, etc.
		this.update = function() {
		this.velocity += this.gravity;
		this.velocity *= 0.9;
		this.y += this.velocity;

		// Keep it stuck to top or bottom
		if (this.y > height) {
			this.y = height;
			this.velocity = 0;
		}
		if (this.y < 0) {
			this.y = 0;
			this.velocity = 0;
		}

		// Every frame it is alive increases the score
		this.score++;
	}
}

*/