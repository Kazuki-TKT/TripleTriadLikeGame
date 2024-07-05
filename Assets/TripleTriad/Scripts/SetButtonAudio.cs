using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TripleTriad
{
    // ボタンに効果音を鳴らす設定
    public class SetButtonAudio : MonoBehaviour
    {
        [SerializeField]
        AudioClip onClickClip;

        void Awake()
        {
            if (onClickClip != null)
            {
                Button[] buttons = FindObjectsOfType<Button>();
                foreach (Button button in buttons)
                {
                    button.onClick.AddListener(() => AudioManager.instance.PlayOneShotClip(onClickClip));
                }
            }
        }

    }
}
