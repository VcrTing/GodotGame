using System;
using System.Collections.Generic;
using System.Text.Json;
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
    public static Node2D LocationNode2DByName(List<Node2D> _node2DList, string n)
    {
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

    public static bool WriteDataToJson(List<System.Collections.Generic.Dictionary<string, object>> data, string filePath)
    {
        try
        {
            // 1. 配置JSON序列化选项（支持Godot的Dictionary和中文等）
            var options = new JsonSerializerOptions
            {
                WriteIndented = true, // 格式化JSON（便于阅读）
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping // 避免中文被转义
            };
            // 2. 序列化数据为JSON字符串
            string jsonString = JsonSerializer.Serialize(data, options);
            // 3. 覆盖写入文件（使用FileAccess）
            using (var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write))
            {
                file.StoreString(jsonString);
            }
            return true;
        }
        catch (FieldAccessException e)
        {
            GD.PrintErr($"文件操作失败: {e.Message}，路径: {filePath}");
            return false;
        }
        catch (JsonException e)
        {
            GD.PrintErr($"JSON序列化失败: {e.Message}");
            return false;
        }
        catch (System.Exception e)
        {
            GD.PrintErr($"错误: {e.Message}");
            return false;
        }
    }
    
    public static Dictionary<string, object> ToCSharpDictionary(Godot.Collections.Dictionary godotDict)
    {
        var csharpDict = new Dictionary<string, object>();

        foreach (var key in godotDict.Keys)
        {
            // 确保键是字符串（如果实际键为其他类型，需修改目标字典的键类型，如int）
            if (key.GetType() != typeof(string))
            {
                GD.PrintErr($"键类型不是string，无法转换：{key.GetType()}");
                continue;
            }
            // 处理值（递归转换嵌套结构）
            object value = ConvertGodotValue(godotDict[key]);
            csharpDict.Add(key.AsString(), value);
        }

        return csharpDict;
    }
    private static object ConvertGodotValue(object value)
    {
        // 如果值是Godot字典，递归转换
        if (value is Godot.Collections.Dictionary godotSubDict)
        {
            return ToCSharpDictionary(godotSubDict);
        }
        // 如果值是Godot数组，转换为C# List<object>
        else if (value is Godot.Collections.Array godotArray)
        {
            var csharpList = new List<object>();
            foreach (var item in godotArray)
            {
                csharpList.Add(ConvertGodotValue(item)); // 递归处理数组元素
            }
            return csharpList;
        }
        // 其他基础类型（int、string、float、Vector2等）直接返回
        else
        {
            return value;
        }
    }
    public static Godot.Collections.Array LoadJsonToArray(string CardsJsonPath)
    {
        if (string.IsNullOrEmpty(CardsJsonPath)) return null;
        var file = FileAccess.Open(CardsJsonPath, FileAccess.ModeFlags.Read);
        if (file == null) return null;
        var json = file.GetAsText();
        file.Close();
        return (Godot.Collections.Array)Godot.Json.ParseString(json);
    }
    public static List<Godot.Collections.Dictionary> LoadJsonToListDict(string CardsJsonPath)
    {
        List<Godot.Collections.Dictionary> availableItems = new List<Godot.Collections.Dictionary>();
        Godot.Collections.Array data = CommonTool.LoadJsonToArray(CardsJsonPath);
        if (data == null) return null;
        foreach (var obj in data)
        {
            var dict = (Godot.Collections.Dictionary)obj;
            if (dict == null) continue;
            availableItems.Add(dict);
        }
        return availableItems;
    }

    public static List<string> SplitStringToList(string sss)
    {
        List<string> plans = new List<string>();
        string[] aa = sss.Split("__");
        foreach (string a in aa)
        {
            if (a != null && a != "" && a.Length > 1)
            {
                plans.Add(a);
            }
        }
        return plans;
    }
    
    public static Godot.Collections.Dictionary LoadJsonToDict(string path) {
        if (Godot.FileAccess.FileExists(path))
        {
            using var file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
            var jsonText = file.GetAsText();
            var json = new Json();
            var err = json.Parse(jsonText);
            if (err == Error.Ok)
            {
                Godot.Collections.Dictionary cardData = json.Data.AsGodotDictionary();
                return cardData;
            }
        }
        return null;
    }
}
