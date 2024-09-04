using System.IO;
using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Newtonsoft.Json;
using HitstunConstants;

public class HitstunRunner : MonoBehaviour
{
    public static HitstunRunner Instance;
    public PlayerInput playerInput;

    // Settings
    public bool showHitboxes = true;
    public bool manualStep = false;
    public CharacterName player1Character;
    public CharacterName player2Character;

    // Rendering
    public CharacterView characterView;
    CharacterView[] characterViews;

    // Character Data
    // 英雄/怪物表
    CharacterData[] characterDatas;

    // Internal
    NativeArray<byte> buffer;
    NativeArray<byte> oldBuffer;
    private bool running;
    private bool nextStep;

    public static int CalcFletcher32(NativeArray<byte> data)
    {
        uint sum1 = 0;
        uint sum2 = 0;

        int index;
        for (index = 0; index < data.Length; ++index)
        {
            sum1 = (sum1 + data[index]) % 0xffff;
            sum2 = (sum2 + sum1) % 0xffff;
        }
        return unchecked((int)((sum2 << 16) | sum1));
    }

    void Awake()
    {
        Instance = this;
        playerInput = GetComponent<PlayerInput>();

        currentControlScheme = playerInput.currentControlScheme;
        playerInput.onDeviceLost += PlayerInput_onDeviceLost;
        playerInput.onDeviceRegained += PlayerInput_onDeviceRegained;
        playerInput.onControlsChanged += PlayerInput_onControlsChanged;
        UpdateUIDisplay();

        InputActionMap playerMap = playerInput.actions.FindActionMap(actionMapPlayerControls);
        InputAction pause = playerMap.FindAction("TogglePause");
        pause.performed += Pause_performed;

        InputActionMap menuMap = playerInput.actions.FindActionMap(actionMapMenuControls);
        InputAction resume = menuMap.FindAction("TogglePause");
        resume.performed += Resume_performed;
    }

    private void PlayerInput_onDeviceLost(PlayerInput obj)
    {
        Debug.Log("Device Lost");
        //playerVisualsBehaviour.SetDisconnectedDeviceVisuals();
    }

    private void PlayerInput_onDeviceRegained(PlayerInput obj)
    {
        Debug.Log("Device Regained");
        UpdateUIDisplay();
    }

    private void PlayerInput_onControlsChanged(PlayerInput obj)
    {
        Debug.Log("Controls Changed");
        if (playerInput.currentControlScheme != currentControlScheme)
        {
            currentControlScheme = playerInput.currentControlScheme;

            //playerVisualsBehaviour.UpdatePlayerVisuals();
            //RemoveAllBindingOverrides();
            UpdateUIDisplay();
        }
    }

    private void Pause_performed(InputAction.CallbackContext obj)
    {
        SwitchFocusedPlayerControlScheme();
        Debug.Log($"Pause → {playerInput.currentActionMap.name}");
    }

    private void Resume_performed(InputAction.CallbackContext obj)
    {
        SwitchFocusedPlayerControlScheme();
        Debug.Log($"Resume → {playerInput.currentActionMap.name}");
    }

    void Start()
    {
        // Fix the FPS
        Application.targetFrameRate = Constants.FPS;
        Time.fixedDeltaTime = 1f / (float)Constants.FPS;

        // Init LocalSession
        LocalSession.Init(new GameState(), new NonGameState());

        // Init NonGameState
        for (int i = 0; i < 1; i++)
        {
            LocalSession.ngs.players = new PlayerConnectionInfo[Constants.NUM_PLAYERS];
            LocalSession.ngs.players[i] = new PlayerConnectionInfo
            {
                handle = i,
                type = PlayerType.LOCAL,
                controllerId = i
            };
            LocalSession.ngs.SetConnectState(i, PlayerConnectState.RUNNING);
        }

        // Init GameState
        LocalSession.gs.Init();

        // load character data from JSON
        LoadCharacterData();

        // Init View
        InitView(LocalSession.gs);
        running = !manualStep;
        nextStep = false;
    }

    void FixedUpdate()
    {
        if (Time.deltaTime < 0.016f || Time.deltaTime > 0.017f)
        {
            Debug.Log("Unstable update tick!" + Time.deltaTime.ToString());
        }
        // handles function key debugging inputs
        HandleDevKeys();
        if (running || nextStep)
        {
            nextStep = false;

            // save old gamestate
            if (oldBuffer.IsCreated)
            {
                oldBuffer.Dispose();
            }
            oldBuffer = GameState.ToBytes(LocalSession.gs);

            // run the frame
            uint[] inputs = LocalSession.RunFrame();

            // save new gamestate
            if (buffer.IsCreated)
            {
                buffer.Dispose();
            }
            buffer = GameState.ToBytes(LocalSession.gs);
            int checksum = CalcFletcher32(buffer);

            // load old gamestate and re-simulate
            GameState.FromBytes(LocalSession.gs, oldBuffer);
            LocalSession.gs.Update(inputs, 0);

            // save new gamestate again
            if (buffer.IsCreated)
            {
                buffer.Dispose();
            }
            buffer = GameState.ToBytes(LocalSession.gs);
            int checksum2 = CalcFletcher32(buffer);

            if (checksum != checksum2)
            {
                Debug.Log(checksum.ToString() + " , " + checksum2.ToString());
            }

            UpdateGameView(LocalSession.gs, LocalSession.ngs);
        }
    }

    void InitView(GameState gs)
    {
        characterViews = new CharacterView[Constants.NUM_PLAYERS];

        for (int i = 0; i < Constants.NUM_PLAYERS; ++i)
        {
            characterViews[i] = Instantiate(characterView, transform);
            characterViews[i].LoadResources(characterDatas[i]);
            characterViews[i].showHitboxes = showHitboxes;
        }
    }

    void UpdateGameView(GameState gs, NonGameState ngs)
    {
        // update characterView objects
        for (int i = 0; i < Constants.NUM_PLAYERS; ++i)
        {
            characterViews[i].showHitboxes = showHitboxes;
            characterViews[i].UpdateCharacterView(gs.characters[i]);
        }
    }

    void HandleDevKeys()
    {
        // quit
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Escape))
        {
            Application.Quit();
        }
        // toggle hitboxes
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F1))
        {
            showHitboxes = !showHitboxes;
            if (showHitboxes)
            {
                Debug.Log("Hitboxes ON");
            }
            else
            {
                Debug.Log("Hitboxes OFF");
            }
        }
        // manual stepping
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F2))
        {
            manualStep = !manualStep;
            if (manualStep)
            {
                Debug.Log("Manual mode on: Press F3 to advance a single frame");
                running = false;
                nextStep = false;
            }
            else
            {
                Debug.Log("Manual mode off");
                running = true;
            }
        }
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F3))
        {
            Debug.Log("Manual step");
            nextStep = true;
        }
        // save and load
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F5))
        {
            Debug.Log("SAVE");
            TestSave();
        }
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F6))
        {
            Debug.Log("LOAD");
            TestLoad();
        }
    }

    void LoadCharacterData()
    {
        characterDatas = new CharacterData[Constants.NUM_PLAYERS];
        string jsonPath = string.Format("Assets/Resources/CharacterData/{0}.json", player1Character.ToString());
        characterDatas[0] = JsonConvert.DeserializeObject<CharacterData>(File.ReadAllText(jsonPath));
        //jsonPath = string.Format("Assets/Resources/CharacterData/{0}.json", player2Character.ToString());
        //characterDatas[1] = JsonConvert.DeserializeObject<CharacterData>(File.ReadAllText(jsonPath));
        LocalSession.characterDatas = characterDatas;
        LocalSession.gs.characterDatas = characterDatas;
    }

    void OnDestroy()
    {
        if (buffer.IsCreated)
        {
            buffer.Dispose();
        }
        if (oldBuffer.IsCreated)
        {
            oldBuffer.Dispose();
        }
    }

    public void TestSave()
    {
        if (buffer.IsCreated)
        {
            buffer.Dispose();
        }
        buffer = GameState.ToBytes(LocalSession.gs);
    }

    public void TestLoad()
    {
        GameState.FromBytes(LocalSession.gs, buffer);
    }


    //Action Maps
    private const string actionMapPlayerControls = "Player Controls";
    private const string actionMapMenuControls = "Menu Controls";
    public bool isPaused;
    public GameObject uiMenu;

    void ToggleTimeScale()
    {
        float newTimeScale = 0f;

        switch (isPaused)
        {
            case true:
                newTimeScale = 0f;
                break;

            case false:
                newTimeScale = 1f;
                break;
        }

        Time.timeScale = newTimeScale;
    }

    public void SwitchFocusedPlayerControlScheme()
    {
        isPaused = !isPaused;

        ToggleTimeScale();

        //TODO: 给角色View传递 isPaused

        switch (isPaused)
        {
            case true:
                playerInput.SwitchCurrentActionMap(actionMapMenuControls);
                break;

            case false:
                playerInput.SwitchCurrentActionMap(actionMapPlayerControls);
                break;
        }

        uiMenu.SetActive(isPaused);
    }

    [SerializeField]
    private string currentControlScheme;
    [Header("Device Display Settings")]
    public DeviceDisplayConfigurator deviceDisplaySettings;
    public UIBehaviour uiBehaviour;

    void UpdateUIDisplay()
    {
        //uiBehaviour.UpdatePlayerIDDisplayText(playerID);

        string deviceName = deviceDisplaySettings.GetDeviceName(playerInput);
        uiBehaviour.UpdatePlayerDeviceNameDisplayText(deviceName);

        Color deviceColor = deviceDisplaySettings.GetDeviceColor(playerInput);
        uiBehaviour.UpdatePlayerIconDisplayColor(deviceColor);
    }
}