using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAIMain : MonoBehaviour {
	public GameObject pip;
	public GameObject testBird;

	public List<GameObject> pipes = new List<GameObject>();

	private int totalPopulation = 30;
	// All active birds (not yet collided with pipe)
	public List<GameObject> activeBirds = new List<GameObject>();
	// All birds for any given population
	public List<GameObject> allBirds = new List<GameObject>();


	public GameObject bestBird;

	// All time high score
	private int highScore = 0;

	// Training or just showing the current best
	private bool runBest = false;

	private int counter;


	// Use this for initialization
	void Start () {
		//Debug.Log (ScreenUtils.GetWorldScreenSize().x+":"+ScreenUtils.GetWorldScreenSize().y);
		pip.SetActive(false);
		testBird.SetActive (false);

		for (int i = 0; i < totalPopulation; i++) {
			GameObject b = Clone (testBird);
			activeBirds.Add (b);
			allBirds.Add (b);
			b.GetComponent<Bird> ().Init (null);
		}

		//testBird.GetComponent<Bird> ().Init (null);
	}

	// Update is called once per frame
	void Update () {
		int cycles = 1;

		for (int n = 0; n < cycles; n++) {
			// Show all the pipes
			for (int i = pipes.Count - 1; i >= 0; i--) {
				Pipe p = pipes [i].GetComponent<Pipe> ();
				p.Move ();
				if (p.OffScreen ()) {
					Destroy (pipes [i]);
					pipes [i] = null;
					pipes.RemoveAt(i);
				}
			}
		}

		// Are we just running the best bird
		if (runBest) {
			bestBird.GetComponent<Bird> ().think (pipes);
			bestBird.GetComponent<Bird> ().move ();
			for (var j = 0; j < pipes.Count; j++) {
				// Start over, bird hit pipe
				Pipe p = pipes [j].GetComponent<Pipe> ();
				if (p.Hits(bestBird.GetComponent<Bird> ())) {
					//resetGame();
					return;
				}
			}
			// Or are we running all the active birds
		} else {
			for (int i = activeBirds.Count - 1; i >= 0; i--) {
				Bird bird = activeBirds[i].GetComponent<Bird>();
				// Bird uses its brain!
				bird.think(pipes);
				bird.move();
				// Check all the pipes
				for (var j = 0; j < pipes.Count; j++) {
					// It's hit a pipe
					Pipe p = pipes [j].GetComponent<Pipe> ();
					if (p.Hits(activeBirds[i].GetComponent<Bird>())) {
						// Remove this bird
						activeBirds[i].SetActive (false);
						activeBirds.RemoveAt(i);

						break;
					}
				}
			}
		}

		if (counter % 50 == 0) {
			pipes.Add(Clone(pip));
		}
		counter++;

		// What is highest score of the current population
		int tempHighScore = 0;
		// If we're training
		if (!runBest) {
			// Which is the best bird?
			GameObject tempBestBird = null;
			for (var i = 0; i < activeBirds.Count; i++) {
				int s = activeBirds[i].GetComponent<Bird>().score;
				if (s > tempHighScore) {
					tempHighScore = s;
					tempBestBird = activeBirds[i];
				}
			}

			// Is it the all time high scorer?
			if (tempHighScore > highScore) {
				highScore = tempHighScore;
				bestBird = tempBestBird;
			}
		} else {
			// Just one bird, the best one so far
			tempHighScore = bestBird.GetComponent<Bird>().score;
			if (tempHighScore > highScore) {
				highScore = tempHighScore;
			}
		}
			
		if (runBest) {
			bestBird.SetActive (true);
		} else {
			for (var i = 0; i < activeBirds.Count; i++) {
				activeBirds[i].SetActive (true);
			}
			// If we're out of birds go to the next generation
			if (activeBirds.Count == 0) {
				nextGeneration();
			}
		}


		/* // debug
		testBird.GetComponent<Bird> ().move ();

		if (Input.GetMouseButton (0)) {
			testBird.GetComponent<Bird> ().up ();
		}
		*/
	}

	public void resetGame() {
		counter = 0;
		for (int i = pipes.Count - 1; i >= 0; i--) {
			Destroy (pipes [i]);
			pipes [i] = null;
			pipes.RemoveAt(i);
		}
		pipes = new List<GameObject> ();
	}

	public void nextGeneration() {
		resetGame ();
		// Normalize the fitness values 0-1
		normalizeFitness(allBirds);
		// Generate a new set of birds
		activeBirds = generate(allBirds);
		// Copy those birds to another array
		allBirds = new List<GameObject>();
		for(int i = 0; i<activeBirds.Count; i++){
			activeBirds [i].SetActive (true);
			allBirds.Add (activeBirds[i]);
		}
	}

	public List<GameObject> generate(List<GameObject> oldBirds) {
		List<Bird> newBirds = new List<Bird>();
		List<GameObject> newBirds2 = new List<GameObject>();
		for (var i = 0; i < oldBirds.Count; i++) {
			// Select a bird based on fitness
			Bird bird = poolSelection(oldBirds);
			newBirds.Add (bird);
		}

		for (int i = allBirds.Count - 1; i >= 0; i--) {
			Destroy (allBirds [i]);
			allBirds [i] = null;
			allBirds.RemoveAt(i);
		}

		for (int i = 0; i < newBirds.Count; i++) {
			GameObject b = Clone (testBird);
			b.GetComponent<Bird> ().Init (newBirds[i].brain);
			newBirds2.Add (b);
		}

		return newBirds2;
	}

	public void normalizeFitness(List<GameObject> birds) {
		// Make score exponentially better?
		for (int i = 0; i < birds.Count; i++) {
			Bird b = birds [i].GetComponent<Bird> ();
			b.score = (int)Mathf.Pow(b.score, 2);
		}

		// Add up all the scores
		int sum = 0;
		for (int i = 0; i < birds.Count; i++) {
			Bird b = birds [i].GetComponent<Bird> ();
			sum += b.score;
		}

		// Divide by the sum
		for (int i = 0; i < birds.Count; i++) {
			Bird b = birds [i].GetComponent<Bird> ();
			b.fitness = (float)b.score / (float)sum;
		}
	}

	// An algorithm for picking one bird from an array
	// based on fitness
	public Bird poolSelection(List<GameObject> birds) {
		// Start at 0
		int index = 0;

		// Pick a random number between 0 and 1
		float r = Random.Range(0f,1f);

		// Keep subtracting probabilities until you get less than zero
		// Higher probabilities will be more likely to be fixed since they will
		// subtract a larger number towards zero
		while (r > 0f) {
			if(index<birds.Count){
				Bird b = birds [index].GetComponent<Bird> ();
				r -= b.fitness;
				// And move on to the next
			}

			if (index < birds.Count) {
				index += 1;
			} else {
				r = 0f;
			}
		}

		// Go back one
		index -= 1;

		// Make sure it's a copy!
		// (this includes mutation)
		Bird b2 = birds [index].GetComponent<Bird> ();
		return b2.copy();
	}

	public GameObject Clone( GameObject go ) {
		var clone = GameObject.Instantiate( go ) as GameObject;
		clone.transform.parent = go.transform.parent;
		clone.SetActive(true);
		return clone;
	}
}

/*
function resetGame() {
  counter = 0;
  pipes = [];
}

// Create the next generation
function nextGeneration() {
  resetGame();
  // Normalize the fitness values 0-1
  normalizeFitness(allBirds);
  // Generate a new set of birds
  activeBirds = generate(allBirds);
  // Copy those birds to another array
  allBirds = activeBirds.slice();
}

// Generate a new population of birds
function generate(oldBirds) {
  var newBirds = [];
  for (var i = 0; i < oldBirds.length; i++) {
    // Select a bird based on fitness
    var bird = poolSelection(oldBirds);
    newBirds[i] = bird;
  }
  return newBirds;
}

// Normalize the fitness of all birds
function normalizeFitness(birds) {
  // Make score exponentially better?
  for (var i = 0; i < birds.length; i++) {
    birds[i].score = pow(birds[i].score, 2);
  }

  // Add up all the scores
  var sum = 0;
  for (var i = 0; i < birds.length; i++) {
    sum += birds[i].score;
  }
  // Divide by the sum
  for (var i = 0; i < birds.length; i++) {
    birds[i].fitness = birds[i].score / sum;
  }
}


// An algorithm for picking one bird from an array
// based on fitness
function poolSelection(birds) {
  // Start at 0
  var index = 0;

  // Pick a random number between 0 and 1
  var r = random(1);

  // Keep subtracting probabilities until you get less than zero
  // Higher probabilities will be more likely to be fixed since they will
  // subtract a larger number towards zero
  while (r > 0) {
    r -= birds[index].fitness;
    // And move on to the next
    index += 1;
  }

  // Go back one
  index -= 1;

  // Make sure it's a copy!
  // (this includes mutation)
  return birds[index].copy();
}

*/