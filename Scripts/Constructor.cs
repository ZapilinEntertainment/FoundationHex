using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Constructor : MonoBehaviour
{
    [SerializeField] private GameObject example, controlPanel, selectorFrame;
    [SerializeField] private Transform deckHoster;
    
    private float[] basicHexesValues = new float[6];
    private float outerRadius, innerRadius;
    private Dictionary<HexPosition, Hex> hexList;
    private Hex selectedHex;
    private const float CONST_0 =1.73205f; 

    private GameObject CreateNewHexPlace(HexPosition hpos)
    {
        var g = Instantiate(example, example.transform.position, example.transform.rotation, deckHoster);
        g.transform.position += GetHexOnCanvasPosition(hpos);        
        g.name = "hex " + hpos.ringIndex.ToString() + ':' + hpos.ringPosition.ToString();
        g.SetActive(true);
        var hex = new Hex(hpos, g.GetComponent<Image>());
        hexList.Add(hpos, hex);            
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
        switch (x)
        {
            case 0: return new Vector3(CONST_0, -1f , 0f).normalized;
            case 1: return Vector3.down ;
            case 2: return new Vector3(-0.9f, -0.4f , 0f);
            case 3: return new Vector3(-0.9f, 0.4f, 0f);
            case 4: return Vector3.up;
            case 5: return new Vector3(0.9f, 0.4f, 0f);
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
        return;
        for (int i = 0; i < 3; i++)
        {
            var pos = new HexPosition(2, i);
            g = CreateNewHexPlace(pos);
            b = g.GetComponent<Button>();
            b.onClick.AddListener(() => this.HexClicked(pos));
            b.enabled = true;
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
            CreateNext(selectedHex.hexPosition);
            selectedHex = null;
        }
        controlPanel.SetActive(false);
        selectorFrame.SetActive(false);
    }
    

    private void CreateNext(HexPosition i_hp)
    {
        HexPosition hpos = HexPosition.zer0;
        switch (i_hp.ringIndex)
        {
            case 1:
                if (i_hp.ringPosition % 2 == 1) hpos = new HexPosition(i_hp.ringPosition, i_hp.ringPosition + 1);
                else hpos = new HexPosition(i_hp.ringPosition + 1, (i_hp.ringPosition -1) /2 * 3);
                break;
        }
        CreateNewHexPlace(hpos);
    }
}
