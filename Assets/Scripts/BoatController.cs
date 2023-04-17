using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoatController : MonoBehaviour {

    [SerializeField] private Rigidbody rb;

    [SerializeField] private float rowForce = 50;

    [SerializeField] private float rowCooldown = 0.3f;

    [SerializeField] private float rowTime = 1f;

    private Coroutine ongoingRow;
    private bool canRow = true;
    
    public void OnRow(InputAction.CallbackContext context) {
        if (ongoingRow != null || !canRow) return;
        canRow = false;
        // ongoingRow = StartCoroutine(Row());
    }
    

    private IEnumerator Row() {
        float timer = rowTime;
        while (timer >= 0) {
            rb.AddForceAtPosition(transform.forward * rowForce, rb.position, ForceMode.Acceleration);
            timer -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(rowCooldown);
        
        ongoingRow = null;
        canRow = true;
    }
    
}
