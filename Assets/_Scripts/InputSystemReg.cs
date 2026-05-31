using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts
{
    public class InputSystemReg : MonoBehaviour
    {
        
        public InputActionAsset inputActionAsset;
        public InputActionMap inputActionMap;
        public InputAction inputAction;


        private void Awake()
        {
            inputActionMap = inputActionAsset.FindActionMap("GameActions");
            inputAction = inputActionMap.FindAction("Fire");
        }
    }
}