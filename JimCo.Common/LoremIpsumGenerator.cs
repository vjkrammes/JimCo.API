using System.Text;

using JimCo.Common.Interfaces;

namespace JimCo.Common;
public class LoremIpsumGenerator : ILoremIpsumGenerator
{
  private const string _source =
      @"lorem ipsum dolor sit amet consectetur adipiscing elit sed do eiusmod tempor incididunt ut labore et dolore magna 
                  aliqua ut enim ad minim veniam quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat duis 
                  aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur excepteur sint 
                  occaecat cupidatat non proident sunt in culpa qui officia deserunt mollit anim id est laborum";

  private static readonly string[] _words;

  static LoremIpsumGenerator() => _words = _source
    .Replace('\r', ' ')
    .Replace('\n', ' ')
    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
    .Distinct()
    .ToArray();

  public string Generate(int minWords, int maxWords, int minSentences = 1, int maxSentences = 1, int paragraphs = 1)
  {
    var random = new Random();
    if (minWords > maxWords)
    {
      (minWords, maxWords) = (maxWords, minWords);
    }
    if (minSentences > maxSentences)
    {
      (minSentences, maxSentences) = (maxSentences, minSentences);
    }
    var numSentences = minSentences == maxSentences ? minSentences : random.Next(minSentences - 1, maxSentences) + 1;
    var numWords = minWords == maxWords ? minWords : random.Next(minWords - 1, maxWords) + 1;
    var sb = new StringBuilder();
    for (var p = 0; p < paragraphs; p++)
    {
      for (var s = 0; s < numSentences; s++)
      {
        for (var w = 0; w < numWords; w++)
        {
          if (w != 0)
          {
            sb.Append(' ');
          }
          var word = _words[random.Next(_words.Length)];
          sb.Append(w == 0 ? word.Capitalize() : word);
        }
        sb.Append(". ");
      }
    }
    return sb.ToString();
  }
}
