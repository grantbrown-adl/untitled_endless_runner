using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Controller))]
public class InputHandler : MonoBehaviour {
    [SerializeField] Controller _playerController;
    [SerializeField] Vector2 _controllerInput;

    private void Awake() {
        _playerController = GetComponent<Controller>();
    }

    private void Update() {
        _controllerInput = Vector2.zero;

        _controllerInput.x = Input.GetAxis("Horizontal");
        _controllerInput.y = Input.GetAxis("Vertical");

        _playerController.SetInput(_controllerInput);


        if (Input.GetKeyDown(KeyCode.R)) {
            // Restart current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
