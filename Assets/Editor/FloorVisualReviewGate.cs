#if UNITY_EDITOR
public static class FloorVisualReviewGate
{
 public static void Validate(){FloorValidation.ValidateAll();FloorRunnerArtValidation.CaptureViews();}
}
#endif
