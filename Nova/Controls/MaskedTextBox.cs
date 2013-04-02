#region License

// 
//  Copyright 2013 Steven Thuriot
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 

#endregion

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Nova.Controls
{
    public class MaskedTextBox : TextBox
    {
        private MaskedTextProvider _MaskedTextProvider;

        /// <summary>
        /// Initializes the <see cref="MaskedTextBox" /> class.
        /// </summary>
        static MaskedTextBox()
        {
            var metaData = new FrameworkPropertyMetadata {CoerceValueCallback = CoerceTextValueCallback};
            TextProperty.OverrideMetadata(typeof (MaskedTextBox), metaData);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaskedTextBox" /> class.
        /// </summary>
        public MaskedTextBox()
        {
            //Disable Cut and Paste.
            CommandBindings.Add(CreateCommandBinding(ApplicationCommands.Paste));
            CommandBindings.Add(CreateCommandBinding(ApplicationCommands.Cut));
        }

        private static CommandBinding CreateCommandBinding(RoutedUICommand routedUICommand)
        {
            CanExecuteRoutedEventHandler canExecuteRoutedEventHandler = (sender, args) =>
            {
                args.CanExecute = false;
                args.Handled = true;
            };

            return new CommandBinding(routedUICommand, null, canExecuteRoutedEventHandler);
        }

        /// <summary>
        /// The mask property
        /// </summary>
        public static readonly DependencyProperty MaskProperty = DependencyProperty.Register("Mask", typeof(string), typeof(MaskedTextBox), new PropertyMetadata(null, MaskChanged));

        /// <summary>
        /// Gets or sets the mask.
        /// </summary>
        /// <value>
        /// The mask.
        /// </value>
        public string Mask
        {
            get { return (string)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }


        /// <summary>
        /// The mask has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void MaskChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var maskedTextBox = (MaskedTextBox)sender;

            var maskedTextProvider = new MaskedTextProvider((string)e.NewValue);
            maskedTextProvider.Set(maskedTextBox.Text);

            maskedTextBox._MaskedTextProvider = maskedTextProvider;
            maskedTextBox.Refresh(0);
        }


        /// <summary>
        /// Coerces the text value callback.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static object CoerceTextValueCallback(DependencyObject sender, object value)
        {
            var maskedTextProvider = ((MaskedTextBox)sender)._MaskedTextProvider;

            if (maskedTextProvider == null)
            {
                return value;
            }

            maskedTextProvider.Set((string)value);

            return maskedTextProvider.LastAssignedPosition == -1
                       ? string.Empty //Default TextProperty.
                       : maskedTextProvider.ToDisplayString();
        }

        /// <summary>
        ///     override this method to replace the characters enetered with the mask
        /// </summary>
        /// <param name="e">Arguments for event</param>
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            var position = SelectionStart;

            var length = Text.Length;
            if (position < length || length == 0)
            {
                position = FindCharacterPosition(position);

                if (Keyboard.IsKeyToggled(Key.Insert))
                {
                    if (_MaskedTextProvider.Replace(e.Text, position))
                        position++;
                }
                else
                {
                    if (_MaskedTextProvider.InsertAt(e.Text, position))
                        position++;
                }

                position = FindCharacterPosition(position);
            }

            Refresh(position);
            e.Handled = true;
        }

        /// <summary>
        ///     override the key down to handle delete of a character
        /// </summary>
        /// <param name="e">Arguments for the event</param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            var position = SelectionStart;
            var selectionlength = SelectionLength;

            var endposition = (selectionlength == 0) ? position : position + selectionlength - 1;

            switch (e.Key)
            {
                case Key.Delete:
                    HandleDeleteKey(position, endposition);
                    e.Handled = true;
                    break;
                case Key.Back:
                    HandleBackKey(position, selectionlength, endposition);
                    e.Handled = true;
                    break;
                case Key.Space:
                    HandleSpaceKey(position); //Because space doesn't trigger OnPreviewTextInput
                    e.Handled = true;
                    break;
            }
        }

        private void HandleBackKey(int position, int selectionlength, int endposition)
        {
            if (position < 0) return;

            if (selectionlength == 0) //Single char
            {
                position = FindCharacterPosition(--position, false);

                if (_MaskedTextProvider.RemoveAt(position))
                {
                    Refresh(position);
                }

                return;
            }

            //Multi char
            if (_MaskedTextProvider.RemoveAt(position, endposition))
            {
                position = FindCharacterPosition(position, false);
                Refresh(position);
            }
        }

        private void HandleSpaceKey(int position)
        {
            if (!_MaskedTextProvider.InsertAt(" ", position)) return;

            Refresh(position);
        }

        private void HandleDeleteKey(int position, int endposition)
        {
            if (position >= Text.Length) return;
            if (!_MaskedTextProvider.RemoveAt(position, endposition)) return;

            Refresh(position);
        }

        private void Refresh(int position)
        {
            Text = _MaskedTextProvider.ToDisplayString();
            SelectionStart = position;
        }

        private int FindCharacterPosition(int startPosition, bool direction = true)
        {
            var position = _MaskedTextProvider.FindEditPositionFrom(startPosition, direction);
            return position == -1 ? startPosition : position;
        }
    }
}