using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenUtils {
	private static Vector2 m_sieze = Vector2.zero;

	public static Vector2 GetWorldScreenSize() {
		float height = Camera.main.orthographicSize * 2.0f;
		float width = height * Screen.width / Screen.height;
		m_sieze.x = width;
		m_sieze.y = height;
		return m_sieze;
	}
}
