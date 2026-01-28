using System.Runtime.InteropServices;

namespace Earmuffs.Helpers;

public static class VolumeHelper
{
    public static float GetVolume()
    {
        IAudioEndpointVolume endpoint = GetEndpoint();
        endpoint.GetMasterVolumeLevelScalar(out float volumeLevel);
        Marshal.ReleaseComObject(endpoint);
        return volumeLevel * 100;
    }

    public static void SetVolume(float volume)
    {
        IAudioEndpointVolume endpoint = GetEndpoint();
        endpoint.SetMasterVolumeLevelScalar(volume / 100, Guid.Empty);
        Marshal.ReleaseComObject(endpoint);
    }

    public static event Action? OnVolumeChange;

    private static IAudioEndpointVolume? _endpoint;
    private static AudioEndpointVolumeCallback? _callback;

    public static void InitOnVolumeChange()
    {
        _endpoint = GetEndpoint();
        _callback = new AudioEndpointVolumeCallback();
        _callback.VolumeChanged += () => OnVolumeChange?.Invoke();
        _endpoint.RegisterControlChangeNotify(_callback);
    }

    private static IAudioEndpointVolume GetEndpoint()
    {
        IMMDeviceEnumerator mmde = (IMMDeviceEnumerator)new MMDeviceEnumerator();
        mmde.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out IMMDevice ppDevice);
        Guid iid = typeof(IAudioEndpointVolume).GUID;
        ppDevice.Activate(ref iid, 0, IntPtr.Zero, out object ppInterface);
        return (IAudioEndpointVolume)ppInterface;
    }
}

[ComImport]
[Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
internal class MMDeviceEnumerator
{
}

internal enum EDataFlow
{
    eRender,
}

internal enum ERole
{
    eMultimedia,
}

[Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IMMDeviceEnumerator
{
    int NotImpl1();

    [PreserveSig]
    int GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role, out IMMDevice ppDevice);
}

[Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IMMDevice
{
    [PreserveSig]
    int Activate(ref Guid iid, int dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);
}

[Guid("657804FA-D6AD-4496-8A60-352752AF4F89"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IAudioEndpointVolumeCallback
{
    [PreserveSig]
    int OnNotify(IntPtr pNotifyData);
}

internal class AudioEndpointVolumeCallback : IAudioEndpointVolumeCallback
{
    public event Action? VolumeChanged;
    public int OnNotify(IntPtr pNotifyData)
    {
        VolumeChanged?.Invoke();
        return 0;
    }
}

[ComImport]
[Guid("5CDF2C82-841E-4546-9722-0CF74078229A")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IAudioEndpointVolume
{
    int RegisterControlChangeNotify([MarshalAs(UnmanagedType.Interface)] IAudioEndpointVolumeCallback pNotify);
    int UnregisterControlChangeNotify([MarshalAs(UnmanagedType.Interface)] IAudioEndpointVolumeCallback pNotify);
    int GetChannelCount(out uint pnChannelCount);
    int SetMasterVolumeLevel(float fLevelDB, Guid pguidEventContext);
    int SetMasterVolumeLevelScalar(float fLevel, Guid pguidEventContext);
    int GetMasterVolumeLevel(out float pfLevelDB);
    int GetMasterVolumeLevelScalar(out float pfLevel);
}