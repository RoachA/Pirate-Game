using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems
{
    public class InputManager : MonoBehaviour
    {
        //get input only. !!!
        //inform the classes seek input.
        //mouse and keyboard
        //also can check mouse pos relative to an anchor assigned by a requesting class.
        //later on inject via zenject 

        public enum Direction
        {
            Left,
            Right,
            Up,
            Down
        }

        public Vector3 MousePos { get; private set; }
        public Vector2 KeyboardInputDirectionVector { get; private set; }
        private Camera _mainCam;

        private void Start()
        {
            _mainCam = Camera.main;
        }

        private void Update()
        {
            GetKeyboardInput();
            GetMouseInput();
        }

        /// <summary>
        /// will return a vector direction and normalized
        /// magnitude depending on where mouse is
        /// with relation to the given world pos
        /// </summary>
        /// <param name="anchorPos">World position of the anchor</param>
        public Vector3 GetMousePosRelativeToAnchor(Vector3 anchorPos)
        {
            Vector3 direction = MousePos - anchorPos;
            direction.Normalize();
            return direction;
        }

        private void GetKeyboardInput()
        {
            Vector3 direction = Vector3.zero;

            if (Input.GetKey(KeyCode.A))
            {
                direction += Vector3.left;
            }

            if (Input.GetKey(KeyCode.W))
            {
                direction += Vector3.up;
            }

            if (Input.GetKey(KeyCode.D))
            {
                direction += Vector3.right;
            }

            if (Input.GetKey(KeyCode.S))
            {
                direction += Vector3.down;
            }

            KeyboardInputDirectionVector = direction.normalized;
        }

        private void GetMouseInput()
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 mouseWorldPos = _mainCam.ScreenToWorldPoint((new Vector3(mousePos.x, mousePos.y, _mainCam.nearClipPlane)));
            MousePos = mouseWorldPos;
        }
    }
}