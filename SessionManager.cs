using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SessionManager : MonoBehaviour
{
    public static SessionManager current { get; private set; }

    private Constructor constructor;
    private Dictionary<(byte,byte), Hex> hexList;
    private Hex[] basicHexes = new Hex[6];
    public bool needHexInfoRecalculation = false;
    private float[] colorsValues = new float[3];
    private float[] sectorsColorsValues = new float[6];
    private ColorCode[] basicColorcodes = new ColorCode[6];
    private float timer;
    private const float RECALCULATION_TIME = 1f;

    void Awake()
    {
        current = this;
    }

    public void PrepareGridData(Constructor c)
    {
        constructor = c;
        basicHexes[0] = hexList[(0, 0)];
        basicHexes[1] = hexList[(0, 1)];
        basicHexes[2] = hexList[(0, 2)];
        basicHexes[3] = hexList[(0, 3)];
        basicHexes[4] = hexList[(0, 4)];
        basicHexes[5] = hexList[(0, 5)];
        basicColorcodes[0] = basicHexes[0].colorcode;
        basicColorcodes[1] = basicHexes[1].colorcode;
        basicColorcodes[2] = basicHexes[2].colorcode;
        basicColorcodes[3] = basicHexes[3].colorcode;
        basicColorcodes[4] = basicHexes[4].colorcode;
        basicColorcodes[5] = basicHexes[5].colorcode;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            timer = RECALCULATION_TIME;
            if (needHexInfoRecalculation)
            {
                colorsValues[0] = 0f; colorsValues[1] = 0f; colorsValues[2] = 0f;
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

                hexList[(0,0)].ChangeText(sectorsColorsValues[0].ToString());
                basicHexes[1].ChangeText(sectorsColorsValues[1].ToString());
                basicHexes[2].ChangeText(sectorsColorsValues[2].ToString());
                basicHexes[3].ChangeText(sectorsColorsValues[3].ToString());
                basicHexes[4].ChangeText(sectorsColorsValues[4].ToString());
                basicHexes[5].ChangeText(sectorsColorsValues[5].ToString());

                constructor.UpdateColorValues(colorsValues);
                needHexInfoRecalculation = false;
            }
        }
    }

    public void AddHex(Hex h)
    {
        if (hexList == null)
        {
            hexList = new Dictionary<(byte,byte), Hex>();
            hexList.Add(h.hexPosition.ToBytes(), h);
        }
        else
        {
            if (hexList.ContainsKey(h.hexPosition.ToBytes())) Debug.Log("error - hex with same position exists");
            else hexList.Add(h.hexPosition.ToBytes(), h);
        }
    }
    public Hex GetHex(HexPosition hpos)
    {
        if (hexList == null || !hexList.ContainsKey(hpos.ToBytes())) return null;
        else return hexList[hpos.ToBytes()];
    }
    public Hex GetHex(byte ringIndex, byte ringPosition)
    {
        if (hexList == null || !hexList.ContainsKey((ringIndex, ringPosition))) return null;
        else return hexList[(ringIndex, ringPosition)];
    }
}
