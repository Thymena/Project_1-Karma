using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueUIController : MonoBehaviour
{
    public static DialogueUIController Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject dialogueTextObject;
    [SerializeField] private TMP_Text dialogueText;

    [Header("Typewriter")]
    [SerializeField] private float charInterval = DialogueSystemDefaults.CharInterval;
    [SerializeField] private float segmentPause = DialogueSystemDefaults.SegmentPause;

    [Header("Audio")]
    [SerializeField] private AudioClip typewriterClip;
    [SerializeField] private AudioSource typewriterAudioSource;

    [Header("Text Fit")]
    [SerializeField] private float textMargin = DialogueSystemDefaults.TextMargin;
    [SerializeField] private float fontSizeMin = DialogueSystemDefaults.FontSizeMin;
    [SerializeField] private float fontSizeMax = DialogueSystemDefaults.FontSizeMax;

    public bool IsPlaying => _sessionCoroutine != null;

    private Coroutine _sessionCoroutine;
    private bool _skipCurrentSegment;
    private bool _playerSkippedDuringSession;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        DetachOverlayCanvasFromWorldParent();
        EnsureCanvasVisible();

        if (dialogueText == null && dialogueTextObject != null)
        {
            dialogueText = dialogueTextObject.GetComponent<TMP_Text>();
        }

        if (typewriterAudioSource == null)
        {
            typewriterAudioSource = GetComponent<AudioSource>();
        }

        ConfigureTextFit();
        SetUIVisible(false);
    }

    private void Reset()
    {
        charInterval = DialogueSystemDefaults.CharInterval;
        segmentPause = DialogueSystemDefaults.SegmentPause;
        textMargin = DialogueSystemDefaults.TextMargin;
        fontSizeMin = DialogueSystemDefaults.FontSizeMin;
        fontSizeMax = DialogueSystemDefaults.FontSizeMax;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        if (_sessionCoroutine == null)
        {
            return;
        }

        if (!Input.anyKeyDown)
        {
            return;
        }

        if (_skipCurrentSegment)
        {
            return;
        }

        _skipCurrentSegment = true;
        _playerSkippedDuringSession = true;
    }

    public bool TryPlayDialogue(IReadOnlyList<string> lines, OneTimeText owner)
    {
        if (IsPlaying || lines == null || lines.Count == 0)
        {
            return false;
        }

        for (int i = 0; i < lines.Count; i++)
        {
            if (!string.IsNullOrEmpty(lines[i]))
            {
                _sessionCoroutine = StartCoroutine(PlayDialogueSession(lines, owner));
                return true;
            }
        }

        return false;
    }

    private IEnumerator PlayDialogueSession(IReadOnlyList<string> lines, OneTimeText owner)
    {
        _playerSkippedDuringSession = false;
        _skipCurrentSegment = false;
        SetUIVisible(true);

        for (int segmentIndex = 0; segmentIndex < lines.Count; segmentIndex++)
        {
            string segment = lines[segmentIndex];
            if (string.IsNullOrEmpty(segment))
            {
                continue;
            }

            _skipCurrentSegment = false;
            yield return TypeSegment(segment);

            if (segmentIndex < lines.Count - 1 && segmentPause > 0f)
            {
                yield return new WaitForSeconds(segmentPause);
            }
        }

        while (Input.anyKey)
        {
            yield return null;
        }

        yield return WaitForCloseInput();

        SetUIVisible(false);
        _sessionCoroutine = null;
        owner?.NotifyDialogueFinished();
    }

    private IEnumerator TypeSegment(string segment)
    {
        if (dialogueText == null)
        {
            Debug.LogError($"{nameof(DialogueUIController)}: dialogueText is not assigned.");
            yield break;
        }

        dialogueText.text = string.Empty;
        dialogueText.ForceMeshUpdate();

        for (int i = 0; i < segment.Length; i++)
        {
            if (_skipCurrentSegment)
            {
                dialogueText.text = segment;
                dialogueText.ForceMeshUpdate();
                yield break;
            }

            dialogueText.text = segment.Substring(0, i + 1);
            PlayTypewriterSound();
            dialogueText.ForceMeshUpdate();

            if (i < segment.Length - 1)
            {
                yield return new WaitForSeconds(charInterval);
            }
        }
    }

    private IEnumerator WaitForCloseInput()
    {
        while (true)
        {
            if (Input.anyKeyDown)
            {
                yield break;
            }

            yield return null;
        }
    }

    private void PlayTypewriterSound()
    {
        if (typewriterClip == null)
        {
            return;
        }

        if (typewriterAudioSource != null)
        {
            typewriterAudioSource.PlayOneShot(typewriterClip);
        }
        else
        {
            AudioSource.PlayClipAtPoint(typewriterClip, transform.position);
        }
    }

    private void SetUIVisible(bool visible)
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(visible);
        }

        GameObject textObject = dialogueTextObject != null
            ? dialogueTextObject
            : dialogueText != null ? dialogueText.gameObject : null;

        if (textObject != null)
        {
            textObject.SetActive(visible);
        }

        if (!visible && dialogueText != null)
        {
            dialogueText.text = string.Empty;
        }
    }

    private void DetachOverlayCanvasFromWorldParent()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay && transform.parent != null)
        {
            transform.SetParent(null);
        }
    }

    private void EnsureCanvasVisible()
    {
        if (transform is RectTransform canvasRect && canvasRect.localScale.sqrMagnitude < 0.001f)
        {
            canvasRect.localScale = Vector3.one;
        }
    }

    private void ConfigureTextFit()
    {
        if (dialogueText == null || dialoguePanel == null)
        {
            return;
        }

        RectTransform panelRect = dialoguePanel.GetComponent<RectTransform>();
        RectTransform textRect = dialogueText.rectTransform;
        if (panelRect == null || textRect == null)
        {
            return;
        }

        panelRect.localScale = Vector3.one;

        textRect.SetParent(panelRect, false);
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = Vector2.zero;
        textRect.offsetMin = new Vector2(textMargin, textMargin);
        textRect.offsetMax = new Vector2(-textMargin, -textMargin);
        textRect.localScale = Vector3.one;

        dialogueText.enableWordWrapping = true;
        dialogueText.overflowMode = TextOverflowModes.Truncate;
        dialogueText.enableAutoSizing = true;
        dialogueText.fontSizeMin = fontSizeMin;
        dialogueText.fontSizeMax = fontSizeMax;
        dialogueText.margin = Vector4.zero;
    }
}
