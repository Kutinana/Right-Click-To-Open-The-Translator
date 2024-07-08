using Cameras;
using Puzzle.Overture.Bottle;
using QFramework;
using System.Collections;
using UnityEngine;

namespace Puzzle.Tutorial.P2
{
    public class Silde : MonoBehaviour
    {
        [Range(0, 2)] public int id;
        public Collider2D col;

        Vector3 m_Offset;
        Vector3 _init_pos;
        Vector3 targetPos;
        float LeftMost;
        float RightMost;

        public int closestPos;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
            _init_pos = transform.position;
            LeftMost = Puzzle.ValidPositions[0];
            RightMost = Puzzle.ValidPositions[4];
        }

        private IEnumerator OnMouseDown()
        {
            Puzzle.HoldingSilde = this;
            col.enabled = false;

            m_Offset = transform.position - TranslatorCameraManager.Camera.ScreenToWorldPoint(new Vector3
                (Input.mousePosition.x, Input.mousePosition.y, 1f));

            while (Input.GetMouseButton(0))
            {
                Vector3 res = TranslatorCameraManager.Camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                    Input.mousePosition.y, 1f)) + m_Offset;
                res = new Vector3(Mathf.Clamp(res.x, LeftMost, RightMost), _init_pos.y, _init_pos.z);

                if (!Puzzle.solved) transform.position = res;
                yield return new WaitForFixedUpdate();
            }
        }
        private float parameter = 0f;
        private IEnumerator MoveToCoroutine()
        {
            while (parameter < 0.99f)
            {
                transform.position = Vector3.Lerp(_init_pos, targetPos, parameter);
                parameter += Time.deltaTime * 1f;
            }
            transform.position = targetPos;

            yield return null;
        }

        private void FindClosestPos()
        {
            if (transform.position.x  < 0.55f) closestPos = 0;
            if (transform.position.x >= 0.55f && transform.position.x < 1.65f) closestPos = 1;
            if (transform.position.x >= 1.65f && transform.position.x < 2.75f) closestPos = 2;
            if (transform.position.x >= 2.75f && transform.position.x < 3.85f) closestPos = 3;
            if (transform.position.x >= 3.85f) closestPos = 4;
            Puzzle.CurrentPosition[id] = closestPos;
        }

        private void OnMouseUp()
        {
            FindClosestPos();
            Puzzle.ReArrangePosition();
            //ArrangePosition();

            Puzzle.HoldingSilde = null;
            col.enabled = true;
        }

        public void ArrangePosition()
        {
            parameter = 0f;
            _init_pos = transform.position;
            targetPos = new Vector3(Puzzle.ValidPositions[closestPos], _init_pos.y, _init_pos.z);
            StartCoroutine(MoveToCoroutine());
        }
    }
}