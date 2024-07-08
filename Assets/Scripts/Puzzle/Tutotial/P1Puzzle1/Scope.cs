using Cameras;
using System.Collections;
using UnityEngine;


namespace Puzzle.Tutorial.P1
{
    public class Scope : MonoBehaviour
    {
        float currentMousePosY;
        float offset;
        Vector3 init_pos;
        Vector3 target_pos;
        [Range(0, 2)]public int id;

        bool holding = false;

        private IEnumerator OnMouseDown()
        {
            holding = true;
            yield return new WaitForFixedUpdate();
        }
        private void OnMouseOver()
        {
            if (holding && Puzzle.enable)
            {
                offset = TranslatorCameraManager.Camera.ScreenToWorldPoint(Input.mousePosition).y - currentMousePosY;
                transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y + offset, -1.2f, 2.3f) , transform.position.z);
            }
            currentMousePosY = TranslatorCameraManager.Camera.ScreenToWorldPoint(Input.mousePosition).y;
            
        }
        private void OnMouseUp()
        {
            if (holding)
            {
                Puzzle.UpdateScopeState(id, transform.position);
            }
            holding = false;
        }
    }
}
