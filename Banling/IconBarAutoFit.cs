using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    /// <summary>
    /// IconBar 高度自适应：挂在 IconBar 根节点上（9-30 建）。
    ///
    /// IconBar 是从右上角往下挂的一列固定尺寸按钮。游戏画布按 1920×1080、Match 0.5 缩放，
    /// 窗口越扁画布越矮（画布高 = 1440 × √(高/宽)），宽高比超过约 2.49:1 时最底下的按钮
    /// 会被屏幕底边切掉（1920×700 时「新手指引」少一截；20:9 手机只剩 53 的余量）。
    ///
    /// 放不下时按顺序让步：
    ///   1. 先压按钮间距（设计值 → minSpacing）；
    ///   2. 还放不下就整列等比缩小，以轴心为基准（IconBar 轴心在右上角，右上角不动），最小到 minScale。
    /// 放得下就保持设计值，不会放大。
    ///
    /// 只在运行时生效，prefab 里存的间距和缩放不会被改，编辑器里摆 UI 看到的仍是设计值。
    /// 只动自己的 spacing 和 localScale；MainUI 的滑入滑出只改 anchoredPosition.x，互不干扰。
    /// 内容按 Upper 对齐计算（IconBar 用的是 UpperCenter）。
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(VerticalLayoutGroup))]
    public class IconBarAutoFit : MonoBehaviour
    {
        [Tooltip("最底下的按钮离屏幕底边至少留多少（画布单位）")]
        public float bottomMargin = 20f;

        [Tooltip("间距最多压到多少；压到这里还放不下才开始整体缩小")]
        public float minSpacing = 8f;

        [Tooltip("最多缩到多少倍，免得极端窗口下小到点不中")]
        [Range(0.3f, 1f)]
        public float minScale = 0.6f;

        private RectTransform _rt;
        private VerticalLayoutGroup _layout;
        private Canvas _canvas;
        private float _designSpacing;
        private Vector3 _designScale;
        private bool _inited;

        // 上次计算用到的输入；都没变就不重算，LateUpdate 里每帧只比这几个数
        private float _lastRootHeight = -1f;
        private float _lastPivotY;
        private float _lastUnit;
        private int _lastActiveMask = -1;

        private void Awake() => Init();

        private void OnEnable()
        {
            Init();
            _canvas = null;           // 可能换过父节点，重新找画布
            _lastRootHeight = -1f;    // 每次显示都重算一次，第一帧就是对的
            Refit();
        }

        private void OnTransformParentChanged() => _canvas = null;

        private void LateUpdate() => Refit();

        private void Init()
        {
            if (_inited) return;
            _rt = (RectTransform)transform;
            _layout = GetComponent<VerticalLayoutGroup>();
            _designSpacing = _layout.spacing;
            _designScale = _rt.localScale;
            _inited = true;
        }

        /// <summary>按当前画布高度重新排一次；画布、位置、显示的按钮都没变时直接返回。</summary>
        public void Refit()
        {
            Init();
            if (_canvas == null) _canvas = GetComponentInParent<Canvas>();
            if (_canvas == null || _rt.parent == null) return;

            var root = (RectTransform)_canvas.rootCanvas.transform;
            Rect rootRect = root.rect;
            if (rootRect.height <= 0f) return;

            // 轴心在根画布里的高度（不受自身缩放影响）；父节点 1 个单位折合根画布多少单位
            float pivotY = root.InverseTransformPoint(_rt.position).y;
            float unit = Mathf.Abs(root.InverseTransformVector(_rt.parent.TransformVector(Vector3.up)).y);
            if (unit <= 0f) return;

            int activeMask = ActiveChildMask();
            if (Mathf.Approximately(rootRect.height, _lastRootHeight) && Mathf.Approximately(pivotY, _lastPivotY)
                && Mathf.Approximately(unit, _lastUnit) && activeMask == _lastActiveMask)
                return;
            _lastRootHeight = rootRect.height;
            _lastPivotY = pivotY;
            _lastUnit = unit;
            _lastActiveMask = activeMask;

            // 轴心往下到「屏幕底边 + 留白」还有多高（父节点单位）
            float room = (pivotY - rootRect.yMin - bottomMargin) / unit;
            float childrenHeight = ChildrenHeight(out int count);
            float gaps = Mathf.Max(0, count - 1);
            // 内容从矩形顶边往下排，轴心往下要占的高度 = 内容总高 - 轴心到顶边那段
            float NeedBelowPivot(float s) =>
                _layout.padding.top + _layout.padding.bottom + childrenHeight + s * gaps - _rt.rect.yMax;

            float spacing = _designSpacing;
            float scale = 1f;
            float need = NeedBelowPivot(_designSpacing);
            if (need > room)
            {
                float tightest = Mathf.Min(minSpacing, _designSpacing);
                float needTight = NeedBelowPivot(tightest);
                if (gaps > 0 && needTight <= room)
                {
                    spacing = _designSpacing - (need - room) / gaps;   // 刚好压到放得下
                }
                else
                {
                    spacing = tightest;
                    scale = needTight > 0f ? Mathf.Clamp(room / needTight, minScale, 1f) : 1f;
                }
            }

            if (!Mathf.Approximately(_layout.spacing, spacing)) _layout.spacing = spacing;
            var targetScale = new Vector3(_designScale.x * scale, _designScale.y * scale, _designScale.z);
            if (_rt.localScale != targetScale) _rt.localScale = targetScale;
        }

        // 跟 VerticalLayoutGroup 的算法一致：不控制子节点高度时用 sizeDelta.y，勾了 Child Scale 再乘缩放
        private float ChildrenHeight(out int count)
        {
            float sum = 0f;
            count = 0;
            for (int i = 0; i < _rt.childCount; i++)
            {
                if (!(_rt.GetChild(i) is RectTransform child) || !child.gameObject.activeSelf) continue;
                var ignorer = child.GetComponent<ILayoutIgnorer>();
                if (ignorer != null && ignorer.ignoreLayout) continue;
                float h = _layout.childControlHeight ? LayoutUtility.GetPreferredHeight(child) : child.sizeDelta.y;
                if (_layout.childScaleHeight) h *= child.localScale.y;
                sum += h;
                count++;
            }
            return sum;
        }

        // 哪些按钮开着（按位），有按钮显隐变化时触发重算
        private int ActiveChildMask()
        {
            int mask = 0;
            for (int i = 0; i < _rt.childCount && i < 31; i++)
                if (_rt.GetChild(i).gameObject.activeSelf) mask |= 1 << i;
            return mask;
        }
    }
}
