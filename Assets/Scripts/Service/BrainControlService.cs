using System;
using System.Collections.Generic;
using Game.Brain;
using sm_application.Service;

namespace Game.Service
{
    public class BrainControlService : IService, IConstructInstaller
    {
        private readonly LinkedList<BrainOwner> _brains = new ();
        private LinkedListNode<BrainOwner> _brainNode;

        private void Update()
        {
            if (_brains.Count == 0) return;

            _brainNode = _brainNode?.Next ?? _brains.First;
            _brainNode.Value.Think();
        }

        public void AddBrain(BrainOwner brainOwner)
        {
            _brains.AddLast(brainOwner);
        }

        public void RemoveBrain(BrainOwner brainOwner)
        {
            _brains.Remove(brainOwner);
        }

        public void Construct()
        {
            throw new NotImplementedException();
        }

        public void Construct(IServiceInstaller installer)
        {
            
        }
    }
}