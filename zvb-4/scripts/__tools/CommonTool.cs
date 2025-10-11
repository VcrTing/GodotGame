using System;
using System.Collections.Generic;
using Godot;
using ZVB4.Interface;

public static class CommonTool
{
    /// <summary>
    /// 生成0~count-1的乱序索引列表
    /// </summary>
    public static List<int> GetShuffledIndexList(int count)
    {
        var idxList = new List<int>();
        for (int i = 0; i < count; i++) idxList.Add(i);
        var rand = new Random();
        for (int i = idxList.Count - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            int temp = idxList[i];
            idxList[i] = idxList[j];
            idxList[j] = temp;
        }
        return idxList;
    }
    public static string GetNameOfNode2D(Node2D node)
    {
        if (node == null) return "";
        IObj obj = node as IObj;
        if (obj == null) return "";
        return obj.GetObjName();
    }
    public static Node2D LocationNode2DByName(List<Node2D> _node2DList, string n) {
        Node2D node = null;
        foreach (var nd in _node2DList)
        {
            IObj other = nd as IObj;
            if (other != null && other.GetObjName() == n)
            {
                node = nd;
                break;
            }
        }
        return node;
    }
}
