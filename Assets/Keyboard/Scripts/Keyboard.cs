using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using System;
using TheHangingHouse.Animations;

public class Keyboard : AnimatableElement
{
    [Header("Set In Inspcetor")]
    public Button closeButton;
    public Transform showingPoint;
    public Transform hidingPoint;
    public AnimationKey animationKey = AnimationKey.EaseInOut * 2f;

    [Header("Typing Parameters")]
    public int maxCharacters = 15;

    [Header("Special Buttons")]
    [SerializeField] private Button[] _spaceButtons;
    [SerializeField] private Button[] _deleteButtons;
    [SerializeField] private Button[] _enterButtons;

    [Header("Events")]
    public UnityEvent<string> onClickKey;
    public UnityEvent onClickSpace;
    public UnityEvent onClickDelete;
    public UnityEvent onClickEnter;

    private Transform _englishContainer;
    private Transform _arabicContainer;

    private KeyButton[] _keys;

    private TMP_InputField _selected;
    private TMP_InputField _cachedSelected;

    private new void Awake()
    {
        base.Awake();

        closeButton.onClick.AddListener(() =>
        {
            Hide();
            _cachedSelected = null;
            _selected = null;
        });

        _keys = transform.GetComponentsInChildren<KeyButton>(true);

        _englishContainer = transform.GetChild(0);
        _arabicContainer = transform.GetChild(1);

        foreach (var keyButton in _keys)
            keyButton.GetComponent<Button>().onClick.AddListener(() => OnClickKey(keyButton.key));
        foreach (var spaceButton in _spaceButtons)
            spaceButton.onClick.AddListener(OnClickSpace);
        foreach (var deleteButton in _deleteButtons)
            deleteButton.onClick.AddListener(OnClickDelete);
        foreach (var enterButton in _enterButtons)
            enterButton.onClick.AddListener(OnClickEnter);
    }

    private void Update()
    {
        if (GetCurrentSelectedInputField() != null)
        {
            _selected = GetCurrentSelectedInputField();

            if (_cachedSelected != _selected &&
                _selected != null)
                Show();

            _cachedSelected = _selected;
        }

    }

    private void OnClickKey(string key)
    {
        onClickKey?.Invoke(key);

        if (_selected == null ||
            _selected.text.Length >= maxCharacters)
            return;

        if (_selected.text.Length == 0)
        {
            _selected.text += key;
            _selected.caretPosition = 1;
        }
        else
        {
            var prev_pos = _selected.caretPosition;
            _selected.text = _selected.text.Insert(_selected.caretPosition, key);
            _selected.caretPosition = prev_pos + 1;
        }

        ActivateKeyboard();
    }

    private void OnClickSpace()
    {
        onClickSpace?.Invoke();

        if (_selected == null || _selected.text.Length == 0 ||
                _selected.text.Length >= maxCharacters)
            return;

        var prev_pos = _selected.caretPosition = _selected.text.Length;
        _selected.text = _selected.text.Insert(_selected.caretPosition, " ");
        _selected.caretPosition = prev_pos + 1;

        ActivateKeyboard();
    }

    private void OnClickDelete()
    {
        onClickDelete?.Invoke();

        if (_selected == null || _selected.text.Length == 0 || _selected.caretPosition - 1 < 0) return;
        var prev_pos = _selected.caretPosition;
        _selected.text = _selected.text.Remove(_selected.caretPosition - 1, 1);
        _selected.caretPosition = prev_pos - 1;

        ActivateKeyboard();   
    }

    private void ActivateKeyboard()
    {
        _selected.Select();
        _selected.ActivateInputField();
    }

    private void OnClickEnter()
    {
        onClickEnter?.Invoke();
    }

    private TMP_InputField GetCurrentSelectedInputField()
    {
        var eventSystem = EventSystem.current;
        var go = eventSystem.currentSelectedGameObject;
        if (go == null) return null;
        return go.GetComponent<TMP_InputField>();
    }

    public override void OnBeforeShow() { }
    public override void OnAfterShow() { }
    public override void OnBeforeHide() { }
    public override void OnAfterHide() { _selected = null; }

    public override IEnumerator ShowingClip(Action callback)
    {
        Debug.Log("Showing Clip");
        yield return CoroutineClips.MoveClip(
            transform,
            showingPoint.position,
            animationKey,
            callback);
    }

    public override IEnumerator NormalClip()
    {
        throw new NotImplementedException();
    }

    public override IEnumerator HidingClip(Action callback)
    {
        yield return CoroutineClips.MoveClip(
            transform,
            hidingPoint.position,
            animationKey,
            callback
            );
    }
}
