namespace Korzh.NLP
{
    public interface ITextSummarizer 
    {
        string Process(string text, string lang = "en");    
    }


}
