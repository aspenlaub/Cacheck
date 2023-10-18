using System.Collections.Generic;
using System.Windows;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.GUI;

/// <summary>
/// Interaction logic for ChangeClassificationWindow.xaml
/// </summary>
public partial class ChangeClassificationWindow {
    private IPosting _Posting;
    public IPosting Posting {
        get => _Posting;
        set {
            _Posting = value;
            Date.Text = _Posting.Date.ToShortDateString();
            Amount.Text = _Posting.Amount.ToString("F2");
            Remark.Text = _Posting.Remark;
        }
    }

    private string _PostingHash;
    public string PostingHash {
        get => _PostingHash;
        set {
            _PostingHash = value;
            Hash.Text = _PostingHash;
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