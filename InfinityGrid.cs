////////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                           ▄ ▄▄        █           
//  author      : 721                                        █▀ █ █▀▀█   █▀▀█ █  █ █▀▀█ 
//                                                           █  █ █▄▄█   █▄▄█ █▄▄█ █▄▄█ ▄ ▄ ▄
//  dingding    : oomh9s4                                                          ▄  █
//                                                                                 ▀▀▀▀
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UI
{
    public enum InfinityScrollType
    {
        Vertical,
        Horizontal
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ContentHorizontalAlign
    {
        Left = 0,
        Right = 1,
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ContentVerticalAlign
    {
        Top = 0,
        Bottom = 1,
    }

    public enum ContentPositionState
    {
        LeftBeyond = 0,     //左侧超出
        RightBeyond = 1,    //右侧超出
        NoneBeyond = 2,     //两侧都没超出
        BothBeyond = 3      //两侧都超出
    }


    /// <summary>
    /// 
    /// </summary>
    public class InfinityGrid : MonoBehaviour
    {
        [SerializeField]
        protected RectOffset m_Padding;
        /// <summary>
        /// 
        /// </summary>
        public InfinityScrollType infinityScrollType;
        /// <summary>
        /// 
        /// </summary>
        public ContentHorizontalAlign contentHorizontalAlign;
        /// <summary>
        /// 
        /// </summary>
        public ContentVerticalAlign contentVerticalAlign;

        /// <summary>
        /// 
        /// </summary>
        //public bool ReverseItemAlign;

        /// <summary>
        /// 
        /// </summary>
        public bool ReverseItemSequence;
        /// <summary>
        /// 
        /// </summary>
        public float oblique_x = 0.0f;
        
        [Range(1,1024)]
        public int CellPerLine = 4;

        /// <summary>
        /// 定位到 iIndex item
        /// </summary>
        /// <param name="v"></param>
        public void LocateTo(int iIndex)
        {
            UpdateContentSize();
            var pos = calculatePosition(iIndex);
            var fullSize = content.rect.size;
            var vpSize = viewport.rect.size;
            if (infinityScrollType == InfinityScrollType.Vertical)
            {
                var linePercent = (pos.y - vpSize.y / 2) / (fullSize.y - vpSize.y);
                if (linePercent < 0) linePercent = 0;
                if (linePercent > 1) linePercent = 1;
                scrollRect.normalizedPosition = new Vector2(0,linePercent);
                OnScrollValueChanged(scrollRect.normalizedPosition);
            }
            if (infinityScrollType == InfinityScrollType.Horizontal)
            {
                var linePercent = (pos.x - vpSize.x / 2) / (fullSize.x - vpSize.x);
                if (linePercent < 0) linePercent = 0;
                if (linePercent > 1) linePercent = 1;
                scrollRect.normalizedPosition = new Vector2(linePercent , 0);
                OnScrollValueChanged(scrollRect.normalizedPosition);
            }
        }
        Coroutine m_scrollCor;
        WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();
        /// <summary>
        /// 滚动到 iIndex item
        /// </summary>
        /// <param name="iIndex"></param>
        /// <param name="fTime"></param>
        public void ScrollTo(int iIndex , float fTime = 0.3f)
        {
            if (m_scrollCor != null)
            {
                StopCoroutine(m_scrollCor);
                m_scrollCor = null;
            }
            m_scrollCor = StartCoroutine(_scrollTo(iIndex, fTime));
        }

        private IEnumerator _scrollTo(int iIndex, float fTime)
        {
            var fTimeStart = Time.realtimeSinceStartup;
            yield return null;
            UpdateContentSize();
            var pos = calculatePosition(iIndex);
            var fullSize = content.rect.size;
            var old = scrollRect.normalizedPosition;
            var vpSize = viewport.rect.size;
            if (infinityScrollType == InfinityScrollType.Vertical)
            {
                var linePercent = (pos.y - vpSize.y / 2) / (fullSize.y - vpSize.y);
                if (linePercent < 0) linePercent = 0;
                if (linePercent > 1) linePercent = 1;
                var tgt = new Vector2(0, linePercent);

                do
                {
                    var t = (Time.realtimeSinceStartup - fTimeStart) / fTime;
                    if (t > 1) t = 1;
                    if (t < 0) t = 0;

                    scrollRect.normalizedPosition = (tgt - old) * t + old;
                    OnScrollValueChanged(scrollRect.normalizedPosition);
                    if (t >= 1) break;
                    yield return _waitForEndOfFrame;
                } while (true);
                //scrollRect.normalizedPosition = new Vector2(0, 1 - linePercent);
            }
            if (infinityScrollType == InfinityScrollType.Horizontal)
            {
                var linePercent = (pos.x - vpSize.x / 2) / (fullSize.x - vpSize.x);
                if (linePercent < 0) linePercent = 0;
                if (linePercent > 1) linePercent = 1;
                var tgt = new Vector2(linePercent, 0);
                do
                {
                    var t = (Time.realtimeSinceStartup - fTimeStart) / fTime;
                    if (t > 1) t = 1;
                    if (t < 0) t = 0;
                    scrollRect.normalizedPosition = (tgt - old) * t + old;
                    OnScrollValueChanged(scrollRect.normalizedPosition);
                    if (t >= 1) break;
                    yield return _waitForEndOfFrame;
                } while (true);
            }

            if (m_scrollCor != null)
            {
                StopCoroutine(m_scrollCor);
                m_scrollCor = null;
            }
            yield break;
        }

        /// <summary>
        /// 
        /// </summary>
        //public int ItemInstanceCount = 16;

        /// <summary>
        /// 
        /// </summary>
        public List<GameObject> ItemTemplates;//= new List<GameObject>();

        /// <summary>
        /// 
        /// </summary>
        private List<UnityObjectPool<GameObject>> m_pools;
        /// <summary>
        /// 
        /// </summary>
        private Func<int, int> m_onGetTemplateIndex;
        public void RegisterGetTemplateIndexFunc(Func<int, int> func)
        {
            m_onGetTemplateIndex = func;
        }

        /// <summary>
        /// 
        /// </summary>
        private Action<TemplateCache, GameObject, int> OnBind;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        public void RegisterBindCallback(Action<TemplateCache, GameObject, int> callback)
        {
            OnBind = callback;
        }
        /// <summary>
        /// 
        /// </summary>
        public float LineSize = 128;
        private float m_finalLineSize = 128;

        public bool LimitSlideWhenContentInMask = false;
        
        /// <summary>
        /// Content当前位置类别（0左侧超出， 1右侧超出， 2中间， 3两侧都超出）
        /// </summary>
        public ContentPositionState ContentPositionType { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        //public float VerticalFit;
        /// <summary>
        /// 
        /// </summary>
        public float SizeFit;
        public bool ScaleFit = false;
        /// <summary>
        /// 
        /// </summary>
        public bool KeepRatio = true;

        private bool m_inited = false;
        /// <summary>
        /// 
        /// </summary>
        private int m_totalItemCount = 0;


        public static Texture2D _defaultMaskTexture;
        public static Sprite _defaultMaskSprite;
        public static Color _dmtColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        private void Start()
        {
            var rectmasks = GetComponentsInChildren<RectMask2D>(true);
            if (rectmasks.Length > 0)
            {
                foreach (var rm in rectmasks)
                {
                    var parent = rm.transform.parent;
                    var go = new GameObject("_sp_mask");
                    go.transform.parent = parent;
                    go.layer = parent.gameObject.layer;
                    var sm = go.AddComponent<SpriteMask>();
                    if (_defaultMaskTexture == null)
                    {
                        _defaultMaskTexture = new Texture2D(1, 1);
                        _defaultMaskTexture.SetPixel(0, 0, _dmtColor);

                        _defaultMaskTexture.Apply();
                        _defaultMaskSprite = Sprite.Create(_defaultMaskTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1, 0, SpriteMeshType.Tight, new Vector4(0, 0, 0, 0));

                        _defaultMaskSprite.name = "_sp_ds";
                    }
                    sm.sprite = _defaultMaskSprite;


                    var pt = parent as RectTransform;
                    var st = go.AddComponent<RectTransform>();
                    st.pivot = new Vector2(0.5f, 0.5f);
                    st.localPosition = Vector3.zero;
                    float sx = pt.rect.size.x;
                    float sy = pt.rect.size.y;
                    st.localScale = new Vector3(sx, sy, 1);
                    break;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int TotalItemCount { get { return m_totalItemCount; }
            set {
                if (m_totalItemCount != value)
                {
                    m_totalItemCount = value;

                    if (!m_inited) Init();
                    UpdateContentSize();
                    ForceRefresh();
                    Refresh();
                }
            }
        }

        public void ForceUpdate(int iTotalCount)
        {
            m_totalItemCount = iTotalCount;

            if (!m_inited) Init();
            UpdateContentSize();
            ForceRefresh();
            Refresh();
        }


        public void ReBind(int iIndex)
        {
            if (iIndex >= m_crtFirstInstanceIndex && iIndex <= m_crtLastInstanceIndex)
            {
                ItemInstance stored;//= instances.Get((uint)i);
                if (m_crtItems.TryGetValue(iIndex, out stored))
                {
                    if (stored.ItemGo != null)
                    {
                        var go = stored.ItemGo;
                        go.transform.localScale = Vector3.one * m_fitScale;
                        if (OnBind != null)
                        {
                            var c = GetItemCache(go);
                            c.TemplateId = GetTemplateId(iIndex);
                            OnBind(GetItemCache(go), go, iIndex);
                        }
                        BindControlCallback(go);
                    }
                }
            }
        }

        public void ReBindAll()
        {
            for (int i = m_crtFirstInstanceIndex; i <= m_crtLastInstanceIndex; i++)
            {
                ReBind(i);
            }
        }


        public void Clear()
        {
            ForceRefresh();
            m_totalItemCount = 0;
            m_crtFirstInstanceIndex = -1;
            m_crtLastInstanceIndex = 0;
        }
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<string, Action<int, GameObject>> m_buttonClickCallbacks = new Dictionary<string, Action<int, GameObject>>();
        private Dictionary<string, Action<int, GameObject, bool>> m_toggleClickCallbacks = new Dictionary<string, Action<int, GameObject, bool>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        public void RegisterButtonCallback(string name, Action<int, GameObject> callback)
        {
            m_buttonClickCallbacks[name] = callback;
        }

        public void UnregisterButtonCallback()
        {
            m_buttonClickCallbacks.Clear();
        }

        public void RegisterToggleCallback(string name, Action<int, GameObject, bool> callback)
        {
            m_toggleClickCallbacks[name] = callback;
        }

        public void UnregisterToggleCallback()
        {
            m_toggleClickCallbacks.Clear();
        }
        /// <summary>
        /// 
        /// </summary>
        private void UpdateContentSize()
        {

            //var rt = scrollRect.GetComponent<RectTransform>();

            var line = Mathf.CeilToInt((float)m_totalItemCount / CellPerLine);
            var fixsize = line * m_finalLineSize;
            //m_fullSize = fixsize;
            //var size = viewport.sizeDelta;
            if (infinityScrollType == InfinityScrollType.Vertical)
            {
                //content.sizeDelta = new Vector2(viewport.rect.width, fixsize);
                //content.sizeDelta = new Vector2(viewport.sizeDelta.x, fixsize);
                fixsize += m_Padding.vertical;
                content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, viewport.rect.width);
                content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, fixsize);
                if (LimitSlideWhenContentInMask)
                {
                    bool isLimit = fixsize < viewport.rect.height;
                    scrollRect.enabled = !isLimit;
                }
            }
            else
            {
                //content.sizeDelta = new Vector2(fixsize , viewport.rect.height);
                fixsize += m_Padding.horizontal;
                content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fixsize);
                content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, viewport.rect.height);
                if (LimitSlideWhenContentInMask)
                {
                    bool isLimit = fixsize < viewport.rect.width;
                    scrollRect.enabled = !isLimit;
                }
            }
        }
        //private float m_fullSize;
        //public float FullSize { get {
        //        return m_fullSize;
        //    } }
        /// <summary>
        /// 
        /// </summary>
        private void Refresh()
        {
            if (m_scrollCor!= null)
            {
                StopCoroutine(m_scrollCor);
                m_scrollCor = null;
            }
            var pos = scrollRect.normalizedPosition;
            OnScrollValueChanged(pos);
        }



        ScrollRect scrollRect;
        RectTransform content;
        RectTransform viewport;
        /// <summary>
        /// 
        /// </summary>
        private void Init()
        {
            if (m_inited) return;
            Canvas.ForceUpdateCanvases();

            scrollRect = GetComponent<ScrollRect>();
            scrollRect.horizontal = infinityScrollType == InfinityScrollType.Horizontal;
            scrollRect.vertical = infinityScrollType == InfinityScrollType.Vertical;
            scrollRect.onValueChanged = new ScrollRect.ScrollRectEvent();
            scrollRect.onValueChanged.AddListener(OnScrollValueChanged);

            content = scrollRect.content;
            viewport = scrollRect.viewport;

            float pivotX = 0;
            float pivotY = 0;
            switch (contentVerticalAlign)
            {
                case ContentVerticalAlign.Top:
                    pivotY = 1;
                    break;
                case ContentVerticalAlign.Bottom:
                    pivotY = 0;
                    break;
            }
            switch (contentHorizontalAlign)
            {
                case ContentHorizontalAlign.Left:
                    pivotX = 0;
                    break;
                case ContentHorizontalAlign.Right:
                    pivotX = 1;
                    break;
            }

            //if (ReverseItemAlign)
            //{
            //    content.pivot = new Vector2(infinityScrollType == InfinityScrollType.Horizontal ? 1 - pivotX : pivotX, infinityScrollType == InfinityScrollType.Vertical ? 1 - pivotY : pivotY);
            //}
            //else
            {
                content.pivot = new Vector2(pivotX, pivotY);
            }
            //content.pivot = new Vector2(pivotX, pivotY);



            m_pools = new List<UnityObjectPool<GameObject>>(ItemTemplates.Count);
            foreach (var t in ItemTemplates)
            {
                //change size
                TryFitSize(t);
                m_pools.Add(new UnityObjectPool<GameObject>(() =>
                {
                    var res = GameObject.Instantiate(t);
                    res.name = t.name;
                    //res.transform
                    //var p = StaticGameObjects.GetNodeOnMonoRoot("_pool_");
                    res.SetActive(false);
                    res.transform.SetParent(content);
                    return res;
                }));
                t.SetActive(false);
            }

            //if (TotalItemCount > 0)
            //{
            //    //first refresh
            //    UpdateContentSize();
            //    Refresh();
            //}

            m_inited = true;
        }

        private void OnEnable()
        {
            if (m_inited)
            {
                
                foreach (var t in ItemTemplates)
                {
                    //change size
                    TryFitSize(t);
                }
            }
        }

        //private float /*m_oriLineSize*/ = 0;
        private void TryFitSize(GameObject t)
        {
            //if (HorizontalFit == 0 && VerticalFit == 0)
            //{
            //    return;
            //}
            var rect = t.GetComponent<RectTransform>();
            if (rect == null) return;
            rect.localScale = Vector3.one;
            //if (m_oriLineSize == 0) m_oriLineSize = LineSize;
            m_finalLineSize = LineSize;
            if (infinityScrollType == InfinityScrollType.Vertical && SizeFit > 0)
            {
                //var fixedSize = scrollRectTransform.rect.width - m_Padding.horizontal;
                //var cellSize = fixedSize / CellPerLine;
                //var targetSize = cellSize * SizeFit;
                //var scale = targetSize / rect.rect.width;
                if (ScaleFit)
                {
                    m_fitScale = SizeFit;
                    rect.localScale = new Vector3(SizeFit, SizeFit, SizeFit);
                    m_finalLineSize = LineSize * SizeFit;
                }
                else
                {
                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.rect.width * SizeFit);
                    if (KeepRatio)
                    {
                        var tgtHeight = rect.rect.height * SizeFit;
                        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tgtHeight);
                        
                    }
                    m_finalLineSize = LineSize * SizeFit;
                }
            }

            if (infinityScrollType == InfinityScrollType.Horizontal && SizeFit > 0)
            {
                //var fixedSize = scrollRectTransform.rect.height - m_Padding.vertical;
                //var cellSize = fixedSize / CellPerLine;
                //var targetSize = cellSize * SizeFit;
                //var scale = targetSize / rect.rect.height;
                if (ScaleFit)
                {
                    m_fitScale = SizeFit;
                    rect.localScale = new Vector3(SizeFit, SizeFit, SizeFit);
                    m_finalLineSize = LineSize * SizeFit;
                }
                else
                {
                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.rect.height * SizeFit);
                    if (KeepRatio)
                    {
                        var tgtWidth = rect.rect.width * SizeFit;
                        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tgtWidth);
                        
                    }
                    m_finalLineSize = LineSize * SizeFit;
                }
            }
            //var cache = //GetItemCache(t);
        }
        //获取当前最大多少行
        public int GetMaxVerticalNum()
        {
            int num = 0;
            if (infinityScrollType == InfinityScrollType.Vertical)
            {
                scrollRect = GetComponent<ScrollRect>();
                var scrollRectTransform = scrollRect.GetComponent<RectTransform>();
                var fixedSize = scrollRectTransform.rect.width - m_Padding.horizontal;
                var cellSize = fixedSize / CellPerLine;
                var targetSize = cellSize * SizeFit;
                num = (int)(scrollRectTransform.rect.height / targetSize);
            }
            else
            {
                num = CellPerLine;
            }
            return num;
        }
        /// <summary>
        /// 
        /// </summary>
        public struct ItemInstance
        {
            public int TemplateIndex;
            public GameObject ItemGo;
        }

        /// <summary>
        /// 
        /// </summary>
        private int m_crtFirstInstanceIndex;
        private int m_crtLastInstanceIndex;
        //private ItemInstance[] instances = new ItemInstance[128];

        //private CircleArray<ItemInstance> instances = new CircleArray<ItemInstance>(128);
        /// <summary>
        /// 显示的item
        /// </summary>
        Dictionary<int, ItemInstance> m_crtItems = new Dictionary<int, ItemInstance>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="go"></param>
        /// <param name="tempId"></param>
        /// <param name="iIndex"></param>
        public void StoreInstance(GameObject go, int tempId, int iIndex)
        {
            var o = new ItemInstance() { TemplateIndex = tempId, ItemGo = go };
            //instances.Set((uint)iIndex, ref o);
            m_crtItems[iIndex] = o;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="go"></param>
        private void BindControlCallback(GameObject go)
        {
            if (go == null) return;
            //var goid = go.GetInstanceID();
            var cache = GetItemCache(go);
            if (cache == null) return;
            var buttons = cache.buttons;
            if (buttons != null)
            {
                foreach (var p in buttons) //(int i = 0; i < buttons.Length; i++)
                {
                    var b = p.Value;
                    var name = p.Key;
                    //Action<int, GameObject> outCb = null;
                    //if (m_buttonClickCallbacks.TryGetValue(name, out outCb))
                    //{
                    b.onClick.RemoveAllListeners();
                    b.onClick.AddListener(() => { _TryCall(name, (int)cache.uid, go); });
                    //}
                }
            }

            var toggles = cache.toggles;
            if (toggles != null)
            {
                foreach (var p in toggles)
                {
                    var b = p.Value;
                    var name = p.Key;
                    b.onValueChanged.RemoveAllListeners();
                    b.onValueChanged.AddListener((bool bOn) => { _TryToggleCall(name, (int)cache.uid, go, bOn); });
                }
            }
        }

        private string m_rootUIName = string.Empty;
        private string GetRootUIName()
        {
            if (string.IsNullOrEmpty(m_rootUIName))
            {
                Transform crt = gameObject.transform;
                Canvas cvs = crt.GetComponent<Canvas>();
                while (crt.parent != null)
                {
                    crt = crt.parent;
                    Canvas v = crt.GetComponent<Canvas>();
                    if (v != null) cvs = v;
                }

                if (cvs != null) m_rootUIName = cvs.gameObject.name;
            }
            return m_rootUIName;
        }

        private void _TryCall(string name, int index, GameObject go)
        {
            Action<int, GameObject> outCb = null;
            var id = string.Format("{0}_{1}", GetRootUIName(), name);
            UI.UISoundCenter.PlayByID(id);
            if (m_buttonClickCallbacks.TryGetValue(name, out outCb))
            {
                outCb(index, go);
            }
        }

        private void _TryToggleCall(string name, int index, GameObject go, bool isToggleOn)
        {
            Action<int, GameObject, bool> outCb = null;
            var id = string.Format("{0}_{1}", GetRootUIName(), name);
            UI.UISoundCenter.PlayByID(id);
            if (m_toggleClickCallbacks.TryGetValue(name, out outCb))
            {
                outCb(index, go, isToggleOn);
            }
        }

        public bool OneByOne = false;
        public static bool OneItemPerFrame = true;
        /// <summary>
        /// 隐藏的item
        /// </summary>
        private Dictionary<int, ItemInstance> m_remainItems = new Dictionary<int, ItemInstance>();
        private struct DelayCreateItem
        {
            public int index;
            public int createFrame;
        }
        private Dictionary<int , DelayCreateItem> m_delayCreateItems = new Dictionary<int , DelayCreateItem>();
        private List<int> _deleted = new List<int>();
        private void Update()
        {
            //for(int i = m_delayCreateItems.Count-1 ; i >=0; --i)
            _deleted.Clear();
            foreach (var p in m_delayCreateItems)
            {
                DelayCreateItem item = p.Value;
                var iIdx = item.index;
                if (item.createFrame > Time.renderedFrameCount)
                {
                    continue;
                }
                if (iIdx< m_crtFirstInstanceIndex || iIdx > m_crtLastInstanceIndex)
                {
                    _deleted.Add(iIdx);
                    continue;
                }

                GameObject go = Alloc(iIdx);
                go.transform.localScale = Vector3.one * m_fitScale;
                var tempId = GetTemplateId(iIdx);
                StoreInstance(go, tempId, iIdx);
                PositionItem(go, item.index);
                GetItemCache(go).uid = (uint)iIdx;
                if (OnBind != null)
                {
                    var c = GetItemCache(go);
                    c.TemplateId = tempId;
                    OnBind(c, go, iIdx);
                }
                BindControlCallback(go);
                if (go != null) go.SetActive(true);
                //m_delayCreateItems.RemoveAt(i);
                _deleted.Add(iIdx);
            }
            for (int i = 0; i < _deleted.Count; i++)
            {
                m_delayCreateItems.Remove(_deleted[i]);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg0"></param>
        private void OnScrollValueChanged(Vector2 arg0)
        {
            UpdateContentSize();
            //Debug.LogFormat("pos :{0},{1}", arg0.x, arg0.y);
            bool isLandscape = ScreenAdapt.AdaptManager.IsLandscape();
            float percent = 0;
            float lineCount = 0;
            float lineCountPerPage = 0;

            float innerWidth = content.rect.width - m_Padding.horizontal;
            float innerHeight = content.rect.height - m_Padding.vertical;

            float xFixPercent = (float)m_Padding.left / content.rect.width;
            float vFixPercent = (float)m_Padding.top / content.rect.height;
            if (infinityScrollType == InfinityScrollType.Vertical)
            {
                percent = 1.0f - arg0.y - vFixPercent;
                lineCount = innerHeight / m_finalLineSize;
                lineCountPerPage = Mathf.CeilToInt(viewport.rect.height / m_finalLineSize);
            }
            else
            {
                percent = arg0.x - xFixPercent;
                lineCount = innerWidth / m_finalLineSize;
                lineCountPerPage = Mathf.CeilToInt(viewport.rect.width / m_finalLineSize);
            }

            //中间cell

            var centerPageIndex = lineCountPerPage / 2 + (int)((Mathf.Max(0, lineCount - lineCountPerPage)) * percent);
            var centerIndex = (int)(centerPageIndex * CellPerLine + (float)CellPerLine / 2);//(centerIndex / CellPerLine) * CellPerLine + CellPerLine / 2;
            var pageItemCount = (lineCountPerPage + 1) * CellPerLine;
            var firstIndex = (int)(centerIndex - pageItemCount / 2);
            if (firstIndex < 0) firstIndex = 0;
            var lastIndex = (int)(centerIndex + pageItemCount / 2);
            if (lastIndex >= TotalItemCount) lastIndex = TotalItemCount - 1;

            //if (ReverseItemAlign)
            //{
            //    var lost = CellPerLine - lastIndex % CellPerLine;
            //    if (lost !=0)
            //    {
            //        firstIndex -= lost;
            //    }
            //}

            //隐藏 hide 的 item
            m_remainItems.Clear();
            for (int iIdx = m_crtFirstInstanceIndex; iIdx <= m_crtLastInstanceIndex; iIdx++)
            {
                int i = iIdx;
                //if (ReverseItemSequence)
                //{
                //    i = TotalItemCount - 1 - i;
                //}
                if (iIdx < firstIndex || iIdx > lastIndex)
                {
                    ClearInstance(i);
                    //var stored = instances.Get((uint)i);//instances[i - m_crtLastInstanceIndex];
                    //if (stored.ItemGo != null)
                    //{
                    //    m_pools[stored.TemplateIndex].DeallocObject(stored.ItemGo);
                    //}
                    //instances.Set((uint)i, ref IINONE);
                }
                else
                {
                    ItemInstance stored;//= instances.Get((uint)i);
                    if (m_crtItems.TryGetValue(i, out stored))
                    {
                        if (stored.ItemGo != null)
                        {
                            m_remainItems[i] = stored;
                        }
                        //instances.Set((uint)i, ref IINONE);
                        m_crtItems.Remove(i);
                    }
                }
            }

            //刷新新的
            m_crtFirstInstanceIndex = firstIndex;
            m_crtLastInstanceIndex = lastIndex;
            var iCreateFrameCount = 0;
            var firstRow = firstIndex / CellPerLine;
            if (firstIndex % CellPerLine != 0)
            {
                firstRow++;
            }
            for (int iIdx = firstIndex; iIdx <= lastIndex; iIdx++)
            {
                int i = iIdx;
                //if (ReverseItemSequence)
                //{
                //    i = TotalItemCount - 1 - i;
                //}
                if (i < 0) continue;
                if (i >= TotalItemCount) break;

                //首先查找
                ItemInstance ii;
                GameObject go = null;
                var tempId = GetTemplateId(i);
                if (m_remainItems.TryGetValue(i, out ii))
                {
                    go = ii.ItemGo;
                    StoreInstance(go, tempId, i);
                    PositionItem(go, iIdx);
                    GetItemCache(go).uid = (uint)i;
                }
                else
                {
                    if (!HasFreeCacheObject(i) && OneByOne)
                    {
                        if (OneItemPerFrame)
                        {
                            var item = new DelayCreateItem() { index = i, createFrame = Time.renderedFrameCount + (++iCreateFrameCount) };
                            m_delayCreateItems[item.index] = item;
                        }
                        else
                        {
                            int row = i / CellPerLine - firstRow;
                            //int col = i % CellPerLine;
                            var item = new DelayCreateItem() { index = i, createFrame = Time.renderedFrameCount + (++iCreateFrameCount - (row * (CellPerLine - 1))) * 2 };
                            m_delayCreateItems[item.index] = item;
                        }
                    }
                    else
                    {
                        go = Alloc(i);
                        go.transform.localScale = Vector3.one * m_fitScale;
                        StoreInstance(go, tempId, i);
                        PositionItem(go, iIdx);
                        GetItemCache(go).uid = (uint)i;
                        if (OnBind != null)
                        {
                            var c = GetItemCache(go);
                            c.TemplateId = tempId;
                            OnBind(c, go, i);
                        }
                        BindControlCallback(go);
                    }
                }

                if (go == null) continue;

                go.SetActive(true);
                //Debug.LogFormat("init item {0}", i);

                if (lineCount > 0 && lineCountPerPage > 0 && CellPerLine == 1 && isLandscape)
                {
                    float fRatio = 0f;
                    float offsetX = 0f;
                    if (lineCount > lineCountPerPage) // divide by 0
                        fRatio = 1.0f / (lineCount - lineCountPerPage);
                    if (fRatio > 0f)
                        offsetX = ((i - centerIndex) + 1.0f - (percent % fRatio) / fRatio) * oblique_x;
                    else
                        offsetX = ((i - centerIndex)+ 1.0f) * oblique_x;
                    
                    var cache = GetItemCache(go);
                    cache.rootRectTransform.anchoredPosition3D = new Vector3(cache.rootRectTransform.anchoredPosition3D.x + offsetX,
                        cache.rootRectTransform.anchoredPosition3D.y, cache.rootRectTransform.anchoredPosition3D.z);
                }
            }


            PositionContent();
        }

        private void PositionContent()
        {
            float viewPortSize = 0;
            float contentPos = 0;
            if (infinityScrollType == InfinityScrollType.Horizontal)
            {
                viewPortSize = scrollRect.viewport.rect.width;
                contentPos = content.localPosition.x;
            }
            else if (infinityScrollType == InfinityScrollType.Vertical)
            {
                viewPortSize = scrollRect.viewport.rect.height;
                contentPos = content.localPosition.y;
            }

            float intervalDistance = viewPortSize - m_finalLineSize * TotalItemCount;
            ContentPositionType =  ContentPositionState.NoneBeyond;
            if (contentPos < -1f && contentPos < intervalDistance)
            {
                ContentPositionType = ContentPositionState.LeftBeyond;
            }
            else if (contentPos > -1f && contentPos > intervalDistance)
            {
                ContentPositionType = ContentPositionState.RightBeyond;
            }
            else if (contentPos < -1f && contentPos > intervalDistance)
            {
                ContentPositionType = ContentPositionState.BothBeyond;
            }
        }


        private Vector3 calculatePosition(int index)
        {
            if (ReverseItemSequence)
            {
                index = TotalItemCount - 1 - index;
            }

            var line = index / CellPerLine;
            var lineIndex = index % CellPerLine;


            if (infinityScrollType == InfinityScrollType.Vertical)
            {
                var width = content.rect.width - m_Padding.horizontal;
                var height = content.rect.height;// - m_Padding.vertical;
                var cellWidth = width / CellPerLine;
                var x = lineIndex * cellWidth + cellWidth / 2 + m_Padding.left;
                var y = height - (m_finalLineSize * line + m_finalLineSize / 2) - m_Padding.top;

                return new Vector3(x, y, 0);
            }
            if (infinityScrollType == InfinityScrollType.Horizontal)
            {
                var width = content.rect.width;// - m_Padding.horizontal;
                var height = content.rect.height - m_Padding.vertical;
                var cellHeight = height / CellPerLine;
                var y = height - (lineIndex * cellHeight + cellHeight / 2) - m_Padding.top;
                var x = (m_finalLineSize * line + m_finalLineSize / 2) + m_Padding.left;
                return new Vector3(x, y, 0);
            }
            return Vector3.zero;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="go"></param>
        /// <param name="index"></param>
        private void PositionItem(GameObject go, int index)
        {
            var pos = calculatePosition(index);
            var cache = GetItemCache(go);

            cache.rootRectTransform.anchorMin = Vector2.zero;
            cache.rootRectTransform.anchorMax = Vector2.zero;
            cache.rootRectTransform.anchoredPosition3D = pos;
            //if (ReverseItemSequence)
            //{
            //    index = TotalItemCount - 1 - index;
            //}

            //var line = index / CellPerLine;
            //var lineIndex = index % CellPerLine;


            //if (infinityScrollType == InfinityScrollType.Vertical)
            //{
            //    var width = content.rect.width;
            //    var height = content.rect.height;
            //    var cellWidth = width / CellPerLine;
            //    var x = lineIndex * cellWidth + cellWidth / 2;
            //    var y = height - (LineSize * line + LineSize / 2);
            //    var cache = GetItemCache(go);

            //    cache.rootRectTransform.anchorMin = Vector2.zero;
            //    cache.rootRectTransform.anchorMax = Vector2.zero;
            //    cache.rootRectTransform.anchoredPosition3D = new Vector3(x, y, 0);
            //}
            //if (infinityScrollType == InfinityScrollType.Horizontal)
            //{
            //    var width = content.rect.width;
            //    var height = content.rect.height;
            //    var cellHeight = height / CellPerLine;
            //    var y = height - (lineIndex * cellHeight + cellHeight / 2);
            //    var x = (LineSize * line + LineSize / 2);
            //    var cache = GetItemCache(go);
            //    cache.rootRectTransform.anchorMin = Vector2.zero;
            //    cache.rootRectTransform.anchorMax = Vector2.zero;
            //    cache.rootRectTransform.anchoredPosition3D = new Vector3(x, y, 0);
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int GetTemplateId(int index)
        {
            var tempId = 0;
            if (m_onGetTemplateIndex != null)
            {
                tempId = m_onGetTemplateIndex(index);
            }
            return tempId;
        }

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<int, TemplateCache> m_caches = new Dictionary<int, TemplateCache>();
        private float m_fitScale = 1.0f;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public TemplateCache GetItemCache(GameObject item)
        {
            TemplateCache res;
            if (m_caches.TryGetValue(item.GetInstanceID(), out res))
            {
                return res;
            }

            res = TemplateCache.GetCache(item);
            m_caches[item.GetInstanceID()] = res;
            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private GameObject Alloc(int index)
        {
            var tempId = GetTemplateId(index);

            if (ItemTemplates == null) return null;
            if (ItemTemplates.Count <= tempId) return null;

            var go = m_pools[tempId].AllocObject();
            go.transform.SetParent(content);
            var cache = TemplateCache.GetCache(go);
            m_caches[go.GetInstanceID()] = cache;
            go.SetActive(true);
            return go;
        }

        private bool HasFreeCacheObject(int index)
        {
            var tempId = GetTemplateId(index);

            if (ItemTemplates == null) return false;
            if (ItemTemplates.Count <= tempId) return false;

            return m_pools[tempId].HasFreeObject();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        private void ClearInstance(int i)
        {
            ItemInstance stored;//= instances.Get((uint)i);//instances[i - m_crtLastInstanceIndex];
            if (m_crtItems.TryGetValue(i, out stored))
            {
                if (stored.ItemGo != null)
                {
                    stored.ItemGo.SetActive(false);
                    m_pools[stored.TemplateIndex].DeallocObject(stored.ItemGo);
                }
                //instances.Set((uint)i, ref IINONE);
                m_crtItems.Remove(i);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ForceRefresh()
        {
            for (int iIdx = m_crtLastInstanceIndex; iIdx >= 0; iIdx--)
            {
                int i = iIdx;
                ClearInstance(i);
            }
            //for (int iIdx = m_crtFirstInstanceIndex; iIdx <= m_crtLastInstanceIndex; iIdx++)
            //{
            //    int i = iIdx;

            //    ClearInstance(i);
            //}
            if (m_crtItems.Count > 0)
            {
                Debug.LogError("ForceRefresh error");
            }
            m_crtItems.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iIndex"></param>
        public GameObject GetItem(int iIndex)
        {
            if (iIndex >= m_crtFirstInstanceIndex && iIndex <= m_crtLastInstanceIndex)
            {
                ItemInstance stored;//= instances.Get((uint)i);
                if (m_crtItems.TryGetValue(iIndex, out stored))
                {
                    return stored.ItemGo;
                }
            }
            return null;
        }

        public void SetHighLight(int iIndex, bool bHighlight, string ctrlName)
        {
            ItemInstance stored;//= instances.Get((uint)i);
            if (m_crtItems.TryGetValue(iIndex, out stored))
            {
                if (stored.ItemGo != null)
                {
                    var go = stored.ItemGo;
                    var c = GetItemCache(go);

                    if (bHighlight)
                    {
                        GameObject ctrl = null;
                        if (c.controls.TryGetValue(ctrlName, out ctrl))
                        {
                            ctrl.SetActive(true);
                        }
                    }
                    else
                    {
                        GameObject ctrl = null;
                        if (c.controls.TryGetValue(ctrlName, out ctrl))
                        {
                            ctrl.SetActive(false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iIndex"></param>
        /// <param name="ctrlName"></param>
        public void HighLight(int iHighLightIndex, string ctrlName)
        {
            for (int iIndex = m_crtFirstInstanceIndex; iIndex <= m_crtLastInstanceIndex; iIndex++)
            {
                ItemInstance stored;//= instances.Get((uint)i);
                if (m_crtItems.TryGetValue(iIndex, out stored))
                {
                    if (stored.ItemGo != null)
                    {
                        var go = stored.ItemGo;
                        var c = GetItemCache(go);

                        if (iIndex == iHighLightIndex)
                        {
                            GameObject ctrl = null;
                            if (c.controls.TryGetValue(ctrlName, out ctrl))
                            {
                                ctrl.SetActive(true);
                            }
                        }
                        else
                        {
                            GameObject ctrl = null;
                            if (c.controls.TryGetValue(ctrlName, out ctrl))
                            {
                                ctrl.SetActive(false);
                            }
                        }
                    }
                }
            }
        }

        public void HighLight2State(int iHighLightIndex, string ctrlName)
        {
            for (int iIndex = m_crtFirstInstanceIndex; iIndex <= m_crtLastInstanceIndex; iIndex++)
            {
                ItemInstance stored;//= instances.Get((uint)i);
                if (m_crtItems.TryGetValue(iIndex, out stored))
                {
                    if (stored.ItemGo != null)
                    {
                        var go = stored.ItemGo;
                        var c = GetItemCache(go);


                        if (iIndex == iHighLightIndex)
                        {
                            GameObject ctrl = null;
                            if (c.controls.TryGetValue(ctrlName, out ctrl))
                            {
                                var childCount = ctrl.transform.childCount;
                                if (childCount >= 1)
                                {
                                    ctrl.transform.GetChild(0).gameObject.SetActive(false);
                                }
                                if (childCount >= 2)
                                {
                                    ctrl.transform.GetChild(1).gameObject.SetActive(true);
                                }
                            }
                        }
                        else
                        {
                            GameObject ctrl = null;
                            if (c.controls.TryGetValue(ctrlName, out ctrl))
                            {
                                var childCount = ctrl.transform.childCount;
                                if (childCount >= 1)
                                {
                                    ctrl.transform.GetChild(0).gameObject.SetActive(true);
                                }
                                if (childCount >= 2)
                                {
                                    ctrl.transform.GetChild(1).gameObject.SetActive(false);
                                }
                            }
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iHighLightIndex"></param>
        /// <param name="ctrlName"></param>
        /// <param name="highlight"></param>
        /// <param name="normal"></param>
        public void HighLightByChangeSprite(int iHighLightIndex, string ctrlName , Sprite highlight , Sprite normal)
        {
            for (int iIndex = m_crtFirstInstanceIndex; iIndex <= m_crtLastInstanceIndex; iIndex++)
            {
                ItemInstance stored;//= instances.Get((uint)i);
                if (m_crtItems.TryGetValue(iIndex, out stored))
                {
                    if (stored.ItemGo != null)
                    {
                        var go = stored.ItemGo;
                        var c = GetItemCache(go);

                        if (iIndex == iHighLightIndex)
                        {
                            Image ctrl = null;
                            if (c.images.TryGetValue(ctrlName, out ctrl))
                            {
                                ctrl.sprite = highlight;
                            }
                        }
                        else
                        {
                            Image ctrl = null;
                            if (c.images.TryGetValue(ctrlName, out ctrl))
                            {
                                ctrl.sprite = normal;
                            }
                        }
                    }
                }
            }
        }

        public void EditorRefresh()
        {
            Canvas.ForceUpdateCanvases();
            for (int iIdx = m_crtLastInstanceIndex; iIdx >= 0; iIdx--)
            {
                int i = iIdx;
                ClearInstance(i);
            }

            scrollRect = GetComponent<ScrollRect>();
            scrollRect.horizontal = infinityScrollType == InfinityScrollType.Horizontal;
            scrollRect.vertical = infinityScrollType == InfinityScrollType.Vertical;

            content = scrollRect.content;
            viewport = scrollRect.viewport;

            float pivotX = 0;
            float pivotY = 0;
            switch (contentVerticalAlign)
            {
                case ContentVerticalAlign.Top:
                    pivotY = 1;
                    break;
                case ContentVerticalAlign.Bottom:
                    pivotY = 0;
                    break;
            }
            switch (contentHorizontalAlign)
            {
                case ContentHorizontalAlign.Left:
                    pivotX = 0;
                    break;
                case ContentHorizontalAlign.Right:
                    pivotX = 1;
                    break;
            }

            {
                content.pivot = new Vector2(pivotX, pivotY);
            }

            if (m_pools != null)
            {
                foreach (var pool in m_pools)
                {
                    pool.Clear();
                }
            }
            m_pools.Clear();
            m_pools = new List<UnityObjectPool<GameObject>>(ItemTemplates.Count);
            foreach (var t in ItemTemplates)
            {
                //change size
                TryFitSize(t);
                m_pools.Add(new UnityObjectPool<GameObject>(() =>
                {
                    var res = GameObject.Instantiate(t);
                    //res.transform
                    //var p = StaticGameObjects.GetNodeOnMonoRoot("_pool_");
                    res.SetActive(false);
                    res.transform.SetParent(content);
                    return res;
                }));
                t.SetActive(false);
            }

            UpdateContentSize();
            ForceRefresh();
            Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        //public void RemoveItemAtIndex(int iIndex)
        //{
        //    //var revIndex = iIndex;
        //    //if (ReverseItemSequence)
        //    //{
        //    //    revIndex = TotalItemCount - 1 - revIndex;
        //    //}
        //    //if (revIndex >= m_crtFirstInstanceIndex && revIndex <= m_crtLastInstanceIndex)
        //    //{
        //    //    ClearInstance(iIndex);
        //    //    if (!ReverseItemSequence)
        //    //    {
        //    //        for (int i = revIndex; i < m_crtLastInstanceIndex; i++)
        //    //        {
        //    //            var realIndex = i;
        //    //            var o = instances.Get((uint)(realIndex + 1));
        //    //            instances.Set((uint)realIndex, ref o);
        //    //        }
        //    //        m_crtLastInstanceIndex--;
        //    //    }
        //    //    else
        //    //    {
        //    //        for (int i = revIndex; i < m_crtLastInstanceIndex; i++)
        //    //        {
        //    //            var realIndex = TotalItemCount - 1 - i;
        //    //            var o = instances.Get((uint)(realIndex - 2));
        //    //            instances.Set((uint)realIndex, ref o);
        //    //        }
        //    //        m_crtLastInstanceIndex--;
        //    //    }

        //    //}
        //    ForceRefresh();
        //    TotalItemCount--;
        //    //Refresh();
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="iIndex"></param>
        //public void InsertItemAtIndex(int iIndex)
        //{
        //    //if (iIndex >= m_crtFirstInstanceIndex && iIndex <= m_crtLastInstanceIndex)
        //    //{
        //    //    for (int i = m_crtLastInstanceIndex; i >= iIndex; i--)
        //    //    {
        //    //        var o = instances.Get((uint)(i));
        //    //        instances.Set((uint)(i+1), ref o);
        //    //    }
        //    //    ClearInstance(iIndex);
        //    //    m_crtLastInstanceIndex++;
        //    //}
        //    ForceRefresh();
        //    TotalItemCount++;
        //}
    }
}
