using System.Collections.Generic;
using UnityEngine;
using HitstunConstants;

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
        var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Bundles/Prefabs/Player.prefab");
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
        Debug.Log($"Anim : {currentState}");

        // x and y position
        //float viewX = ((character.position.x - Constants.BOUNDS_WIDTH / 2) / Constants.SCALE);
        //float viewY = (character.position.y / Constants.SCALE);
        //float viewZ = ((character.position.z - Constants.BOUNDS_WIDTH / 2) / Constants.SCALE);
        //transform.position = new Vector3(viewX, viewY, viewZ);
        // 朝摄像机方向移动
        float viewX = character.position.x / Constants.SCALE; //定点数转浮点
        float viewY = character.position.y / Constants.SCALE;
        float viewZ = character.position.z / Constants.SCALE;
        transform.position = new Vector3(viewX, viewY, viewZ);



        float maxY = Constants.BOUNDS_HEIGHT / Constants.SCALE;
        float normY = viewY / maxY;
        shadowProjector.orthographicSize = shadowSize * (1.0f - normY) * (1.0f - normY);
    }
}