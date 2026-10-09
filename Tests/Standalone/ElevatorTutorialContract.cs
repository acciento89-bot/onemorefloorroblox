using System;
using Kamilunavo.OneMoreFloor.Core;
class ElevatorTutorialContract
{
 static int checks;
 static void Check(bool condition,string message){checks++;if(!condition)throw new Exception(message);}
 static void Main(){
  var t=new ElevatorTutorial();Check(t.Stage==ElevatorTutorialStage.Timing,"starts with timing");
  t.Landing(false,1);Check(t.Stage==ElevatorTutorialStage.Perfect,"first actual landing teaches perfect next");
  t.Landing(false,2);Check(t.Stage==ElevatorTutorialStage.Perfect,"normal landing cannot pass perfect lesson");t.AtCheckpoint();Check(t.Stage==ElevatorTutorialStage.Perfect,"checkpoint cannot skip actual perfect lesson");
  t.Landing(true,3);Check(t.Stage==ElevatorTutorialStage.Checkpoint,"actual perfect advances to checkpoint lesson");
  t.AtCheckpoint();Check(t.Stage==ElevatorTutorialStage.Choice,"checkpoint enables bank or risk lesson");
  t.Risk();Check(t.Stage==ElevatorTutorialStage.Risk,"explicit risk choice advances");
  t.Landing(true,5);Check(t.Stage==ElevatorTutorialStage.Bank,"actual risk landing teaches next checkpoint banking");
  t.AtCheckpoint();Check(t.Stage==ElevatorTutorialStage.Bank,"second checkpoint retains bank lesson");
  t.Banked();Check(t.Complete,"bank completes real practice");
  t=new ElevatorTutorial();t.Risk();Check(t.Stage==ElevatorTutorialStage.Timing,"out of order risk ignored");t.Banked();Check(!t.Complete,"out of order banking cannot complete");
  t.Landing(true,1);Check(t.Stage==ElevatorTutorialStage.Perfect,"first perfect still gives distinct perfect lesson");t.Landing(true,2);t.AtCheckpoint();t.Banked();Check(t.Complete,"safe bank branch completes without forced risk");
  Console.WriteLine("ELEVATOR_TUTORIAL_CONTRACT_PASS checks="+checks);
 }
}
