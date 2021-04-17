using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Constructor : MonoBehaviour
{
    [SerializeField] private GameObject example, controlPanel, selectorFrame;
    [SerializeField] private Transform deckHoster;
    [SerializeField] private Text[] colorCountLabels = new Text[3];

    private float[] colorsValues = new float[3]; 
    private float[] sectorsColorsValues = new float[6];
    private ColorCode[] basicColorcodes = new ColorCode[6];
    private float outerRadius, innerRadius;
    private Dictionary<HexPosition, Hex> hexList;
    private Hex selectedHex;
    private const float CONST_0 =1.73205f, RECALCULATION_TIME = 1f;
    private const int RINGS_LIMIT = 4;
    public bool needInfoRecalculation = false;
    private float timer;

    private GameObject CreateNewHexPlace(HexPosition hpos)
    {
        var g = Instantiate(example, example.transform.position, example.transform.rotation, deckHoster);
        g.transform.position += GetHexOnCanvasPosition(hpos);
        g.name = "hex " + hpos.ringIndex.ToString() + ':' + hpos.ringPosition.ToString();
        var b = g.GetComponent<Button>();
        b.onClick.AddListener(() => this.HexClicked(hpos.Copy));
        b.enabled = true;
        g.SetActive(true);
        var h = new Hex(hpos, g.GetComponent<Image>());
        hexList.Add(hpos, h);
        return g;
    }
    private Hex CreateNewHex(HexPosition hpos, ColorCode cc, HexSpriteType hst)
    {
        var g = Instantiate(example, example.transform.position, example.transform.rotation, deckHoster);
        g.transform.position += GetHexOnCanvasPosition(hpos);
        g.name = "hex " + hpos.ringIndex.ToString() + ':' + hpos.ringPosition.ToString();
        g.SetActive(true);
        var hex = new Hex(hpos, g.GetComponent<Image>(), cc, hst);
        hexList.Add(hpos, hex);
        return hex;
    }
    private Vector3 GetHexOnCanvasPosition(HexPosition hpos)
    {
        switch (hpos.ringIndex)
        {
            case 0: return GetMainDirectionVector(hpos.ringPosition) * 2f * innerRadius;
                /*
            case 1:
                float s;
                if (hpos.ringPosition % 2 == 1) s = 4f * innerRadius;
                else s = 2f * outerRadius;
                return (Quaternion.AngleAxis(30f * hpos.ringPosition, Vector3.forward) * Vector3.up) * s;
                */
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
        hexList = new Dictionary<HexPosition, Hex>();
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

        for (int i =0; i < 6; i++)
        {
            basicColorcodes[i] = hexList[new HexPosition(0, i)].colorcode;
        }

        GameObject g;
        Button b;
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
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            timer = RECALCULATION_TIME;
            if (needInfoRecalculation)
            {
                colorsValues[0] = 0f; colorsValues[1] = 0f;colorsValues[2] = 0f;
                sectorsColorsValues[0] = 0f; sectorsColorsValues[1] = 0f; sectorsColorsValues[2] = 0f;
                sectorsColorsValues[3] = 0f; sectorsColorsValues[4] = 0f; sectorsColorsValues[5] = 0f;
                (ColorCode color, float val) colorValue;
                ColorCode cc;
                byte sector;
                foreach (var h in hexList.Values)
                {
                    colorValue = h.GetColouredValue();
                    cc = colorValue.color;
                    if (cc == ColorCode.White) continue;
                    else
                    {                        
                        switch (cc)
                        {
                            case ColorCode.Red:
                                colorsValues[0] += colorValue.val;                                
                                break;
                            case ColorCode.Green:
                                colorsValues[1] += colorValue.val;
                                break;
                            case ColorCode.Blue:
                                colorsValues[2] += colorValue.val;
                                break;
                        }
                        sector = h.hexPosition.DefineSector();
                        if (basicColorcodes[sector] == cc) sectorsColorsValues[sector] += colorValue.val;
                    }
                }
                hexList[new HexPosition(0, 0)].ChangeText(sectorsColorsValues[0].ToString());
                hexList[new HexPosition(0, 1)].ChangeText(sectorsColorsValues[1].ToString());
                hexList[new HexPosition(0, 2)].ChangeText(sectorsColorsValues[2].ToString());
                hexList[new HexPosition(0, 3)].ChangeText(sectorsColorsValues[3].ToString());
                hexList[new HexPosition(0, 4)].ChangeText(sectorsColorsValues[4].ToString());
                hexList[new HexPosition(0, 5)].ChangeText(sectorsColorsValues[5].ToString());
                colorCountLabels[0].text = colorsValues[0].ToString();
                colorCountLabels[1].text = colorsValues[1].ToString();
                colorCountLabels[2].text = colorsValues[2].ToString();
            }
        }

        if (Input.GetKeyDown("x"))
        {
            foreach (var h in hexList.Values) h.SetColor(ColorCode.White);
        }
    }


    public void HexClicked(HexPosition hpos)
    {
        if (hexList.TryGetValue(hpos, out selectedHex))
        {
            controlPanel.SetActive(true);
            selectorFrame.transform.position = selectedHex.position;
            selectorFrame.transform.SetAsLastSibling();
            selectorFrame.SetActive(true);

            HexPosition pos;
            Hex h;
            for (byte i = 0; i < 6; i++)
            {
                pos = hpos.GetNeighbour(i);
                if (hexList.TryGetValue(pos, out h))
                {
                    h.SetColor(ColorCode.Red);
                }
            }
            
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
        if (selectedHex != null)
        {
            ColorCode cc = ColorCode.White;
            if (i == 0) cc = ColorCode.Red;
            else
            {
                if (i == 1) cc = ColorCode.Green;
                else cc = ColorCode.Blue;
            }
            selectedHex.SetColor(cc);
            selectedHex.SetButtonActivity(false);
            //NEXT HEX
            var nextPos = selectedHex.hexPosition.GetNextPosition();
            if (nextPos.ringIndex < RINGS_LIMIT) CreateNewHexPlace(nextPos);
            needInfoRecalculation = true;
            //
            
            selectedHex = null;
        }
        controlPanel.SetActive(false);
        selectorFrame.SetActive(false);
    }
    
}
