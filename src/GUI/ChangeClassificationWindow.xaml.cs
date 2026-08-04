using System.Collections.Generic;
using System.Windows;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.GUI;

/// <summary>
/// Interaction logic for ChangeClassificationWindow.xaml
/// </summary>
public partial class ChangeClassificationWindow {
    public IPosting Posting {
        get;
        set {
            field = value;
            Date.Text = field.Date.ToShortDateString();
            Amount.Text = field.Amount.ToString("F2");
            Remark.Text = field.Remark;
        }
    }

    public string PostingHash {
        get;
        set {
            field = value;
            Hash.Text = field;
        }
    }

    public string SelectedClassification { get; set; }

    public ChangeClassificationWindow() {
        InitializeComponent();
    }

    public void SetClassificationChoices(List<string> choices) {
        Classification.Items.Clear();
        choices.ForEach(c => Classification.Items.Add(c));
    }

    private void OnSaveButtonClick(object sender, RoutedEventArgs e) {
        SelectedClassification = Classification.SelectedValue as string;
        if (string.IsNullOrEmpty(SelectedClassification)) {
            MessageBox.Show("Please select a classification", Title, MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        DialogResult = true;
    }
}