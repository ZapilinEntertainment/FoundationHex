using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ColorCode : byte { White, Red, Green, Blue }
public enum HexSpriteType : byte { Normal, Fixed, Available, Frame }
public struct HexPosition
{
    public byte ringIndex, ringPosition;
    public static readonly HexPosition zer0 = new HexPosition(0, 0), unexist = new HexPosition(255, 255);
    public HexPosition Copy { get { return new HexPosition(ringIndex, ringPosition); } }

    public HexPosition(byte index, byte position)
    {
        ringIndex = index;
        ringPosition = position;
    }
    public HexPosition(int index, int position)
    {
        if (index < 0 || index > 255) { index = 255; Debug.Log("error in hexpositon index"); }
        if (position < 0 || position > 255) { position = 255; Debug.Log("error in hexposition pos"); }
        ringIndex = (byte)index;
        ringPosition = (byte)position;
    }
    public byte DefineSector()
    {
        if (ringIndex == 0) return ringPosition;
        else return (byte)(ringPosition / (ringIndex + 1));
    }
    public byte DefineInsectorPosition()
    {
        if (ringIndex == 0) return ringPosition;
        else {
            int segmentsCount = ringIndex + 1;
            int a = ringPosition / segmentsCount;
            if (ringPosition % segmentsCount == 0) return 0;
            else return (byte)(ringPosition - a * segmentsCount);
        }
    }
    public int InsectorToOnring(int ring, int sector, int pos)
    {
        return sector * (ring + 1) + pos;
    }
    public HexPosition GetNextPosition()
    {
        var sectors = ringIndex + 1;
        var inpos = DefineInsectorPosition();
        if (inpos == sectors - 1) return new HexPosition(ringIndex + 1, DefineSector() * (ringIndex + 2));
        else return new HexPosition(ringIndex, ringPosition + 1);
    }
    public HexPosition GetNeighbour(byte direction)
    {
        byte sector = DefineSector();
        var p = DefineInsectorPosition();
        int ring = 0, position = 0;
        switch (direction)
        {
            case 0: // up
                {
                    switch (sector)
                    {
                        case 0:
                            ring = ringIndex + 1;
                            position = p;
                            break;
                        case 1:
                            {
                                if (p == 0)
                                {
                                    ring = ringIndex + 1;
                                    position = InsectorToOnring(ring, 0, ring);
                                }
                                else
                                {
                                    ring = ringIndex;
                                    position = ringPosition - 1;
                                }
                                break;
                            }
                        case 2:
                            {
                                if (p == 0)
                                {
                                    ring = ringIndex;
                                    position = ringPosition - 1;
                                }
                                else
                                {
                                    ring = ringIndex - 1;
                                    position = InsectorToOnring(ringIndex - 1, sector, p - 1);                                    
                                }
                                break;
                            }
                        case 3:
                            {
                                ring = ringIndex - 1;
                                if (p == ringIndex)
                                {                                    
                                    position = InsectorToOnring(ringIndex - 1, sector + 1, 0);
                                }
                                else
                                {
                                    position = InsectorToOnring(ringIndex - 1, 3, p);
                                }
                                break;
                            }
                        case 4:
                            ring = ringIndex;
                            position = ringPosition + 1;
                            break;
                        case 5:
                            ring = ringIndex + 1;
                            position = InsectorToOnring(ringIndex + 1, sector, p + 1);
                            break;
                    }
                    break;
                }
            case 1: //up-right
                {
                    switch (sector)
                    {
                        case 0:
                            ring = ringIndex + 1;
                            position = p + 1;
                            break;
                        case 1:
                            ring = ringIndex + 1;
                            position = InsectorToOnring(ringIndex + 1, sector, p);
                            break;
                        case 2:
                            if (p == 0)
                            {
                                ring = ringIndex + 1;
                                position = InsectorToOnring(ringIndex + 1, sector-1, ringIndex + 1);
                            }
                            else
                            {
                                ring = ringIndex;
                                position = ringPosition - 1;
                            }
                            break;
                        case 3:
                            if (p == 0)
                            {
                                ring = ringIndex;
                                position = ringPosition - 1;
                            }
                            else {
                                ring = ringIndex - 1;
                                position = InsectorToOnring(ring, sector, p - 1);
                            }
                            break;
                        case 4:
                            {
                                ring = ringIndex - 1;
                                if (p == ringIndex) position = InsectorToOnring(ring, sector + 1, 0);
                                else position = InsectorToOnring(ring, sector, p);
                                break;
                            }
                        case 5:
                            {
                                ring = ringIndex;
                                if (p == ringIndex) position = 0;
                                else position = ringPosition + 1;
                                break;
                            }
                    }
                    break;
                }
            case 2: // right - down
                {
                    switch (sector)
                    {
                        case 0:
                            {
                                ring = ringIndex;
                                position = ringPosition + 1;
                                break;
                            }
                        case 1:
                            {
                                ring = ringIndex + 1;
                                position = InsectorToOnring(ring, sector, p + 1);
                                break;
                            }
                        case 2:
                            {
                                ring = ringIndex + 1;
                                position = InsectorToOnring(ring, sector, p);
                                break;
                            }
                        case 3:
                            {
                                if (p == 0)
                                {
                                    ring = ringIndex + 1;
                                    position = InsectorToOnring(ring, sector - 1, ring);
                                }
                                else
                                {
                                    ring = ringIndex;
                                    position = ringPosition - 1;
                                }
                                break;
                            }
                        case 4:
                            {
                                if (p == 0)
                                {
                                    ring = ringIndex;
                                    position = ringPosition - 1;
                                }
                                else
                                {
                                    ring = ringIndex - 1;
                                    position = InsectorToOnring(ring, sector, p - 1);
                                }
                                break;
                            }
                        case 5:
                            {
                                ring = ringIndex - 1;
                                if (p == ringIndex)
                                {
                                    position = 0;
                                }
                                else
                                {
                                    position = InsectorToOnring(ring, sector, p);
                                }
                                break;
                            }
                    }
                    break;
                }
            case 3: // down 
                {
                    switch (sector)
                    {
                        case 0:
                            {
                                ring = ringIndex - 1;
                                if (p == ringIndex) position = InsectorToOnring(ring, 1, 0);
                                else position = InsectorToOnring(ring, 0, p);
                                break;
                            }
                        case 1:
                            {
                                ring = ringIndex;
                                position = ringPosition + 1;
                                break;
                            }
                        case 2:
                            {
                                ring = ringIndex + 1;
                                position = InsectorToOnring(ring, sector, p + 1);
                                break;
                            }
                        case 3:
                            {
                                ring = ringIndex + 1;
                                position = InsectorToOnring(ring, sector, p);
                                break;
                            }
                        case 4:
                            {
                                if (p == 0)
                                {
                                    ring = ringIndex + 1;
                                    position = InsectorToOnring(ring, sector - 1, ring);
                                }
                                else
                                {
                                    ring = ringIndex;
                                    position = ringPosition - 1;
                                }
                                break;
                            }
                        case 5:
                            {
                                if (p == 0)
                                {
                                    ring = ringIndex;
                                    position = ringPosition - 1;
                                }
                                else
                                {
                                    ring = ringIndex - 1;
                                    position = InsectorToOnring(ring, sector, p - 1);
                                }
                                break;
                            }
                    }
                    break;
                }
            case 4: // left - down
                {
                    switch (sector)
                    {
                        case 0:
                            {
                                if (p == 0)
                                {
                                    ring = ringIndex;
                                    position = (ringIndex + 1) * 6 - 1;
                                }
                                else
                                {
                                    ring = ringIndex - 1;
                                    position = ringPosition - 1;
                                }
                                break;
                            }
                        case 1:
                            {
                                ring = ringIndex - 1;
                                if (p == ringIndex) position = InsectorToOnring(ring, sector + 1, 0);
                                else position = InsectorToOnring(ring, sector, p);
                                break;
                            }
                        case 2:
                            {
                                ring = ringIndex;
                                position = ringPosition + 1;
                                break;
                            }
                        case 3:
                            {
                                ring = ringIndex + 1;
                                position = InsectorToOnring(ring, sector, p + 1);
                                break;
                            }
                        case 4:
                            {
                                ring = ringIndex + 1;
                                position = InsectorToOnring(ring, sector, p);
                                break;
                            }
                        case 5:
                            {
                                if (p == 0)
                                {
                                    ring = ringIndex + 1;
                                    position = InsectorToOnring(ring, sector - 1, ring);
                                }
                                else
                                {
                                    ring = ringIndex;
                                    position = ringPosition - 1;
                                }
                                break;
                            }
                    }
                    break;
                }
            case 5: // left - up
                {
                    switch (sector)
                    {
                        case 0:
                            {
                                if (p == 0)
                                {
                                    ring = ringIndex + 1;
                                    position = InsectorToOnring(ring, 5, ring);
                                }
                                else
                                {
                                    ring = ringIndex;
                                    position = ringPosition - 1;
                                }
                                break;
                            }
                        case 1:
                            {
                                if (p == 0)
                                {
                                    ring = ringIndex;
                                    position = ringPosition - 1;
                                }
                                else
                                {
                                    ring = ringIndex - 1;
                                    position = InsectorToOnring(ring, sector, p - 1);
                                }
                                break;
                            }
                        case 2:
                            {
                                ring = ringIndex - 1;
                                if (p == ringIndex) position = InsectorToOnring(ring, sector + 1, 0);
                                else position = InsectorToOnring(ring, sector, p);
                                break;
                            }
                        case 3:
                            {
                                ring = ringIndex;
                                position = ringPosition + 1;
                                break;
                            }
                        case 4:
                            {
                                ring = ringIndex + 1;
                                position = InsectorToOnring(ring, sector, p + 1);
                                break;
                            }
                        case 5:
                            {
                                ring = ringIndex + 1;
                                position = InsectorToOnring(ring, sector, p);
                                break;
                            }
                    }
                    break;
                }
        }
        if (ring < 0 || position < 0) return unexist;
        else  return new HexPosition(ring, position);
    }
    public (byte,byte) ToBytes()
    {
        return (ringIndex, ringPosition);
    }

    public static byte GetOppositeDirection(byte x)
    {
        int d = x + 3;
        if (d > 5) d -= 6;
        return (byte)d;
    }
}
public sealed class HexBoost {
    public Hex giver { get; private set; }
    public Hex receiver { get; private set; }
    public float value { get; private set; }
    private int ID;
    private static int nextID = 1;

    private HexBoost() {
        ID = nextID++;
    }
    public HexBoost(Hex i_giver, Hex i_receiver, float i_val) : this()
    {
        giver = i_giver;
        receiver = i_receiver;
        value = i_val;
    }

    public override bool Equals(object obj)
    {
        // Check for null values and compare run-time types.
        if (obj == null || GetType() != obj.GetType())
            return false;
        else return ID == (obj as HexBoost).ID;
    }
    public static bool operator ==(HexBoost A, HexBoost B)
    {
        if (ReferenceEquals(A, null))
        {
            return ReferenceEquals(B, null);
        }
        return A.Equals(B);
    }
    public static bool operator !=(HexBoost A, HexBoost B)
    {
        return !(A == B);
    }
}

public sealed class Hex
{
    public override bool Equals(object obj)
    {
        // Check for null values and compare run-time types.
        if (obj == null || GetType() != obj.GetType())
            return false;
        else return ID == (obj as Hex).ID;
    }
    public static bool operator ==(Hex A, Hex B)
    {
        if (ReferenceEquals(A, null))
        {
            return ReferenceEquals(B, null);
        }
        return A.Equals(B);
    }
    public static bool operator !=(Hex A, Hex B)
    {
        return !(A == B);
    }

    public ColorCode colorcode { get; private set; }
    private Text valueText {
        get
        {
            if (_valueText == null) {
                _valueText = image?.transform.GetChild(0).GetComponent<Text>();
                _valueText.gameObject.SetActive(true);
            }
            return _valueText;
        }
    }
    private Text _valueText;
    public HexPosition hexPosition { get; private set; }
    public HexSpriteType spriteType
    {
        get { return _spriteType; }
        private set
        {
            if (image != null)
                _spriteType = value;
            image.sprite = GetHexSprite(value);
        }
    }
    private HexSpriteType _spriteType;    
    public Vector3 position { get { return image?.transform.position ?? Vector3.zero; } } 
    private Image image;
    private float colorValue = DEFAULT_VALUE;
    private int ID;  private static int nextID = 1;
    private Dictionary<byte, HexBoost> hexBoosts;

    private const float BOOST_VAL = 0.2f, DEFAULT_VALUE = 1f;
    private static readonly Color redColor = Color.Lerp(Color.red, Color.white, 0.7f),
        blueColor = Color.Lerp(Color.blue, Color.white, 0.7f),
        greenColor = Color.Lerp(Color.green, Color.white, 0.7f);
    private static readonly Sprite[] hexSprites;
    public static Sprite GetHexSprite(HexSpriteType hst)
    {
        switch (hst)
        {
            case HexSpriteType.Fixed: return hexSprites[0];
            case HexSpriteType.Available: return hexSprites[1];            
            case HexSpriteType.Frame: return hexSprites[2];
            default: return hexSprites[3];
        }
    }


    static Hex() {
        hexSprites = Resources.LoadAll<Sprite>("hex");
     }
    public static Color GetColorByColorcode(ColorCode cc)
    {
        switch (cc)
        {
            case ColorCode.Red: return redColor;
            case ColorCode.Blue: return blueColor;
            case ColorCode.Green: return greenColor;
            default: return Color.white;
        }
    }

    private Hex()
    {
        ID = nextID++;
    }
    public Hex(HexPosition i_pos, Image i_img) : this()
    {
        hexPosition = i_pos;
        image = i_img;
        colorcode = ColorCode.White;
        spriteType = HexSpriteType.Available;        
    }
    public Hex(HexPosition i_pos, Image i_img, ColorCode i_clr) : this()
    {
        hexPosition = i_pos;
        image = i_img;
        colorcode = i_clr;
        spriteType = HexSpriteType.Normal;
        image.color = GetColorByColorcode(colorcode);
    }
    public Hex(HexPosition i_pos, Image i_img, ColorCode i_clr, HexSpriteType hst) : this()
    {
        hexPosition = i_pos;
        image = i_img;
        colorcode = i_clr;
        spriteType = hst;
        image.color = GetColorByColorcode(colorcode);
    }

    public (ColorCode, float) GetColouredValue()
    {
        if (colorcode == ColorCode.White || spriteType != HexSpriteType.Normal) return (ColorCode.White, 0f);
        else return (colorcode, colorValue);
    }

    public void SetColor(ColorCode cc)
    {
        colorcode = cc;
        image.color = GetColorByColorcode(colorcode);
        if (spriteType == HexSpriteType.Available)
        {
            // UNBLOCKING
            spriteType = HexSpriteType.Normal;
        }
    }
    public void SetImage(HexSpriteType hst)
    {
        spriteType = hst;
        if (hst == HexSpriteType.Frame || hst == HexSpriteType.Available) colorcode = ColorCode.White;
    }
    public void SetButtonActivity(bool x)
    {
        image.GetComponent<Button>().enabled = x;
    }
    public void ChangeText(string s)
    {
        var t = image.transform.GetChild(0);;
        t.GetComponent<Text>().text = s;
        t.gameObject.SetActive(true);
    }

    public void CheckNeighbours()
    { 
        if (hexBoosts != null)
        {
            foreach (var hb in hexBoosts)
            {
                if (hb.Value.giver != this) hb.Value.giver.RemoveBoost(HexPosition.GetOppositeDirection(hb.Key));
                else hb.Value.receiver.RemoveBoost(HexPosition.GetOppositeDirection(hb.Key));
            }
            hexBoosts.Clear();
            colorValue = 0f;
            hexBoosts = null;
        }        
        //
        if (colorcode != ColorCode.White) {
            var sm = SessionManager.current;
            // for blue color:
            ColorCode outcomingBoostColor = ColorCode.Red, incomingBoostColor = ColorCode.Green;
            float outcomingBoostValue = 1f + BOOST_VAL, incomingBoostValue = outcomingBoostValue;
            if (colorcode != ColorCode.Blue)
            {
                if (colorcode == ColorCode.Red)
                {
                    outcomingBoostColor = ColorCode.Green;
                    outcomingBoostValue = 1f - BOOST_VAL;
                    incomingBoostColor = ColorCode.Blue;                    
                }
                else
                {
                    if (colorcode == ColorCode.Green)
                    {
                        outcomingBoostColor = ColorCode.Blue;
                        incomingBoostColor = ColorCode.Red;
                        incomingBoostValue = 1f - BOOST_VAL;
                    }
                }
            }
            HexBoost hb;
            HexPosition hpos;
            Hex h; 
            for (byte i = 0; i < 6; i++)
            {
                hpos = hexPosition.GetNeighbour(i);
                h = sm.GetHex(hpos);                
                if (h != null )
                {
                    if (h.colorcode == outcomingBoostColor && h.spriteType != HexSpriteType.Fixed)
                    {
                        hb = new HexBoost(this, h, outcomingBoostValue);
                        if (hexBoosts == null) hexBoosts = new Dictionary<byte, HexBoost>();
                        hexBoosts.Add(i, hb);
                        h.AddBoost(hb, HexPosition.GetOppositeDirection(i));
                    }
                    else
                    {
                        if (h.colorcode == incomingBoostColor)
                        {
                            hb = new HexBoost(h, this, incomingBoostValue);
                            if (hexBoosts == null) hexBoosts = new Dictionary<byte, HexBoost>();
                            hexBoosts.Add(i, hb);
                            h.AddBoost(hb, HexPosition.GetOppositeDirection(i));
                        }
                    }
                }
            }
            BoostsRecalculation();
        }
    }
    public void AddBoost(HexBoost hb, byte side)
    {
        if (hexBoosts == null) hexBoosts = new Dictionary<byte, HexBoost>();
        if (hexBoosts.ContainsKey(side))
        {
            Debug.Log("add boost error");
            return;
        }
        hexBoosts.Add(side,hb);
        BoostsRecalculation();
    }    
    public void RemoveBoost(byte side)
    {
        if (hexBoosts != null)
        {
            if (hexBoosts.ContainsKey(side))
            {
                hexBoosts.Remove(side);
                BoostsRecalculation();
                if (hexBoosts.Count == 0) hexBoosts = null;
            }            
        }
    }
    private void BoostsRecalculation()
    {
        float prevVal = colorValue;
        colorValue = DEFAULT_VALUE;
        if (hexBoosts != null) 
        {
            foreach (var h in hexBoosts.Values)
            {
                if (h.receiver == this) colorValue *= h.value;
            }
        }
        if (colorValue != prevVal)
        {
            SessionManager.current.needHexInfoRecalculation = true;
            if (spriteType == HexSpriteType.Normal) valueText.text = string.Format("{0:0.##}", colorValue);
        }
    }
}
