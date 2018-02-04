# NavigationDrawer YouTube
https://youtu.be/YQ1EJJZBHyE

# Q & A
<h2>How do I implement listviewitem click events and multi view content area?</h2>
To access the list view selected item event you need the code

<code>
  selectionChanged="ListViewMenu_SelectionChanged"
</code>

and in c# code you need to get what item was selected:
```C#
  UserControl usc = null;
  GridMain.Children.Clear();

  switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
  {
      case "ItemHome":
          usc = new UserControlHome();
          GridMain.Children.Add(usc);
          break;
      case "ItemCreate":
          usc = new UserControlCreate();
          GridMain.Children.Add(usc);
          break;
      default:
          break;
  }
```

To create multi view, you will need some UserControls and call them into switch case.
