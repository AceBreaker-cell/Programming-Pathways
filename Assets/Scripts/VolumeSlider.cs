using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

/// <summary>
/// Attach script ini ke GameObject yang sama dengan Slider,
/// ATAU assign slider-nya lewat Inspector.
/// Mendukung AudioMixer (recommended) atau AudioListener.volume (simple).
/// </summary>
public class VolumeSlider : MonoBehaviour
{
    [Header("=== SLIDER REFERENCE ===")]
    [SerializeField] private Slider slider;

    [Header("=== AUDIO MIXER (Opsional) ===")]
    [Tooltip("Assign AudioMixer jika kamu pakai Mixer. Kosongkan untuk pakai AudioListener.volume")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string mixerParamName = "MasterVolume"; // Nama exposed parameter di Mixer

    [Header("=== UI LABEL (Opsional) ===")]
    [Tooltip("Label text yang menampilkan nilai volume (misal: '75%')")]
    [SerializeField] private TextMeshProUGUI volumeLabel;

    [Header("=== ICONS (Opsional) ===")]
    [Tooltip("Icon speaker mute")]
    [SerializeField] private GameObject iconMute;
    [Tooltip("Icon speaker normal")]
    [SerializeField] private GameObject iconSound;

    // -------------------------------------------------------
    private const string VOLUME_KEY = "MasterVolume";
    private float previousVolume = 1f;

    private void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();
    }

    private void Start()
    {
        if (slider == null)
        {
            Debug.LogWarning("[VolumeSlider] Tidak ada Slider ditemukan!");
            return;
        }

        // Setup range slider 0.0 – 1.0
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;

        // Load volume yang tersimpan
        float saved = PlayerPrefs.GetFloat(VOLUME_KEY, 1f);
        slider.value = saved;
        ApplyVolume(saved);

        // Daftarkan event
        slider.onValueChanged.AddListener(OnSliderChanged);

        UpdateUI(saved);
    }

    // -------------------------------------------------------
    //  EVENT
    // -------------------------------------------------------
    private void OnSliderChanged(float value)
    {
        ApplyVolume(value);
        PlayerPrefs.SetFloat(VOLUME_KEY, value);
        PlayerPrefs.Save();
        UpdateUI(value);
    }

    // -------------------------------------------------------
    //  APPLY VOLUME
    // -------------------------------------------------------
    private void ApplyVolume(float value)
    {
        if (audioMixer != null)
        {
            // AudioMixer pakai desibel: 0.0001 – 1.0 → -80dB – 0dB
            float db = value > 0.0001f ? Mathf.Log10(value) * 20f : -80f;
            audioMixer.SetFloat(mixerParamName, db);
        }
        else
        {
            // Fallback: AudioListener global
            AudioListener.volume = value;
        }
    }

    // -------------------------------------------------------
    //  UPDATE UI
    // -------------------------------------------------------
    private void UpdateUI(float value)
    {
        // Label persentase
        if (volumeLabel != null)
            volumeLabel.text = Mathf.RoundToInt(value * 100f) + "%";

        // Icon mute / sound
        bool isMuted = value <= 0.001f;
        if (iconMute != null)  iconMute.SetActive(isMuted);
        if (iconSound != null) iconSound.SetActive(!isMuted);
    }

    // -------------------------------------------------------
    //  PUBLIC HELPERS (bisa dipanggil dari button, dll.)
    // -------------------------------------------------------

    /// <summary>Toggle mute on/off</summary>
    public void ToggleMute()
    {
        if (slider == null) return;

        if (slider.value > 0.001f)
        {
            previousVolume = slider.value;
            slider.value = 0f;
        }
        else
        {
            slider.value = previousVolume > 0.001f ? previousVolume : 1f;
        }
    }

    /// <summary>Set volume langsung (0.0 – 1.0)</summary>
    public void SetVolume(float value)
    {
        if (slider != null)
            slider.value = Mathf.Clamp01(value);
    }

    private void OnDestroy()
    {
        if (slider != null)
            slider.onValueChanged.RemoveListener(OnSliderChanged);
    }
}
