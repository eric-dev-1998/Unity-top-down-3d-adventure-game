using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warp : MonoBehaviour
{
    public enum direction { Up, Down, Left, Right };
    public direction dir;

    public PlayerAnimations playerAnim;

    public Transform targetScenePosition;

    public GameObject currentScene;
    public GameObject nextScene;

    bool isPlayerOnTrigger = false;

    private void Update()
    {
        checkCollision();
    }

    void checkCollision()
    {
        if (isPlayerOnTrigger)
        {
            float currentPlayerFacingDirection = playerAnim.anim.GetFloat("direction");

            switch (dir)
            {
                case direction.Up:
                    if (currentPlayerFacingDirection == 0)
                        if (Input.GetKeyDown(KeyCode.Space))
                            warp();
                    break;

                case direction.Down:
                    if (currentPlayerFacingDirection == 1)
                        if (Input.GetKeyDown(KeyCode.Space))
                            warp();
                    break;

                case direction.Left:
                    if (currentPlayerFacingDirection == 2)
                        if (Input.GetKeyDown(KeyCode.Space))
                            warp();
                    break;

                case direction.Right:
                    if (currentPlayerFacingDirection == 3)
                        if (Input.GetKeyDown(KeyCode.Space))
                            warp();
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            isPlayerOnTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isPlayerOnTrigger = false;
        }
    }

    void warp()
    {
        playerAnim.gameObject.transform.position = targetScenePosition.position;
        Camera.main.transform.position = new Vector3(targetScenePosition.position.x, Camera.main.transform.position.y, targetScenePosition.transform.position.z);

        currentScene.SetActive(false);
        nextScene.SetActive(true);

        nextScene.GetComponent<SceneProperties>().applySettings();

        isPlayerOnTrigger = false;
    }
}
