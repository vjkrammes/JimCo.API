namespace JimCo.Common.Interfaces;
public interface ILoremIpsumGenerator
{
  string Generate(int minwords, int maxwords, int minsentences, int maxsentences, int paragraphs);
}
