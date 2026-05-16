using System.IO;
using MemoryPack;
using UnityEngine;

public class TestOldVersion : MonoBehaviour
{
    private const string OldFileName = "file1_old_a_bcd.bin";
    private const string CurrentFileName = "file2_current_a_bcde.bin";

    void Start()
    {
        RunCompatibilityDemo();
    }

    [ContextMenu("Run Compatibility Demo")]
    public void RunCompatibilityDemo()
    {
        SaveOldVersionFile();
        LoadOldVersionFileAsCurrent();
        SaveCurrentVersionFile();
    }

    [ContextMenu("1. Save Old Version File")]
    public void SaveOldVersionFile()
    {
        var oldData = new OldVersionA
        {
            B = 10,
            Nested = new OldVersionNestedData
            {
                C = "old-c",
                D = 30.5f
            }
        };

        var bytes = MemoryPackSerializer.Serialize(oldData);
        var path = GetPath(OldFileName);
        File.WriteAllBytes(path, bytes);

        Debug.Log($"Saved old A(B,C,D) binary to: {path}, size: {bytes.Length}");
    }

    [ContextMenu("2. Load Old File As Current Version")]
    public void LoadOldVersionFileAsCurrent()
    {
        var path = GetPath(OldFileName);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"Old version file does not exist, create it first: {path}");
            return;
        }

        var bytes = File.ReadAllBytes(path);
        var currentData = MemoryPackSerializer.Deserialize<A>(bytes);

        Debug.Log(
            "Loaded old A(B,Nested(C,D)) binary as current A(B,Nested(C,D,F),E): " +
            //$"B={currentData.B}, " +
            $"Nested.C={currentData.Nested?.C}, " +
            $"Nested.D={currentData.Nested?.D}, " +
            $"Nested.F={currentData.Nested?.F}, " +
            $"E={currentData.E}");
    }

    [ContextMenu("3. Save Current Version File")]
    public void SaveCurrentVersionFile()
    {
        //var currentData = new A
        //{
        //    B = 100,
        //    Nested = new NestedData
        //    {
        //        C = "current-c",
        //        D = 300.5f,
        //        F = "new-nested-f"
        //    },
        //    E = 400
        //};

        //var bytes = MemoryPackSerializer.Serialize(currentData);
        //var path = GetPath(CurrentFileName);
        //File.WriteAllBytes(path, bytes);

        //Debug.Log($"Saved current A(B,C,D,E) binary to: {path}, size: {bytes.Length}");
    }

    private static string GetPath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }
}

[MemoryPackable(GenerateType.VersionTolerant)]
public partial class OldVersionA
{
    [MemoryPackOrder(0)]
    public int B { get; set; }

    [MemoryPackOrder(1)]
    public OldVersionNestedData Nested { get; set; } = new();
}

[MemoryPackable(GenerateType.VersionTolerant)]
public partial class A
{
    //[MemoryPackOrder(0)]
    //public int B { get; set; }

    [MemoryPackOrder(1)]
    public NestedData Nested { get; set; } = new();

    [MemoryPackOrder(2)]
    public int E { get; set; }
}

[MemoryPackable(GenerateType.VersionTolerant)]
public partial class OldVersionNestedData
{
    [MemoryPackOrder(0)]
    public string C { get; set; } = string.Empty;

    [MemoryPackOrder(1)]
    public float D { get; set; }
}

[MemoryPackable(GenerateType.VersionTolerant)]
public partial class NestedData
{
    [MemoryPackOrder(0)]
    public string C { get; set; } = string.Empty;

    [MemoryPackOrder(1)]
    public float D { get; set; }

    [MemoryPackOrder(2)]
    public string F { get; set; } = string.Empty;
}
