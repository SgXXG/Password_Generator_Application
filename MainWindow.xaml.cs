using Microsoft.Win32;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Password_Generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Random random = new Random();

        private static readonly char[] UppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private static readonly char[] LowercaseLetters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        private static readonly char[] Digits = "0123456789".ToCharArray();
        private static readonly char[] Symbols = "!@#$%^&*()_-+=[]{}|;:,.<>?".ToCharArray();

        public MainWindow ()
        {
            InitializeComponent ();
        }

        private void GeneratePassword_Click ( object sender, RoutedEventArgs e )
        {
            int length = int.TryParse(PasswordLength.Text, out int len) ? len : 8; // Default to 8 if parsing fails
            bool includeUppercase = IncludeUppercase.IsChecked ?? false;
            bool includeNumbers = IncludeNumbers.IsChecked ?? false;
            bool includeSymbols = IncludeSymbols.IsChecked ?? false;

            GeneratedPassword.Text = GeneratePassword (length, includeUppercase, includeNumbers, includeSymbols);
        }

        private void SaveToFile_Click ( object sender, RoutedEventArgs e )
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog ();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText (saveFileDialog.FileName, "Password Length: " +  PasswordLength.Text.ToString() + "\n" + "Password: " + GeneratedPassword.Text.ToString() + "\n");
            }
        }

        private string GeneratePassword ( int length, bool includeUppercase, bool includeNumbers, bool includeSymbols )
        {
            if (length < 1) throw new ArgumentException ("Password length must be at least 1.", nameof (length));

            var charPool = new StringBuilder();
            charPool.Append (LowercaseLetters);


            if (includeUppercase) charPool.Append (UppercaseLetters);
            if (includeNumbers) charPool.Append (Digits);
            if (includeSymbols) charPool.Append (Symbols);

            var poolArray = charPool.ToString().ToCharArray();
            var passwordChars = new char[length];

            using (var rng = RandomNumberGenerator.Create ())
            {
                byte[] randomBytes = new byte[length];
                rng.GetBytes (randomBytes); // Fill the array with cryptographically secure random bytes.

                for (int i = 0; i < length; i++)
                {
                    int poolIndex = randomBytes[i] % poolArray.Length; // Use modulo to map the byte value to a valid index.
                    passwordChars[i] = poolArray[poolIndex];
                }
            }

            return new string (passwordChars);
        }
    }

}