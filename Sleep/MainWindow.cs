using System;
using System.IO;
using System.Media;
using System.Threading;
using Gtk;

public partial class MainWindow : Gtk.Window
{
    Thread countDown;
    FileStream beepStream;
    SoundPlayer beepPlayer;

    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
        Title = "Спати 0.6";
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        if (countDown != null && countDown.IsAlive)
        {
            countDown.Abort();
        }
        BeepDespose();
        Application.Quit();
        a.RetVal = true;
    }

    private void BeepDespose()
    {
        if (beepPlayer != null)
        {
            beepPlayer.Dispose();
        }
        if (beepStream != null)
        {
            beepStream.Dispose();
        }
    }

    protected void Gotosleep(object sender, EventArgs e)
    {
        if (countDown != null && countDown.IsAlive) countDown.Abort();
        countDown = new Thread(new ParameterizedThreadStart(ShudDown));
        countDown.Start(min.Text);
    }

    private void ShudDown(object minuts)
    {
        int mins = 0;

        bool parseResult = int.TryParse(minuts.ToString(), out mins);

        if (!parseResult) return;

        progress.Adjustment.Lower = 0;
        progress.Adjustment.Upper = mins;
        progress.Adjustment.Value = mins;
        progress.Text = mins.ToString();

        if (mins > 0)
        {
            for (int i = mins - 1; i >= 0; i--)
            {
                if (i == 0) Beep();
                Thread.Sleep(6000);
                progress.Text = i.ToString();
                progress.Adjustment.Value = i;

            }
        }
        else
        {
            Beep();
            Thread.Sleep(4000);
        }

        //Application.Quit();
        System.Diagnostics.Process.Start("shutdown", "+0");
    }

    protected void Stop(object sender, EventArgs e)
    {
        if (countDown != null && countDown.IsAlive)
        {
            countDown.Abort();
            countDown = null;
            progress.Text = "";
            progress.Adjustment.Value = 0;
        }
    }

    private void Beep()
    {
        try
        {
            BeepDespose();
            beepStream = new FileStream(System.IO.Directory.GetCurrentDirectory() + "/beep.wav", FileMode.Open, FileAccess.Read, FileShare.Read);
            beepPlayer = new SoundPlayer(beepStream);
            beepPlayer.Play();
        }
        catch { }
    }
}
