using System.Windows.Forms;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;

namespace ReReflection
{
  [ActionHandler("ReReflection.About")]
  public class AboutAction : IActionHandler
  {
    public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
    {
      // return true or false to enable/disable this action
      return true;
    }

    public void Execute(IDataContext context, DelegateExecute nextExecute)
    {
      MessageBox.Show(
        "ReReflection\nYuriy Zanichkovskyy\n\nA set of refactoring for Reflection API",
        "About ReReflection",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information);
    }
  }
}
