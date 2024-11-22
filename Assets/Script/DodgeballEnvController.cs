using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class DodgeballEnvController : MonoBehaviour {  
    public int playerMaxHP = 1;
    //Players Remaining
    private int numberOfBluePlayersRemaining = 2;
    private int numberOfOrangePlayersRemaining = 1; 
    // Each Group for the player teams
    private SimpleMultiAgentGroup Team0AgentGroup;
    private SimpleMultiAgentGroup Team1AgentGroup;
    [System.Serializable]
    public class PlayerInfo
    {
        public DodgeballAgent Agent;
        public int HitPointsRemaining;
        [HideInInspector]
        public Vector3 StartingPos;
        [HideInInspector]
        public Quaternion StartingRot;
        [HideInInspector]
        public Rigidbody Rb;
        [HideInInspector]
        public Collider Col;
        [HideInInspector]
        public int TeamID;
    }
    // List of Players on each team with their informations
    public List<PlayerInfo> Team0Players;
    public List<PlayerInfo> Team1Players;
    /*
    [System.Serializable]
    public class BallInfo
    {
        public GameObject ballObject;
        public Rigidbody ballRb;
    }
    public List<BallInfo> GameBalls;*/
    public GameObject ball;
    public Rigidbody ballRb;
    private int resetTimer;
    public int MaxEnvironmentSteps;
    private bool initializedGame;
    public float hitBonus = 1f;
    public float timeBonus = 1f;
    

    void Start() {
        Initialize();
    }

    void Initialize() {
        
        Team0AgentGroup = new SimpleMultiAgentGroup();
        Team1AgentGroup = new SimpleMultiAgentGroup();
        ResetBall();
        // Initializing Agent Behavior Parameters and adding them to their respective groups
        foreach (var player in Team0Players) {
            player.Agent.Initialize();
            player.Agent.HitPointsRemaining = playerMaxHP;
            player.Agent.behaviorParameters.TeamId = 0;
            player.TeamID = 0;
            player.Agent.NumberOfTimesPlayerCanBeHit = playerMaxHP;
            Team0AgentGroup.RegisterAgent(player.Agent);
        }

        foreach (var player in Team1Players) {
            player.Agent.Initialize();
            player.Agent.HitPointsRemaining = playerMaxHP;
            player.Agent.behaviorParameters.TeamId = 1;
            player.TeamID = 1;
            player.Agent.NumberOfTimesPlayerCanBeHit = playerMaxHP;
            Team1AgentGroup.RegisterAgent(player.Agent);
        }
        initializedGame = true;
        ResetScene();

    }

    void FixedUpdate() {
        if (!initializedGame) return;
        resetTimer += 1;
        if (resetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
        {
            Team0AgentGroup.GroupEpisodeInterrupted();
            Team1AgentGroup.GroupEpisodeInterrupted();
            ResetScene();
        }
    }

    void ResetScene() {
        resetTimer = 0;

        numberOfBluePlayersRemaining = 2;
        numberOfOrangePlayersRemaining = 1;
        
        foreach (var player in Team0Players) {
            player.Agent.HitPointsRemaining = playerMaxHP;
            player.Agent.gameObject.SetActive(true);
            player.Agent.ResetAgent();
            Team0AgentGroup.RegisterAgent(player.Agent);
        }
        foreach (var player in Team1Players) {
            player.Agent.HitPointsRemaining = playerMaxHP;
            player.Agent.gameObject.SetActive(true);
            player.Agent.ResetAgent();
            Team1AgentGroup.RegisterAgent(player.Agent);
        }
        ResetBall();

    }

    void ResetBall() {
        /*
        foreach (var ball in GameBalls) {
            ball.ballObject.transform.localPosition = new Vector3();
            ball.ballRb
        } */
        ball.transform.localPosition = new Vector3(Random.Range(-4f, 4f), 2f, 0f);
        ballRb.angularVelocity = Vector3.zero;
        ballRb.linearVelocity = Vector3.zero;
        ballRb.useGravity = true;
    }

    public void PlayerHit(DodgeballAgent hit, DodgeballAgent thrower) {
        // Get team Id's of players involved
        int hitTeamID = hit.teamID;
        Debug.Log($"HIT {hitTeamID}");
        int throwerTeamID = thrower.teamID;
        Debug.Log($"THROW {throwerTeamID}");
        // If the bool statement is true return first option, otherwise second
        SimpleMultiAgentGroup HitAgentGroup = hitTeamID == 1 ? Team1AgentGroup : Team0AgentGroup;
        Debug.Log($"HITTER {HitAgentGroup}");
        SimpleMultiAgentGroup ThrowAgentGroup = throwerTeamID == 1 ? Team1AgentGroup : Team0AgentGroup;
        Debug.Log($"THROWER {ThrowAgentGroup}");  
        if (hit.HitPointsRemaining <= 1) {
            // If the bool statement is true return first option, otherwise second
            numberOfBluePlayersRemaining -= hitTeamID == 1 ? 0 : 1;
            numberOfOrangePlayersRemaining -= hitTeamID == 1 ? 1 : 0;
            if (numberOfBluePlayersRemaining == 0 || numberOfOrangePlayersRemaining == 0) {
                ThrowAgentGroup.AddGroupReward(2.0f - timeBonus * (resetTimer / MaxEnvironmentSteps));
                HitAgentGroup.AddGroupReward(-1.0f);
                ThrowAgentGroup.EndGroupEpisode();
                HitAgentGroup.EndGroupEpisode();
                Debug.Log($"Team {throwerTeamID} Won");
                ResetScene();
            } else {
                hit.gameObject.SetActive(false);
            }
            
        } else {
            hit.HitPointsRemaining--;
            thrower.AddReward(hitBonus);
        }
    }


}

