
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.boxtank{
    
    /// <summary>
    /// control the rotation of the left and right wheels and the scrolling of the track
    /// </summary>
    public class TankTrackComp : MonoBehaviour
    {
        /// <summary>
        /// move speed, m/s
        /// </summary>
        [Range(-50,50)]
        public float moveSpeed = 20;
            
        /// <summary>
        /// reference of the track and wheels
        /// </summary>
        public MeshRenderer track;
        public List<Transform> wheels;
        
        private void RotateWheels(float rotateSpeed)
        {
            if (wheels != null)
            {
                foreach (var wheel in wheels)
                {
                    wheel.Rotate(Vector3.right, rotateSpeed);
                }
            }
        }
            
        private void ScrollTrack(float speed)
        {
            if (track != null && track.sharedMaterial != null)
            {
                track.material.SetFloat("_TrackScrollSpeed", speed);
            }
        }
        
        void Update()
        {
            //计算旋转速度
            float rotateSpeed = moveSpeed * 360 / (2 * Mathf.PI * 0.1f);
            //旋转车轮
            RotateWheels(-rotateSpeed);
            //滚动履带
            ScrollTrack(moveSpeed*0.015f);
        }
    }
}
