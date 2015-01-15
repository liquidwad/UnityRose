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
	public bool chase;
	public bool skill;
	public bool defaultAttack;
	public string nextSkill;
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
	
	public virtual void Exit(bool crossFade = true)
	{
		if( !crossFade )
			animation.Stop(clipName);
	}
	
	public virtual void Entry(bool crossFade = true)
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
	
	public virtual State Evaluate(ref StateParams stateParams) { 
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
	
	public override State Evaluate(ref StateParams stateParams)
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
	
	public override State Evaluate(ref StateParams stateParams)
	{
		State result = base.Evaluate(ref stateParams);
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

public class SkillState: State
{
	private Queue<string> skillQueue;
	private State nextState;
	
	public SkillState(string name, State nextState, GameObject gameObject)
	:base(name, gameObject, WrapMode.Once)
	{
		skillQueue = new Queue<string>();
		this.nextState = nextState;
	}
	
	public override void Entry( bool crossFade = true )
	{
			
	}
	
	public override State Evaluate(ref StateParams stateParams)
	{
		// if there is a skill pending
		if(stateParams.nextSkill != "")
		{
			// enqueue and consume it
			skillQueue.Enqueue(stateParams.nextSkill);
			stateParams.nextSkill = "";
		}
		
		// Once a skill has been commanded, it won't stop till the animation ends
		if( !animation.IsPlaying(clipName) )
		{
			// First, make sure we are still in this state
			State result = base.Evaluate(ref stateParams );
			
			if(result!=this)
				return result;
				
			// if we are in this state, check if there are still skills in the queue
			if( skillQueue.Count > 0 )
			{
				// Play the next skill in the queue (without crossfade)
				clipName = skillQueue.Dequeue();
				base.Entry(false);
			}
			else
			{
				return nextState;
			}

		}
		
		return this;
			
	}
	
	public override void Exit(bool crossFade = false )
	{
		skillQueue.Clear();
		animation.Stop (clipName);
	}
}

public class AttackMachine: State
{
	private Dictionary<string, State> states;
	private State currentState;
	
	public AttackMachine(string name, GameObject gameObject)
		:base( name, gameObject )	
	{
		states = new Dictionary<string, State>();
		List<string> defaultAttacks = new List<string>(3);
		defaultAttacks.Add ("attack1");
		defaultAttacks.Add ("attack2");
		defaultAttacks.Add("attack3");
		states.Add ("defaultAttack", new MultiAnimationState(defaultAttacks, gameObject));
		states.Add ("skill", new SkillState("skill", states["defaultAttack"], gameObject));
		states.Add ("chase", new State("run", gameObject));
	
		// connect every state to every other state
		states["defaultAttack"].connections.Add (new StateConnection(states["skill"], "skill"));
		states["defaultAttack"].connections.Add (new StateConnection(states["chase"], "chase"));
		states["skill"].connections.Add (new StateConnection( states["defaultAttack"], "defaultAttack"));
		states["skill"].connections.Add (new StateConnection( states["chase"], "chase"));
		states["chase"].connections.Add (new StateConnection( states["skill"], "skill"));
		states["chase"].connections.Add (new StateConnection( states["defaultAttack"], "defaultAttack"));
		
	}
	
	public override void Entry( bool crossFade = false )
	{
		currentState = states["defaultAttack"];
		currentState.Entry();
	}
	
	public override State Evaluate( ref StateParams stateParams )
	{
		State result = base.Evaluate( ref stateParams );
		if( result != this )
			return result;
		
		
		result = currentState.Evaluate( ref stateParams);
		
		if(result != currentState)
		{
			currentState.Exit(true);
			currentState = result;
			currentState.Entry(true);
		}	
		
		return this;
	}
	
	public override void Exit(bool crossFade = false)
	{
		currentState.Exit( crossFade );
	}
	
}
public class PlayerState : State
{
	private Dictionary<string, State> states;
	private State currentState;
	
	public PlayerState(string name, GameObject gameObject)
		:base(name, gameObject)
	{
		// Generate list of states
		states = new Dictionary<string, State>();
		states.Add ("standing", new State("standing", gameObject));
		states.Add ("stand", new TransitionState("stand", states["standing"], gameObject));
		states.Add ("walk", new State("walk", gameObject));
		states.Add ("standWalk", new TransitionState("stand", states["walk"], gameObject));
		states.Add ("sitting", new State("sitting", gameObject));
		states.Add ("sit", new TransitionState("sit", states["sitting"], gameObject));
		states.Add ("AttackMachine", new AttackMachine("attack", gameObject));
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
		
	}
	
	public override void Entry(bool crossFade = false)
	{	
		currentState = states["standing"];
		currentState.Entry();
	}
	
	public override void Exit(bool crossFade = false)
	{
		currentState.Exit( crossFade );
	}
	
	public override State Evaluate(ref StateParams stateParams)
	{	
		State result = currentState.Evaluate(ref stateParams);
		
		if(result != currentState)
		{
			currentState.Exit(true);
			currentState = result;
			currentState.Entry(true);
		}	
		
		return this;
	}
	
}
