using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI
{
    class Billboard : MonoBehaviour
    {
        private Transform mainCameraTransform;

        private void Start()
        {
            mainCameraTransform = Camera.main.transform;
        }

        private void LateUpdate()
        {
            transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward,
                mainCameraTransform.rotation * Vector3.up);
        }
    }
}
