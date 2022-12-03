using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenAppLevel : MonoBehaviour
{
    [HideInInspector]public int CurrentLevel=1; [HideInInspector]public int MaxX=3;
    [SerializeField] public SquareBlocks[] Blocksf = new SquareBlocks[200];
    [SerializeField] private obstacle[] GetObstacles = new obstacle[19];
    public Block[] blocksp;
    [SerializeField]
    GameObject LevelParent;
    [SerializeField]
    GameObject[] blockpref = new GameObject[7];
    [SerializeField]
    Vector3 vector2position;
    public void lvl(int level)
    {
        TextAsset text = (TextAsset)Resources.Load("" + level);
        openLeveltxt(text.text);
    }
    public void openLeveltxt(string mapText)
    {
        string[] vs = null;
        string[] lines = mapText.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        int mapline = 0;
        foreach (string line in lines)
        {
            if (line.StartsWith("MODE"))
            {
                string modeSting = line.Replace("MODE", string.Empty).Trim();
            }
            else if (line.StartsWith("SIZE"))
            { }
            else if (line.StartsWith("LIMIT"))
            {
                string blockString = line.Replace("LIMIT", string.Empty).Trim();
                string[] sizes = blockString.Split(new string[] { "/" }, System.StringSplitOptions.RemoveEmptyEntries);
            }
            else if (line.StartsWith("COLOR LIMIT ")) { }
            else if (line.StartsWith("STARS")) { }
            else if (line.StartsWith("COLLECT COUNT")) { }
            else if (line.StartsWith("COLLECT ITEMS")) { }
            else
            {
                string[] st = line.Split(new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < st.Length; i++)
                {
                    Blocksf[(mapline * MaxX) + i] = new SquareBlocks();
                    Blocksf[(mapline * MaxX) + i].blck = int.Parse(st[i][0].ToString());
                }
                mapline++;
            }
        }
    }
    public void OnappMatch()
    {
        foreach (var bl in allBlocks)
        {
            if (bl != null) Destroy(bl.gameObject);
        }
        allBlocks = new List<GameObject>();
        blocksp = new Block[GetObstacles.Length*3];
        for (int row = 0; row < GetObstacles.Length; row++)
        {
            for (int col = 0; col < MaxX; col++)
            {
                Createblock(col, row);
            }
        }
    }
    public List<GameObject> allBlocks = new List<GameObject>();
    public void Createblock(int i, int j)
    {
        //print(blockpref[Blocksf[(j * MaxX) + i].blck]);
        GameObject vblck = (GameObject)Instantiate(blockpref[Blocksf[(j * MaxX) + i].blck], GetObstacles[j].transform.position + (vector2position*(i+1)), blockpref[Blocksf[(j * MaxX) + i].blck].transform.rotation);
        //GetObstacles[j].transform.SetParent(LevelParent.transform);
        blocksp[j * MaxX + i] = vblck.GetComponent<Block>();
        vblck.GetComponent<Block>().row = j;
        vblck.GetComponent<Block>().col = i;
        vblck.GetComponent<Block>().types = 1;
        allBlocks.Add(vblck);
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
