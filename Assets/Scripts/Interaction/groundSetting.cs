using UnityEngine;

public class GroundSetting : MonoBehaviour
{
    private void Start()
    {
        if (!GameObject.Find("TempPlayer").GetComponent<PlayerController>().EnableGroundCheck)
        {
            GameObject.Find("TempPlayer").GetComponent<PlayerController>().EnableGroundCheck = true;
        }
    }
}