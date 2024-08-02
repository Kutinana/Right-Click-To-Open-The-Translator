using QFramework;
using System.Collections;
using UnityEngine;

public class ElevatorController: MonoBehaviour
{
    SpriteRenderer mask;
    Transform currentFloor;
    private void Awake()
    {
        mask = transform.Find("Mask").GetComponent<SpriteRenderer>();
        currentFloor = transform.Find("F1");

        TypeEventSystem.Global.Register<Puzzle.InCenter.Elevator.Puzzle.MoveTo>(e =>
            MoveToFloor(e.floor)
        ).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void MoveToFloor(int floor)
    {
        init_color = mask.color;
        StartCoroutine(Mask());
        switch (floor)
        {
            case 1:
                if (transform.Find("F1") != currentFloor)
                {
                    currentFloor.gameObject.SetActive(false);
                    transform.Find("F1").gameObject.SetActive(true);
                    currentFloor = transform.Find("F1");
                }
                break;
            case 2:
                if (transform.Find("F2") != currentFloor)
                {
                    currentFloor.gameObject.SetActive(false);
                    transform.Find("F2").gameObject.SetActive(true);
                    currentFloor = transform.Find("F2");
                }
                break;
            case 3:
                if (transform.Find("F3") != currentFloor)
                {
                    currentFloor.gameObject.SetActive(false);
                    transform.Find("F3").gameObject.SetActive(true);
                    currentFloor = transform.Find("F3");
                }
                break;
            default:
                break;
        }
    }

    float parameter = 0f;
    Color init_color;

    private IEnumerator Mask()
    {
        PlayerInput.Instance.DisableInputActions();
        while (parameter < 0.99f)
        {
            mask.color = new Color(init_color.r, init_color.g, init_color.b, Mathf.Lerp(0, 1, parameter));
            parameter += Time.deltaTime * 1.5f;
        }

        parameter = 0f;
        yield return new WaitForSeconds(0.5f);

        while (parameter < 0.99f)
        {
            mask.color = new Color(init_color.r, init_color.g, init_color.b, Mathf.Lerp(1, 0, parameter));
            parameter += Time.deltaTime * 1.5f;
        }
        mask.color = new Color(init_color.r, init_color.g, init_color.b, 0);
        PlayerInput.Instance.EnableInputActions();
        yield return null;
    }
}