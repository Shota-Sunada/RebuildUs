using System.Diagnostics;
using System.Text;

namespace RebuildUs;

#if DEBUG
public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance;

    private TextMeshPro _text;
    private readonly StringBuilder _sb = new();
    private Process _currentProcess;
    private TimeSpan _lastProcessorTime;
    private DateTime _lastTime;
    private double _cpuUsage;
    private readonly float _updateInterval = 0.5f;
    private float _timer = 0f;
    private double _chiSquare = 0.0;
    private int _totalSamples = 0;

    public DebugManager(IntPtr ptr) : base(ptr) { }

    private void Start()
    {
        Instance = this;
        _currentProcess = Process.GetCurrentProcess();
        _lastProcessorTime = _currentProcess.TotalProcessorTime;
        _lastTime = DateTime.Now;

        var hudManager = FastDestroyableSingleton<HudManager>.Instance;
        if (hudManager != null)
        {
            _text = Instantiate(hudManager.TaskPanel.taskText, hudManager.transform);
            _text.transform.localPosition = new Vector3(-5.2f, 3.2f, -100f);
            _text.fontSize = 1.5f;
            _text.alignment = TextAlignmentOptions.TopLeft;
            _text.color = UnityEngine.Color.white;
            _text.outlineWidth = 0.1f;
            _text.outlineColor = UnityEngine.Color.black;
            _text.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _updateInterval)
        {
            _timer = 0f;
            CalculateCpuUsage();
            UpdateText();
        }

        // Toggle debug display with Shift+D
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.D))
        {
            _text.enabled = !_text.enabled;
        }

        // Check random bias with Shift+R
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
        {
            CheckRandomBias();
        }
    }

    private void CalculateCpuUsage()
    {
        var currentTime = DateTime.Now;
        var currentProcessorTime = _currentProcess.TotalProcessorTime;

        var usage = (currentProcessorTime.TotalMilliseconds - _lastProcessorTime.TotalMilliseconds) /
                    (currentTime.Subtract(_lastTime).TotalMilliseconds * Environment.ProcessorCount);

        _cpuUsage = usage * 100.0;

        _lastTime = currentTime;
        _lastProcessorTime = currentProcessorTime;
    }

    private void UpdateText()
    {
        if (_text == null) return;

        _sb.Clear();
        // _sb.Append("Game Version: ").Append(AmongUsClient.Instance.GameVersion.ToString()).AppendLine();
        _sb.Append("Mod Version: ").Append(RebuildUs.MOD_VERSION).AppendLine();
        _sb.Append("CPU: ").Append(_cpuUsage.ToString("F1")).Append('%').AppendLine();
        _sb.Append("Memory: ").Append((_currentProcess.WorkingSet64 / 1024 / 1024).ToString()).Append("MB");

        if (_totalSamples > 0)
        {
            _sb.AppendLine();
            _sb.Append("Total Samples: ").Append(_totalSamples).AppendLine();
            _sb.Append("Chi-Square: ").Append(_chiSquare.ToString("F2"));
        }

        _text.text = _sb.ToString();
    }

    private void CheckRandomBias()
    {
        const int sampleSize = 10000;
        const int range = 10;
        int[] counts = new int[range];
        var rnd = RebuildUs.Instance.Rnd;

        for (int i = 0; i < sampleSize; i++)
        {
            int value = rnd.Next(range);
            counts[value]++;
        }

        double expected = sampleSize / (double)range;
        double chiSquare = 0.0;

        var logSb = new StringBuilder();
        logSb.AppendLine("Random Bias Check Results:");
        for (int i = 0; i < range; i++)
        {
            double deviation = counts[i] - expected;
            chiSquare += deviation * deviation / expected;
            logSb.AppendLine($"Value {i}: {counts[i]} (expected: {expected:F1}, deviation: {deviation:F1})");
        }
        logSb.AppendLine($"Chi-Square Statistic: {chiSquare:F2}");

        Logger.LogInfo(logSb.ToString(), "RandomBias");

        _totalSamples = sampleSize;
        _chiSquare = chiSquare;
    }
}
#endif

public static class Debug
{
    public static void Initialize()
    {
#if DEBUG
        ClassInjector.RegisterTypeInIl2Cpp<DebugManager>();
#endif
    }

    public static void CreateDebugManager(HudManager hudManager)
    {
#if DEBUG
        if (DebugManager.Instance == null && hudManager != null)
        {
            hudManager.gameObject.AddComponent<DebugManager>();
        }
#endif
    }
}
