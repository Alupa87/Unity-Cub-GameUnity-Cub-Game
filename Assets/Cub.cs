using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class Cub : MonoBehaviour
{   
    // public Sprite leftWalkSprite;
    // public Sprite rightWalkSprite;
    public Sprite[] walkSprites; // 走路动画帧数组（拖入多张走路图）
    private float walkFrameTimer; // 走路帧计时器
    private int currentWalkFrame; // 当前走路帧序号
    public float walkFrameRate = 0.1f; // 每帧切换间隔（越小越快）
    public Sprite standSprite;   // 站立图
    public Sprite jumpSprite;    // 跳跃图
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
    public bool life =true;
    private float live_time=10f;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb2D;
    private bool isOnGround = false;
    private Keyboard keyboard;
    private Mouse mouse;
    private bool isPlayer = true;

    void Start()
    {   
        // 初始化输入设备
        keyboard = Keyboard.current;
        mouse = Mouse.current;
        mainCamera = Camera.main;

        QualitySettings.vSyncCount =0;
        Application.targetFrameRate=120;
        rb2D = GetComponent<Rigidbody2D>();
        rb2D.freezeRotation = true;
        spriteRenderer = GetComponent<SpriteRenderer>();

        
        // if (!isPlayer)
        // {
        //     enabled = false;
        //     return;
        // }
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

        // ✅ 第二步：判空后再赋值图片（避免空引用）
        if (standSprite != null)
        {
            spriteRenderer.sprite = standSprite;
            spriteRenderer.color = Color.white; // 强制不透明
        }
        else
        {
            Debug.LogError("请把站立图拖到 Cub 脚本的 standSprite 槽位！");
        }

        // 初始化分数
        UpdateScoreText();
    }

    void FixedUpdate()
    {   
        if(!isPlayer||! life) return;
        
        float horizontalInput = 0;
        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) horizontalInput = -1;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) horizontalInput = 1;
        }
        rb2D.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb2D.linearVelocity.y);
        if (spriteRenderer != null && walkSprites != null && walkSprites.Length > 0 && horizontalInput!=0 && isOnGround)
        {
            // 计时器计时
            walkFrameTimer += Time.deltaTime;
            if (walkFrameTimer >= walkFrameRate)
            {
                // 切换到下一帧
                currentWalkFrame = (currentWalkFrame + 1) % walkSprites.Length;
                spriteRenderer.sprite = walkSprites[currentWalkFrame];
                walkFrameTimer = 0;
            }
            if (horizontalInput>0) spriteRenderer.flipX=true;
            if (horizontalInput<0) spriteRenderer.flipX=false;
        }
        else if(isOnGround)
        {
            spriteRenderer.sprite=standSprite;
        }
        else
        {
            if(jumpSprite!=null)
            {spriteRenderer.sprite=jumpSprite;
            spriteRenderer.flipX=horizontalInput<0;}
        }
    }

    void Update()
    {
        if (!isPlayer) return;
        if (true)
        {
            live_time-=Time.deltaTime;
            live_time=Mathf.Max(live_time,0);

            if(live_time<=0)
            {
                life=false;
                spriteRenderer.sprite=deathSprite;
            }
        }
        if(keyboard.rKey.wasPressedThisFrame)
        {
            life=true;
            live_time=10f;
        }
        if (!life) return;
        if (canjump && keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
        {
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, 0);
            rb2D.AddForce(Vector2.up * 400);
            jumpcount++;
            if (jumpcount >= 5 && !isOnGround)
            {
                canjump = false;
            }
        }

        
        // if (spriteRenderer != null && horizontalInput!=0 && isOnGround)
        // {
        //     // 计时器计时
        //     walkFrameTimer += Time.deltaTime;
        //     if (walkFrameTimer >= walkFrameRate)
        //     {
        //         // 切换到下一帧
        //         currentWalkFrame = (currentWalkFrame + 1) % walkSprites.Length;
        //         spriteRenderer.sprite = walkSprites[currentWalkFrame];
        //         walkFrameTimer = 0;
        //     }
        //     if (horizontalInput>0) spriteRenderer.flipX=true;
        //     if (horizontalInput<0) spriteRenderer.flipX=false;
        // }
        // else if(isOnGround)
        // {
        //     spriteRenderer.sprite=standSprite;
        // }
        // else
        // {
        //     spriteRenderer.sprite=jumpSprite;
        // }


        
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

        // 鼠标左键生成方块（保留原有逻辑）
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

    // 碰撞检测（保留原有逻辑）
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.name == "Ground"&&isPlayer)
        {
            isOnGround = true;
            jumpcount = 0;
            canjump = true;
            // if (spriteRenderer != null)
            // {
            //     spriteRenderer.color = new Color(Random.value, Random.value, Random.value);
            // }
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.name == "Ground")
        {
            isOnGround = false;
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null&&liveText !=null)
        {
            scoreText.text = "score:" + score;
            liveText.text = "Left time:" +live_time;
        }
        else
        {
            Debug.LogWarning("请把 ScoreText 拖到 Cub 脚本的 scoreText 槽位！");
        }
    }
}