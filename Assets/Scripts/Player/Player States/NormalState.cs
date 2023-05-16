using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character.State
{
    public class NormalState : IState
    {
        public void Tick()
        {
            //show interaction
        }

        public void OnEnter()
        {
            //Debug.Log("normal state start");
        }

        public void OnExit()
        {
            
        }
    }
}
