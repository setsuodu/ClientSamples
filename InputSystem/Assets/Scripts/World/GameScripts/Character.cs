using System.IO;
using System.Collections.Generic;
using UnityEngine;
using HitstunConstants;

[System.Serializable]
public class Character
{
    public Vector3Int position;
    public Vector3Int velocity;
    //public bool facingRight;
    //public bool onTop;
    //public Vector3Int rotation;
    public uint rotationY;
    public CharacterState state;
    public uint framesInState;
    public uint blockStun;
    public uint hitStun;
    public List<HitBox> hitBoxes;
    public Projectile projectile;

    // Input Buffer
    private uint[] inputBuffer;
    private uint currentBufferPos;

    public Character()
    {
        // position and velocity
        position = new Vector3Int(0, 0, 0);
        velocity = new Vector3Int(0, 0, 0);
        // character state
        state = CharacterState.STAND;
        framesInState = 0;
        // input Buffer
        currentBufferPos = 0;
        inputBuffer = new uint[Constants.INPUT_BUFFER_SIZE];
        for (int i = 0; i < Constants.INPUT_BUFFER_SIZE; i++)
        {
            inputBuffer[i] = 0;
        }
        // hitboxes list
        hitBoxes = new List<HitBox>();
        // projectile
        projectile = new Projectile();
    }

    public void Serialize(BinaryWriter bw)
    {
        // position
        bw.Write(position.x);
        bw.Write(position.y);
        bw.Write(position.z);
        // velocity
        bw.Write(velocity.x);
        bw.Write(velocity.y);
        bw.Write(velocity.z);
        // booleans
        //bw.Write(facingRight);
        //bw.Write(onTop);
        // state
        bw.Write((int)state);
        bw.Write(framesInState);
        bw.Write(blockStun);
        bw.Write(hitStun);
        // input buffer
        for (int i = 0; i < Constants.INPUT_BUFFER_SIZE; i++)
        {
            bw.Write(inputBuffer[i]);
        }
        bw.Write(currentBufferPos);
        // hitbox list
        bw.Write(hitBoxes.Count);
        foreach (HitBox hitBox in hitBoxes)
        {
            hitBox.Serialize(bw);
        }
        //projectile
        projectile.Serialize(bw);
    }

    public void Deserialize(BinaryReader br)
    {
        // position
        position.x = br.ReadInt32();
        position.y = br.ReadInt32();
        position.z = br.ReadInt32();
        // velocity
        velocity.x = br.ReadInt32();
        velocity.y = br.ReadInt32();
        velocity.z = br.ReadInt32();
        // booleans
        //facingRight = br.ReadBoolean();
        //onTop = br.ReadBoolean();
        // state
        state = (CharacterState)br.ReadInt32();
        framesInState = br.ReadUInt32();
        blockStun = br.ReadUInt32();
        hitStun = br.ReadUInt32();
        // input buffer
        inputBuffer = new uint[Constants.INPUT_BUFFER_SIZE];
        for (int i = 0; i < Constants.INPUT_BUFFER_SIZE; ++i)
        {
            inputBuffer[i] = br.ReadUInt32();
        }
        currentBufferPos = br.ReadUInt32();
        // hitbox list
        int hitBoxCount = br.ReadInt32();
        for (int i = 0; i < hitBoxCount; i++)
        {
            HitBox hitbox = new HitBox();
            hitBoxes.Add(hitbox);
            hitbox.Deserialize(br);
        }
        //projectile
        projectile = new Projectile();
        projectile.Deserialize(br);
    }

    public void ParseInputsToBuffer(uint inputs)
    {
        uint sanitizedInputs = inputs;
        // if up+down, register none of them
        if ((inputs & (uint)KeyPress.KEY_UP) != 0 && (inputs & (uint)KeyPress.KEY_DOWN) != 0)
        {
            sanitizedInputs &= ~(uint)KeyPress.KEY_UP;
            sanitizedInputs &= ~(uint)KeyPress.KEY_DOWN;
        }
        //if left+right, register none of them
        if ((inputs & (uint)KeyPress.KEY_LEFT) != 0 && (inputs & (uint)KeyPress.KEY_RIGHT) != 0)
        {
            sanitizedInputs &= ~(uint)KeyPress.KEY_LEFT;
            sanitizedInputs &= ~(uint)KeyPress.KEY_RIGHT;
        }

        // convert keypresses to inputs (left/right to back/forward, depending on facing direction)
        uint convertedInputs = 0;

        //左
        if ((sanitizedInputs & (uint)KeyPress.KEY_LEFT) != 0)
        {
            convertedInputs |= (uint)Inputs.INPUT_LEFT;
        }
        //右
        if ((sanitizedInputs & (uint)KeyPress.KEY_RIGHT) != 0)
        {
            convertedInputs |= (uint)Inputs.INPUT_RIGHT;
        }
        //前
        if ((sanitizedInputs & (uint)KeyPress.KEY_UP) != 0)
        {
            convertedInputs |= (uint)Inputs.INPUT_UP;
        }
        //后
        if ((sanitizedInputs & (uint)KeyPress.KEY_DOWN) != 0)
        {
            convertedInputs |= (uint)Inputs.INPUT_DOWN;
        }
        //跳
        if ((sanitizedInputs & (uint)KeyPress.KEY_JUMP) != 0)
        {
            convertedInputs |= (uint)Inputs.INPUT_JUMP;
        }
        //蹲
        if ((sanitizedInputs & (uint)KeyPress.KEY_CROUCH) != 0)
        {
            convertedInputs |= (uint)Inputs.INPUT_CROUCH;
        }
        convertedInputs |= ((sanitizedInputs & (uint)KeyPress.KEY_LP) != 0) ? (uint)Inputs.INPUT_LP : (uint)Inputs.INPUT_nLP;
        convertedInputs |= ((sanitizedInputs & (uint)KeyPress.KEY_MP) != 0) ? (uint)Inputs.INPUT_MP : (uint)Inputs.INPUT_nMP;
        convertedInputs |= ((sanitizedInputs & (uint)KeyPress.KEY_HP) != 0) ? (uint)Inputs.INPUT_HP : (uint)Inputs.INPUT_nHP;
        convertedInputs |= ((sanitizedInputs & (uint)KeyPress.KEY_LK) != 0) ? (uint)Inputs.INPUT_LK : (uint)Inputs.INPUT_nLK;
        convertedInputs |= ((sanitizedInputs & (uint)KeyPress.KEY_MK) != 0) ? (uint)Inputs.INPUT_MK : (uint)Inputs.INPUT_nMK;
        convertedInputs |= ((sanitizedInputs & (uint)KeyPress.KEY_HK) != 0) ? (uint)Inputs.INPUT_HK : (uint)Inputs.INPUT_nHK;


        // update buffer position
        currentBufferPos = (currentBufferPos + 1) % Constants.INPUT_BUFFER_SIZE;
        inputBuffer[currentBufferPos] = convertedInputs;
    }

    // index is relative, 0 is latest, -1 is one in the past etc
    public uint GetInputsByRelativeIndex(int index)
    {
        int newIndex = (int)currentBufferPos + index;
        if (newIndex < 0)
        {
            newIndex += Constants.INPUT_BUFFER_SIZE;
        }
        return inputBuffer[newIndex % Constants.INPUT_BUFFER_SIZE];
    }

    public void FlipInputBufferInputs()
    {
        for (int i = 0; i < Constants.INPUT_BUFFER_SIZE; i++)
        {
            bool forward = (inputBuffer[i] & (uint)Inputs.INPUT_RIGHT) != 0;
            bool back = (inputBuffer[i] & (uint)Inputs.INPUT_LEFT) != 0;

            inputBuffer[i] &= ~(uint)Inputs.INPUT_RIGHT;
            inputBuffer[i] &= ~(uint)Inputs.INPUT_LEFT;

            if (forward)
            {
                inputBuffer[i] |= (uint)Inputs.INPUT_LEFT;
            }
            if (back)
            {
                inputBuffer[i] |= (uint)Inputs.INPUT_RIGHT;
            }
        }
    }

    // Array元素全部归零
    public void FlushBuffer()
    {
        for (int i = 0; i < Constants.INPUT_BUFFER_SIZE; i++)
        {
            inputBuffer[i] = 0;
        }
    }

    public bool CheckSequence(uint[] sequence, int maxDuration)
    {
        int w = sequence.Length - 1;
        for (int i = 0; i < maxDuration; i++)
        {
            uint inputs = GetInputsByRelativeIndex(-i);

            // remove either motions or buttons from input in order to compare
            if (Motions.isMotionInput(sequence[w]))
            {
                // remove all buttons from input
                inputs &= ~(uint)Inputs.INPUT_LP;
                inputs &= ~(uint)Inputs.INPUT_MP;
                inputs &= ~(uint)Inputs.INPUT_HP;
                inputs &= ~(uint)Inputs.INPUT_LK;
                inputs &= ~(uint)Inputs.INPUT_MK;
                inputs &= ~(uint)Inputs.INPUT_HK;
                inputs &= ~(uint)Inputs.INPUT_nLP;
                inputs &= ~(uint)Inputs.INPUT_nMP;
                inputs &= ~(uint)Inputs.INPUT_nHP;
                inputs &= ~(uint)Inputs.INPUT_nLK;
                inputs &= ~(uint)Inputs.INPUT_nMK;
                inputs &= ~(uint)Inputs.INPUT_nHK;
                //inputs &= ~(uint)Inputs.INPUT_JUMP;
                //inputs &= ~(uint)Inputs.INPUT_CROUCH;
            }

            if (Motions.isMotionInput(sequence[w]))
            {
                // after removing buttons, motions need to match exactly
                if (inputs == sequence[w]) w--;
            }
            else
            {
                // buttons only need to be pressed
                if ((inputs & (uint)sequence[w]) != 0) w--;
            }

            if (w == -1) return true;
        }
        return false;
    }

    public void SetCharacterState(CharacterState _state)
    {
        if (state != _state)
        {
            state = _state;
            framesInState = 0;
            hitBoxes.Clear();
        }
    }

    public Box GetCollisionBox(CharacterData data)
    {
        int xMin, xMax, yMin, yMax;
        int[] collisionBox = isAttacking() ? data.attacks[state.ToString()].collisionBox : data.animations[state.ToString()].collisionBox;
        //if (facingRight)
        //{
            xMin = position.x + collisionBox[0];
            xMax = position.x + collisionBox[1];
        //}
        //else
        //{
        //    xMin = position.x - collisionBox[1];
        //    xMax = position.x - collisionBox[0];
        //}
        yMin = position.y + collisionBox[2];
        yMax = position.y + collisionBox[3];

        Box box = new Box(xMin, xMax, yMin, yMax);
        return box;
    }

    public bool GetHurtBoxes(CharacterData data, out List<Box> boxes)
    {
        boxes = new List<Box>();
        Animation animationData;
        if (isAttacking())
        {
            animationData = data.attacks[state.ToString()];
        }
        else
        {
            animationData = data.animations[state.ToString()];
        }
        if (animationData.hurtBoxes is null) return false;

        // check if the hurtboxes are static in this state
        uint index = animationData.staticHurtBox ? 0 : framesInState;

        // get the hurtboxes
        if (animationData.hurtBoxes.ContainsKey(index))
        {
            int[][] hurtBoxes = animationData.hurtBoxes[index];
            for (int i = 0; i < hurtBoxes.Length; i++)
            {
                boxes.Add(new Box(hurtBoxes[i]));
            }
            return true;
        }
        return false;
    }

    public bool isAttacking()
    {
        return state == CharacterState.STAND_LP || state == CharacterState.CROUCH_MK || state == CharacterState.HADOUKEN;
    }

    public bool IsAirborne()
    {
        return (state == CharacterState.JUMP_NEUTRAL || state == CharacterState.JUMP_FORWARD || state == CharacterState.JUMP_BACKWARD) && framesInState > Constants.PREJUMP_FRAMES;
    }

    public bool IsIdle()
    {
        return state == CharacterState.STAND ||
               state == CharacterState.WALK_LEFT ||
               state == CharacterState.WALK_RIGHT ||
               state == CharacterState.WALK_FORWARD ||
               state == CharacterState.WALK_BACKWARD ||
               state == CharacterState.CROUCH ||
               state == CharacterState.CROUCH_TO_STAND ||
               state == CharacterState.STAND_TO_CROUCH;
    }

    public bool IsCrouch()
    {
        return state == CharacterState.CROUCH ||
               state == CharacterState.STAND_TO_CROUCH ||
               state == CharacterState.BLOCK_LOW ||
               state == CharacterState.HIT_CROUCH ||
               state == CharacterState.CROUCH_MK;
    }

    public bool IsStand()
    {
        return state == CharacterState.STAND ||
               state == CharacterState.WALK_LEFT ||
               state == CharacterState.WALK_RIGHT ||
               state == CharacterState.WALK_FORWARD ||
               state == CharacterState.WALK_BACKWARD ||
               state == CharacterState.CROUCH_TO_STAND ||
               state == CharacterState.DASH_FORWARD ||
               state == CharacterState.DASH_BACKWARD ||
               state == CharacterState.BLOCK_STAND ||
               state == CharacterState.BlOCK_HIGH ||
               state == CharacterState.HIT_STAND ||
               (state == CharacterState.JUMP_NEUTRAL && framesInState <= Constants.PREJUMP_FRAMES) ||
               (state == CharacterState.JUMP_FORWARD && framesInState <= Constants.PREJUMP_FRAMES) ||
               (state == CharacterState.JUMP_BACKWARD && framesInState <= Constants.PREJUMP_FRAMES);
    }

    // 只和防御有关
    public bool IsInCorner()
    {
        return position.x < Constants.PUSHBACK_CORNER_THRESH || position.x > Constants.BOUNDS_WIDTH - Constants.PUSHBACK_CORNER_THRESH;
    }

    public bool IsBlockingLow()
    {
        return IsIdle() && CheckSequence(new uint[] { (uint)Inputs.INPUT_DOWN | (uint)Inputs.INPUT_LEFT }, 1);
    }
    public bool IsBlockingMid()
    {
        return IsBlockingHigh() || IsBlockingLow();
    }
    public bool IsBlockingHigh()
    {
        return IsIdle() && CheckSequence(new uint[] { (uint)Inputs.INPUT_LEFT }, 1);
    }

    public void UpdateCharacter(CharacterData data)
    {
        framesInState++;
        // update hitboxes
        foreach (HitBox hitBox in hitBoxes)
        {
            hitBox.enabled = hitBox.startingFrame <= framesInState && hitBox.startingFrame + hitBox.duration >= framesInState;
        }
        // update projectiles
        if (projectile.active)
        {
            projectile.activeSince++;
            // if out of bounds, deactivate
            if (projectile.position.x - 2000 > Constants.BOUNDS_WIDTH || projectile.position.x < -2000 || projectile.position.y > Constants.BOUNDS_HEIGHT || projectile.position.y < 0)
            {
                projectile.active = false;
            }
            // if too far away from character, deactivate
            if (Mathf.Abs(projectile.position.x - position.x) - 2000 > Constants.MAX_CHARACTER_DISTANCE)
            {
                projectile.active = false;
            }
        }

        switch (state)
        {
            // IDLE STATE
            case CharacterState.STAND:
            // WALK_FORWARD STATE
            case CharacterState.WALK_FORWARD: //W
            // WALK_BACKWARD STATE
            case CharacterState.WALK_BACKWARD: //S
            // CROUCH_TO_STAND STATE - technically already standing
            case CharacterState.WALK_LEFT: //A
            case CharacterState.WALK_RIGHT: //D
            case CharacterState.CROUCH_TO_STAND:
                if (CheckGroundedSpecials(data)) break;
                if (CheckStandingAttacks(data)) break;
                if (CheckDash()) break;
                if (CheckJump()) break;
                if (CheckCrouch()) break;
                if (CheckWalk(data)) break;
                // default idle
                if (state == CharacterState.CROUCH_TO_STAND)
                {
                    if (framesInState >= data.animations[state.ToString()].totalFrames)
                    {
                        SetCharacterState(CharacterState.STAND);
                    }
                }
                else
                {
                    SetCharacterState(CharacterState.STAND);
                }
                velocity.x = 0;
                velocity.y = 0;
                velocity.z = 0;
                break;
            // CROUCH STATE
            case CharacterState.CROUCH:
            // STAND_TO_CROUCH STATE - technically already crouching
            case CharacterState.STAND_TO_CROUCH:
                if (CheckGroundedSpecials(data)) break;
                if (CheckCrouchingAttacks(data)) break;
                if (CheckDash()) break;
                if (CheckJump()) break;
                if (CheckStand()) break;
                // default crouch
                if (state == CharacterState.STAND_TO_CROUCH)
                {
                    if (framesInState >= data.animations[state.ToString()].totalFrames)
                    {
                        SetCharacterState(CharacterState.CROUCH);
                    }
                }
                velocity.x = 0;
                velocity.y = 0;
                velocity.z = 0;
                break;
            // JUMP_NEUTRAL STATE
            case CharacterState.JUMP_NEUTRAL:
                if (framesInState < Constants.PREJUMP_FRAMES)
                {
                    velocity.x = 0;
                    velocity.y = 0;
                    velocity.z = 0;
                    break;
                }
                if (framesInState == Constants.PREJUMP_FRAMES)
                {
                    velocity.x = 0;
                    velocity.y = Constants.JUMP_VELOCITY_Y;
                    velocity.z = 0;
                    break;
                }
                velocity.x = 0;
                velocity.y += Constants.GRAVITY / Constants.FPS;
                velocity.z = 0;

                if (position.y <= 0)
                {
                    SetCharacterState(CharacterState.STAND);
                }
                break;
            // JUMP_FORWARD STATE
            case CharacterState.JUMP_FORWARD:
                if (framesInState < Constants.PREJUMP_FRAMES)
                {
                    velocity.x = 0;
                    velocity.y = 0;
                    velocity.z = 0;
                    break;
                }
                if (framesInState == Constants.PREJUMP_FRAMES)
                {
                    velocity.x = data.constants.JUMP_VELOCITY_X;
                    velocity.y = Constants.JUMP_VELOCITY_Y;
                    velocity.z = 0;
                    break;
                }
                velocity.x = velocity.x;
                velocity.y += Constants.GRAVITY / Constants.FPS;
                velocity.z = 0;

                if (position.y <= 0)
                {
                    SetCharacterState(CharacterState.STAND);
                }
                break;
            // JUMP_BACKWARD STATE
            case CharacterState.JUMP_BACKWARD:
                if (framesInState < Constants.PREJUMP_FRAMES)
                {
                    velocity.x = 0;
                    velocity.y = 0;
                    velocity.z = 0;
                    break;
                }
                if (framesInState == Constants.PREJUMP_FRAMES)
                {
                    velocity.x = -data.constants.JUMP_VELOCITY_X;
                    velocity.y = Constants.JUMP_VELOCITY_Y;
                    velocity.z = 0;
                    break;
                }
                velocity.x = velocity.x;
                velocity.y += Constants.GRAVITY / Constants.FPS;
                velocity.z = 0;

                if (position.y <= 0)
                {
                    SetCharacterState(CharacterState.STAND);
                }
                break;
            // DASH_FORWARD STATE
            case CharacterState.DASH_FORWARD:
            // DASH_BACKWARD STATE
            case CharacterState.DASH_BACKWARD:
                int dx = data.animations[state.ToString()].dx[framesInState];
                velocity.x = dx;
                velocity.y = 0;
                if (framesInState >= data.animations[state.ToString()].totalFrames - 1)
                {
                    SetCharacterState(CharacterState.STAND);
                    //Debug.Log("Dash");
                }
                break;
            // BLOCK_HIGH STATE
            case CharacterState.BlOCK_HIGH:
            // BLOCK_STAND STATE
            case CharacterState.BLOCK_STAND:
                if (blockStun > (data.animations[state.ToString()].distinctSprites - 1) * 4)
                {
                    blockStun--;
                    framesInState--;
                }
                else if (blockStun > 0)
                {
                    blockStun--;
                }
                else
                {
                    SetCharacterState(CharacterState.STAND);
                }
                velocity.x += Constants.FRICTION;
                velocity.x = Mathf.Min(velocity.x, 0);
                velocity.y = velocity.y;
                break;
            // BLOCK_LOW STATE
            case CharacterState.BLOCK_LOW:
                if (blockStun > (data.animations[state.ToString()].distinctSprites - 1) * 4)
                {
                    blockStun--;
                    framesInState--;
                }
                else if (blockStun > 0)
                {
                    blockStun--;
                }
                else
                {
                    SetCharacterState(CharacterState.CROUCH);
                }
                velocity.x += Constants.FRICTION;
                velocity.x = Mathf.Min(velocity.x, 0);
                velocity.y = velocity.y;
                break;
            // HIT_STAND STATE
            case CharacterState.HIT_STAND:
                if (hitStun > (data.animations[state.ToString()].distinctSprites - 1) * 4)
                {
                    hitStun--;
                    framesInState--;
                }
                else if (hitStun > 0)
                {
                    hitStun--;
                    //Debug.Log("HIT_STAND");
                }
                else
                {
                    SetCharacterState(CharacterState.STAND);
                }
                velocity.x += Constants.FRICTION;
                velocity.x = Mathf.Min(velocity.x, 0);
                break;
            // HIT_CROUCH STATE
            case CharacterState.HIT_CROUCH:
                if (hitStun > (data.animations[state.ToString()].distinctSprites - 1) * 4)
                {
                    hitStun--;
                    framesInState--;
                }
                else if (hitStun > 0)
                {
                    hitStun--;
                }
                else
                {
                    SetCharacterState(CharacterState.CROUCH);
                }
                velocity.x += Constants.FRICTION;
                velocity.x = Mathf.Min(velocity.x, 0);
                break;
            // STAND_LP STATE
            case CharacterState.STAND_LP:
                velocity.x += Constants.FRICTION;
                velocity.x = Mathf.Min(velocity.x, 0);
                // check for cancels
                if (CheckSpecialCancel(data)) break;
                // end state
                if (framesInState >= data.attacks[state.ToString()].totalFrames - 1)
                {
                    SetCharacterState(CharacterState.STAND);
                }
                break;
            // CROUCH_MK STATE
            case CharacterState.CROUCH_MK:
                velocity.x += Constants.FRICTION;
                velocity.x = Mathf.Min(velocity.x, 0);
                // check for cancels
                if (CheckSpecialCancel(data)) break;
                // end state
                if (framesInState >= data.attacks[state.ToString()].totalFrames - 1)
                {
                    SetCharacterState(CharacterState.CROUCH);
                }
                break;
            // HADOUKEN STATE
            case CharacterState.HADOUKEN:
                velocity.x += Constants.FRICTION;
                velocity.x = Mathf.Min(velocity.x, 0);
                // check for cancels
                // spawn projectile
                if (framesInState == data.attacks[state.ToString()].spawnsProjectileAt && !projectile.active)
                {
                    projectile.active = true;
                    projectile.activeSince = 0;
                    projectile.facingRight = true;
                    projectile.position.x = position.x + Constants.PROJECTILE_DISPLACE;
                    projectile.position.y = Constants.PROJECTILE_HEIGHT;
                    projectile.velocity.x = data.projectiles["FIREBALL"].dx;
                    projectile.velocity.y = 0;
                }
                // end state
                if (framesInState >= data.attacks[state.ToString()].totalFrames - 1)
                {
                    SetCharacterState(CharacterState.STAND);
                }
                break;
            default:
                Debug.Log("Character State invalid:" + state.ToString());
                velocity.x = 0;
                velocity.y = 0;
                velocity.z = 0;
                break;
        }
    }

    public bool CheckGroundedSpecials(CharacterData data)
    {
        // [↓→] + [I] + 非CD中
        if (CheckSequence(Motions.QCF, Constants.LENIENCY_QF) && 
            CheckSequence(new uint[] { (uint)Inputs.INPUT_nMP, (uint)Inputs.INPUT_MP }, 3) && 
            !projectile.active)
        {
            FlushBuffer();
            SetCharacterState(CharacterState.HADOUKEN);
            return true;
        }
        return false;
    }

    public bool CheckSpecialCancel(CharacterData data)
    {
        // only cancel from block or hit
        int isBlockOrHit = hitBoxes.FindIndex(hitbox => hitbox.used);
        if (isBlockOrHit >= 0)
        {
            // only cancel in window
            uint lowerWindow = data.attacks[state.ToString()].specialCancelWindow[0] - Constants.LENIENCY_CANCEL;
            uint upperWindow = data.attacks[state.ToString()].specialCancelWindow[1] + Constants.LENIENCY_CANCEL;
            if (framesInState >= lowerWindow && framesInState <= upperWindow)
            {
                return CheckGroundedSpecials(data);
            }
        }
        return false;
    }

    public bool CheckStandingAttacks(CharacterData data)
    {
        if (CheckSequence(new uint[] { (uint)Inputs.INPUT_LP, (uint)Inputs.INPUT_nLP }, Constants.LENIENCY_BUFFER))
        {
            SetCharacterState(CharacterState.STAND_LP);
            // prepare the hitboxes
            //foreach (HitBox hb in data.attacks[state.ToString()].hitBoxes)
            //{
            //    HitBox hitBox = new HitBox(hb);
            //    hitBox.enabled = false;
            //    hitBox.used = false;
            //    hitBoxes.Add(hitBox);
            //}
            return true;
        }
        return false;
    }

    public bool CheckCrouchingAttacks(CharacterData data)
    {
        if (CheckSequence(new uint[] { (uint)Inputs.INPUT_nMK, (uint)Inputs.INPUT_MK }, Constants.LENIENCY_BUFFER))
        {
            SetCharacterState(CharacterState.CROUCH_MK);
            // prepare the hitboxes
            foreach (HitBox hb in data.attacks[state.ToString()].hitBoxes)
            {
                HitBox hitBox = new HitBox(hb);
                hitBox.enabled = false;
                hitBox.used = false;
                hitBoxes.Add(hitBox);
            }
            return true;
        }
        return false;
    }

    public bool CheckDash()
    {
        if (CheckSequence(Motions.DASH_FORWARD, Constants.LENIENCY_DASH))
        {
            FlushBuffer();
            SetCharacterState(CharacterState.DASH_FORWARD);
            return true;
        }
        if (CheckSequence(Motions.DASH_BACKWARD, Constants.LENIENCY_DASH))
        {
            FlushBuffer();
            SetCharacterState(CharacterState.DASH_BACKWARD);
            return true;
        }
        return false;
    }

    public bool CheckJump()
    {
        //if (CheckSequence(new uint[] { (uint)Inputs.INPUT_JUMP | (uint)Inputs.INPUT_RIGHT }, Constants.LENIENCY_BUFFER))
        //{
        //    SetCharacterState(CharacterState.JUMP_FORWARD);
        //    velocity.x = 0;
        //    velocity.y = 0;
        //    velocity.z = 0;
        //    return true;
        //}
        //if (CheckSequence(new uint[] { (uint)Inputs.INPUT_JUMP | (uint)Inputs.INPUT_LEFT }, Constants.LENIENCY_BUFFER))
        //{
        //    SetCharacterState(CharacterState.JUMP_BACKWARD);
        //    velocity.x = 0;
        //    velocity.y = 0;
        //    velocity.z = 0;
        //    return true;
        //}
        if (CheckSequence(new uint[] { (uint)Inputs.INPUT_JUMP }, Constants.LENIENCY_BUFFER))
        {
            SetCharacterState(CharacterState.JUMP_NEUTRAL);
            velocity.x = 0;
            velocity.y = 0;
            velocity.z = 0;
            return true;
        }
        return false;
    }

    public bool CheckCrouch()
    {
        uint latestInput = GetInputsByRelativeIndex(0);
        if ((latestInput & (uint)Inputs.INPUT_CROUCH) != 0)
        {
            SetCharacterState(CharacterState.STAND_TO_CROUCH);
            velocity.x = 0;
            velocity.y = 0;
            velocity.z = 0;
            return true;
        }
        return false;
    }

    public bool CheckStand()
    {
        uint latestInput = GetInputsByRelativeIndex(0);
        if ((latestInput & (uint)Inputs.INPUT_CROUCH) == 0)
        {
            SetCharacterState(CharacterState.CROUCH_TO_STAND);
            velocity.x = 0;
            velocity.y = 0;
            velocity.z = 0;
            return true;
        }
        return false;
    }

    public bool CheckWalk(CharacterData data)
    {
        uint latestInput = GetInputsByRelativeIndex(0);
        if ((latestInput & (uint)Inputs.INPUT_LEFT) != 0)
        {
            //SetCharacterState(CharacterState.WALK_LEFT);
            SetCharacterState(CharacterState.WALK_FORWARD);
            velocity.x = data.constants.WALK_LEFT;
            velocity.y = 0;
            velocity.z = 0;
            return true;
        }
        if ((latestInput & (uint)Inputs.INPUT_RIGHT) != 0)
        {
            //SetCharacterState(CharacterState.WALK_RIGHT);
            SetCharacterState(CharacterState.WALK_FORWARD);
            velocity.x = data.constants.WALK_RIGHT;
            velocity.y = 0;
            velocity.z = 0;
            return true;
        }
        if ((latestInput & (uint)Inputs.INPUT_UP) != 0)
        {
            SetCharacterState(CharacterState.WALK_FORWARD);
            velocity.x = 0;
            velocity.y = 0;
            velocity.z = data.constants.WALK_FORWARD;
            return true;
        }
        if ((latestInput & (uint)Inputs.INPUT_DOWN) != 0)
        {
            //SetCharacterState(CharacterState.WALK_BACKWARD);
            SetCharacterState(CharacterState.WALK_FORWARD);
            velocity.x = 0;
            velocity.y = 0;
            velocity.z = data.constants.WALK_BACKWARD;
            return true;
        }
        return false;
    }
}