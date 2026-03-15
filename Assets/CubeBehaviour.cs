using UnityEngine;
using TMPro; // 如果用到TextMeshPro，需引入

// 这是所有方块的通用类，所有方块都挂载这个类
public class CubeBehaviour : MonoBehaviour
{
    // 【可选】如果需要给不同方块设置个性化参数，可暴露公共字段（在Inspector中设置）
    [Header("方块基础参数")]
    public int cubeID; // 每个方块的唯一ID（区分不同方块）
    public string cubeName; // 方块名称
    public Color cubeColor = Color.white; // 方块颜色

    // 【核心】你提到的cub相关功能（示例：比如方块被点击时执行cub逻辑）
    private TMP_Text tipText; // 示例：方块的提示文本

    // 初始化：所有方块挂载这个类后，启动时都会执行
    void Start()
    {
        // 初始化方块外观（示例）
        GetComponent<Renderer>().material.color = cubeColor;
        
        // 初始化cub相关逻辑（示例：获取文本组件）
        tipText = GetComponentInChildren<TMP_Text>();
        if (tipText != null)
        {
            tipText.text = $"方块{cubeID}: {cubeName}";
        }
        
        // 执行通用的cub初始化逻辑
        InitCubLogic();
    }

    // 所有方块共享的cub核心逻辑
    private void InitCubLogic()
    {
        Debug.Log($"方块{cubeID}的cub逻辑已初始化");
        // 这里写你的cub相关代码（比如阵亡、移动、交互等）
    }

    // 【示例】cub核心功能：方块被点击时触发的逻辑（所有方块通用）
    void OnMouseDown()
    {
        ExecuteCubAction();
    }

    // 所有方块共享的cub执行方法
    public void ExecuteCubAction()
    {
        Debug.Log($"方块{cubeID}执行cub动作！");
        // 比如：方块变色、播放动画、显示文本等
        GetComponent<Renderer>().material.color = Color.red;
        if (tipText != null)
        {
            tipText.text = $"方块{cubeID}：cub动作触发！";
        }
    }
}