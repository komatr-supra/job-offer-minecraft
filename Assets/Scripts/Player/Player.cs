using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Character.State;
namespace Character
{    
    public class Player : MonoBehaviour
    {
        #region VARIABLES
        [Range(0.001f, 0.1f)]
        [SerializeField] private float sphereCastRadius = 0.01f;
        [SerializeField] private float interactDistance = 3f;
        [SerializeField] private GameObject visualPrefab;
        [SerializeField] private LayerMask layerMaskGround;
        [SerializeField] private Transform lookTransform;
        private Inventory inventory;
        private Transform selectedTransform;
        public Transform SelectedTransform => selectedTransform;
        public Action onSelectedChange;
        private StateMachine playerStateMachine;
        private bool mainActionKey;
        private bool secondaryActionKey;   
        #endregion

        //this is called after creation
        public void Init(PlayerBuildingLink buildingLink)
        {
            inventory = GetComponent<Inventory>();
            playerStateMachine = new StateMachine();
            IState idle = new NormalState();
            IState digging = new DiggingState(this, buildingLink);
            IState building = new BuildingState(this, buildingLink, inventory);
            playerStateMachine.AddAnyTransition(idle, NoInput());

            playerStateMachine.AddTransition(idle, digging, () => mainActionKey);
            playerStateMachine.AddTransition(idle, building, () => secondaryActionKey && selectedTransform != null);

            playerStateMachine.SetState(idle);
        }       
        //this is for state condition 
        private Func<bool> NoInput() => () => !mainActionKey && !secondaryActionKey;
        //calls
        public void OnMainAction(InputValue inputValue)
        {
            mainActionKey = inputValue.isPressed;
        }
        public void OnSecondaryAction(InputValue inputValue)
        {
            secondaryActionKey = inputValue.isPressed;
        }
        private void Update()
        {
            //check if player is looking at same block
            Transform newTargetTransform = null;
            if(MakeRaycast(out RaycastHit raycast)) newTargetTransform = raycast.collider.transform;            
            if(newTargetTransform != selectedTransform)
            {
                selectedTransform = newTargetTransform;
                onSelectedChange?.Invoke();
            }
            playerStateMachine.Tick();
        }
        public bool MakeRaycast(out RaycastHit raycastHit)
        {
            return Physics.SphereCast(lookTransform.position, sphereCastRadius, lookTransform.forward, out raycastHit, interactDistance, layerMaskGround);
        }
        //for inventory
        private void OnTriggerEnter(Collider other) {
            //handle drop
            if(!other.CompareTag("Drop")) return;
            if(other.TryGetComponent<Drop>(out Drop drop))
            {
                inventory.AddBlockToInventory(drop.BlocksSO, 1);
                Destroy(other.gameObject);
            }
        }
    }
}
