namespace Kamilunavo.OneMoreFloor.Core
{
 public enum ElevatorTutorialStage { Timing, Perfect, Checkpoint, Choice, Risk, Bank, Complete }
 /// <summary>Progression is driven by actual landings and explicit checkpoint decisions.</summary>
 public sealed class ElevatorTutorial
 {
  public ElevatorTutorialStage Stage{get;private set;}
  public bool Complete=>Stage==ElevatorTutorialStage.Complete;
  public void Landing(bool perfect,int floor){if(floor<=0)return;if(Stage==ElevatorTutorialStage.Timing)Stage=ElevatorTutorialStage.Perfect;else if(Stage==ElevatorTutorialStage.Perfect&&perfect)Stage=ElevatorTutorialStage.Checkpoint;else if(Stage==ElevatorTutorialStage.Risk)Stage=ElevatorTutorialStage.Bank;}
  public void AtCheckpoint(){if(Stage==ElevatorTutorialStage.Checkpoint)Stage=ElevatorTutorialStage.Choice;}
  public void Risk(){if(Stage==ElevatorTutorialStage.Choice)Stage=ElevatorTutorialStage.Risk;}
  public void Banked(){if(Stage==ElevatorTutorialStage.Choice||Stage==ElevatorTutorialStage.Bank)Stage=ElevatorTutorialStage.Complete;}
 }
}
