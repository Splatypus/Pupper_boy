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
        //if turned off, do no particles
        ParticleSystem ps = worldObject.GetComponentInChildren<ParticleSystem>();
        //check is system is null
        if (ps == null) {
            return;
        }
        if (state == PowerStates.Off)
        {
            ps.Stop();
        }
        //if turned on change particles to the right color
        else
        {
            ps.Play();
            ParticleSystem.MainModule module = ps.main;
            if (state == PowerStates.Blue)
                module.startColor = new Color(0.0f, 0.0f, 1.0f);
            else if (state == PowerStates.Red)
                module.startColor = new Color(1.0f, 0.0f, 0.0f);
        }   
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
        //state = PowerStates.Off;
        SetState(PowerStates.Off);
        foreach (Gamepiece g in adjacents)
        {
            if (g.IsPowered())
                SetState(g.state);
        }
    }
}


//takes two input. If either input is the prefered state, it supplies that power. If both are another power, it supplies that instead
public class ColorGate : Gamepiece {
    public PowerStates preferedState;

    public ColorGate(GameObject _inWorld, bool _locked, PowerStates _prefState){
        preferedState = _prefState;
        worldObject = _inWorld;
        isLocked = _locked;
    }

    //you can always place a color gate unless the output slot is already powered
    public override bool CanPlace(Gamepiece[] adjacents){
        return !(adjacents[0].IsPowered() || adjacents[2].IsPowered());
    }

    //only powered with 2 power inputs. State is prefered state unless both inputs are the other color
    public override void SetInitialPowerState(Gamepiece[] adjacents){
        if (adjacents[1].state == preferedState || adjacents[3].state == preferedState) {
            SetState(preferedState);
        } else if (adjacents[1].state == adjacents[3].state && adjacents[1].state != PowerStates.Off) {
            SetState(adjacents[1].state);
        } else {
            SetState(PowerStates.Off);
        }
    }
}


//takes one input and supplies inverted power
public class Inverter : Gamepiece {
    public Inverter(GameObject _inWorld, bool _locked) {
        worldObject = _inWorld;
        isLocked = _locked;
        direction = 0;
    }

    //requires only one powered
    public override bool CanPlace(Gamepiece[] adjacents){
        int numPowered = 0;
        foreach (Gamepiece g in adjacents){
            if (g.IsPowered())
                numPowered++;
        }
        return numPowered <= 1;
    }

    //there should at most be one powered thing adjacent to this. Set it to the opposite state
    public override void SetInitialPowerState(Gamepiece[] adjacents)
    {
        //if theres an adjacent powered node, we want to match that
        //state = PowerStates.Off;
        SetState(PowerStates.Off);
        foreach (Gamepiece g in adjacents){
            if (g.IsPowered()){
                if (g.state == PowerStates.Red) {
                    SetState(PowerStates.Blue);
                } else if(g.state == PowerStates.Blue) {
                    SetState(PowerStates.Red);
                }
            }
        }
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

    //Cannot change the state of a source node
    public override void SetState(PowerStates newstate){        
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
