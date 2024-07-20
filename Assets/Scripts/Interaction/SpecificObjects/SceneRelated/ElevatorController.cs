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

        TypeEventSystem.Global.Register<Puzzle.InCenter.Elevator.Puzzle.MoveTo>(e =>
            MoveToFloor(e.floor)
        ).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void MoveToFloor(int floor)
    {
        init_color = mask.color;
        StartCoroutine(Mask());
        currentFloor.gameObject.SetActive(false);
        switch (floor)
        {
            case 1:
                transform.Find("F1").gameObject.SetActive(true);
                break;
            case 2:
                transform.Find("F2").gameObject.SetActive(true);
                break;
            case 3:
                transform.Find("F3").gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    float parameter = 0f;
    Color init_color;

    private IEnumerator Mask()
    {
        GameObject.Find("TempPlayer").GetComponent<PlayerInput>().DisableInputActions();
        while (parameter < 0.99f)
        {
            mask.color = new Color(init_color.r, init_color.g, init_color.b, Mathf.Lerp(0, 1, parameter));
            parameter += Time.deltaTime * 2f;
        }

        parameter = 0f;
        yield return new WaitForSeconds(0.5f);

        while (parameter < 0.99f)
        {
            mask.color = new Color(init_color.r, init_color.g, init_color.b, Mathf.Lerp(1, 0, parameter));
            parameter += Time.deltaTime * 2f;
        }
        mask.color = new Color(init_color.r, init_color.g, init_color.b, 0);
        GameObject.Find("TempPlayer").GetComponent<PlayerInput>().EnableInputActions();
        yield return null;
    }
}