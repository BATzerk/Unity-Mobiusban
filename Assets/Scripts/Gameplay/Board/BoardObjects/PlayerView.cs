using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerView : BoardOccupantView {
    // Components
    [SerializeField] private Image i_body=null;
    [SerializeField] private ParticleSystem ps_blowUp=null;
	// References
	public Player MyPlayer { get; private set; }


	// ----------------------------------------------------------------
	//  Initialize
	// ----------------------------------------------------------------
	override public void Initialize (BoardView _myBoardView, BoardObject bo) {
		MyPlayer = bo as Player;
		base.Initialize (_myBoardView, bo);
	}

    // ----------------------------------------------------------------
    //  Update Visuals
    // ----------------------------------------------------------------
    public override void UpdateVisualsPostMove() {
        base.UpdateVisualsPostMove();
        
        if (MyPlayer.IsDead) {
            OnBlowUp();
        }
    }
    
    // ----------------------------------------------------------------
    //  Doers
    // ----------------------------------------------------------------
    private void OnBlowUp() {
        StartCoroutine (BlowUpWithDelayCoroutine());
    }
    // This is a quick hack-in to get a blow-up delay. Fine for our purposes now.
    private IEnumerator BlowUpWithDelayCoroutine () {
        yield return new WaitForSeconds (0.1f);

        // Particle burst!
        //Beam beamThatKilledMe = MyPlayer.BeamThatKilledMe;
        //Color beamColor = Colors.GetBeamColor (beamThatKilledMe.ChannelID);
        //GameUtils.SetParticleSystemColor (ps_blowUp, beamColor);
        ps_blowUp.Emit (12);
        // Hide my body!
        i_body.enabled = false;
    }
    

}

