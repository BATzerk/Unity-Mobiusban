using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class Colors {
    public static Color GetBeamColor(int channelID) {
        switch (channelID) {
        case 0: return new ColorHSB(128/255f,220/255f,200/255f).ToColor();
        case 1: return new ColorHSB(  5/255f,250/255f,245/255f).ToColor();
        case 2: return new ColorHSB( 28/255f,200/255f,245/255f).ToColor();
        case 3: return new ColorHSB( 58/255f,220/255f,200/255f).ToColor();
        case 4: return new ColorHSB(180/255f,250/255f,200/255f).ToColor();
        case 5: return new ColorHSB(220/255f,150/255f,245/255f).ToColor();
        default: return Color.red; // Oops! Too many colors.
        }
    }
}
