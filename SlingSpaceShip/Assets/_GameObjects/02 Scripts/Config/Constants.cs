using UnityEngine;

public static class Constants
{
    public static class EnemyData
    {
        // Enemy Move Speed
        public static readonly float EnemyIdleMoveSpeed = 5.0f;
        public static readonly float EnemyPatrollingMoveSpeed = 7.5f;

        // Enemy State Durations
        public static readonly Vector2 IdleDuration = new Vector2(2.0f, 5.0f);

        public static readonly float IdleMinDistFromGround = 4.0f;
        public static readonly float IdleMaxDistFromGround = 6.0f;
        public static readonly float MaxPetrolHorizontalDist = 20.0f;
        
        // Range
        public static readonly float PlayerShootingRange = 10.0f;
        public static readonly float PlayerDetectionRange = 35.0f;
        public static readonly float PlayerFollowRange = 70.0f;
    }
}
