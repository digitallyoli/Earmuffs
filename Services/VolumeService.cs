using CommunityToolkit.Mvvm.ComponentModel;
using Earmuffs.Helpers;

namespace Earmuffs.Services;

public partial class VolumeService : ObservableObject
{
    public bool HardLimitToggle
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                SettingsHelper.Set("HardLimitToggle", value);
                HardLimitVolume();
            }
        }
    }
    public int HardLimitValue
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                SettingsHelper.Set("HardLimitValue", value);
                HardLimitVolume();
            }
        }
    } = 100;
    public bool SoftLimitToggle
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                SettingsHelper.Set("SoftLimitToggle", value);
                _ = SoftLimitVolume();
            }
        }
    }
    public int SoftLimitValue
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                SettingsHelper.Set("SoftLimitValue", value);
                _ = SoftLimitVolume();
            }
        }
    } = 100;
    public int SoftLimitDelay
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                SettingsHelper.Set("SoftLimitDelay", value);
                _ = SoftLimitVolume();
            }
        }
    }
    public int SoftLimitDuration
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {

                SettingsHelper.Set("SoftLimitDuration", value);
                _ = SoftLimitVolume();
            }
        }
    }

    public VolumeService()
    {
        HardLimitToggle = SettingsHelper.Get("HardLimitToggle", false);
        HardLimitValue = SettingsHelper.Get("HardLimitValue", 80);
        SoftLimitToggle = SettingsHelper.Get("SoftLimitToggle", false);
        SoftLimitValue = SettingsHelper.Get("SoftLimitValue", 60);
        SoftLimitDelay = SettingsHelper.Get("SoftLimitDelay", 180);
        SoftLimitDuration = SettingsHelper.Get("SoftLimitDuration", 180);

        VolumeHelper.OnVolumeChange += OnVolumeChange;
        VolumeHelper.InitOnVolumeChange();
    }

    private bool _isChanging = false;

    private void OnVolumeChange()
    {
        HardLimitVolume();
        if (_isChanging == false)
        {
            _ = SoftLimitVolume();
        }
    }

    private void HardLimitVolume()
    {
        if (HardLimitToggle && VolumeHelper.GetVolume() > HardLimitValue)
        {
            VolumeHelper.SetVolume(HardLimitValue);
        }
    }

    private CancellationTokenSource? _cts;

    private async Task SoftLimitVolume()
    {
        float currentVolume = VolumeHelper.GetVolume();
        _cts?.Cancel();
        if (SoftLimitToggle && currentVolume > SoftLimitValue)
        {
            CancellationTokenSource newCts = new();
            _cts = newCts;
            await Task.Delay(SoftLimitDelay * 1000, newCts.Token).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
            if (newCts.IsCancellationRequested)
            {
                return;
            }
            int interval = 2;
            int steps = SoftLimitDuration / interval;
            float volumeAdjustStep = (currentVolume - SoftLimitValue) / steps;
            for (int i = 0; i < steps && !newCts.IsCancellationRequested; i++)
            {
                _isChanging = true;
                float vol = VolumeHelper.GetVolume();
                VolumeHelper.SetVolume(vol -= volumeAdjustStep);
                await Task.Delay(5);
                _isChanging = false;
                await Task.Delay(interval * 1000);
            }
        }
    }
}