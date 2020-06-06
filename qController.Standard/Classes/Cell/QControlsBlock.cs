using System;
using System.Collections.Generic;
using Xamarin.Forms;
using qController.Communication;

namespace qController.Cell
{
    public class QControlsBlock : Frame
    {
        Grid mainG;
        QController qController;


        public QControlsBlock(QController controller)
        {
            qController = controller;

            Margin = new Thickness(10);
            Padding = new Thickness(0);
            IsVisible = true;


            BackgroundColor = Color.Transparent;
            //highlight Button Grid
            //BackgroundColor = Color.FromHex("FF0000");

            List<QCommand> commands = new List<QCommand>();
            commands.Add(QCommands.PREVIOUS);
            commands.Add(QCommands.PAUSE);
            commands.Add(QCommands.NEXT);
            commands.Add(QCommands.PREVIEW);
            commands.Add(QCommands.PANIC);
            commands.Add(QCommands.RESUME);
            //setCustomButtons(commands);
            setDefaultButtons();

        }

        void setDefaultButtons()
        {
            List<QButton> buttons = new List<QButton>();

            buttons.Add(new QButton(QCommands.PREVIOUS));
            buttons.Add(new QButton(QCommands.PANIC));
            buttons.Add(new QButton(QCommands.NEXT));
            buttons.Add(new QButton(QCommands.PREVIEW));
            buttons.Add(new QButton(QCommands.PAUSE));
            buttons.Add(new QButton(QCommands.RESUME));

            mainG = new Grid
            {
                Padding = new Thickness(0),
                RowDefinitions = {
                    new RowDefinition{Height = GridLength.Star},
                    new RowDefinition{Height = GridLength.Star},
                    new RowDefinition{Height = GridLength.Star}
                },
                ColumnDefinitions = {
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)}
                },
                Margin = new Thickness(0)
            };

            int row = 0;
            int column = 0;
            for (int i = 0; i < buttons.Count; i++)
            {
                QButton b = buttons[i];
                b.Clicked += sendOSC;
                mainG.Children.Add(b, column, row);
                row++;
                if (row == 3)
                {
                    row = 0;
                    column = 2;
                }
            }

            QButton goButton = new QButton(QCommands.GO);
            goButton.Clicked += sendOSC;
            mainG.Children.Add(goButton, 1, 0);
            Grid.SetRowSpan(goButton, 3);
            Content = mainG;
        }

        public void setCustomButtons(List<QCommand> commands)
        {
            mainG = new Grid
            {
                Padding = new Thickness(0),
                RowDefinitions = {
                    new RowDefinition{Height = GridLength.Auto},
                    new RowDefinition{Height = GridLength.Auto},
                    new RowDefinition{Height = GridLength.Auto}
                },
                ColumnDefinitions = {
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)}
                },
                Margin = new Thickness(0)
            };

            int row = 0;
            int column = 0;
            for (int i = 0; i < commands.Count; i++)
            {
                QButton b = new QButton(commands[i]);
                b.Clicked += sendOSC;
                mainG.Children.Add(b, column, row);
                row++;
                if (row == 3)
                {
                    row = 0;
                    column = 2;
                }
            }

            QButton goButton = new QButton(QCommands.GO);
            goButton.Clicked += sendOSC;
            mainG.Children.Add(goButton, 1, 0);
            Grid.SetRowSpan(goButton, 3);
            Content = mainG;
        }

        void sendOSC(object sender, EventArgs e)
        {
            if (((QButton)sender).qCommand.type == "WORKSPACE")
            {
                string workspace_prefix = "/workspace/" + qController.qWorkspace.workspace_id;
                string command = workspace_prefix + ((QButton)sender).qCommand.osc;
                qController.qClient.sendTCP(command);
            }
        }
    }
}
