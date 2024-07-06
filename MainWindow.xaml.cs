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
                File.WriteAllText (saveFileDialog.FileName, "Password Length: " +  PasswordLength.Text.ToString() + "\n" + "Password: " + GeneratedPassword.Text.ToString() + "\n" + PasswordStrength.Text.ToString() + "\n" + CrackTime.Text.ToString() + "\n");
            }
        }

        private void PasswordChanged ( object sender, TextChangedEventArgs e )
        {
            string password = ((TextBox)sender).Text;
            string strength = EvaluatePasswordStrength (password);
            PasswordStrength.Text = "Your password is: " + strength;

            string timeToCrack = EstimateTimeToCrack(password);
            CrackTime.Text = $"Estimated time to crack: {timeToCrack}";
        }

        public static string EvaluatePasswordStrength ( string password )
        {
            int score = 0;

            if (password.Length <= 1) return "Very Weak";

            if (password.Length < 4) return "Weak";

            if (password.Length >= 8) score++;
            if (password.Length >= 12) score++;
            if (password.Length >= 16) score++;
            if (password.Length >= 20) score++;

            if (password.Any(char.IsLetter)) score++;
            if (password.Any(char.IsDigit)) score++;
            if (password.Any(char.IsSymbol)) score++;
            if (password.Any (ch => !char.IsLetterOrDigit (ch))) score++;

                return score switch
            {
                0 => "Very Weak",
                1 => "Weak",
                2 => "Moderate",
                3 => "Strong",
                4 => "Very Strong",
                _ => "Impossible Strong",
            };
        }

        public static string EstimateTimeToCrack ( string password )
        {
            long complexity = 1;
            if (password.Any (char.IsLower)) complexity *= 26;
            if (password.Any (char.IsUpper)) complexity *= 26;
            if (password.Any (char.IsDigit)) complexity *= 10;
            if (password.Any (ch => !char.IsLetterOrDigit (ch))) complexity *= 32; 

            long attemptsPerSecond = 100000; 

            long seconds = (long)Math.Pow(complexity, password.Length) / attemptsPerSecond;

            if ((seconds < 60) && (seconds > 0)) return "Less than a minute";
            if ((seconds < 3600) && (seconds > 0)) return $"{seconds / 60} minutes";
            if ((seconds < 86400) && (seconds > 0)) return $"{seconds / 3600} hours";
            if ((seconds < 2592000) && (seconds > 0)) return $"{seconds / 86400} days";
            if ((seconds < 31104000) && (seconds > 0)) return $"{seconds / 2592000} months";
            return $"{Math.Abs(seconds) / 31104000} years";
        }

        private string GeneratePassword ( int length, bool includeUppercase, bool includeNumbers, bool includeSymbols )
        {
            if (length < 1)
            {
                MessageBox.Show ("Password length must be at least 1.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return "";
            }

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