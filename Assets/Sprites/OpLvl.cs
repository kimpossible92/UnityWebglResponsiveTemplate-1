using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpLvl : MonoBehaviour
{
    [SerializeField] OpenAppLevel GetManager; 
    public MapLevel2 currentLevel;
    public void SetLevel(int levelInt)
    {
        GetManager.lvl(levelInt);
        GetManager.OnappMatch();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
