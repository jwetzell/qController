using System;
using System.Collections.Generic;
using qController.Communication;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace qController.Cell
{
    public class QControlsBlock : Frame
    {
        Microsoft.Maui.Controls.Compatibility.Grid mainG;

        EventHandler callback;

        public QControlsBlock(EventHandler callback)
        {
            this.callback = callback;

            Margin = new Thickness(10);
            Padding = new Thickness(0);
            IsVisible = true;


            BackgroundColor = Colors.Transparent;
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

            mainG = new Microsoft.Maui.Controls.Compatibility.Grid
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
                b.Clicked += callback;
                mainG.Children.Add(b, column, row);
                row++;
                if (row == 3)
                {
                    row = 0;
                    column = 2;
                }
            }

            QButton goButton = new QButton(QCommands.GO);
            goButton.Clicked += callback;
            mainG.Children.Add(goButton, 1, 0);
            Microsoft.Maui.Controls.Compatibility.Grid.SetRowSpan(goButton, 3);
            Content = mainG;
        }

        public void setCustomButtons(List<QCommand> commands)
        {
            mainG = new Microsoft.Maui.Controls.Compatibility.Grid
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
                b.Clicked += callback;
                mainG.Children.Add(b, column, row);
                row++;
                if (row == 3)
                {
                    row = 0;
                    column = 2;
                }
            }

            QButton goButton = new QButton(QCommands.GO);
            goButton.Clicked += callback;
            mainG.Children.Add(goButton, 1, 0);
            Microsoft.Maui.Controls.Compatibility.Grid.SetRowSpan(goButton, 3);
            Content = mainG;
        }
    }
}
