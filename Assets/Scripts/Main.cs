using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// original code from Daniel Shiffman https://github.com/CodingTrain/Toy-Neural-Network-JS
public class NNTrainData {
	public List<float>inputs = new List<float>();
	public List<float>targets = new List<float>();
}

public class Main : MonoBehaviour {
	public List<NNTrainData> training_data = new List<NNTrainData>();

	public RenderTexture renderTexture;
	public RawImage img;
	Texture2D texture;

	public List<float> testInputs = new List<float>();

	private NeuralNetwork nn;

	// Use this for initialization
	void Start () {
		texture = new Texture2D (renderTexture.width, renderTexture.height);
		img.material.mainTexture =  texture;

		testInputs.Add (0f);
		testInputs.Add (0f);

		// XOR data
		training_data.Add (new NNTrainData ());
		training_data [0].inputs.Add(0);
		training_data [0].inputs.Add(1);
		training_data [0].targets.Add(1);

		training_data.Add (new NNTrainData ());
		training_data [1].inputs.Add(1);
		training_data [1].inputs.Add(0);
		training_data [1].targets.Add(1);

		training_data.Add (new NNTrainData ());
		training_data [2].inputs.Add(0);
		training_data [2].inputs.Add(0);
		training_data [2].targets.Add(0);

		training_data.Add (new NNTrainData ());
		training_data [3].inputs.Add(1);
		training_data [3].inputs.Add(1);
		training_data [3].targets.Add(0);


		nn = new NeuralNetwork(2,4,1);

		/*
		NeuralNetwork nn = new NeuralNetwork(2,2,1);
		for(var i = 0; i<10000; i++){
			int index = (int)Random.Range (0f, training_data.Count)>>0;
			NNTrainData data = training_data[index];
			nn.train(data.inputs,data.targets);
		}

		List<float> o0 = nn.feedforward (training_data [0].inputs);
		List<float> o1 = nn.feedforward (training_data [1].inputs);
		List<float> o2 = nn.feedforward (training_data [2].inputs);
		List<float> o3 = nn.feedforward (training_data [3].inputs);
		Debug.Log (o0[0]);
		Debug.Log (o1[0]);
		Debug.Log (o2[0]);
		Debug.Log (o3[0]);
		*/
	}
	
	// Update is called once per frame
	void Update () {

		for(var i = 0; i<100; i++){
			int index = (int)Random.Range (0f, training_data.Count)>>0;
			NNTrainData data = training_data[index];
			nn.train(data.inputs,data.targets);
		}


		RenderTexture.active = renderTexture; 

		int resolution = 10;
		int cols = renderTexture.width;
		int rows = renderTexture.height;

		texture.ReadPixels (new Rect (0, 0, renderTexture.width, renderTexture.height), 0, 0);
		for (int i = 0; i < cols; i++) {
			for (int j = 0; j < rows; j++) {

				float x1 = ((float)i/(float)cols);//return 0-1.0
				float x2 = ((float)j/(float)rows);//return 0-1.0
				testInputs[0] = x1;
				testInputs[1] = x2;



				List<float> y = nn.feedforward(testInputs);

				texture.SetPixel (i,j, new Color (y[0], y[0], y[0]));
			}
		}
		texture.Apply (); 
		RenderTexture.active = null; 
	}
}
