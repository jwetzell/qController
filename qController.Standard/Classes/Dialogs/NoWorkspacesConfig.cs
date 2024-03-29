﻿using System;
using Acr.UserDialogs;

namespace qController.Dialogs
{
    public class NoWorkspacesConfig : ConfirmConfig
    {
        public NoWorkspacesConfig(Action<bool> action)
        {
            Message = "QLab doesn't have any workspaces open?";
            OkText = "Disconnect";
            OnAction = action;
        }
    }
}
