using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Cub : MonoBehaviour
{   
    public Sprite[] walkSprites; 
    private float walkFrameTimer; 
    private int currentWalkFrame; 
    public float walkFrameRate = 0.1f; 
    public Sprite standSprite;   
    public Sprite jumpSprite;    
    public Sprite deathSprite;
    private Camera mainCamera;
    private Vector3 lastMousePos;
    public float dragSpeed = 0.01f;
    public int score = 0;
    public TMP_Text scoreText;
    public TMP_Text liveText;
    public float moveSpeed = 5f;
    public int jumpcount = 0;
    public bool canjump = true;
    public bool life = true;
    private float live_time = 10f;
    public bool stickwall = false; // 贴墙标记

    [SerializeField] private LayerMask groundLayer; // 仅地面层
    [SerializeField] private LayerMask wallLayer;   // 新增：仅墙壁层（必须单独分层！）
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb2D;
    private bool isOnGround = false;
    private Keyboard keyboard;
    private Mouse mouse;
    [SerializeField] private bool isPlayer = true;

    // 新增：重力参数（避免硬编码）
    private float normalGravityScale; // 默认重力（读取rb2D初始值）
    public float wallGravityScale = 2f; // 贴墙时重力（自主下落）

    void Start()
    {   
        // 初始化输入设备
        keyboard = Keyboard.current;
        mouse = Mouse.current;
        mainCamera = Camera.main;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
        rb2D = GetComponent<Rigidbody2D>();
        rb2D.freezeRotation = true;
        // 保存默认重力（避免硬编码1f，适配Inspector面板设置）
        normalGravityScale = rb2D.gravityScale;
        
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb2D == null)
        {
            Debug.LogError("给角色添加 Rigidbody2D 组件！");
            enabled = false;
            return;
        }
        if (spriteRenderer == null)
        {
            Debug.LogError("给角色添加 SpriteRenderer 组件！");
            enabled = false;
            return;
        }

        if (standSprite != null)
        {
            spriteRenderer.sprite = standSprite;
            spriteRenderer.color = Color.white;
        }
        else
        {
            Debug.LogError("请把站立图拖到 Cub 脚本的 standSprite 槽位！");
        }

        UpdateScoreText();
    }

    void FixedUpdate()
    {   
        if(!isPlayer || !life) return;
        
        float horizontalInput = 0;
        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) horizontalInput = -1;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) horizontalInput = 1;
        }

        // 贴墙时锁定水平输入（避免贴墙时仍能左右移动）
        if (!stickwall)
        {
            rb2D.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb2D.linearVelocity.y);
        }
        else
        {
            rb2D.linearVelocity = new Vector2(0,rb2D.linearVelocity.y);
        }

        // 动画逻辑（保留）
        if (spriteRenderer != null && walkSprites != null && walkSprites.Length > 0 && horizontalInput != 0 && isOnGround)
        {
            walkFrameTimer += Time.deltaTime;
            if (walkFrameTimer >= walkFrameRate)
            {
                currentWalkFrame = (currentWalkFrame + 1) % walkSprites.Length;
                spriteRenderer.sprite = walkSprites[currentWalkFrame];
                walkFrameTimer = 0;
            }
            spriteRenderer.flipX = horizontalInput > 0;
        }
        else if(isOnGround)
        {
            spriteRenderer.sprite = standSprite;
        }
        else
        {
            if(jumpSprite != null)
            {
                spriteRenderer.sprite = jumpSprite;
                spriteRenderer.flipX = horizontalInput < 0;
            }
        }
    }

    void Update()
    {
        if (!isPlayer) return;

        // 存活时间逻辑
        live_time -= Time.deltaTime;
        live_time = Mathf.Max(live_time, 0);

        if(keyboard.rKey.isPressed)
        {
            life = true;
            live_time = 10f;
            spriteRenderer.sprite = standSprite;
        }

        if(live_time <= 0)
        {
            life = false;
            spriteRenderer.sprite = deathSprite;
            return; // 死亡后直接退出，避免逻辑混乱
        }

        if (!life) return;

        // 跳跃逻辑
        if (canjump && keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, 0);
            rb2D.AddForce(Vector2.up * 400);
            jumpcount++;
            if (jumpcount >= 5 && !isOnGround)
            {
                canjump = false;
            }
            // 跳跃时重置贴墙状态
            stickwall = false;
            rb2D.gravityScale = normalGravityScale;
        }

        // 相机拖拽逻辑（保留）
        if (mouse != null && mouse.rightButton.isPressed)
        {
            Vector2 currentMouseScreenPos = mouse.position.ReadValue();
            if (lastMousePos == Vector3.zero)
            {
                lastMousePos = new Vector3(currentMouseScreenPos.x, currentMouseScreenPos.y, 0);
            }
            Vector3 mouseDelta = lastMousePos - new Vector3(currentMouseScreenPos.x, currentMouseScreenPos.y, 0);
            float safeDeltaX = mouseDelta.x * dragSpeed * 0.01f;
            float safeDeltaY = mouseDelta.y * dragSpeed * 0.01f;
            
            Vector3 newCamPos = mainCamera.transform.position;
            newCamPos.x += safeDeltaX;
            newCamPos.y += safeDeltaY;
            newCamPos.x = Mathf.Clamp(newCamPos.x, -1000, 1000);
            newCamPos.y = Mathf.Clamp(newCamPos.y, -1000, 1000);
            mainCamera.transform.position = newCamPos;
            
            lastMousePos = new Vector3(currentMouseScreenPos.x, currentMouseScreenPos.y, 0);
        }
        else
        {
            lastMousePos = Vector3.zero;
        }

        // 生成方块逻辑（保留）
        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            Vector2 screenPos = mouse.position.ReadValue();
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -10));
            mousePos.z = 0;
            
            GameObject newCube = Instantiate(gameObject, mousePos, Quaternion.identity);
            newCube.GetComponent<Cub>().isPlayer = false;
            newCube.transform.localScale = new Vector3(Random.Range(1, 3), Random.Range(1, 3), 1);
            score++;
        }

        UpdateScoreText();
    }

    // 首次碰撞（仅落地判定）
    void OnCollisionEnter2D(Collision2D other)
    {
        if (!isPlayer) return;

        // 落地判定
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            foreach (ContactPoint2D contact in other.contacts)
            {
                if(contact.normal.y > 0.7f)
                {
                    isOnGround = true;
                    jumpcount = 0;
                    canjump = true;
                    // 落地时重置贴墙状态和重力
                    stickwall = false;
                    rb2D.gravityScale = normalGravityScale;
                    break;
                }
            }
        }
    }

    // 持续碰撞（核心：贴墙判定 + 实时落地验证）
    void OnCollisionStay2D(Collision2D other)
    {
        if(((1<< other.gameObject.layer)&groundLayer)!=0 && isPlayer)
        {
            // 先重置贴墙状态，避免误判
            stickwall = false;
            
            foreach(ContactPoint2D contact in other.contacts)
            {
                // 只在"未落地+接触竖直墙"时才标记贴墙
                if(!isOnGround && Mathf.Abs(contact.normal.x) > 0.3f && contact.normal.y < 0.7f)
                {
                    stickwall = true;
                    rb2D.gravityScale = 2f; // 保证自主下落
                }
                // 实时验证是否落地
                if(contact.normal.y > 0.7f)
                {
                    isOnGround = true;
                    stickwall = false;
                    rb2D.gravityScale = 3f;
                }
            }
        }
    }

    // 碰撞退出（重置状态）
    void OnCollisionExit2D(Collision2D other)
    {
        if (!isPlayer) return;

        // 离开地面
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            isOnGround = false;
        }

        stickwall =false;
        rb2D.gravityScale =3f;
    }

    void UpdateScoreText()
    {
        if (scoreText != null && liveText != null)
        {
            scoreText.text = "score:" + score;
            liveText.text = "Left time:" + Mathf.Round(live_time); // 四舍五入，避免小数过多
        }
        else
        {
            Debug.LogWarning("请把 ScoreText/LiveText 拖到 Cub 脚本对应槽位！");
        }
    }
}