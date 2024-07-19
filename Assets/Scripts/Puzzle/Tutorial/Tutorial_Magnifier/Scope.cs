using Cameras;
using QFramework;
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
        AudioPlayer audioplayer;
        [Range(0, 2)]public int id;

        bool holding = false;

        private IEnumerator OnMouseDown()
        {
            holding = true;
            if (audioplayer == null) audioplayer = AudioKit.PlaySound("ClockHands", true);
            yield return new WaitForFixedUpdate();
        }
        private void OnMouseOver()
        {
            if (holding && Puzzle.Instance.enable)
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
                if (audioplayer != null) audioplayer.Stop();
                audioplayer = null;
            }
            holding = false;
        }

        public void Initialize() => Puzzle.UpdateScopeStateWithoutCheck(id, transform.position);
    }
}
