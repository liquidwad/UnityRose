using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public struct StateParams
{
	public bool sitting;
	public bool standing;
	public bool walking;
	public bool targetLocked;
	public bool attacking;
	public bool inRangeOfTarget;
}


public class StateConnection
{
	public State nextState;
	private string paramName;
	
	public bool condition(StateParams stateParams)
	{
		Type type = typeof(StateParams);
		bool value = (bool)type.GetField(paramName).GetValue(stateParams);
		return value;	
	} 
	
	public StateConnection(State nextState, string parameter)
	{
		this.nextState = nextState;
		this.paramName = parameter;
		
	}
}


public class State
{
	
	public string Name { get; set; }
	public GameObject gameObject { get; set; }
	public StateParams stateParams { get; set; }
	protected string clipName { get; set; }
	public List<StateConnection> connections { get; set; }
	protected Animation animation;
	protected WrapMode wrapMode;
	
	public State(string name, GameObject gameObject, WrapMode wrapMode = WrapMode.Loop)
	{
		clipName = Name = name;
		this.gameObject = gameObject;
		this.wrapMode = wrapMode;
		connections = new List<StateConnection>();
		animation = gameObject.GetComponent<Animation>(); // TODO: add exception if no animation	
	}
	
	public virtual void Exit(bool crossFade = false)
	{
		if( !crossFade )
			animation.Stop(clipName);
	}
	
	public virtual void Entry(bool crossFade = false)
	{
		animation = gameObject.GetComponent<Animation>();
		if(animation)
		{
			animation.wrapMode = wrapMode;
			if(crossFade)
				animation.CrossFade(clipName);
			else
				animation.Play(clipName);
		}
	}
	
	public virtual State Evaluate(StateParams stateParams) { 
		foreach(StateConnection connection in connections)
		{
			if(connection.condition(stateParams))
				return connection.nextState;
		}
		
		return this; 
	}
	
	
}

public class TransitionState : State
{
	private State nextState;
	public TransitionState(string name, State nextState, GameObject gameObject)
		:base(name, gameObject, WrapMode.Once)
	{
		this.nextState = nextState;
	}
	
	public override State Evaluate(StateParams stateParams)
	{
		if(animation.IsPlaying(clipName))
			return this;
		else
			return nextState;
	}
}

public class MultiAnimationState : State
{
	private List<string> clips;
	int clipIndex;
	bool randomize;
	public MultiAnimationState( List<string> clips, GameObject gameObject, bool randomize = true)
		:base("", gameObject, WrapMode.Once)
	{
		this.clips = clips;
		this.randomize = randomize;
		
		if( randomize )
			clipIndex = UnityEngine.Random.Range(0, clips.Count);
		else
			clipIndex = 0;
			
		clipName = clips[clipIndex];
	}
	
	public override State Evaluate(StateParams stateParams)
	{
		State result = base.Evaluate(stateParams);
		if( result != this )
			return result;
			
		if( !animation.IsPlaying(clipName) )
		{
			if( randomize )
				clipIndex = UnityEngine.Random.Range(0, clips.Count);
			else
				clipIndex = (clipIndex + 1)%clips.Count;
				
			clipName = clips[clipIndex]; 
			animation.Play(clipName);
		}
		
		return this; 
	}
}

public class AttackMachine: State
{
	private Dictionary<string, State> states;
	private State currentState;
	
	public AttackMachine(string name, GameObject gameObject)
		:base( name, gameObject )	
	{
	}
	
	public override void Entry( bool crossFade = false )
	{
		List<string> defaultAttacks = new List<string>(3);
		defaultAttacks[0] = "attack0";
		defaultAttacks[1] = "attack1";
		defaultAttacks[2] = "attack2";
		states.Add ("attack1", new MultiAnimationState(defaultAttacks, gameObject));
		states.Add ("attack2", new State("standing", gameObject));
		states.Add ("attack3", new State("standing", gameObject));
		states.Add ("skill1", new State("standing", gameObject));
		states.Add ("skill2", new State("standing", gameObject));
		states.Add ("skill3", new State("standing", gameObject));
		states.Add ("skill4", new State("standing", gameObject));
		states.Add ("skill5", new State("standing", gameObject));
		states.Add ("skill6", new State("standing", gameObject));
		states.Add ("skill7", new State("standing", gameObject));
		states.Add ("skill8", new State("standing", gameObject));
		
		
	}
	
}
public class PlayerState : State
{
	private Dictionary<string, State> states;
	private State currentState;
	
	public PlayerState(string name, GameObject gameObject)
		:base(name, gameObject)
	{
	}
	
	public override void Entry(bool crossFade = false)
	{
		// Generate list of states
		states = new Dictionary<string, State>();
		states.Add ("standing", new State("standing", gameObject));
		states.Add ("stand", new TransitionState("stand", states["standing"], gameObject));
		states.Add ("walk", new State("walk", gameObject));
		states.Add ("standWalk", new TransitionState("stand", states["walk"], gameObject));
		states.Add ("sitting", new State("sitting", gameObject));
		states.Add ("sit", new TransitionState("sit", states["sitting"], gameObject));
		states.Add ("AttackMachine", new State("attack", gameObject));
		states.Add ("standAttack", new TransitionState("stand", states["AttackMachine"], gameObject));
		
		
		states["standing"].connections.Add (new StateConnection(states["walk"], "walking"));
		states["standing"].connections.Add (new StateConnection(states["sit"], "sitting"));
		states["standing"].connections.Add (new StateConnection(states["AttackMachine"], "targetLocked"));
		
		states["sitting"].connections.Add ( new StateConnection(states["standWalk"], "walking"));
		states["sitting"].connections.Add ( new StateConnection(states["stand"], "standing"));
		states["sitting"].connections.Add ( new StateConnection(states["standAttack"], "targetLocked"));
		
		states["walk"].connections.Add ( new StateConnection(states["standing"], "standing"));
		states["walk"].connections.Add ( new StateConnection(states["AttackMachine"], "targetLocked"));
		
		
		states["AttackMachine"].connections.Add ( new StateConnection(states["standing"], "standing"));
		states["AttackMachine"].connections.Add ( new StateConnection(states["walk"], "walking"));
		
		/*
		// Add state connection
		states["idle"].connections.Add (new StateConnection(states["walk"], "walking"));
		states["idle"].connections.Add (new StateConnection(states["sitting"], "sitting"));
		states["idle"].connections.Add (new StateConnection(states["AttackMachine"], "targetLocked"));
		
		states["standing"].connections.Add (new StateConnection(states["walk"], "walking"));
		states["standing"].connections.Add (new StateConnection(states["sitting"], "sitting"));
		states["standing"].connections.Add (new StateConnection(states["AttackMachine"], "targetLocked"));
		
		states["walk"].connections.Add (new StateConnection(states["idle"], "standing"));
		states["walk"].connections.Add (new StateConnection(states["AttackMachine"], "targetLocked"));
		
		states["standWalk"].connections.Add (new StateConnection(states["idle"], "standing"));
		states["standWalk"].connections.Add (new StateConnection(states["AttackMachine"], "targetLocked"));
		
		states["sitting"].connections.Add (new StateConnection(states["standing"], "standing"));
		states["sitting"].connections.Add (new StateConnection(states["standWalk"], "walking"));
		
		states["AttackMachine"].connections.Add (new StateConnection(states["walk"], "walking"));
		states["AttackMachine"].connections.Add (new StateConnection(states["idle"], "standing"));
		states["AttackMachine"].connections.Add (new StateConnection(states["sitting"], "sitting"));
		*/
		
		currentState = states["standing"];
		currentState.Entry();
	}
	
	public override void Exit(bool crossFade = false)
	{
		currentState.Exit( crossFade );
	}
	
	public override State Evaluate(StateParams stateParams)
	{	
		State result = currentState.Evaluate(stateParams);
		
		if(result != currentState)
		{
			currentState.Exit(true);
			currentState = result;
			currentState.Entry(true);
		}	
		
		return this;
	}
	
}
