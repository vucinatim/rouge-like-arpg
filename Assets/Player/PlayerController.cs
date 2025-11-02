using UnityEngine;

using FishNet.Object;

public class PlayerController : NetworkBehaviour
{

    public override void OnStartClient()
    {
        base.OnStartClient();

        // Disable player movement for all clients except the local player
        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        // Find the main camera
        GameObject mainCameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        CameraFollow cameraFollow = mainCameraObject.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.AssignCameraToPlayer(transform);
        }
        else
        {
            Debug.LogError("Main camera not found!");
        }
    }


}
