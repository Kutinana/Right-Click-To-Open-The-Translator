using Cameras;
using Puzzle.Overture.Bottle;
using QFramework;
using System.Collections;
using UnityEngine;

namespace Puzzle.InCenter.Astrolable
{
    public class Cube : MonoBehaviour
    {
        public CubeType cubeType;
        //public Collider2D col;

        Vector3 m_Offset;
        Vector3 _init_pos;
        Vector3 targetPos;

        private int originalPoint = -1;

        private void Awake()
        {
            //col = GetComponent<Collider2D>();
            _init_pos = transform.position;
        }

        private IEnumerator OnMouseDown()
        {
            if (Puzzle.Instance.solved) yield break;
            AudioKit.PlaySound("InteractClick", volumeScale: .5f);
            Puzzle.HoldingCube = this;
            //col.enabled = false;

            m_Offset = transform.position - TranslatorCameraManager.Camera.ScreenToWorldPoint(new Vector3
                (Input.mousePosition.x, Input.mousePosition.y, 1f));

            while (Input.GetMouseButton(0) && this.enabled)
            {
                Vector3 res = TranslatorCameraManager.Camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                    Input.mousePosition.y, 1f)) + m_Offset;
                res = new Vector3(res.x, res.y, _init_pos.z);

                transform.position = res;
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

        private void OnMouseUp()
        {
            FindClosestPos();

            Puzzle.HoldingCube = null;
        }

        private void FindClosestPos()
        {
            AudioKit.PlaySound("Cube-Slide", volumeScale: 0.8f);
            int ClosestPoint = -1;
            float minDist = 1000;
            for (int i = 0; i < 8; i++)
            {
                float dist = (Puzzle.Instance.ValidPoints[i].position - transform.position).magnitude;
                if (minDist > dist)
                {
                    minDist = dist;
                    ClosestPoint = i;
                }
            }

            if (originalPoint != -1)
            {
                Puzzle.Instance.cubeTypes[originalPoint] = CubeType.None;
                Puzzle.Instance.CubesInBlock[originalPoint] = null;
            }

            if (minDist <= Puzzle.ERROR)
            {
                if (Puzzle.Instance.CubesInBlock[ClosestPoint] != null)
                {
                    Puzzle.Instance.CubesInBlock[ClosestPoint].Initialize();
                }

                transform.position = Puzzle.Instance.ValidPoints[ClosestPoint].position;
                originalPoint = ClosestPoint;
                Puzzle.Instance.cubeTypes[ClosestPoint] = this.cubeType;
                Puzzle.Instance.CubesInBlock[ClosestPoint] = this;
            }
            else
            {
                if (originalPoint != -1)
                {
                    Puzzle.Instance.cubeTypes[ClosestPoint] = CubeType.None;
                    Puzzle.Instance.CubesInBlock[ClosestPoint] = null;
                }
                Initialize();
            }
        }

        public void Initialize()
        {
            originalPoint = -1;
            transform.position = _init_pos;
        }
    }
}