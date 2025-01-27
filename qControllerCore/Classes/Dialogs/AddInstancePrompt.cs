using Acr.UserDialogs;
using qController.Communication;

namespace qController.Dialogs
{
    public class AddInstancePrompt : PromptConfig
    {
        public AddInstancePrompt()
        {

            Title = "Enter IP of QLab Computer";
            Message = "Enter an IP Address to save";
            OkText = "Next";
			OnTextChanged = args =>
            {
                //IPAddress ugh
                args.IsValid = IPHelper.IsValidAddress(args.Value);
            };
            OnAction = (qAddress) =>
            {
                if (!qAddress.Ok)
                    return;

                UserDialogs.Instance.Prompt(new PromptConfig
                {
                    Title = "Enter Name",
                    Message = "Enter a Name for " + qAddress.Text,
                    OkText = "Save",
                    OnAction = (qName) => {
                        if (!qName.Ok)
                            return;
                        QStorage.AddInstance(qName.Text, qAddress.Text);
                        App.showToast("Manual Workspace Added!");
                    }

                });
            };
        }
    }
}
