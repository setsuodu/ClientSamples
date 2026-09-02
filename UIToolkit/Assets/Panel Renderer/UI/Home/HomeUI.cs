using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public sealed class HomeUI : MonoBehaviour
{
    [SerializeField]
    private VisualTreeAsset loginUxml;

    private PanelRenderer panelRenderer;

    private VisualElement homeRoot;
    private VisualElement loginLayer;

    private VisualElement loginPopup;

    private Button loginButton;

    private int uiVersion = -1;

    private void Awake()
    {
        panelRenderer = GetComponent<PanelRenderer>();

        panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    private void OnDestroy()
    {
        if (panelRenderer != null)
        {
            panelRenderer.UnregisterUIReloadCallback(OnUIReload);
        }

        UnbindHomeEvents();
        UnbindLoginEvents();

        loginPopup = null;
        loginLayer = null;
        homeRoot = null;
    }

    private void OnUIReload(
        PanelRenderer renderer,
        VisualElement root,
        int version)
    {
        /*
         * Unity 6000.5.x:
         *
         * UIReload can happen more than once.
         * Do not blindly Add() dynamic UI every time.
         */

        if (uiVersion == version)
        {
            return;
        }

        uiVersion = version;

        // --------------------------------------------------
        // 1. Clean references to previous UI
        // --------------------------------------------------

        UnbindHomeEvents();
        UnbindLoginEvents();

        loginPopup = null;

        // --------------------------------------------------
        // 2. Cache new root
        // --------------------------------------------------

        homeRoot = root;

        loginLayer = homeRoot.Q<VisualElement>("login-layer");

        if (loginLayer == null)
        {
            Debug.LogError(
                "[HomeUI] login-layer was not found."
            );

            return;
        }

        // --------------------------------------------------
        // 3. Bind Home
        // --------------------------------------------------

        loginButton =
            homeRoot.Q<Button>("btn-login");

        if (loginButton == null)
        {
            Debug.LogError(
                "[HomeUI] btn-login was not found."
            );

            return;
        }

        loginButton.clicked += OpenLogin;

        // --------------------------------------------------
        // 4. UI Reload means this is a fresh root.
        //    Do not carry old VisualElement references.
        // --------------------------------------------------

        loginPopup = null;
    }

    // ======================================================
    // HOME
    // ======================================================

    private void UnbindHomeEvents()
    {
        if (loginButton != null)
        {
            loginButton.clicked -= OpenLogin;
            loginButton = null;
        }
    }

    // ======================================================
    // LOGIN
    // ======================================================

    private void OpenLogin()
    {
        if (loginLayer == null)
        {
            return;
        }

        // Already open.
        if (loginPopup != null)
        {
            return;
        }

        if (loginUxml == null)
        {
            Debug.LogError(
                "[HomeUI] Login UXML is not assigned."
            );

            return;
        }

        // ----------------------------------------------
        // Instantiate Login UXML
        // ----------------------------------------------

        TemplateContainer instance =
            loginUxml.Instantiate();

        if (instance == null)
        {
            Debug.LogError(
                "[HomeUI] Failed to instantiate Login UXML."
            );

            return;
        }

        loginPopup = instance;

        // ----------------------------------------------
        // Add to Home's popup layer
        // ----------------------------------------------

        loginLayer.Add(loginPopup);

        // ----------------------------------------------
        // Bind Login controls
        // ----------------------------------------------

        BindLoginEvents(loginPopup);
    }

    private void BindLoginEvents(
        VisualElement popup)
    {
        Button closeButton =
            popup.Q<Button>("btn-close");

        Button submitButton =
            popup.Q<Button>("btn-submit");

        if (closeButton != null)
        {
            closeButton.clicked += CloseLogin;
        }

        if (submitButton != null)
        {
            submitButton.clicked += SubmitLogin;
        }
    }

    private void UnbindLoginEvents()
    {
        if (loginPopup == null)
        {
            return;
        }

        Button closeButton =
            loginPopup.Q<Button>("btn-close");

        Button submitButton =
            loginPopup.Q<Button>("btn-submit");

        if (closeButton != null)
        {
            closeButton.clicked -= CloseLogin;
        }

        if (submitButton != null)
        {
            submitButton.clicked -= SubmitLogin;
        }
    }

    // ======================================================
    // LOGIN ACTIONS
    // ======================================================

    private void CloseLogin()
    {
        if (loginPopup == null)
        {
            return;
        }

        UnbindLoginEvents();

        /*
         * Remove() only detaches the VisualElement.
         *
         * The VisualElement itself becomes unreachable
         * and can be collected later.
         */
        if (loginPopup.parent != null)
        {
            loginPopup.RemoveFromHierarchy();
        }

        loginPopup = null;
    }

    private void SubmitLogin()
    {
        if (loginPopup == null)
        {
            return;
        }

        TextField username =
            loginPopup.Q<TextField>("input-username");

        TextField password =
            loginPopup.Q<TextField>("input-password");

        string user =
            username != null
                ? username.value
                : string.Empty;

        string pass =
            password != null
                ? password.value
                : string.Empty;

        Debug.Log(
            $"Login: user={user}, password={pass}"
        );

        // Actual login logic goes here.

        CloseLogin();
    }
}