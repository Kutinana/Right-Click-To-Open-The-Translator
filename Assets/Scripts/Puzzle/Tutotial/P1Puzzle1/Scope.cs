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
                transform.position = new Vector3(transform.position.x, transform.position.y + offset, transform.position.z);
            }
            currentMousePosY = TranslatorCameraManager.Camera.ScreenToWorldPoint(Input.mousePosition).y;
            
        }
        private void OnMouseUp()
        {
            if (holding)
            {
                Puzzle.UpdateScopeState(id, transform.localPosition.y);
            }
            holding = false;
        }

        public void ScopeFinish()
        {
            transform.GetComponent<Collider2D>().enabled = false;
            init_pos = transform.localPosition;
            target_pos = new Vector3(init_pos.x, Puzzle.CorrectPosY[id], init_pos.z);
            StartCoroutine(MoveToCorrectPos());
        }

        private float parameter = 0f;

        private IEnumerator MoveToCorrectPos()
        {
            while (parameter < 0.99f)
            {
                transform.localPosition = Vector3.Lerp(init_pos, target_pos, parameter);
                parameter += Time.deltaTime * 0.3f;
            }
            Puzzle.UpdateScopeStateWithoutCheck(id, Puzzle.CorrectPosY[id]);

            yield return null;
        }
    }
}
