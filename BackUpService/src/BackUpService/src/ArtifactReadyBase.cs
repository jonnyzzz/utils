namespace BackUpService
{
  public delegate void ArtifactReady(long fileId, string file, bool temporaty);

  public class ArtifactReadyBase
  {
    public event ArtifactReady OnArtifact;

    public void FireAtrifact(long fileId, string file, bool temporary)
    {
      if (OnArtifact != null)
      {
        OnArtifact(fileId, file, temporary);
      }
    }    
  }
}