using UnityEngine;

namespace PRIAHunt
{
    public struct GameplayInput
    {
        public Vector2 rotacionVisual;
        public Vector2 direccionMovimiento;
        public bool salto;
    }
    
    public class PlayerInput : MonoBehaviour
    {
        private GameplayInput _input;
        public GameplayInput inputActual => _input;

        public void Reset()
        {
            _input.direccionMovimiento = default;
            _input.salto = false;
        }

        public void Update()
        {
            if (Cursor.lockState != CursorLockMode.Locked)
                return;
            
            _input.rotacionVisual += new Vector2(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"));
            
            var direccion = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            _input.direccionMovimiento = direccion.normalized;
            
            _input.salto |= Input.GetButtonDown("Jump");
        }
    }
}

