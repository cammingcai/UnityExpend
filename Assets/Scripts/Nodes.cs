﻿
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nodes : BaseNode
{
    /// <summary>
    /// 只在内存中分配一次
    /// </summary>
    private static Text text;
    public override void Start()
    {
        
        text = GameObject.Find("Canvas/CurrentClickNodeText").GetComponent<Text>();
        base.Start();
        GetComponent<Button>().onClick.AddListener(() =>
        {
            SwitchNode();
            Text txt = transform.GetChild(0).GetComponent<Text>();
            text.text = txt.text;
        });
    }
}
public abstract class BaseNode : MonoBehaviour
{
    /// <summary>
    /// 当前节点组的父容器
    /// </summary>
    public RectTransform parentRectTransform;
    /// <summary>
    /// 如果当前节点存在子节点
    /// </summary>
    public List<BaseNode> childNodes;
    /// <summary>
    /// 上一个被选中的节点是哪一个
    /// </summary>
    public static BaseNode CurrentNode { get; private set; }
    /// <summary>
    /// 当前节点是否被打开
    /// </summary>
    private bool isOpen = false;
    /// <summary>
    /// 计算要容器的目标大小
    /// </summary>
    private float EndContainerHeight;
    /// <summary>
    /// 计算容器的初始大小
    /// </summary>
    private float StartContainerHeight;
    /// <summary>
    /// 弹力速度，改变可更改弹性速度,可自己手动更改
    /// </summary>
    [Range(0.0f, 1f)] private float EleasticSpeed = 0.5f;

    public virtual void Start()
    {
        if (parentRectTransform != null)
        {
            StartContainerHeight = parentRectTransform.rect.height;

            if (this.childNodes != null && this.childNodes.Count != 0)
            {
                float space = parentRectTransform.GetComponent<VerticalLayoutGroup>().spacing;
                for (int i = 0; i < this.childNodes.Count; i++)
                {
                    EndContainerHeight += childNodes[i].GetComponent<RectTransform>().rect.height + space;
                    Debug.LogError("value"+(i+1)+"y="+childNodes[i].GetComponent<RectTransform>().sizeDelta.y);
                }

                EndContainerHeight += this.parentRectTransform.rect.height;
            }
            
         
        }
    }
    /// <summary>
    /// 点击按钮时，切换节点的开关，以及改变父容器的大小
    /// </summary>
    public void SwitchNode()
    {
        if (CurrentNode != null && CurrentNode.childNodes.Count != 0)
        {
            //如果点击的是节点本身
            if (CurrentNode == this && CurrentNode.childNodes != null)
            {
                this.isOpen = !isOpen;
                StartCoroutine(CotainerSiwtch(isOpen, CurrentNode));
                foreach (var node in CurrentNode.childNodes)
                {
                    //关闭上一个被打开的Node
                    //node.gameObject.SetActive(isOpen);
                    node.isOpen = false;
                }
                CurrentNode = null;
                return;
            }
            else
            {
                if (this.childNodes.Count == 0) return;

                StartCoroutine(CotainerSiwtch(false, CurrentNode));

                foreach (var node in CurrentNode.childNodes)
                {
                    //关闭当前节点的所有子节点
                    //node.gameObject.SetActive(false);
                    node.isOpen = false;
                }
                CurrentNode.isOpen = false;
                CurrentNode = null;
            }
        }
        if (this.childNodes != null && this.childNodes.Count != 0)
        {
            
            this.isOpen = !isOpen;
            
            StartCoroutine(CotainerSiwtch(this.isOpen, this));
            foreach (var node in this.childNodes)
            {
                //需要启用当前对象
                //node.gameObject.SetActive(this.isOpen);
                node.isOpen = this.isOpen;
            }
            CurrentNode = this;
            CurrentNode.childNodes = this.childNodes;

        }
    }
    /// <summary>
    ///容器开关，控制容器的打开关闭
    /// </summary>
    /// <param name="open"></param>
    /// <returns></returns>
    private  IEnumerator CotainerSiwtch(bool open, BaseNode nodes)
    {
        if (this.parentRectTransform != null)
        {
            float t = 0;

            float targetValue = open ? EndContainerHeight : StartContainerHeight;

            while (t < EleasticSpeed)
            {
                yield return null;
                float value = Mathf.Lerp(nodes.parentRectTransform.sizeDelta.y, targetValue, t / EleasticSpeed);
                
                Debug.LogError("value="+value);
                t += Time.deltaTime;
                nodes.parentRectTransform.sizeDelta = new Vector2(nodes.parentRectTransform.sizeDelta.x, value);
                nodes.parentRectTransform.position = Vector3.zero;
            }
        }
    }
}