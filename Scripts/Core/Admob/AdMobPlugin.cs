using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Core.Admob
{
    [AddComponentMenu("Mugd/Ads/AdMob")]
    public class AdMobPlugin : MonoBehaviour
    {
        public string PublisherId = "YOUR_PUBLISHER_ID";
        public bool LoadOnStart = true;
        public bool LoadOnReconfigure = true;
        public float RefreshInterval = 30;
        public AdSize Size = AdSize.Banner;
        public AdOrientation Orientation = AdOrientation.Horizontal;
        public AdHorizontalPosition HorizontalPosition = AdHorizontalPosition.CenterHorizontal;
        public AdVerticalPosition VerticalPosition = AdVerticalPosition.Bottom;
        public bool IsTesting = true;
        public bool GuessSelfDeviceId = true;
        public string[] TestDeviceIds = {"TEST_DEVICE_ID"};
        public AdMobTarget Target = new AdMobTarget();

        private AndroidJavaObject _plugin;
        private bool _visible = true;

        public static AdMobPlugin Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void Start()
        {
            Initialize();
            if (LoadOnStart)
            {
                Load();
            }
            StartCoroutine(Refresh());
        }

        public bool IsVisible()
        {
            return _visible;
        }

        private IEnumerator Refresh()
        {
            while (true)
            {
                if (RefreshInterval <= 0) continue;
                yield return new WaitForSeconds(RefreshInterval < 30 ? 30 : RefreshInterval);
                Load();
            }
        }

#if UNITY_ANDROID

        private void Initialize()
        {
            var pluginClass = new AndroidJavaClass("com.guillermonkey.unity.admob.AdMobPlugin");
            _plugin = pluginClass.CallStatic<AndroidJavaObject>(
                "getInstance",
                PublisherId,
                IsTesting,
                TestDeviceIds,
                GuessSelfDeviceId,
                (int) Size,
                (int) Orientation,
                (int) HorizontalPosition,
                (int) VerticalPosition);
        }

        public void Reconfigure()
        {
            _plugin.Call(
                "reconfigure",
                PublisherId,
                IsTesting,
                TestDeviceIds,
                GuessSelfDeviceId,
                (int) Size,
                (int) Orientation,
                (int) HorizontalPosition,
                (int) VerticalPosition);
            if (LoadOnReconfigure) Load();
        }

        public void SetTarget()
        {
            _plugin.Call(
                "setTarget",
                (int) Target.Gender,
                Target.Birthday.Year,
                (int) Target.Birthday.Month,
                Target.Birthday.Day,
                Target.Keywords,
                Target.Location.Latitude,
                Target.Location.Longitude,
                Target.Location.Altitude);
        }

        public void Load()
        {
            if (_visible) _plugin.Call("load");
        }

        public void Show()
        {
            _plugin.Call("show");
            _visible = true;
        }

        public void Hide()
        {
            _plugin.Call("hide");
            _visible = false;
        }

        public string GetLastError()
        {
            return (_plugin.Call<string>("getLastError"));
        }

        public int GetReceived()
        {
            return (_plugin.Call<int>("getReceived"));
        }
#else
        private void Initialize()
        {
        }

        public void Reconfigure()
        {
        }

        public void SetTarget()
        {
        }

        public void Load()
        {
        }

        public void Show()
        {
            _visible = true;
        }

        public void Hide()
        {
            _visible = false;
        }

        public string GetLastError()
        {
            return string.Empty;
        }

        public int GetReceived()
        {
            return 0;
        }
#endif
    }

    public enum AdSize
    {
        Banner,
        IabMrect,
        IabBanner,
        IabLeaderboard,
        SmartBanner
    };

    public enum AdOrientation
    {
        Horizontal,
        Vertical
    };

    public enum AdHorizontalPosition
    {
        CenterHorizontal,
        Left,
        Right
    };

    public enum AdVerticalPosition
    {
        CenterVertical,
        Top,
        Bottom
    };

    public enum AdGender
    {
        Unknown,
        Male,
        Female
    };

    public enum AdMonth
    {
        January,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    };

    [Serializable]
    public class AdDateTime
    {
        public int Day;
        public AdMonth Month;
        public int Year;
    }

    [Serializable]
    public class AdLocation
    {
        public double Altitude = double.NaN;
        public double Latitude = double.NaN;
        public double Longitude = double.NaN;
    }

    [Serializable]
    public class AdMobTarget
    {
        public AdDateTime Birthday = new AdDateTime();
        public AdGender Gender = AdGender.Unknown;
        public string[] Keywords = new string[0];
        public AdLocation Location = new AdLocation();
    }
}