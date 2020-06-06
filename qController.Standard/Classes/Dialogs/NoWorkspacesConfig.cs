using System;
using Acr.UserDialogs;

namespace qController.Dialogs
{
    public class NoWorkspacesConfig : ConfirmConfig
    {
        public NoWorkspacesConfig()
        {
            Message = "QLab doesn't have any workspaces open?";
            OkText = "Disconnect";
        }
    }
}
