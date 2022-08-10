using BepInEx;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Mirror;

namespace AntiTrypophobia
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        AssetBundle _assetBundle;
        public static Plugin Instance;

        public Material fertilizerDry;
        public Material fertilizerWet;

        public bool materialsAssigned;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        void Start()
        {
            //BundleLoader();
            LoadAssetBundle("anti-trypophobia");
            BuildMaterials();
        }

        void Update()
        {
            AssignMaterials();
        }

        void BuildMaterials()
        {
            //var prefab = _assetBundle.LoadAsset<GameObject>("Inv_OpalShovel");
            fertilizerDry = _assetBundle.LoadAsset<Material>("tilledDirt1");
            fertilizerWet = _assetBundle.LoadAsset<Material>("wetTilledDirt1");
            _assetBundle.Unload(false);
            Logger.LogInfo("Asset Bundle Loaded: " + fertilizerDry.name);
            Logger.LogInfo("Asset Bundle Loaded:" + fertilizerWet.name);
        }

        void AssignMaterials()
        {
            if (NetworkMapSharer.share.localChar is null) return;
            if (materialsAssigned) return;
            Logger.LogInfo("Assigning Materials.");
            WorldManager.manageWorld.tileTypes[12].gameObject.GetComponent<TileTypes>().myTileMaterial = fertilizerDry;
            WorldManager.manageWorld.tileTypes[13].gameObject.GetComponent<TileTypes>().myTileMaterial = fertilizerWet;
            Logger.LogInfo("Materials Assigned.");
            materialsAssigned = true;
        }

        private Stream GetEmbeddedAssetBundle(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var presumed = assembly.GetManifestResourceNames().ToList().Find(resource => resource.Contains(name));

            if (!string.IsNullOrEmpty(presumed)) return assembly.GetManifestResourceStream(presumed);

            Logger.LogError($"Unable to find any embedded resource with name: {name}.");
            return null;
        }

        private void LoadAssetBundle(string name)
        {
            var resource = GetEmbeddedAssetBundle(name);
            _assetBundle = AssetBundle.LoadFromStream(resource);

            if (_assetBundle != null) return;

            Logger.LogError("Unable to load embedded asset bundle.");
            return;
        }

        private void UnloadAssetBundle()
        {
            _assetBundle.Unload(true);
        }

        void OnDestroy()
        {
            UnloadAssetBundle();
        }
    }
}
