using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamepiece {
    
    public enum PowerStates { Red, Blue, Off};
    public PowerStates state = PowerStates.Off;
    public bool isLocked = false; //if the player can move this object
    public int direction = 0; //the direction this piece is facing. 0 is north, going clockwise to 3 being west
    public GameObject worldObject;

    //if this piece can be placed based on what is adjacent to it. 
    //Adjacents is an array of the 4 adjacent pieces, starting with the one north of this one and going clockwise
    public virtual bool CanPlace(Gamepiece[] adjacents) {
        return true;
    }


    //sets the power state of this node based on adjacent ones. Usually called right after it's placed, or when the power state of adjacent nodes change
    public virtual void SetInitialPowerState(Gamepiece[] adjacents) {

    }

    //if there is anything connected to this piece
    public virtual bool IsPowered() {
        return state != PowerStates.Off;
    }

    //sets the power state of the node. Changes its color or whatever it needs to do too
    public virtual void SetState(PowerStates newstate) {
        state = newstate;
    }
}


//node that has nothing that is sent with a bounds issue or an empty space. Is always unpowered. Stops me from having to check for null every time I look at adjacent nodes
public class EmptyNode : Gamepiece {
    public EmptyNode() {
        state = PowerStates.Off;
        isLocked = true;
        direction = 0;
    }
}


//the standard node
public class BlackieNode : Gamepiece{
    public BlackieNode(GameObject _inWorld, bool _locked) {
        worldObject = _inWorld;
        isLocked = _locked;
        direction = 0;
    }


    //can only be places if no more than one adjacent node is powered
    public override bool CanPlace(Gamepiece[] adjacents)
    {
        int numPowered = 0;
        foreach (Gamepiece g in adjacents)
        {
            if (g.IsPowered())
                numPowered++;
        }
        return numPowered <= 1;
    }

    //there should only be at most one powered node next to this one when this is called. Set it to the same power state as that one, or to off if there isnt another powered one
    public override void SetInitialPowerState(Gamepiece[] adjacents)
    {
        //if theres an adjacent powered node, we want to match that
        state = PowerStates.Off;
        foreach (Gamepiece g in adjacents)
        {
            if (g.IsPowered())
                SetState(g.state);
        }
    }
}


//takes two input and one output. If either input is the prefered state, so is the output.
public class ColorGate : Gamepiece {
    public PowerStates preferedState;

    public ColorGate(GameObject _inWorld, bool _locked, PowerStates _prefState){
        preferedState = _prefState;
        worldObject = _inWorld;
        isLocked = _locked;
    }

}


//takes one input and one output. Output is opposite of input
public class Inverter : Gamepiece {
    public Inverter(GameObject _inWorld, bool _locked) {
        worldObject = _inWorld;
        isLocked = _locked;
        direction = 0;
    }

}


//source of power
public class SourceNode : Gamepiece {
    public SourceNode(GameObject _inWorld, PowerStates _color) {
        worldObject = _inWorld;
        isLocked = true;
        direction = 0;
        state = _color;
    }
}


//goal node. Supply these with power of the right color to win
public class GoalNode : Gamepiece {
    public GoalNode(GameObject _inWorld, PowerStates _color) {
        worldObject = _inWorld;
        isLocked = true;
        direction = 0;
        powerColor = _color;
        state = PowerStates.Off;
    }
    //the desired color for it to be
    public PowerStates powerColor;

    //Goals will never supply power. Always return false here
    public override bool IsPowered() {
        return false;
    }

    //If there is an adjacent thing powered with the right color, then this powers
    public override void SetInitialPowerState(Gamepiece[] adjacents)
    {
        //if theres an adjacent powered node, we want to match that
        state = PowerStates.Off;
        foreach (Gamepiece g in adjacents)
        {
            if (g.IsPowered() && g.state == powerColor)
                SetState(g.state);
        }
    }
}
