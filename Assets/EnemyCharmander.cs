using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum TypeEnemyAttack
{
    tolchock,
    fire
}
public class EnemyCharmander : MonoBehaviour
{
    protected bool onoff = false;
    [SerializeField] GameObject Lamp;
    public bool islocked = false;
    int i1 = 0, i3=0;
    [SerializeField]LayerMask buttonLayer;
    protected List<Transform> GetTransformsRaycast = new List<Transform>();
    protected Transform CenterPos;
    [SerializeField] GameObject FieldFire;protected int RotateNum = 0,oldrotNum=0,newRotNum=0;
    protected TypeEnemyAttack CurrentAttack;
    IEnumerator GetTimerThreeSec()
    {
        yield return new WaitForSeconds(2.0f);
        //EveryFrame();
        yield return new WaitForSeconds(2.0f);
    }
    public IEnumerator GetEnumerator()
    {
        switch (islocked)
        {

            case true:
                while (i1 < 10)
                {
                    //print("0");
                    Lamp.transform.Translate(Vector2.down * 0.5f);
                    i1++;
                    yield return new WaitForSeconds(0.3f);
                }
                //islocked = false;
                break;
            case false:
                while (i1 >= 1)
                {
                    //print("1");
                    Lamp.transform.Translate(Vector2.up * 0.5f);
                    i1--;
                    yield return new WaitForSeconds(0.3f);
                }
                //islocked = true;
                break;
        }
    }
    protected int tick1;
    protected IEnumerator GetEnumReload(Vector3 tovec)
    {
        while (isFire1)
        {
            print(tick1);
            if (tick1 >= 225) { tick1 = 0; }
            tick1++;
            yield return new WaitForSeconds(1.0f);
        }
    }
    public void UpdDownWall()
    {
        Lamp.transform.Translate(Vector2.down * 2);
    }
    public void ButtonOnOff(bool value)
    {
        onoff = value;
    }
    Vector3 tStartPos1;
    // Use this for initialization
    void Start()
    {
        
        isFire1 = true;
        //FieldFire.GetComponent<ParentFire>().enabled = false;
        tStartPos1 = transform.localPosition;
        //GameMode.THIS.AddButtonLamps(this);
        for (int i = 0; i < 8; i++)
        {
            GetTransformsRaycast.Add(transform.Find("GameObject ("+i+")"));
        }
        CenterPos = transform.Find(name);
        StartCoroutine(GetTimerThreeSec());
    }
    protected int tick = 0;
    protected const float koef= 0.2f;
    protected bool isFire1=false;protected RaycastHit GetHit = new RaycastHit();
    [SerializeField]
    private Vector3 bulletRotation;
    [SerializeField]
    private float throwForce = 600f;
    protected void AddHits(RaycastHit hit)
    {
        if (GetHit.transform.position != hit.transform.position) { }
    }
    #region CharFire
    protected void EveryFrame()
    {
        if (isFire1 == false) { tick1 = 0; }
        for (int i = 0; i < 65; i++)
        {
            if (MMDebug.Raycast3DBoolean(transform.position + new Vector3(i * 0.2f, 0, 0),
                Vector3.up, 9.5f, buttonLayer, Color.blue, true))
            {
                CurrentAttack = (TypeEnemyAttack)Random.Range(0, 2);
                if (RotateNum != 1)
                {
                    //print(RotateNum); 
                    CharManderFire(new Vector3(i * 0.2f, 0, 0));
                    RotateNum = 1;
                }
            }
            if (MMDebug.Raycast3DBoolean(transform.position + new Vector3(0, 0, i * 0.2f),
               Vector3.up, 9.5f, buttonLayer, Color.blue, true))
            {
                if (RotateNum != 2)
                {
                    //print(RotateNum);
                    CharManderFire(new Vector3(0, 0, i * 0.2f)); RotateNum = 2;
                }
            }
            if (MMDebug.Raycast3DBoolean(transform.position + new Vector3(0, 0, -i * 0.2f),
               Vector3.up, 9.5f, buttonLayer, Color.blue, true))
            {
                if (RotateNum != 3)
                {
                    CharManderFire(new Vector3(0, 0, -i * 0.2f)); RotateNum = 3;
                }
            }
            if (MMDebug.Raycast3DBoolean(transform.position + new Vector3(-i * 0.2f, 0, 0),
                Vector3.up, 9.5f, buttonLayer, Color.blue, true))
            {
                if (RotateNum != 4)
                {
                    CharManderFire(new Vector3(-i * 0.2f, 0, 0)); RotateNum = 4;
                }
            }
            if (MMDebug.Raycast3DBoolean(transform.position + new Vector3(-i * 0.2f, 0, -i * 0.2f),
                Vector3.up, 9.5f, buttonLayer, Color.blue, true))
            {
                if (RotateNum != 5)
                {
                    CharManderFire(new Vector3(-i * 0.2f, 0, -i * 0.2f)); RotateNum = 5;
                }
            }
            if (MMDebug.Raycast3DBoolean(transform.position + new Vector3(i * 0.2f, 0, i * 0.2f),
                Vector3.up, 9.5f, buttonLayer, Color.blue, true))
            {
                if (RotateNum != 6)
                {
                    CharManderFire(new Vector3(i * 0.2f, 0, i * 0.2f)); RotateNum = 6;
                }
            }
            if (MMDebug.Raycast3DBoolean(transform.position + new Vector3(i * 0.2f, 0, -i * 0.2f),
                Vector3.up, 9.5f, buttonLayer, Color.blue, true))
            {
                if (RotateNum != 7)
                {
                    CharManderFire(new Vector3(i * 0.2f, 0, -i * 0.2f)); RotateNum = 7;
                }
            }
            if (MMDebug.Raycast3DBoolean(transform.position + new Vector3(-i * 0.2f, 0, i * 0.2f),
                Vector3.up, 9.5f, buttonLayer, Color.blue, true))
            {
                if (RotateNum != 8)
                {
                    CharManderFire(new Vector3(-i * 0.2f, 0, i * 0.2f)); RotateNum = 8;
                }
            }
        }
    }
    #endregion
    // Update is called once per frame
    void Update()
    {
        EverFrame2();
        transform.localPosition = tStartPos1;
    }
    private bool _fire = true;
    [SerializeField]
    private Vector2 _fireDelay;
    private IEnumerator FireDelay(float delay)
    {
        _fire = false;
        yield return new WaitForSeconds(delay);
        _fire = true;
    }
    public void EverFrame2()
    {
        if (!_fire) return;
        CharManderFire(Vector3.back);
        StartCoroutine(FireDelay(Random.Range(_fireDelay.x, _fireDelay.y)));
    }
    private const float koef2 = 10.5f;
    private float distance1;
    protected void CharManderFire(Vector3 tovec)
    {
        GetComponent<CharacterMotor3D>().RotTowards(transform.position + tovec);
        if (CurrentAttack == TypeEnemyAttack.fire)
        {
            GameObject loadInst = Instantiate(FieldFire, FieldFire.transform.position, Quaternion.identity);
            loadInst.GetComponent<ParentFire>().enabled = true;
            loadInst.GetComponent<ParentFire>().SetUp(true);
            loadInst.transform.localScale = new Vector3(2.25f, 2.25f, 2.25f);
        }
        if (CurrentAttack == TypeEnemyAttack.tolchock)
        {
            GameObject loadInst = Instantiate(FieldFire, FieldFire.transform.position, Quaternion.identity);
            loadInst.GetComponent<ParentFire>().enabled = true;
            loadInst.GetComponent<ParentFire>().SetUp(true);
            loadInst.transform.localScale = new Vector3(2.85f, 2.85f, 2.85f);
        }
    }
}
