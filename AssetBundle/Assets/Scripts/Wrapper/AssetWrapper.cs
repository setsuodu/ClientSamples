using UnityEngine;

public interface AssetWrapper
{
    public void LoadPrefab();
    public void LoadTexture2D();
    public void LoadBytes();
}

public class AssetBundleWrapper : AssetWrapper
{
    public void LoadPrefab()
    {
    
    }

    public void LoadTexture2D()
    {

    }

    public void LoadBytes()
    {

    }
}

public class AddressableWrapper : AssetWrapper
{
    public void LoadPrefab()
    {

    }

    public void LoadTexture2D()
    {

    }

    public void LoadBytes()
    {

    }
}