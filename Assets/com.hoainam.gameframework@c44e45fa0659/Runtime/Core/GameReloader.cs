
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameReloader : SingletonMonoBehaviour<GameReloader>
{
	public UnityAction OnReload;
	
	public string loadScene;
	public void Reload()
	{
		OnReload?.Invoke();
		
		Destroy(gameObject);
		SceneManager.LoadScene(loadScene);
	}
}