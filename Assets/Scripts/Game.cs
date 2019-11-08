using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
	
    // Start is called before the first frame update
    void Awake()
    {
		QualitySettings.vSyncCount = 0; // VSync must be disabled to allow targetFrameRate
		Application.targetFrameRate = 60;
    }
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			QuitGame();
		}

		if (Input.GetKeyDown(KeyCode.KeypadEnter))
		{
			RestartLevel();
		}
	}

	void RestartLevel()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	void QuitGame()
	{
		UnityEditor.EditorApplication.isPlaying = false;
		//Application.Quit();
	}
	
}
