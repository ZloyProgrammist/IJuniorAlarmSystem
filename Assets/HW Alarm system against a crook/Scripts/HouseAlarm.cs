using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent (typeof(AudioSource))]
public class HouseAlarm : MonoBehaviour
{
    [SerializeField] private AudioSource _alarmAudio;

    [Range(0f, 1f)][SerializeField] private float _minVolume = 0f;
    [Range(0f, 1f)][SerializeField] private float _maxVolume = 1f;

    [SerializeField, Min(0f)] private float _volumeIncreaseSpeed = 0.35f;
    [SerializeField, Min(0f)] private float _volumeDecreaseSpeed = 0.5f;
    [SerializeField] private bool _isProcessing = true;

    private int _thiefCount;
    private Collider _collider;

    public int ThiefCount => _thiefCount;
    public event Action<int> OnThiefCountChanged;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _alarmAudio = GetComponent<AudioSource>();

        _collider.isTrigger = true;
    }

    private void Update()
    {
        if (_isProcessing)
            ProcessAlarmAudio();
    }

    private void OnTriggerEnter(Collider other)
    {
        Thief thied = other.GetComponent<Thief>();

        if (thied == null)
            return;

        _thiefCount++;

        if (_thiefCount == 1)
            _isProcessing = true;

        NotifyThiefCountChanged();
    }

    private void OnTriggerExit(Collider other)
    {
        Thief thied = other.GetComponent<Thief>();

        if (thied == null)
            return;

        _thiefCount--;

        if (_thiefCount == 0)
            _isProcessing = true;

        NotifyThiefCountChanged();
    }

    private void NotifyThiefCountChanged()
    {
        OnThiefCountChanged?.Invoke(_thiefCount);
    }

    private void ProcessAlarmAudio()
    {
        if (_alarmAudio == null)
            return;

        if (_thiefCount > 0)
            ProcessAlarmIncreasing();
        else
            ProcessAlarmDecreasing();
    }

    private void ProcessAlarmIncreasing()
    {
        if (_alarmAudio.isPlaying == false)
            _alarmAudio.Play();

        _alarmAudio.volume = Mathf.MoveTowards(_alarmAudio.volume, _maxVolume, _volumeIncreaseSpeed * Time.deltaTime);

        if (_alarmAudio.volume == _maxVolume)
            _isProcessing = false;
    }

    private void ProcessAlarmDecreasing()
    {
        bool isAlreadySilent = Mathf.Approximately(_alarmAudio.volume, _minVolume);
        
        if (isAlreadySilent && _alarmAudio.isPlaying == false)
        {
            _isProcessing = false;
            return;
        } 

        _alarmAudio.volume = Mathf.MoveTowards(_alarmAudio.volume, _minVolume, _volumeDecreaseSpeed * Time.deltaTime);

        if (_isProcessing && Mathf.Approximately(_alarmAudio.volume, _minVolume))
        {
            _alarmAudio.Stop();
            _isProcessing = false;
        }
    }
}
