using UnityEngine;
using HitstunConstants;

public static class LocalSession
{
    public static GameState gs;
    public static NonGameState ngs;
    public static CharacterData[] characterDatas;

    public static void Init(GameState _gs, NonGameState _ngs)
    {
        gs = _gs;
        ngs = _ngs;
    }

    public static uint[] RunFrame()
    {
        uint[] inputs = new uint[ngs.players.Length];
        for (int i = 0; i < inputs.Length; ++i)
        {
            //inputs[i] = ReadInputs(ngs.players[i].controllerId);
            inputs[i] = ReadInputs();
        }
        gs.Update(inputs, 0);
        return inputs;
    }

    public static uint ReadInputs()
    {
        uint input = 0;
        var playerInput = HitstunRunner.Instance.playerInput;
        // 遍历所有Action
        if (playerInput.actions["Movement"].IsPressed())
        {
            // 键盘只能读取8方向，手柄更细腻，综合下来只设置8方向
            var move = playerInput.actions["Movement"].ReadValue<Vector2>();
            //Debug.Log($"Movement -- {move} -- {ConvertDir(move)}");
            input = ConvertDir(move);
        }
        if (playerInput.actions["Attack"].IsPressed())
        {
            input |= (uint)KeyPress.KEY_LP;
            //Debug.Log($"Attack -- input={input}");
        }
        if (playerInput.actions["Jump"].IsPressed())
        {
            input |= (uint)KeyPress.KEY_JUMP;
            //Debug.Log($"Jump -- input={input}");
        }
        if (playerInput.actions["Crouch"].IsPressed())
        {
            input |= (uint)KeyPress.KEY_CROUCH;
            //Debug.Log($"CROUCH -- input={input}");
        }
        return input;
    }

    // Vector2 转 8方向
    enum compassDir
    {
        KEY_RIGHT = 0, KEY_RIGHT_UP   = 1,
        KEY_UP    = 2, KEY_LEFT_UP    = 3,
        KEY_LEFT  = 4, KEY_LEFT_DOWN  = 5,
        KEY_DOWN  = 6, KEY_RIGHT_DOWN = 7,
    };
    static uint ConvertDir(Vector2 vector)
    {
        // actual conversion code:
        float angle = Mathf.Atan2(vector.y, vector.x);
        int octant = Mathf.RoundToInt(8 * angle / (2 * Mathf.PI) + 8) % 8;

        uint input = 0;

        compassDir dir = (compassDir)octant;  // typecast to enum: 0 -> E etc.
        switch (dir)
        {
            case compassDir.KEY_RIGHT:
                input |= (uint)KeyPress.KEY_RIGHT;
                break;
            case compassDir.KEY_RIGHT_UP:
                input |= (uint)KeyPress.KEY_RIGHT;
                input |= (uint)KeyPress.KEY_UP;
                break;
            case compassDir.KEY_UP:
                input |= (uint)KeyPress.KEY_UP;
                break;
            case compassDir.KEY_LEFT_UP:
                input |= (uint)KeyPress.KEY_LEFT;
                input |= (uint)KeyPress.KEY_UP;
                break;
            case compassDir.KEY_LEFT:
                input |= (uint)KeyPress.KEY_LEFT;
                break;
            case compassDir.KEY_LEFT_DOWN:
                input |= (uint)KeyPress.KEY_LEFT;
                input |= (uint)KeyPress.KEY_DOWN;
                break;
            case compassDir.KEY_DOWN:
                input |= (uint)KeyPress.KEY_DOWN;
                break;
            case compassDir.KEY_RIGHT_DOWN:
                input |= (uint)KeyPress.KEY_RIGHT;
                input |= (uint)KeyPress.KEY_DOWN;
                break;
        }
        return input;
    }
}