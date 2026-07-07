
using System;
using UnityEngine;

namespace _Scripts
{
    public class MovementAIController : MovementController
    {
        private enum AIState
        {
            SearchLadder,          
            MoveToLadder,          
            ClimbLadder,           
            WaitForUpperAlignment  
        }
        
        [Header("AI")]
        [SerializeField] private Vector3 ladderSearchHalfExtents = new Vector3(5f, 0.1f, 1f);
        [SerializeField] private Vector3 ladderSearchCenterOffset = new Vector3(0f, 0.6f, 0f);
        [SerializeField] private LayerMask ladderLayer;
        
        [Header("AI Movement")]
        [SerializeField] private float targetReachThreshold = 0.05f;
        
        private AIState state = AIState.SearchLadder;
        private Transform targetLadder;
        
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Vector3 center = transform.position + ladderSearchCenterOffset;
            Gizmos.DrawWireCube(center, ladderSearchHalfExtents * 2f);
        }

        protected override void Update()
        {
            UpdateState();
            base.Update();
        }

        private void UpdateState()
        {
            switch (state)
            {
                case AIState.SearchLadder:
                    UpdateSearchLadder();
                    break;
                
                case AIState.MoveToLadder:
                    UpdateMoveToLadder();
                    break;
                
                case AIState.ClimbLadder:
                    UpdateClimb();
                    break;
                
                case AIState.WaitForUpperAlignment:
                    UpdateWaitForUpperAlignment();
                    break;
            }
        }

        private void UpdateSearchLadder()
        {
            SetMovementMode(MovementMode.None);
            SetMovementInput(Vector2.zero);
            
            targetLadder = GetNearestLadder();
            
            if (targetLadder != null)
                state = AIState.MoveToLadder;
        }

        private void UpdateMoveToLadder()
        {
            if (IsOnLadder)
            {
                targetLadder = null;
                state = AIState.ClimbLadder;
                return;
            }

            if (targetLadder == null)
            {
                state = AIState.SearchLadder;
                return;
            }
                
            SetMovementMode(MovementMode.Platform);

            float xDifference = targetLadder.position.x - transform.position.x;
            if (Mathf.Abs(xDifference) < targetReachThreshold)
            {
                SetMovementInput(Vector2.zero);
                return;
            }
            
            MovementInput = xDifference > 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
        }

        private void UpdateClimb()
        {
            SetMovementMode(MovementMode.Ladder);
            SetMovementInput(new Vector2(0f, 1f));

            if (IsOnLadder)
                return;

            LadderSegmentInfo.LadderSegmentType? exitedType = LastExitedLadderType;
            ClearLastExitedLadder();

            targetLadder = null;

            if (exitedType == LadderSegmentInfo.LadderSegmentType.Upper)
            {
                state = AIState.WaitForUpperAlignment;
                return;
            }

            state = AIState.MoveToLadder;
        }

        private void UpdateWaitForUpperAlignment()
        {
            SetMovementMode(MovementMode.None);
            SetMovementInput(Vector2.zero);

            Transform ladderInFront  = GetLadderInFront();

            if (ladderInFront == null)
                return;

            targetLadder = ladderInFront;
            state = AIState.ClimbLadder;
        }

        private Transform GetNearestLadder()
        {
            Collider[] hits = GetVisibleLadders();
            
            Transform nearest = null;
            float nearestDistance = float.MaxValue;
            
            foreach (Collider hit in hits)
            {
                float distance = Mathf.Abs(hit.transform.position.x - transform.position.x);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = hit.transform;
                }
            }

            return nearest;
        }
        
        private Transform GetLadderInFront()
        {
            Collider[] hits = GetVisibleLadders();

            foreach (Collider hit in hits)
            {
                float xDifference = Mathf.Abs(hit.transform.position.x - transform.position.x);

                if (xDifference <= targetReachThreshold)
                    return hit.transform;
            }

            return null;
        }

        private Collider[] GetVisibleLadders()
        {
            Vector3 center = transform.position + ladderSearchCenterOffset;
            
            return Physics.OverlapBox(center, ladderSearchHalfExtents,
                Quaternion.identity, ladderLayer);
        }
        
        public void ResetTarget()
        {
            targetLadder = null;
            state = AIState.SearchLadder;
            SetMovementMode(MovementMode.None);
            SetMovementInput(Vector2.zero);
        }
    }
}
