using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// original code from Daniel Shiffman https://github.com/CodingTrain/Toy-Neural-Network-JS
public class NNMatrix {
	public int rows;
	public int cols;
	public List<List<float>> data;

	public NNMatrix(int _rows, int _cols) {
		this.rows = _rows;
		this.cols = _cols;
		this.data = new List<List<float>>();

		for(int i = 0; i<rows; i++) {
			this.data.Add (new List<float> ());
			for(var j = 0; j<this.cols; j++) {
				this.data[i].Add(0f);
			}
		}
	}

	public static NNMatrix fromList(List<float> list){
		NNMatrix m = new NNMatrix(list.Count,1);
		for(int i = 0; i<list.Count; i++) {
			m.data[i][0] = list[i];
		}
		return m;
	}

	public static NNMatrix subtract(NNMatrix a, NNMatrix b) {
		NNMatrix result = new NNMatrix(a.rows,a.cols);
		for(int i = 0; i<result.rows; i++) {
			for(int j = 0; j<result.cols; j++) {
				result.data[i][j] = a.data[i][j] - b.data[i][j];
			}
		}
		return result;
	}

	public List<float> toList() {
		List<float> list = new List<float>();
		for(int i = 0; i<this.rows; i++) {
			for(int j = 0; j<this.cols; j++) {
				list.Add(this.data[i][j]);
			}
		}
		return list;
	}

	public void randomize() {
		for(int i = 0; i<this.rows; i++) {
			for(int j = 0; j<this.cols; j++) {
				this.data [i] [j] = Random.Range (-1f, 1f);
			}
		}
	}

	public void add<Type>(Type n) {
		if(n is NNMatrix) {
			for(int i = 0; i<this.rows; i++) {
				for(int j = 0; j<this.cols; j++) {
					NNMatrix mat = (NNMatrix)(object)n;
					this.data[i][j] += mat.data[i][j];
				}
			}
		} else {
			for(int i = 0; i<this.rows; i++) {
				for(int j = 0; j<this.cols; j++) {
					float val = (float)(object)n;
					this.data[i][j] += val;
				}
			}
		}
	}

	public static NNMatrix transpose(NNMatrix matrix) {
		NNMatrix result = new NNMatrix(matrix.cols,matrix.rows);

		for(int i = 0; i<matrix.rows; i++) {
			for(int j = 0; j<matrix.cols; j++) {
				result.data[j][i] = matrix.data[i][j];
			}
		}
		return result;
	}

	public static NNMatrix multiply(NNMatrix a, NNMatrix b) {
		if(a.cols != b.rows){
			Debug.Log("Columns of A must match rows of B.");
			return null;
		}

		NNMatrix result = new NNMatrix(a.rows,b.cols);

		for(int i = 0; i<result.rows; i++) {
			for(int j = 0; j<result.cols; j++) {

				float sum = 0f;
				for(int k = 0; k < a.cols; k++){
					sum += a.data[i][k]*b.data[k][j];
				}

				result.data[i][j] = sum;
			}
		}

		return result;
	}


	public void multiply<Type>(Type n) {
		if(n is NNMatrix) {
			for(int i = 0; i<this.rows; i++) {
				for(int j = 0; j<this.cols; j++) {
					NNMatrix mat = (NNMatrix)(object)n;
					this.data[i][j] *= mat.data[i][j];
				}
			}
		} else {
			for(int i = 0; i<this.rows; i++) {
				for(int j = 0; j<this.cols; j++) {
					float val = (float)(object)n;
					this.data[i][j] *= val;
				}
			}
		}
	}

	public void map(System.Func<float,float> fn) {
		for(int i = 0; i<this.rows; i++) {
			for(int j = 0; j<this.cols; j++) {
				float val = this.data[i][j];
				this.data[i][j] = fn(val);
			}
		}
	}

	public static NNMatrix map(NNMatrix matrix, System.Func<float,float> func) {
		NNMatrix result = new NNMatrix(matrix.rows,matrix.cols);
		for(int i = 0; i<matrix.rows; i++) {
			for(int j = 0; j<matrix.cols; j++) {
				float val = matrix.data[i][j];
				result.data[i][j] = func(val);
			}
		}

		return result;
	}

}