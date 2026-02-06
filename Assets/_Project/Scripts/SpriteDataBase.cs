using System.Collections.Generic;
using UnityEngine;

public class SpriteDataBase : MonoBehaviour
{
    #region Singleton
    private static SpriteDataBase instance;
    public static SpriteDataBase Instance => instance;


    public List<Sprite> sprites;

    private Dictionary<string, Sprite> lookup;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
            return;
        }

        instance = this;

        lookup = new Dictionary<string, Sprite>();
        foreach (var sprite in sprites)
            lookup[sprite.name] = sprite;
    }
    #endregion
    public Sprite Get(string id)
    {
        return lookup.TryGetValue(id, out var sprite) ? sprite : null;
    }
}
