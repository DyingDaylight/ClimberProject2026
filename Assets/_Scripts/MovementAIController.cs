
using System;
using UnityEngine;

namespace _Scripts
{
    public class MovementAIController : MovementController
    {
        [SerializeField] private float ladderSearchRadius = 5f;
        [SerializeField] private Vector3 ladderSearchHalfExtents = new Vector3(5f, 0.1f, 1f);
        [SerializeField] private Vector3 ladderSearchCenterOffset = new Vector3(0f, 0.6f, 0f);
        [SerializeField] private LayerMask ladderLayer;
        
        private Transform targetLadder;
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Vector3 center = transform.position + ladderSearchCenterOffset;
            Gizmos.DrawWireCube(center, ladderSearchHalfExtents * 2f);
        }
        
        protected override void Update()
        {
            Debug.Log("LaddersCount: " + ladders.Count);
            if (ladders.Count > 0)
            {
                targetLadder = null;
                SetMovementInput(new Vector2(0, 1));
                movementMode = MovementMode.Ladder;
                base.Update();
                return;
            }

            if (targetLadder == null)
            {
                targetLadder = GetNearestLadder();
            }

            Debug.Log("targetLadder: " + targetLadder);
            
            if (targetLadder == null)
            {
                SetMovementInput(Vector2.zero);
                base.Update();
                return;    
            }
            
            float xDifference = targetLadder.position.x - transform.position.x;
            Debug.Log("xDifference: " + xDifference);
            if (Mathf.Abs(xDifference) < 0.05f)
            {
                MovementInput = Vector2.zero;
            }
            else
            {
                MovementInput = xDifference > 0 ? new Vector2(1, 0) : new Vector2(-1, 0);
            }

            movementMode = MovementMode.Platform;
            base.Update();
        }

        private Transform GetNearestLadder()
        {
            //Collider[] hits = Physics.OverlapSphere(transform.position, ladderSearchRadius, ladderLayer);

            Vector3 center = transform.position + ladderSearchCenterOffset;
            Collider[] hits = Physics.OverlapBox(center, ladderSearchHalfExtents,
                Quaternion.identity, ladderLayer
            );

            
            Transform nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (Collider hit in hits)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = hit.transform;
                }
            }

            return nearest;
        }
    }
}
