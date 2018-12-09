
using System.Windows.Forms;

namespace ServerTreeView.Settings
{
    public static class CustomMessageBox
    {
        public static DialogResult Show(Control owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        {
            var messageBoxOptions = options;
            if (IsRightToLeft(owner))
            {
                messageBoxOptions = options | MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign;
            }
            return MessageBox.Show(owner, text, caption, buttons, icon, defaultButton, messageBoxOptions);
        }

        private static bool IsRightToLeft(Control owner)
        {
            var control = owner;

            return control?.RightToLeft == RightToLeft.Yes;

            // If no parent control is available, ask the CurrentUICulture
            // if we are running under right-to-left.
        }
    }
}