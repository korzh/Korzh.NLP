namespace Korzh.NLP
{
    public interface INlpServiceProvider
    {
        ILanguageDetector GetLanguageDetector();

        IStopWordFilter GetStopWordFilter(string lang);

        IStemmer GetStemmer(string lang);

        INamedEntitiesExtractor GetNamedEntitiesExtractor(string lang);

        IQuestionGenerator GetQuestionGenerator(string lang);
    }
}
