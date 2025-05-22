using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBase : MonoBehaviour
{
    [SerializeField] protected Theme theme;
    /// <summary>
    /// different theme has different sprites
    /// </summary>
    [SerializeField] protected GameObject[] sprites;
    /// <summary>
    /// the current sprite that is SetActive
    /// </summary>
    protected GameObject activeSprite;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        ChangeTheme(theme);
    }
    public virtual void ChangeTheme(Theme theme)
    {
        this.theme=theme;
        for(int i=sprites.Length-1;i>-1;--i)
            if(sprites[i]!=null){
                activeSprite=sprites[i];
                activeSprite.SetActive(i==(int)theme);
            }
    }
    public enum Theme
    {
        Forest,
        Card,
        Swamp,
        Rural
    }
}
