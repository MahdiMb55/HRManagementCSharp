using HRManagement.WinForms.Forms;
using Microsoft.Extensions.Hosting;

namespace HRManagement.WinForms.Startup;

public sealed record BootstrapResult(
    bool IsSuccess,
    IHost? Host,
    MainForm? MainForm,
    string? UserMessage)
{
    public static BootstrapResult Success(IHost host, MainForm mainForm) => new(true, host, mainForm, null);
    public static BootstrapResult Failure(string message) => new(false, null, null, message);
}
