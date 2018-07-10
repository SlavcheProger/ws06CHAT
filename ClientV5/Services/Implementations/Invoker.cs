using System.Threading;
using System.Windows.Forms;

internal class Invoker
{
    internal CommonDialog InvokeDialog;
    private Thread InvokeThread;
    private DialogResult InvokeResult;

    internal Invoker(CommonDialog dialog)
    {
        InvokeDialog = dialog;
        InvokeThread = new Thread(new ThreadStart(InvokeMethod));
        InvokeThread.SetApartmentState(ApartmentState.STA);
        InvokeResult = DialogResult.None;
    }

    internal DialogResult Invoke()
    {
        InvokeThread.Start();
        InvokeThread.Join();
        return InvokeResult;
    }

    private void InvokeMethod()
    {
        InvokeResult = InvokeDialog.ShowDialog();
    }
}