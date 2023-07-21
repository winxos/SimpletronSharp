using System.Windows.Forms;
public partial class SynchronizedScrollRichTextBox : System.Windows.Forms.RichTextBox
{
    public SynchronizedScrollRichTextBox Synchronized { get; set; }
    public const int WM_VSCROLL = 0x115;
    public const int EM_LINESCROLL = 0xB6;

    protected override void WndProc(ref System.Windows.Forms.Message msg)
    {
        if (msg.Msg == WM_VSCROLL || msg.Msg == EM_LINESCROLL)
        {
            if (Synchronized != null)
            {
                Message message = msg;
                message.HWnd = Synchronized.Handle;
                Synchronized.PubWndProc(ref message);
            }
        }
        base.WndProc(ref msg);
    }

    public void PubWndProc(ref System.Windows.Forms.Message msg)
    {
        base.WndProc(ref msg);
    }
}
