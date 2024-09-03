using System;

namespace HitstunConstants
{
    public static class Constants
    {
        // Misc
        public const int FRAME_DELAY = 2;
        //public const int NUM_PLAYERS = 2;
        public const int NUM_PLAYERS = 1;
        public const int INPUT_BUFFER_SIZE = 60;
        public const int FPS = 60;

        // Camera
        public const float SCALE = 1000.0f;
        public const float CAM_LOWER_BOUND = -4.3f;
        public const float CAM_UPPER_BOUND = 4.3f;

        // Game
        public const int BOUNDS_WIDTH = 12000;
        public const int BOUNDS_HEIGHT = 4000;
        public const int INITIAL_CHARACTER_DISPLACEMENT = 1000;
        public const int MAX_CHARACTER_DISTANCE = 3500;

        // Leniencies
        public const int LENIENCY_BUFFER = 5;
        public const int LENIENCY_DASH = 10;
        public const int LENIENCY_QF = 10;
        public const int LENIENCY_DP = 15;
        public const int LENIENCY_DOUBLE_QF = 20;
        public const int LENIENCY_CANCEL = 5;

        // Jump parameters
        public const int PREJUMP_FRAMES = 3;
        public const int JUMP_HEIGHT = 1000;
        public const float TIME_TO_PEAK = 0.3f;
        public const int GRAVITY = (int)(-(2 * JUMP_HEIGHT) / (TIME_TO_PEAK * TIME_TO_PEAK));
        public const int JUMP_VELOCITY_Y = (int)(2 * JUMP_HEIGHT / TIME_TO_PEAK);

        // pushback
        public const int FRICTION = 300;
        public const int PUSHBACK_CORNER_THRESH = 100;

        // projectile properties
        public const int PROJECTILE_HEIGHT = 720;
        public const int PROJECTILE_DISPLACE = 600;
    }

    // 搓招
    public static class Motions
    {
        public static readonly uint[] DASH_FORWARD = { (uint)Inputs.INPUT_RIGHT, (uint)Inputs.INPUT_NEUTRAL, (uint)Inputs.INPUT_RIGHT };
        public static readonly uint[] DASH_BACKWARD = { (uint)Inputs.INPUT_LEFT, (uint)Inputs.INPUT_NEUTRAL, (uint)Inputs.INPUT_LEFT };
        public static readonly uint[] QCF = { (uint)Inputs.INPUT_DOWN, (uint)Inputs.INPUT_DOWN | (uint)Inputs.INPUT_RIGHT, (uint)Inputs.INPUT_RIGHT };
        public static readonly uint[] QCB = { (uint)Inputs.INPUT_DOWN, (uint)Inputs.INPUT_DOWN | (uint)Inputs.INPUT_LEFT, (uint)Inputs.INPUT_LEFT };
        public static readonly uint[] DOUBLE_QCF = { (uint)Inputs.INPUT_DOWN, (uint)Inputs.INPUT_DOWN | (uint)Inputs.INPUT_RIGHT, (uint)Inputs.INPUT_RIGHT, (uint)Inputs.INPUT_DOWN, (uint)Inputs.INPUT_DOWN | (uint)Inputs.INPUT_RIGHT, (uint)Inputs.INPUT_RIGHT };
        public static readonly uint[] DP = { (uint)Inputs.INPUT_RIGHT, (uint)Inputs.INPUT_DOWN, (uint)Inputs.INPUT_DOWN | (uint)Inputs.INPUT_RIGHT };

        public static bool isMotionInput(Inputs input)
        {
            return isMotionInput((uint)input);
        }
        public static bool isMotionInput(uint input)
        {
            return input < 16;
        }
    }

    [Flags] //硬件键
    public enum KeyPress : uint
    {
        KEY_LEFT = (1 << 0), //A
        KEY_RIGHT = (1 << 1), //D
        KEY_UP = (1 << 2), //W
        KEY_DOWN = (1 << 3), //S
        KEY_JUMP = (1 << 4), //Space
        KEY_CROUCH = (1 << 5), //Ctrl
        KEY_LP = (1 << 6), //U
        KEY_MP = (1 << 7), //I
        KEY_HP = (1 << 8), //O
        KEY_LK = (1 << 9), //J
        KEY_MK = (1 << 10), //K
        KEY_HK = (1 << 11), //L
    }

    [Flags] //组合键
    public enum Inputs : uint
    {
        INPUT_NEUTRAL = 0, //NONE
        INPUT_LEFT = (1 << 0), //A
        INPUT_RIGHT = (1 << 1), //D
        INPUT_UP = (1 << 2), //W
        INPUT_DOWN = (1 << 3), //S
        INPUT_JUMP = (1 << 4), //Space
        INPUT_CROUCH = (1 << 5), //Ctrl
        INPUT_LP = (1 << 6), //U
        INPUT_MP = (1 << 7), //I
        INPUT_HP = (1 << 8), //O
        INPUT_LK = (1 << 9), //J
        INPUT_MK = (1 << 10), //K
        INPUT_HK = (1 << 11), //L
        INPUT_nLP = (1 << 12), //U + Ctrl
        INPUT_nMP = (1 << 13), //I + Ctrl
        INPUT_nHP = (1 << 14), //O + Ctrl
        INPUT_nLK = (1 << 15), //J + Ctrl
        INPUT_nMK = (1 << 16), //K + Ctrl
        INPUT_nHK = (1 << 17), //L + Ctrl
    }

    public enum PlayerType
    {
        LOCAL = 0,
        REMOTE,
    };

    public enum PlayerConnectState
    {
        CONNECTING = 0,
        SYNCHRONIZING,
        RUNNING,
        DISCONNECTED,
        DISCONNECTING,
    };

    public enum CharacterName
    {
        KEN = 0
    }

    // 动画状态
    public enum CharacterState
    {
        // animations
        STAND = 0,
        CROUCH,
        WALK_LEFT, //A
        WALK_RIGHT, //D
        WALK_FORWARD, //W
        WALK_BACKWARD, //S
        STAND_TO_CROUCH,
        CROUCH_TO_STAND,
        JUMP_NEUTRAL, //垂直跳
        JUMP_FORWARD,
        JUMP_BACKWARD,
        DASH_FORWARD,
        DASH_BACKWARD,
        BlOCK_HIGH, //防御↑
        BLOCK_STAND,
        BLOCK_LOW,
        HIT_STAND, //挨打
        HIT_CROUCH,

        // attacks
        CROUCH_MK,
        HADOUKEN
    }
}