using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent (typeof(AudioSource))]
public class HouseAlarm : MonoBehaviour
{
    [SerializeField] private AudioSource _alarmAudio;

    [Range(0f, 1f)][SerializeField] private float _minVolume = 0f;
    [Range(0f, 1f)][SerializeField] private float _maxVolume = 1f;

    [SerializeField, Range(0f, 1f)] private float _volumeIncreaseSpeed = 0.35f;
    [SerializeField, Range(0f, 1f)] private float _volumeDecreaseSpeed = 0.5f;

    private int _thiefCount;
    private Collider _collider;
    private Coroutine _alarmCoroutine;

    public int ThiefCount => _thiefCount;
    public event Action<int> OnThiefCountChanged;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _alarmAudio = GetComponent<AudioSource>();

        _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Thief thied = other.GetComponent<Thief>();

        if (thied == null)
            return;

        _thiefCount++;

        if (_thiefCount == 1)
            StartAlarm();

        NotifyThiefCountChanged();
    }

    private void OnTriggerExit(Collider other)
    {
        Thief thied = other.GetComponent<Thief>();

        if (thied == null)
            return;

        _thiefCount--;

        if (_thiefCount == 0)
            StopAlarm();

        NotifyThiefCountChanged();
    }

    private void NotifyThiefCountChanged()
    {
        OnThiefCountChanged?.Invoke(_thiefCount);
    }

    private void StartAlarm()
    {
        if (_alarmAudio == null)
            return;

        if (_alarmCoroutine != null)
            StopCoroutine(_alarmCoroutine);

        _alarmCoroutine = StartCoroutine(ProcessAlarmIncreasing());
    }

    private void StopAlarm()
    {
        if (_alarmAudio == null)
            return;

        if (_alarmCoroutine != null)
            StopCoroutine(_alarmCoroutine);

        _alarmCoroutine = StartCoroutine(ProcessAlarmDecreasing());
    }

    private IEnumerator ProcessAlarmIncreasing()
    {
        if (_alarmAudio.isPlaying == false)
            _alarmAudio.Play();

        while (Mathf.Approximately(_alarmAudio.volume, _maxVolume) == false)
        {
            _alarmAudio.volume = Mathf.MoveTowards(_alarmAudio.volume, _maxVolume, _volumeIncreaseSpeed * Time.deltaTime);  
            yield return null;
        }

        _alarmAudio.volume = _maxVolume;
        _alarmCoroutine = null;
    }

    private IEnumerator ProcessAlarmDecreasing()
    {   
        while (Mathf.Approximately(_alarmAudio.volume, _minVolume) == false)
        {
            _alarmAudio.volume = Mathf.MoveTowards(_alarmAudio.volume, _minVolume, _volumeDecreaseSpeed * Time.deltaTime);
            yield return null;
        }

        _alarmAudio.volume = _minVolume;
        _alarmAudio.Stop();
        _alarmCoroutine = null;
    }
}
