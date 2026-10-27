using UnityEngine;

namespace GameLogic
{
    /// <summary>留声机插画的播放动画；转速、唱臂角度和过渡时间保存在 UI prefab。</summary>
    public sealed class GramophoneArtworkAnimation : MonoBehaviour
    {
        [SerializeField] private RectTransform record;
        [SerializeField] private RectTransform tonearm;
        [SerializeField] private float recordDegreesPerSecond;
        [SerializeField] private float playingArmAngle;
        [SerializeField] private float pausedArmAngle;
        [SerializeField] private float idleArmAngle;
        [SerializeField] private float armTransitionSeconds;

        private bool _available;
        private bool _paused;
        private float _armVelocity;

        public void SetPlaybackState(bool available, bool paused)
        {
            _available = available;
            _paused = paused;
        }

        private void OnEnable()
        {
            // 隐藏时音乐可以继续播放；重新打开时同步当前状态，不重置唱片角度。
            _armVelocity = 0f;
            if (tonearm != null) tonearm.localRotation = Quaternion.Euler(0f, 0f, TargetArmAngle);
        }

        private float TargetArmAngle => !_available ? idleArmAngle
            : _paused ? pausedArmAngle : playingArmAngle;

        private void Update()
        {
            float delta = Time.unscaledDeltaTime;
            if (record != null && _available && !_paused)
                record.Rotate(0f, 0f, -recordDegreesPerSecond * delta);
            if (tonearm == null) return;
            float angle = Mathf.SmoothDampAngle(tonearm.localEulerAngles.z, TargetArmAngle,
                ref _armVelocity, Mathf.Max(0.01f, armTransitionSeconds), Mathf.Infinity, delta);
            tonearm.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
