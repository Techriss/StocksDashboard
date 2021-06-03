using System;
using System.Windows;
using FreakinStocksUI.Models;

namespace FreakinStocksUI.ViewModels
{
    /// <summary>
    /// Logic for an information or confirmation Prompt
    /// </summary>
    class PromptViewModel : ViewModelBase
    {
        /// <summary>
        /// The prompt's header in the top-left corner with general information
        /// </summary>
        public string Header { get; init; }

        /// <summary>
        /// The main content of the prompt. The prompt should adjust to fit the content.
        /// </summary>
        public string Content { get; init; }

        /// <summary>
        /// The end result set when the user confirms or closes a prompt
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the user approved the confirmation, <see langword="false"/> when not approved, closed or the prompt was for information
        /// </value>
        public bool Result { get; private set; }

        /// <summary>
        /// Represents whether the prompt is informative or a confirmation
        /// </summary>
        public bool Confirmation { get; set; }

        public Visibility ConfirmButtonsVisibility => Confirmation ? Visibility.Visible : Visibility.Collapsed;
        public Visibility OKButtonsVisibility => !Confirmation ? Visibility.Visible : Visibility.Collapsed;


        /// <summary>
        /// Closes the prompt setting the dialog result to <see langword="false"/>
        /// </summary>
        public RelayCommand CloseCommand => new(() =>
        {
            (Source as Window).DialogResult = false;
            (Source as Window).Close();
        });

        /// <summary>
        /// Closes the prompt setting the dialog result to <see langword="true"/> and approves the action
        /// </summary>
        public RelayCommand ConfirmCommand => new(() =>
        {
            (Source as Window).DialogResult = true;
            (Source as Window).Close();
        });



        /// <summary>
        /// Creates an instance of interaction logic for a prompt
        /// </summary>
        /// <param name="header">The top-left corner prompt header with general information</param>
        /// <param name="content">The main prompt content</param>
        /// <param name="source">The prompt view</param>
        /// <param name="confirmation">Whether the prompt is only informative or a confirmation dialog</param>
        public PromptViewModel(string header, string content, Window source, bool confirmation = false)
        {
            this.Source = source;
            this.Header = header;
            this.Content = content;
            this.Confirmation = confirmation;
        }

        /// <summary>
        /// Creates an instance of interaction logic for a prompt notifying of an unexpected error
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/> which Occured</param>
        /// <param name="source">The prompt view</param>
        public PromptViewModel(Exception ex, Window source)
        {
            this.Source = source;
            this.Header = "Unexpected Error";
            this.Content = $"An exception has occured: { ex }";
            this.Confirmation = false;
        }
    }
}