using TMPro;
using UnityEngine;

public class ThiefCounter : MonoBehaviour
{
    [SerializeField] private HouseAlarm _alarmZone;
    [SerializeField] private TMP_Text _thiefCountText;

    private void OnEnable()
    {
        if (_alarmZone == null)
            return;

        _alarmZone.OnThiefCountChanged += ChangeThiefCount;
    }

    private void OnDisable()
    {
        if (_alarmZone != null)
            _alarmZone.OnThiefCountChanged -= ChangeThiefCount;
    }

    private void ChangeThiefCount(int thiefCount)
    {
        if (_thiefCountText == null)
            return;

        _thiefCountText.text = thiefCount.ToString();
    }
}
