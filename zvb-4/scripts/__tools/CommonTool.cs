using System;
using System.Collections.Generic;

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
}
