using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// original code from Daniel Shiffman https://github.com/CodingTrain/Toy-Neural-Network-JS

public class NNUtils {
	public static System.Func<float,float> mutate = (x) => {
		if (Random.Range (0f, 1.0f) < 0.1f) {
			float offset = Random.Range (-0.1f, 0.1f);
			float newx = x + offset;
			return newx;
		}
		return x;
	};
}
