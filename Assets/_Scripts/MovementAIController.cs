
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
        [SerializeField] private Vector3 ladderVertSearchHalfExtents = new Vector3(0.1f, 5f, 1f);
        [SerializeField] private LayerMask ladderLayer;
        
        [Header("AI Movement")]
        [SerializeField] private float targetReachThreshold = 0.05f;
        [SerializeField] private float nextTierMinYAboveHead = 0.05f;
        [SerializeField] private float nextTierMaxYAboveHead = 1.5f;
        [SerializeField] private float ladderConnectionXThreshold = 0.1f;
        [SerializeField] private float ladderConnectionZThreshold = 0.1f;
        
        
        private AIState state = AIState.SearchLadder;
        private Transform targetLadder;
        
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Vector3 center = transform.position + ladderSearchCenterOffset;
            Gizmos.DrawWireCube(center, ladderSearchHalfExtents * 2f);
            
            Gizmos.color = Color.chocolate;
            Vector3 center2 = new Vector3(
                bodyCollider.bounds.center.x,
                bodyCollider.bounds.center.y + ladderVertSearchHalfExtents.y,
                bodyCollider.bounds.center.z);
            Gizmos.DrawWireCube(center2, ladderVertSearchHalfExtents * 2f);

            
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, platformSearchBoxHalfExtents * 2f);
        }

        protected override void Update()
        {
            if (!isInitialized)
                return;
            
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
                state = AIState.ClimbLadder;
                return;
            }
            
            MovementInput = xDifference > 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
        }

        private void UpdateClimb()
        {
            SetMovementMode(MovementMode.Ladder);
            SetMovementInput(new Vector2(0f, 1f));

            if (IsOnLadder 
                && currentLadderInfo != null 
                && currentLadderInfo.Type == LadderSegmentInfo.LadderSegmentType.Upper 
                && IsHeadNearTopOfCurrentLadder()
                && !HasConnectedLowerLadderAboveHead())
            {
                state = AIState.WaitForUpperAlignment;
                return;
            }
            
            if (IsOnLadder)
                return;

            targetLadder = null;
            
            if (TrySnapToPlatformBelow())
            {
                state = AIState.SearchLadder;
                return;
            }
            
            SetMovementMode(MovementMode.None);
            SetMovementInput(Vector2.zero);
        }
        
        private void UpdateWaitForUpperAlignment()
        {
            SetMovementMode(MovementMode.Ladder);
            SetMovementInput(Vector2.zero);

            Transform ladderAbove = GetConnectedLowerLadderAboveHead();

            if (ladderAbove == null)
                return;

            targetLadder = ladderAbove;
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
        
        private Transform GetConnectedLowerLadderAboveHead()
        {
            float bodyUpperY = bodyCollider.bounds.max.y;
            
            Collider[] hits = GetVisibleLaddersFromHead(bodyCollider.bounds.max);

            foreach (Collider hit in hits)
            {
                if (!hit.TryGetComponent(out LadderSegmentInfo ladderInfo))
                    continue;

                if (ladderInfo.Type != LadderSegmentInfo.LadderSegmentType.Lower)
                    continue;

                float yDifference = hit.transform.position.y - bodyUpperY;

                if (yDifference < nextTierMinYAboveHead)
                    continue;

                if (yDifference > nextTierMaxYAboveHead)
                    continue;

                float xDifference = Mathf.Abs(hit.transform.position.x - transform.position.x);
                float ZDifference = Mathf.Abs(hit.transform.position.z - transform.position.z);

                if (xDifference > ladderConnectionXThreshold || ZDifference > ladderConnectionZThreshold)
                    continue;

                return hit.transform;
            }

            return null;
        }
        
        private bool HasConnectedLowerLadderAboveHead()
        {
            return GetConnectedLowerLadderAboveHead() != null;
        }
        
        private Collider[] GetVisibleLaddersFromHead(Vector3 headPosition)
        {
            Vector3 center = new Vector3(
                bodyCollider.bounds.center.x,
                bodyCollider.bounds.center.y + ladderVertSearchHalfExtents.y,
                bodyCollider.bounds.center.z);

            return Physics.OverlapBox(center, ladderSearchHalfExtents, Quaternion.identity, ladderLayer);
        }

        private Collider[] GetVisibleLadders()
        {
            Vector3 center = transform.position + ladderSearchCenterOffset;
            
            return Physics.OverlapBox(center, ladderSearchHalfExtents, Quaternion.identity, ladderLayer);
        }
        
        private bool IsHeadNearTopOfCurrentLadder()
        {
            if (currentLadderCollider == null)
                return false;

            float headY = bodyCollider.bounds.max.y + 0.1f;
            float ladderTopY = currentLadderCollider.bounds.max.y;

            return headY >= ladderTopY;
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
