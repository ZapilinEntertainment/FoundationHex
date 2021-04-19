using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Constructor : MonoBehaviour
{
    [SerializeField] private GameObject example, controlPanel, selectorFrame;
    [SerializeField] private Transform deckHoster;
    [SerializeField] private Text[] colorCountLabels = new Text[3];
    
    private float outerRadius, innerRadius;   
    private Hex selectedHex;
    private const float CONST_0 =1.73205f;
    private const int RINGS_LIMIT = 4;
    
 
    private SessionManager sm;

    private GameObject CreateNewHexPlace(HexPosition hpos)
    {
        var g = Instantiate(example, example.transform.position, example.transform.rotation, deckHoster);
        g.transform.position += GetHexOnCanvasPosition(hpos);
        g.name = "hex " + hpos.ringIndex.ToString() + ':' + hpos.ringPosition.ToString();
        var b = g.GetComponent<Button>();
        b.onClick.AddListener(() => this.HexClicked(hpos.Copy));
        b.enabled = true;
        g.SetActive(true);
        sm.AddHex(new Hex(hpos, g.GetComponent<Image>()));
        return g;
    }
    private Hex CreateNewHex(HexPosition hpos, ColorCode cc, HexSpriteType hst)
    {
        var g = Instantiate(example, example.transform.position, example.transform.rotation, deckHoster);
        g.transform.position += GetHexOnCanvasPosition(hpos);
        g.name = "hex " + hpos.ringIndex.ToString() + ':' + hpos.ringPosition.ToString();
        g.SetActive(true);
        var hex = new Hex(hpos, g.GetComponent<Image>(), cc, hst);
        sm.AddHex(hex);
        return hex;
    }
    private Vector3 GetHexOnCanvasPosition(HexPosition hpos)
    {
        switch (hpos.ringIndex)
        {
            case 0: return GetMainDirectionVector(hpos.ringPosition) * 2f * innerRadius;
                
            default:
                {
                    byte sector = hpos.DefineSector();                    
                    var dir = GetMainDirectionVector(sector) * (innerRadius * 2f * (hpos.ringIndex + 1));
                    dir += GetInsectorDirection(sector)* hpos.DefineInsectorPosition() * 2f * innerRadius;
                    return dir;
                }
        }
    }
    private Vector3 GetMainDirectionVector(byte x)
    {
        return (Quaternion.AngleAxis(60f * x, Vector3.back) * Vector3.up);
    } 
    private Vector3 GetInsectorDirection(byte x)
    {
        // normalized
        // набольших расстояниях будет сказываться погрешность!
        switch (x)
        {
            case 0: return new Vector3(0.866025f, -0.5f , 0f).normalized;
            case 1: return Vector3.down ;
            case 2: return new Vector3(-0.866025f, -0.5f, 0f).normalized;
            case 3: return new Vector3(-0.866025f, 0.5f, 0f).normalized;
            case 4: return Vector3.up;
            case 5: return new Vector3(0.866025f, 0.5f, 0f).normalized;
            default:return Vector3.zero;
        }
    }


    private void Start()
    {
        sm = SessionManager.current;
        innerRadius = example.GetComponent<RectTransform>().rect.width * 0.4f;
        outerRadius = innerRadius * 2f / CONST_0;

        HexPosition hpos = new HexPosition(0, 0);
        CreateNewHex(hpos, ColorCode.Red, HexSpriteType.Fixed);
        hpos = new HexPosition(0, 1);
        CreateNewHex(hpos, ColorCode.Blue, HexSpriteType.Fixed);
        hpos = new HexPosition(0, 2);
        CreateNewHex(hpos, ColorCode.Green, HexSpriteType.Fixed);
        hpos = new HexPosition(0, 3);
        CreateNewHex(hpos, ColorCode.Green, HexSpriteType.Fixed);
        hpos = new HexPosition(0, 4);
        CreateNewHex(hpos, ColorCode.Blue, HexSpriteType.Fixed);
        hpos = new HexPosition(0, 5);
        CreateNewHex(hpos, ColorCode.Red, HexSpriteType.Fixed);


        GameObject g;
        Button b;
        for (int i = 0; i < 12; i+=2)
        {
            var pos = new HexPosition(1, i);
            g = CreateNewHexPlace(pos);
            b = g.GetComponent<Button>();
            b.onClick.AddListener(() => this.HexClicked(pos));
            b.enabled = true;
        }

        sm.PrepareGridData(this);
        /*
        for (int i =0; i < 12; i++)
        {
            var pos = new HexPosition(1, i);
            g = CreateNewHexPlace(pos);
            b = g.GetComponent<Button>();          
            b.onClick.AddListener (() => this.HexClicked(pos));
            b.enabled = true;
        }
        for (int i = 0; i < 18; i ++)
        {
            var pos = new HexPosition(2, i);
            g = CreateNewHexPlace(pos);
            b = g.GetComponent<Button>();
            b.onClick.AddListener(() => this.HexClicked(pos));
            b.enabled = true;
        }
        for (int i = 0; i < 24; i ++)
        {
            var pos = new HexPosition(3, i);
            g = CreateNewHexPlace(pos);
            b = g.GetComponent<Button>();
            b.onClick.AddListener(() => this.HexClicked(pos));
            b.enabled = true;
        }
        */
    }

  public void UpdateColorValues(float[] s)
    {
        colorCountLabels[0].text = s[0].ToString();
        colorCountLabels[1].text = s[1].ToString();
        colorCountLabels[2].text = s[2].ToString();
    }


    public void HexClicked(HexPosition hpos)
    {
        selectedHex = sm.GetHex(hpos);
        if (selectedHex != null)
        {
            controlPanel.transform.position = selectorFrame.transform.position + Vector3.right * 75f;
            controlPanel.SetActive(true);
            selectorFrame.transform.position = selectedHex.position;
            selectorFrame.transform.SetAsLastSibling();
            selectorFrame.SetActive(true);            
        }
        else
        {
            if (selectorFrame.activeSelf)
            {
                selectorFrame.SetActive(false);
                controlPanel.SetActive(false);
            }
        }
    }

    public void ControlPanel_SetColor(int i)
    {
        if (selectedHex != null )
        {
            ColorCode cc = ColorCode.White;
            if (i == 0) cc = ColorCode.Red;
            else
            {
                if (i == 1) cc = ColorCode.Green;
                else cc = ColorCode.Blue;
            }
            if (selectedHex.spriteType == HexSpriteType.Available)
            {                
                selectedHex.SetColor(cc);
                selectedHex.CheckNeighbours();
                //NEXT HEX
                var nextPos = selectedHex.hexPosition.GetNextPosition();
                if (nextPos.ringIndex < RINGS_LIMIT) CreateNewHexPlace(nextPos);
                sm.needHexInfoRecalculation = true;
                //               
            }
            else
            {
                if (selectedHex.spriteType == HexSpriteType.Normal && selectedHex.colorcode != cc)
                {
                    selectedHex.SetColor(cc);
                    selectedHex.CheckNeighbours();
                    sm.needHexInfoRecalculation = true;
                }
            }
            selectedHex = null;
        }
        controlPanel.SetActive(false);
        selectorFrame.SetActive(false);
    }
    
}
