using Game.Health;
using Game.Service;
using sm_application.Extension;
using sm_application.Service;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Game.Brain
{
    [DisallowMultipleComponent]
    public class BrainOwner : MonoBehaviour
    {
        [SerializeField] private Brain _brain;
        [SerializeField] private GameObject _target;
        [SerializeField] private HealthBase _targetHealth;
        [SerializeField] private TransformInfo _transformInfoTarget;
        [FormerlySerializedAs("_characterController")] [SerializeField, ReadOnlyField] private Character _character;
        [SerializeField, ReadOnlyField] NavMeshAgent _navMeshAgent;

        private BrainControlService _brainControl;

        private bool _isTargetExist;

        public NavMeshAgent NavMeshAgent => _navMeshAgent;
        public GameObject Target => _target;
        public HealthBase TargetHealth => _targetHealth;
        public bool IsTargetExist => _target != null && _target.activeInHierarchy;
        public Character Character => _character;

        private void OnEnable()
        {
            _brainControl.AddBrain(this);
        }

        private void OnDisable()
        {
            _brainControl.RemoveBrain(this);
        }

        private void Awake()
        {
            _brainControl = Services.Get<BrainControlService>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _character = GetComponent<Character>();
        }

        public void Think()
        {
            _brain.Think(this);
        }

        public void SetTarget(GameObject target)
        {
            _target = target;
            _targetHealth = target.GetComponent<HealthBase>();
        }
    }
}