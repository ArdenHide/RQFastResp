using Newtonsoft.Json;

namespace RQFastResp;

internal static class Program
{
    private static string lastPath = "";

    [STAThread]
    private static void Main()
    {
        var fileDialog = new OpenFileDialog
        {
            Filter = "JSON Files (*.json)|*.json|All files (*.*)|*.*",
            InitialDirectory = lastPath
        };

        if (fileDialog.ShowDialog() != DialogResult.OK) return;

        lastPath = Path.GetDirectoryName(fileDialog.FileName);
        var path = fileDialog.FileName;

        if (!File.Exists(path))
        {
            MessageBox.Show("File not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var json = File.ReadAllText(path);
        var bosses = JsonConvert.DeserializeObject<List<Boss>>(json);
        var show = new List<string>();

        foreach (var boss in bosses)
        {
            if (!boss.HasTrack || boss.RespawnTime == TimeSpan.FromHours(168)) continue;

            var resAt = boss.KillingLog.Last().Add(boss.RespawnTime);
            show.Add($"{boss.BossName} | Рес в {resAt} | Умер в {boss.KillingLog.Last()}");
        }

        var msg = string.Join(Environment.NewLine, show);
        Clipboard.SetText(msg);
        MessageBox.Show(msg, "Скопированно в буфер обмена", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly | MessageBoxOptions.ServiceNotification);
    }
}

public class Boss
{
    public string BossName { get; set; }
    public bool HasTrack { get; set; }
    public TimeSpan RespawnTime { get; set; }
    public List<DateTime> KillingLog { get; set; }
}