using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;
using Character.State;
namespace Character
{    
    public class Player : MonoBehaviour
    {
        [Range(0.001f, 0.1f)]
        [SerializeField] private float sphereCastRadius = 0.01f;
        [SerializeField] private float interactDistance = 3f;
        [SerializeField] private GameObject visualPrefab;
        [SerializeField] private LayerMask layerMaskGround;
        [SerializeField] private Transform lookTransform;
        private Transform selectedTransform;
        public Transform SelectedTransform => selectedTransform;
        public Action onSelectedChange;
        private StateMachine playerStateMachine;
        private IState currentState ;
        private bool mainActionKey;
        private bool secondaryActionKey;        
        public void Init(PlayerBuildingLink buildingLink)
        {
            playerStateMachine = new StateMachine();
            IState idle = new NormalState();
            IState digging = new DiggingState(this, buildingLink);
            IState building = new BuildingState(this, buildingLink); // ADD INVENTORY
            playerStateMachine.AddAnyTransition(idle, NoInput());

            playerStateMachine.AddTransition(idle, digging, () => mainActionKey);
            playerStateMachine.AddTransition(idle, building, () => secondaryActionKey && selectedTransform != null);

            playerStateMachine.SetState(idle);
        }        
        
        public void OnMainAction(InputValue inputValue)
        {
            mainActionKey = inputValue.isPressed;
        }
        private void Update()
        {
            Transform newTargetTransform = null;
            if(MakeRaycast(out RaycastHit raycast))
            {
                newTargetTransform = raycast.collider.transform;
            }
            if(newTargetTransform != selectedTransform)
            {
                selectedTransform = newTargetTransform;
                onSelectedChange?.Invoke();
            }
            playerStateMachine.Tick();
        }
        Func<bool> NoInput() => () => !mainActionKey && !secondaryActionKey;
        public void OnSecondaryAction(InputValue inputValue)
        {
            secondaryActionKey = inputValue.isPressed;
        }
        public bool MakeRaycast(out RaycastHit raycastHit)
        {
            return Physics.SphereCast(lookTransform.position, sphereCastRadius, lookTransform.forward, out raycastHit, interactDistance, layerMaskGround);
        }
        
    }
}
