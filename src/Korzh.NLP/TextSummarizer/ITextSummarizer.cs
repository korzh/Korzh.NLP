namespace Korzh.NLP
{
    public interface ITextSummarizer 
    {
        string Summarize(string text, string lang = "en");    
    }
}
