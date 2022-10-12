using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShip : PirateShip
{

    private const float CIRCLE_CAST_RADIUS = 0.5F;

    [Space]
    [SerializeField] private int scoreWorth = 1;
    [SerializeField] private GameManager gameManager;
    [Space]
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] protected Transform player;
    [Space]
    [SerializeField] private bool playerOnSight;
    [SerializeField] private List<PathNode> path = new List<PathNode>();
    [Space]
    [Header("Pathfinding")]
    [SerializeField] private float updatePathInterval = 5f;
    [SerializeField] protected EnumEnemyPathfindingStatus pathFinderStatus;
    [SerializeField] private bool followingPath => currentlyFollowingPathToPlayer;
    [SerializeField] private float desiredDistanceFromPlayer = 0.5f;
    

    private bool currentlyFollowingPathToPlayer = false;
    private float marginOfToleranceToReachDestination = 0.1f;

    private int terrainLayermask;

    protected delegate void OnPathStatusChange(EnumEnemyPathfindingStatus previousStatus);
    protected OnPathStatusChange onPathStatusChange;

    protected delegate void WhileStationary();
    protected WhileStationary whileStationary;

    protected void Start()
    {

        terrainLayermask = LayerMask.GetMask("TerrainObstacles");
        
        onPathStatusChange += PathStatusChanged;

        InvokeRepeating("UpdatePathStatus", 0f, updatePathInterval);

    }

    private void LateUpdate()
    {
        switch (pathFinderStatus)
        {
            case EnumEnemyPathfindingStatus.Stationary:
                if(whileStationary != null)
                {
                    whileStationary();
                }
                break;
            case EnumEnemyPathfindingStatus.FollowingPathToPlayer:
                FollowPath();
                break;
            case EnumEnemyPathfindingStatus.GoingStraightToThePlayer:
                GoStraightToPlayer();
                break;
        }
    }

    private bool CheckIfPlayerIsOnSight()
    {
        Vector2 direction = player.position - this.transform.position;
        float distance = Vector2.Distance(this.transform.position, player.position);
        var hit = Physics2D.CircleCast(this.transform.position, CIRCLE_CAST_RADIUS, direction, distance, terrainLayermask);
        
        if (hit)
        {
            Debug.DrawLine(this.transform.position, hit.point, Color.red);
            return false;
        }

        Debug.DrawLine(this.transform.position, hit.point, Color.green);
        return true;
    }

    private void PathStatusChanged(EnumEnemyPathfindingStatus previousStatus)
    {
        switch (pathFinderStatus)
        {
            case EnumEnemyPathfindingStatus.Stationary:
                //nothing
                break;
            case EnumEnemyPathfindingStatus.FollowingPathToPlayer:
                GetPathToPlayer();
                break;
            case EnumEnemyPathfindingStatus.GoingStraightToThePlayer:
                break;
        }
    }

    private void GoStraightToPlayer()
    {
        LookAtPlayer();

        MoveShip(Vector2.up);

        bool closeEnoughFromPlayer = Mathf.Abs(Vector2.Distance(this.transform.position, player.position)) <= desiredDistanceFromPlayer;
        if (closeEnoughFromPlayer)
        {
            UpdatePathStatus();
        }
        else if (!CheckIfPlayerIsOnSight())
        {
            UpdatePathStatus();
        }
    }

    private void FollowPath()
    {

        Vector2? destination = GetNextDestinationOnPath();

        if (destination == null)
        {
            UpdatePathStatus();
        }
        else  
        {
            Vector2 destinationDir = ((Vector2)destination) - (Vector2)thisRigidBody.position;
            float destinationAngle = Mathf.Atan2(destinationDir.y, destinationDir.x) * Mathf.Rad2Deg - 90f;
            thisRigidBody.rotation = destinationAngle;

            MoveShip(Vector2.up);
        }
    }

    private Vector2? GetNextDestinationOnPath()
    {
        if(path == null)
        {
            GetPathToPlayer();
        }

        if (path.Count == 0)
        {
            return null;
        }

        if ( Vector2.Distance(this.transform.position, path[0].worldPosition) <= marginOfToleranceToReachDestination)
        {
            if (UpdatePathStatus())
            {
                return null;
            }

            path.RemoveAt(0);

            if (path.Count == 0) return null;

            return path[0].worldPosition;
        }
        
        return path[0].worldPosition;
    }

    private bool UpdatePathStatus()
    {

        this.playerOnSight = CheckIfPlayerIsOnSight();
        bool closeEnoughFromPlayer = Mathf.Abs(Vector2.Distance(this.transform.position, player.position)) <= desiredDistanceFromPlayer;
        
        if (closeEnoughFromPlayer && playerOnSight)
        {
            bool statusChanged = SetPathStatus(EnumEnemyPathfindingStatus.Stationary);

            if (statusChanged)
            {
                thisRigidBody.velocity = Vector2.zero;
            }

            return statusChanged;
        }
        else if (playerOnSight)
        {
            return SetPathStatus(EnumEnemyPathfindingStatus.GoingStraightToThePlayer);
        }
        else if(!playerOnSight)
        {
            bool statusChanged = SetPathStatus(EnumEnemyPathfindingStatus.FollowingPathToPlayer);

            if (!statusChanged && path.Count == 0){
                GetPathToPlayer();
            }

            return statusChanged;
        }

        return false;
    }

    private void GetPathToPlayer()
    {
        path = pathfinder.FindPath(this.transform.position, player.position, 3);

        if(path == null){
            throw new System.Exception(this.gameObject.name + " is stuck. It has found no path to the player.");
        }
        else
        {
            path.Reverse();
        }
    }

    private bool SetPathStatus(EnumEnemyPathfindingStatus value)
    {
        EnumEnemyPathfindingStatus previousStatus = pathFinderStatus;
        pathFinderStatus = value;
        if (previousStatus != pathFinderStatus)
        {
            if (onPathStatusChange != null)
            {
                onPathStatusChange(previousStatus);
            }

            return true;
        }
        return false;
    }

    protected void LookAtPlayer()
    {
        var playerDir = player.position - this.transform.position;
        float angle = Mathf.Atan2(playerDir.y, playerDir.x) * Mathf.Rad2Deg - 90f;
        thisRigidBody.rotation = angle;
    }

    public void SetCannonBallPool(PrefabPool frontPool, PrefabPool sidePool)
    {
        this.sideCannonBallPool = sidePool;
        this.frontalCannonBallPool = frontPool;
    }

    public void SetPlayerReference(Transform player)
    {
        this.player = player;
    }

    public void SetPathFinder(Pathfinder pathfinder)
    {
        this.pathfinder = pathfinder;
    }

    public void SetGameManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
        onDeath += GiveScoreToManager;
    }

    public void GiveScoreToManager()
    {
        gameManager.AddScore(scoreWorth);
    }
}
