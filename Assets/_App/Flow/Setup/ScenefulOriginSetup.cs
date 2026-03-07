using System;
using System.Collections;
using DigitalLove.Casual.Analytics;
using DigitalLove.Casual.Flow.Setup;
using DigitalLove.Global;
using DigitalLove.XR;
using UnityEngine;

namespace DigitalLove.App
{
    public class ScenefulOriginSetup : OriginSetup
    {
        private const float SecsBeforeShowingExceptionPanel = 10;

        [SerializeField] private SceneSetupRequiredPanel sceneSetupRequiredPanel;
        [SerializeField] private DebugBool forceSceneSetupPanel;
        [SerializeField] private SetupFunnelHelper setupFunnelHelper;
        [SerializeField] private MRUKUtil mrukUtil;

        public override void Setup(Action onComplete)
        {
            if (Application.isEditor)
            {
                onComplete();
            }
            else
            {
                CheckScenePermission(onComplete);
            }
        }

        private void CheckScenePermission(Action onComplete)
        {
            string permissionId = OVRPermissionsRequester.GetPermissionId(OVRPermissionsRequester.Permission.Scene);
            float secsBeforeShowingExceptionPanel = SecsBeforeShowingExceptionPanel;
            IEnumerator WaitForPermissionRoutine()
            {
                while (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(permissionId))
                {
                    secsBeforeShowingExceptionPanel -= Time.deltaTime;
                    bool hasToShowPanel = secsBeforeShowingExceptionPanel <= 0 || forceSceneSetupPanel.Value;
                    if (hasToShowPanel && !sceneSetupRequiredPanel.IsActive)
                        sceneSetupRequiredPanel.Show(Application.Quit, Application.productName);
                    yield return null;
                }
                setupFunnelHelper.SendHasScenePermissionEvent();
                sceneSetupRequiredPanel.Hide();
                LoadOrCreateSceneModel(onComplete);
            }
            StartCoroutine(WaitForPermissionRoutine());
        }

        private void LoadOrCreateSceneModel(Action onComplete)
        {
            mrukUtil.HasASceneModel(hasASceneModel =>
            {
                if (hasASceneModel)
                {
                    onComplete();
                }
                else
                {
                    mrukUtil.CreateSceneModel(hasBeenCreated =>
                    {
                        if (hasBeenCreated)
                        {
                            onComplete();
                        }
                        else
                        {
                            sceneSetupRequiredPanel.Show(Application.Quit, Application.productName);
                        }
                    });
                }
            });
        }
    }
}