using System.Collections.Generic;
using UnityEngine;
using HitstunConstants;
using UnityEditor;

// PlayerController
public class CharacterView : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Projector shadowProjector;
    public HitboxView hitboxPrefab;
    public ProjectileView projectilePrefab;
    public float zDistance = 2.0f;
    public float shadowSize = 0.5f;
    public float shadowOffset = -0.03f;
    public bool showHitboxes { get; set; }
    private CharacterData data;
    private Dictionary<string, Sprite[]> sprites;
    private HitboxView collisionBoxView;
    private HitboxView projectileBoxView;
    private ProjectileView projectileView;
    private List<HitboxView> hitboxViews;
    private List<HitboxView> hurtboxViews;

    public int currentFrame;
    private Transform model;
    private Animator animator;

    public void Awake()
    {
        sprites = new Dictionary<string, Sprite[]>();
        hitboxViews = new List<HitboxView>();
        hurtboxViews = new List<HitboxView>();

        collisionBoxView = Instantiate(hitboxPrefab, transform);
        collisionBoxView.spriteRenderer.color = new Color(0f, 1f, 0f, .5f);
        collisionBoxView.spriteRenderer.sortingLayerName = "COLLISIONBOX";

        projectileView = Instantiate(projectilePrefab, transform);
        projectileView.spriteRenderer.sortingLayerName = "PROJECTILE";

        projectileBoxView = Instantiate(hitboxPrefab, transform);
        projectileBoxView.spriteRenderer.color = new Color(1f, 0f, 0f, .5f);
        projectileBoxView.spriteRenderer.sortingLayerName = "HITBOX";
    }

    // 读取动画对应的Sprites
    // 读取模型、动画
    public void LoadResources(CharacterData _data)
    {
        data = _data;

        //var prefab = ResManager.LoadPrefab($"Prefabs/{data.name}");
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Bundles/Prefabs/Player.prefab");
        model = Instantiate(prefab, transform).transform;
        model.name = "model";
        animator = model.GetComponentInChildren<Animator>();
        //BattleEvent.doSetAnimeSpeed += SetAnimeSpeed;
        animator.speed = 1; //这里要播放Idle。PlayLoop()时，会设为0。
    }

    public void UpdateCharacterView(Character character)
    {
        // display correct sprite based on state
        CharacterState currentState = character.state;
        Animation currentAnimation = character.isAttacking() ? data.attacks[currentState.ToString()] : data.animations[currentState.ToString()];
        currentFrame = (int)character.framesInState % currentAnimation.totalFrames;
        //if (currentState == CharacterState.DIE && character.framesInState >= currentAnimation.totalFrames)
        //{
        //    return; //持续++。死亡动画播放完，彻底不再播放动画。
        //}
        animator.Play(currentAnimation.animationName, 0, (float)currentFrame / currentAnimation.totalFrames); //卡顿

        // x and y position
        float viewX = ((character.position.x - Constants.BOUNDS_WIDTH / 2) / Constants.SCALE);
        float viewY = (character.position.y / Constants.SCALE);
        transform.position = new Vector3(viewX, viewY, zDistance);

        float maxY = Constants.BOUNDS_HEIGHT / Constants.SCALE;
        float normY = viewY / maxY;
        shadowProjector.orthographicSize = shadowSize * (1.0f - normY) * (1.0f - normY);

        // sprite facing direction
        spriteRenderer.flipX = character.facingRight;
        float flipShadow = (character.facingRight) ? shadowOffset : -shadowOffset;
        shadowProjector.transform.position = new Vector3(viewX + flipShadow, viewY + 2.0f, zDistance + 0.15f);

        // set correct drawing layer
        if (character.onTop)
        {
            spriteRenderer.sortingLayerName = "PLAYER_TOP";
        }
        else
        {
            spriteRenderer.sortingLayerName = "PLAYER_BOTTOM";
        }

        // projectile
        if (character.projectile.active)
        {
            projectileView.spriteRenderer.enabled = true;
            int index;
            if (character.projectile.activeSince < 2)
            {
                index = 0;
            }
            else
            {
                index = ((int)character.projectile.activeSince % (data.projectiles["FIREBALL"].distinctSprites - 1)) + 1;
            }
            projectileView.spriteRenderer.sprite = sprites["FIREBALL"][index];
            projectileView.spriteRenderer.flipX = character.projectile.facingRight;
            float projectileViewX = ((character.projectile.position.x - Constants.BOUNDS_WIDTH / 2) / Constants.SCALE);
            float projectileViewY = (character.projectile.position.y / Constants.SCALE);
            projectileView.transform.position = new Vector3(projectileViewX, projectileViewY, zDistance);

        }
        else
        {
            projectileView.spriteRenderer.enabled = false;
        }

        // boxes
        if (showHitboxes)
        {
            // collisionBox
            if (currentAnimation.collisionBox != null)
            {
                collisionBoxView.spriteRenderer.enabled = true;
                collisionBoxView.setRect(viewX, viewY, zDistance, character.facingRight, currentAnimation.collisionBox);
            }
            else
            {
                collisionBoxView.spriteRenderer.enabled = false;
            }

            // projectile
            if (character.projectile.active)
            {
                projectileBoxView.spriteRenderer.enabled = true;
                float projectileViewX = ((character.projectile.position.x - Constants.BOUNDS_WIDTH / 2) / Constants.SCALE);
                float projectileViewY = (character.projectile.position.y / Constants.SCALE);
                projectileBoxView.setRect(projectileViewX, projectileViewY, zDistance, character.projectile.facingRight, new int[] { -25, 25, -100, 100 });
            }
            else
            {
                projectileBoxView.spriteRenderer.enabled = false;
            }


            //hitboxes
            // deactivate all hitboxviews
            foreach (HitboxView hitboxView in hitboxViews)
            {
                hitboxView.spriteRenderer.enabled = false;
            }

            if (character.hitBoxes.Count > 0)
            {
                int diff = character.hitBoxes.Count - hitboxViews.Count;
                // instanciate additional hitboxviews, if needed
                if (diff > 0)
                {
                    for (int i = 0; i < diff; i++)
                    {
                        HitboxView hitboxView = Instantiate(hitboxPrefab, transform);
                        hitboxView.spriteRenderer.color = new Color(1f, 0f, 0f, .5f);
                        hitboxView.spriteRenderer.sortingLayerName = "HITBOX";
                        hitboxView.spriteRenderer.enabled = false;
                        hitboxViews.Add(hitboxView);
                    }
                }

                // set the hitboxviews to the correct place
                for (int i = 0; i < character.hitBoxes.Count; i++)
                {
                    if (!character.hitBoxes[i].enabled) continue;
                    hitboxViews[i].setRect(viewX, viewY, zDistance, character.facingRight, character.hitBoxes[i].GetCoords());
                    hitboxViews[i].spriteRenderer.enabled = true;
                }
            }

            //hurtboxes
            // deactivate all hurtboxviews
            foreach (HitboxView hurtboxView in hurtboxViews)
            {
                hurtboxView.spriteRenderer.enabled = false;
            }

            List<Box> hurtboxes;
            if (character.GetHurtBoxes(data, out hurtboxes))
            {
                int diff = hurtboxes.Count - hurtboxViews.Count;
                // instanciate additional hurtboxViews, if needed
                if (diff > 0)
                {
                    for (int i = 0; i < diff; i++)
                    {
                        HitboxView hurtBoxView = Instantiate(hitboxPrefab, transform);
                        hurtBoxView.spriteRenderer.color = new Color(0f, 0f, 1f, .5f);
                        hurtBoxView.spriteRenderer.sortingLayerName = "HURTBOX";
                        hurtboxViews.Add(hurtBoxView);
                    }
                }
                // set the hurtboxViews to the correct place
                foreach (HitboxView hurtboxView in hurtboxViews)
                {
                    hurtboxView.spriteRenderer.enabled = true;
                    hurtboxView.setRect(viewX, viewY, zDistance, character.facingRight, hurtboxes[0].GetCoords());
                    hurtboxes.RemoveAt(0);
                    if (hurtboxes.Count <= 0) break;
                }
            }
        }
        else
        {
            // deactivate collisionboxview
            collisionBoxView.spriteRenderer.enabled = false;
            projectileBoxView.spriteRenderer.enabled = false;
            // deactivate all hitboxviews
            foreach (HitboxView hitboxView in hitboxViews)
            {
                hitboxView.spriteRenderer.enabled = false;
            }
            // deactivate all hurtboxviews
            foreach (HitboxView hurtboxView in hurtboxViews)
            {
                hurtboxView.spriteRenderer.enabled = false;
            }
        }
    }
}