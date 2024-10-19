
using DataSystem;
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.InEnergy.Submarine
{
    public class Coordinate : MonoBehaviour
    {
        private List<Character> characters;
        private CanvasGroup m_waterEffect;
        private Image m_light;
        private Transform m_submarine;
        private Transform m_mark;
        private int currentCoroutine;
        public List<Sprite> waterEffectSprites;
        public List<Sprite> lightSprites;

        private void Awake()
        {
            characters = new List<Character>()
            {
                transform.Find("Characters/1").GetComponent<Character>(),
                transform.Find("Characters/2").GetComponent<Character>(),
                transform.Find("Characters/3").GetComponent<Character>(),
                transform.Find("Characters/4").GetComponent<Character>()
            };

            transform.Find("Button/up1").GetComponent<Button>().onClick.AddListener(() => CharacterUp(0));
            transform.Find("Button/up2").GetComponent<Button>().onClick.AddListener(() => CharacterUp(1));
            transform.Find("Button/up3").GetComponent<Button>().onClick.AddListener(() => CharacterUp(2));
            transform.Find("Button/up4").GetComponent<Button>().onClick.AddListener(() => CharacterUp(3));


            transform.Find("Button/down1").GetComponent<Button>().onClick.AddListener(() => CharacterDown(0));
            transform.Find("Button/down2").GetComponent<Button>().onClick.AddListener(() => CharacterDown(1));
            transform.Find("Button/down3").GetComponent<Button>().onClick.AddListener(() => CharacterDown(2));
            transform.Find("Button/down4").GetComponent<Button>().onClick.AddListener(() => CharacterDown(3));

            transform.Find("Button/Confirm").GetComponent<Button>().onClick.AddListener(() => ConfirmCoordinates());

            m_waterEffect = transform.Find("WaterEffect").GetComponent<CanvasGroup>();
            m_submarine = transform.Find("Animation/submarine");
            m_mark = transform.Find("Screen Mark");
            m_light = transform.Find("Screen Light").GetComponent<Image>();

            if (GameProgressData.GetPuzzleProgress(Puzzle.Instance.Id) == PuzzleProgress.Solved)
            {
                m_light.sprite = lightSprites[0];
            }
        }

        private void CharacterUpdate(int id)
        {
            AudioKit.PlaySound("Cube-Slide", volume: 0.8f);
            characters[id].data = Puzzle.Instance.numbers[Puzzle.Instance.m_coordinates[id]];
            characters[id].Refresh();
        }

        private void CharacterUp(int id)
        {
            int current = Puzzle.Instance.m_coordinates[id];
            int target = (current + 1) % 8;
            Puzzle.Instance.m_coordinates[id] = target;
            CharacterUpdate(id);
        }

        private void CharacterDown(int id)
        {
            int current = Puzzle.Instance.m_coordinates[id];
            int target = current - 1;
            target = target >= 0 ? target % 8 : (target + 8) % 8;
            Puzzle.Instance.m_coordinates[id] = target;
            CharacterUpdate(id);
        }

        private void ConfirmCoordinates()
        {
            transform.Find("Button/Confirm").GetComponent<Button>().interactable = false;
            AudioKit.PlaySound("023BoxMoving");
            parameter = 0f;
            parameter_mark = 0f;
            float x_offset = Puzzle.Instance.m_coordinates[0] * 50 + Puzzle.Instance.m_coordinates[1] * 10;
            float y_offset = 0 - Puzzle.Instance.m_coordinates[2] * 25 - Puzzle.Instance.m_coordinates[3] * 5;
            submarine_initialPosition = m_submarine.localPosition;
            submarine_targetPosition = new Vector3(m_submarine.localPosition.x, m_submarine.localPosition.y - 300, m_submarine.localPosition.z);
            mark_initialPosition = m_mark.localPosition;
            mark_middlePosition = new Vector3(mark_initialPosition.x + x_offset, mark_initialPosition.y, mark_initialPosition.z);
            mark_targetPosition = new Vector3(mark_middlePosition.x, mark_middlePosition.y + y_offset, mark_middlePosition.z);
            currentCoroutine += 1;
            StartCoroutine(ScreenAnimation());
            currentCoroutine += 1;
            StartCoroutine(SubmarineMoving());
            currentCoroutine += 1;
            StartCoroutine(WaterAnimation());

            if (Puzzle.PuzzleFinish())
            {
                StartCoroutine(CoordinateCorrect());
                m_light.sprite = lightSprites[0];
            }
            else
            {
                m_light.sprite = lightSprites[1];
                StartCoroutine(EnableButton());
            }
        }

        private IEnumerator WaterAnimation()
        {
            //Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(m_waterEffect, 1, 0.5f);
            AudioKit.PlaySound("023Dive", volume: 0.6f);
            m_waterEffect.alpha = 1;
            yield return new WaitForSeconds(0.1f);
            m_waterEffect.GetComponent<Image>().sprite = waterEffectSprites[1];
            yield return new WaitForSeconds(0.1f);
            m_waterEffect.GetComponent<Image>().sprite = waterEffectSprites[2];
            yield return new WaitForSeconds(0.1f);
            m_waterEffect.GetComponent<Image>().sprite = waterEffectSprites[3];
            Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(m_waterEffect, 0, 0.5f);
            yield return new WaitForSeconds(0.1f);
            //m_waterEffect.GetComponent<Image>().sprite = waterEffectSprites[0];
            m_waterEffect.alpha = 0;

            yield return new WaitForSeconds(1f);

            m_waterEffect.alpha = 1;
            yield return new WaitForSeconds(0.1f);
            m_waterEffect.GetComponent<Image>().sprite = waterEffectSprites[1];
            yield return new WaitForSeconds(0.1f);
            AudioKit.PlaySound("023ComeUp", volume: 0.6f);
            m_waterEffect.GetComponent<Image>().sprite = waterEffectSprites[2];
            yield return new WaitForSeconds(0.1f);
            m_waterEffect.GetComponent<Image>().sprite = waterEffectSprites[3];
            Kuchinashi.CanvasGroupHelper.FadeCanvasGroup(m_waterEffect, 0, 0.5f);
            yield return new WaitForSeconds(0.1f);
            //m_waterEffect.GetComponent<Image>().sprite = waterEffectSprites[0];
            m_waterEffect.alpha = 0;

            currentCoroutine -= 1;
        }

        float parameter = 0f;
        Vector3 submarine_initialPosition;
        Vector3 submarine_targetPosition;
        float parameter_mark = 0f;
        Vector3 mark_initialPosition;
        Vector3 mark_middlePosition;
        Vector3 mark_targetPosition;

        private IEnumerator SubmarineMoving()
        {
            while (parameter < 0.99f)
            {
                m_submarine.localPosition = Vector3.Lerp(submarine_initialPosition, submarine_targetPosition, parameter);
                parameter += Time.deltaTime * 2f;

                yield return new WaitForFixedUpdate();
            }
            m_submarine.localPosition = submarine_targetPosition;

            yield return new WaitForSeconds(1f);
            if (Puzzle.PuzzleFinish()) transform.Find("Animation/submarine/item").gameObject.SetActive(true);
            parameter = 0f;

            while (parameter < 0.99f)
            {
                m_submarine.localPosition = Vector3.Lerp(submarine_targetPosition, submarine_initialPosition, parameter);
                parameter += Time.deltaTime * 2f;

                yield return new WaitForFixedUpdate();
            }
            m_submarine.localPosition = submarine_initialPosition;

            currentCoroutine -= 1;
        }

        private IEnumerator CoordinateCorrect()
        {
            yield return new WaitUntil(() => currentCoroutine <= 0);
            Puzzle.Instance.enabled = false;
            yield return null;
        }

        private IEnumerator EnableButton()
        {
            yield return new WaitUntil(() => currentCoroutine <= 0);

            transform.Find("Button/Confirm").GetComponent<Button>().interactable = true;
            m_light.sprite = lightSprites[2];

            yield return null;
        }

        private IEnumerator ScreenAnimation()
        {
            while (parameter_mark < 0.99f)
            {
                m_mark.localPosition = Vector3.Lerp(mark_initialPosition, mark_middlePosition, parameter_mark);
                parameter_mark += Time.deltaTime * 4f;

                yield return new WaitForFixedUpdate();
            }
            m_mark.localPosition = mark_middlePosition;

            parameter_mark = 0f;

            while (parameter_mark < 0.99f)
            {
                m_mark.localPosition = Vector3.Lerp(mark_middlePosition, mark_targetPosition, parameter_mark);
                parameter_mark += Time.deltaTime * 4f;

                yield return new WaitForFixedUpdate();
            }
            m_mark.localPosition = mark_targetPosition;

            parameter_mark = 0f;
            yield return new WaitForSeconds(1f);

            while (parameter_mark < 0.99f)
            {
                m_mark.localPosition = Vector3.Lerp(mark_targetPosition, mark_middlePosition, parameter_mark);
                parameter_mark += Time.deltaTime * 4f;

                yield return new WaitForFixedUpdate();
            }
            m_mark.localPosition = mark_middlePosition;

            parameter_mark = 0f;

            while (parameter_mark < 0.99f)
            {
                m_mark.localPosition = Vector3.Lerp(mark_middlePosition, mark_initialPosition, parameter_mark);
                parameter_mark += Time.deltaTime * 4f;

                yield return new WaitForFixedUpdate();
            }
            m_mark.localPosition = mark_initialPosition;

            currentCoroutine -= 1;
        }
    }
}