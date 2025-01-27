using System;
using Acr.UserDialogs;
using Microsoft.Maui.ApplicationModel;

namespace qController.Dialogs
{
    public class DonatePrompt : ConfirmConfig
    {
        public DonatePrompt()
        {

            Title = "Support the Project";
            Message = "This application will never be a paid app or contain any sort of ads, but if you choose to show your support you may do so by clicking the Donate button below. Thank you!";
            OkText = "Donate";
            OnAction = (response) =>
            {
                if (response)
                    Launcher.OpenAsync(new Uri("https://linktr.ee/JoelWetzell"));
            };
        }
    }
}
