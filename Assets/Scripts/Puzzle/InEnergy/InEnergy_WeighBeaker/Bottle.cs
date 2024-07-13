using System.Collections;
using System.Collections.Generic;
using Cameras;
using DataSystem;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.InEnergy.WeighBeaker
{
    public enum BottleType
    {
        Four,
        Six,
        Nine
    }

    public class Bottle : MonoBehaviour
    {
        public InteractTarget Type;

        public Collider2D col;
        private Image image;
        private Animator filledEffect;

        public bool IsOnBalance = false;

        public List<Sprite> States;
        public int State = 0;

        public Vector3 UnderTap;
        public Vector3 OnBalance;
        
        Vector3 m_Offset;
        Vector3 m_TargetScreenVec;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
            image = transform.Find("Image").GetComponent<Image>();
            filledEffect = transform.Find("Filled").GetComponent<Animator>();
        }

        public void ChangeState(int target)
        {
            if (target == -1)
            {
                State = States.Count - 1;
                image.sprite = States[^1];

                filledEffect.enabled = true;
            }
            else
            {
                State = target;
                image.sprite = States[State];
            }
        }

        private IEnumerator OnMouseDown()
        {
            col.enabled = false;
            Puzzle.Instance.HoldingBottle = this;

            AudioKit.PlaySound("ItemUp");

            m_TargetScreenVec = TranslatorCameraManager.Camera.WorldToScreenPoint(transform.position);
            m_Offset = transform.position - TranslatorCameraManager.Camera.ScreenToWorldPoint(new Vector3
                (Input.mousePosition.x, Input.mousePosition.y, 1f));

            while (Input.GetMouseButton(0))
            {
                Vector3 res = TranslatorCameraManager.Camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                    Input.mousePosition.y, 1f)) + m_Offset;
                
                transform.position = res;
                yield return new WaitForFixedUpdate();
            }
        }

        private void OnMouseOver()
        {
            if (Puzzle.Instance.HoldingBottle == null ||
                Puzzle.Instance.HoldingBottle == this) return;
            
            Puzzle.Instance.Target = Type;
        }

        private void OnMouseExit()
        {
            if (Puzzle.Instance.HoldingBottle == null) return;
            Puzzle.Instance.Target = InteractTarget.None;
        }

        private void OnMouseUp()
        {
            if (Puzzle.Instance.HoldingBottle == this)
            {
                if (Puzzle.Instance.Target == InteractTarget.Tap)
                {
                    if (State < States.Count - 1)
                    {
                        StartCoroutine(FillWater());
                        return;
                    }
                }
                else if (Puzzle.Instance.Target == InteractTarget.Tank)
                {
                    ChangeState(0);
                }
                else if (Puzzle.Instance.Target == InteractTarget.Balance)
                {
                    if (Puzzle.Instance.OnBalanceBottle != null)
                    {
                        Puzzle.Instance.OnBalanceBottle.transform.localPosition = Vector3.zero;
                        Puzzle.Instance.OnBalanceBottle = null;
                    }

                    Puzzle.Instance.OnBalanceBottle = this;
                    if (Type == InteractTarget.Bottle_9)
                    {
                        if (State > 7)
                        {
                            Puzzle.Instance.Balance.ChangeState(0);
                            transform.localPosition = new Vector3(OnBalance.x, OnBalance.y - 130);
                        }
                        else if (State == 7)
                        {
                            
                            Puzzle.Instance.Balance.ChangeState(1);
                            transform.localPosition = new Vector3(OnBalance.x, OnBalance.y - 65);
                        }
                        else
                        {
                            Puzzle.Instance.Balance.ChangeState(2);
                            transform.localPosition = OnBalance;
                        }
                    }
                    else
                    {
                        Puzzle.Instance.Balance.ChangeState(2);
                        transform.localPosition = OnBalance;
                    }
                    col.enabled = true;
                    Puzzle.Instance.HoldingBottle = null;
                    return;
                }
                else if (Puzzle.Instance.Target == InteractTarget.None)
                {
                    if (Puzzle.Instance.OnBalanceBottle == this)
                    {
                        Puzzle.Instance.OnBalanceBottle = null;
                        Puzzle.Instance.Balance.ChangeState(2);
                    }
                }
                else
                {
                    CalculateWater();
                }
            }
            transform.localPosition = Vector3.zero;
            col.enabled = true;

            Puzzle.Instance.HoldingBottle = null;
        }

        private void CalculateWater()
        {
            var target = Puzzle.Instance.Bottles[Puzzle.Instance.Target];
            var targetRemaining = target.States.Count - 1 - target.State;

            Debug.Log(targetRemaining, this);

            if (State > targetRemaining)
            {
                target.ChangeState(-1);
                ChangeState(State - targetRemaining);
            }
            else
            {
                target.ChangeState(target.State + State);
                ChangeState(0);
            }
        }

        private IEnumerator FillWater()
        {
            transform.localPosition = UnderTap;
            Puzzle.Instance.Tap.ChangeState(true);

            for (var i = State; i < States.Count - 1; i++)
            {
                image.sprite = States[i];
                yield return new WaitForSeconds(0.1f);
            }
            filledEffect.enabled = true;
            State = States.Count - 1;
            Puzzle.Instance.Tap.ChangeState(false);

            yield return new WaitForSeconds(0.5f);

            filledEffect.enabled = false;
            transform.localPosition = Vector3.zero;
            col.enabled = true;
            Puzzle.Instance.HoldingBottle = null;
        }
    }
}