using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace qController
{
    public class QCueListView : Frame
    {
        ListView listView;
        public QCueListView()
        {
            Padding = new Thickness(5);
            CornerRadius = 20;
            BackgroundColor = Color.FromHex("D8D8D8");
            Grid mainG = new Grid
            {
                Padding = new Thickness(0),
                RowDefinitions =
                {
                    new RowDefinition{Height = GridLength.Star}
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition{Width = new GridLength(1,GridUnitType.Star)}
                }
            };
            listView = new ListView();
            mainG.Children.Add(listView, 0, 0);
            Margin = new Thickness(10);
            Content = mainG;
        }

        public void UpdateWithWorkspace(QWorkSpace qWorkspace)
        {
            List<string> cueLists = new List<string>();
            foreach( var cueList in qWorkspace.data )
            {
                cueLists.Add(cueList.listName);
                foreach(var cue in cueList.cues)
                {
                    if(cue.number == "")
                        cueLists.Add("\t" + cue.listName);
                    else
                        cueLists.Add("\t" + cue.number + " - " + cue.listName);
                }
            }
            listView.ItemsSource = cueLists.ToArray();
        }
    }
}