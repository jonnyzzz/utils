namespace EugenePetrenko.BibParser.BibTex
{
  public enum BibRecordType
  {
    [RequiredFields(BibField.Author, BibField.Journal, BibField.Pages, BibField.Title, BibField.Volume, BibField.Year)]
    [OptionalFields(BibField.Language)]
    ARTICLE

  }
}