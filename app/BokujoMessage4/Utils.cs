using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FastColoredTextBoxNS;

namespace BokujoMessage4
{
    internal class Utils
    {
        internal static int RecountLenght(int lenght)
        {
            int inp0 = lenght % 4;
            return 4 - inp0;
        }

        internal static void BuildAutocompleteMenu(AutocompleteMenu popupMenu)
        {
            List<AutocompleteItem> items = new List<AutocompleteItem>();

            foreach (var item in Program.AConfig.KeyWords)
                items.Add(new AutocompleteItem(item));

            items.Add(new Utils.InsertSpaceSnippet());
            items.Add(new Utils.InsertSpaceSnippet(@"^(\w+)([=<>!:]+)(\w+)$"));
            items.Add(new Utils.InsertEnterSnippet());

            //set as autocomplete source
            popupMenu.Items.SetAutocompleteItems(items);
            popupMenu.SearchPattern = @"[\w\.:=!<>]";
        }

        internal static GameSeries getSeries(int i)
        {
            switch (i)
            {
                case 0:
                    return GameSeries.SOSUS;
                case 1:
                    return GameSeries.SOSJP;
                case 2:
                    return GameSeries.SOSEU;
            }
            return GameSeries.SOSUS;
        }

        internal static void setRandomBackGroundPanel(int randNum, Form form)
        {
            switch (randNum)
            {
                case 0:
                    form.BackgroundImage = Properties.Resources.anb_bg_0;
                    break;
                case 1:
                    form.BackgroundImage = Properties.Resources.anb_bg_1;
                    break;
                case 2:
                    form.BackgroundImage = Properties.Resources.anb_bg_2;
                    break;
                case 3:
                    form.BackgroundImage = Properties.Resources.anb_bg_3;
                    break;
                case 4:
                    form.BackgroundImage = Properties.Resources.anb_bg_4;
                    break;
                case 5:
                    form.BackgroundImage = Properties.Resources.anb_bg_5;
                    break;
                case 6:
                    form.BackgroundImage = Properties.Resources.anb_bg_6;
                    break;
                case 7:
                    form.BackgroundImage = Properties.Resources.anb_bg_7;
                    break;
                case 8:
                    form.BackgroundImage = Properties.Resources.anb_bg_8;
                    break;
                case 9:
                    form.BackgroundImage = Properties.Resources.anb_bg_9;
                    break;
                case 10:
                    form.BackgroundImage = Properties.Resources.anb_bg_10;
                    break;
                case 11:
                    form.BackgroundImage = Properties.Resources.anb_bg_11;
                    break;
                case 12:
                    form.BackgroundImage = Properties.Resources.sos2_bg_1;
                    break;
                case 13:
                    form.BackgroundImage = Properties.Resources.sos2_bg_2;
                    break;
                case 14:
                    form.BackgroundImage = Properties.Resources.sos2_bg_3;
                    break;
                case 15:
                    form.BackgroundImage = Properties.Resources.sos2_bg_4;
                    break;
                case 16:
                    form.BackgroundImage = Properties.Resources.sos2_bg_5;
                    break;
            }
        }

        internal class AlphanumComparatorFast : IComparer
        {
            public int Compare(object x, object y)
            {
                string s1 = x as string;
                if (s1 == null)
                {
                    return 0;
                }
                string s2 = y as string;
                if (s2 == null)
                {
                    return 0;
                }

                int len1 = s1.Length;
                int len2 = s2.Length;
                int marker1 = 0;
                int marker2 = 0;

                // Walk through two the strings with two markers.
                while (marker1 < len1 && marker2 < len2)
                {
                    char ch1 = s1[marker1];
                    char ch2 = s2[marker2];

                    // Some buffers we can build up characters in for each chunk.
                    char[] space1 = new char[len1];
                    int loc1 = 0;
                    char[] space2 = new char[len2];
                    int loc2 = 0;

                    // Walk through all following characters that are digits or
                    // characters in BOTH strings starting at the appropriate marker.
                    // Collect char arrays.
                    do
                    {
                        space1[loc1++] = ch1;
                        marker1++;

                        if (marker1 < len1)
                        {
                            ch1 = s1[marker1];
                        }
                        else
                        {
                            break;
                        }
                    } while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

                    do
                    {
                        space2[loc2++] = ch2;
                        marker2++;

                        if (marker2 < len2)
                        {
                            ch2 = s2[marker2];
                        }
                        else
                        {
                            break;
                        }
                    } while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

                    // If we have collected numbers, compare them numerically.
                    // Otherwise, if we have strings, compare them alphabetically.
                    string str1 = new string(space1);
                    string str2 = new string(space2);

                    int result;

                    if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
                    {
                        int thisNumericChunk = int.Parse(str1);
                        int thatNumericChunk = int.Parse(str2);
                        result = thisNumericChunk.CompareTo(thatNumericChunk);
                    }
                    else
                    {
                        result = str1.CompareTo(str2);
                    }

                    if (result != 0)
                    {
                        return result;
                    }
                }
                return len1 - len2;
            }
        }

        internal class SearchResult
        {
            public Range Range { get; set; }

            public SearchResult(Range range)
            {
                Range = range;
            }

            public override string ToString()
            {
                return Range.tb.Lines[Range.Start.iLine];
            }
        }

        internal class DeclarationSnippet : SnippetAutocompleteItem
        {
            public DeclarationSnippet(string snippet)
                : base(snippet)
            {
            }

            public override CompareResult Compare(string fragmentText)
            {
                var pattern = Regex.Escape(fragmentText);
                if (Regex.IsMatch(Text, "\\b" + pattern, RegexOptions.IgnoreCase))
                    return CompareResult.Visible;
                return CompareResult.Hidden;
            }
        }

        /// <summary>
        /// Divides numbers and words: "123AND456" -> "123 AND 456"
        /// Or "i=2" -> "i = 2"
        /// </summary>
        internal class InsertSpaceSnippet : AutocompleteItem
        {
            string pattern;

            public InsertSpaceSnippet(string pattern)
                : base("")
            {
                this.pattern = pattern;
            }

            public InsertSpaceSnippet()
                : this(@"^(\d+)([a-zA-Z_]+)(\d*)$")
            {
            }

            public override CompareResult Compare(string fragmentText)
            {
                if (Regex.IsMatch(fragmentText, pattern))
                {
                    Text = InsertSpaces(fragmentText);
                    if (Text != fragmentText)
                        return CompareResult.Visible;
                }
                return CompareResult.Hidden;
            }

            public string InsertSpaces(string fragment)
            {
                var m = Regex.Match(fragment, pattern);
                if (m == null)
                    return fragment;
                if (m.Groups[1].Value == "" && m.Groups[3].Value == "")
                    return fragment;
                return (m.Groups[1].Value + " " + m.Groups[2].Value + " " + m.Groups[3].Value).Trim();
            }

            public override string ToolTipTitle
            {
                get
                {
                    return Text;
                }
            }
        }

        /// <summary>
        /// Inerts line break after '}'
        /// </summary>
        internal class InsertEnterSnippet : AutocompleteItem
        {
            Place enterPlace = Place.Empty;

            public InsertEnterSnippet()
                : base("[Line break]")
            {
            }

            public override CompareResult Compare(string fragmentText)
            {
                var r = Parent.Fragment.Clone();
                while (r.Start.iChar > 0)
                {
                    if (r.CharBeforeStart == '}')
                    {
                        enterPlace = r.Start;
                        return CompareResult.Visible;
                    }

                    r.GoLeftThroughFolded();
                }

                return CompareResult.Hidden;
            }

            public override string GetTextForReplace()
            {
                //extend range
                Range r = Parent.Fragment;
                Place end = r.End;
                r.Start = enterPlace;
                r.End = r.End;
                //insert line break
                return Environment.NewLine + r.Text;
            }

            public override void OnSelected(AutocompleteMenu popupMenu, SelectedEventArgs e)
            {
                base.OnSelected(popupMenu, e);
                if (Parent.Fragment.tb.AutoIndent)
                    Parent.Fragment.tb.DoAutoIndent();
            }

            public override string ToolTipTitle
            {
                get
                {
                    return "Insert line break after '}'";
                }
            }
        }

    }
}
