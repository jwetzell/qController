using Acr.UserDialogs;
using qController.Helpers;

namespace qController.UI.Dialogs
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
                    Title = "Enter Port",
                    Message = "Enter OSC port for " + qAddress.Text,
                    OkText = "Save",
                    InputType = InputType.Number,
                    OnTextChanged = args =>
                    {
                        args.IsValid = args.Value.Length > 0;
                    },
                    OnAction = (qPort) => {
                        if (!qPort.Ok)
                            return;

                        QStorage.AddServer(qAddress.Text, int.Parse(qPort.Text));
                        App.showToast("Manual QLab Instance Added!");
                    }

                });

            };
        }
    }
}
