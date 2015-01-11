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
		return value == true;	
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
	private string transitionClip;
	private bool transition;
	
	public override void Entry (bool crossFade = false)
	{
		transition = true;
		animation.wrapMode = WrapMode.Once;
		if( crossFade )
			animation.CrossFade( transitionClip );
		else
			animation.Play (transitionClip);
	}
	
	public override State Evaluate(StateParams stateParams)
	{
		if(animation.IsPlaying(transitionClip))
			return this;
		else if(transition)
		{
			animation.Stop (transitionClip);
			transition = false;
			animation.wrapMode = wrapMode;
			animation.Play (clipName);
			return this;
		}
		else
			return base.Evaluate(stateParams);	
	}
	
	public override void Exit( bool crossFade = false)
	{
		if(!crossFade)
		{
			if( animation.IsPlaying( transitionClip ) )
				animation.Stop ( transitionClip );
			else
				animation.Stop ( clipName );
		}
	}
	
	public TransitionState(string name, string transitionClip, GameObject gameObject, WrapMode wrapMode = WrapMode.Loop)
		: base(name, gameObject, wrapMode)
	{
		this.transitionClip = transitionClip;
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
		states.Add ("idle", new State("standing", gameObject));
		states.Add ("standing", new TransitionState("standing", "stand", gameObject));
		states.Add ("walk", new State("walk", gameObject));
		states.Add ("sitting", new TransitionState("sitting", "sit", gameObject));
		states.Add ("AttackMachine", new State("attack", gameObject));
		
		// Add state connections
		states["idle"].connections.Add (new StateConnection(states["walk"], "walking"));
		states["idle"].connections.Add (new StateConnection(states["sitting"], "sitting"));
		states["idle"].connections.Add (new StateConnection(states["AttackMachine"], "targetLocked"));
		
		states["standing"].connections.Add (new StateConnection(states["walk"], "walking"));
		states["standing"].connections.Add (new StateConnection(states["sitting"], "sitting"));
		states["standing"].connections.Add (new StateConnection(states["AttackMachine"], "targetLocked"));
		
		states["walk"].connections.Add (new StateConnection(states["idle"], "standing"));
		states["walk"].connections.Add (new StateConnection(states["AttackMachine"], "targetLocked"));
		
		states["sitting"].connections.Add (new StateConnection(states["standing"], "standing"));
		states["sitting"].connections.Add (new StateConnection(states["walk"], "walking"));
		
		states["AttackMachine"].connections.Add (new StateConnection(states["walk"], "walking"));
		states["AttackMachine"].connections.Add (new StateConnection(states["idle"], "standing"));
		states["AttackMachine"].connections.Add (new StateConnection(states["sitting"], "sitting"));
		
		currentState = states["idle"];
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
