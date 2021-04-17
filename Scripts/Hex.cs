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
}

public class Hex
{
    public override bool Equals(object obj)
    {
        // Check for null values and compare run-time types.
        if (obj == null || GetType() != obj.GetType())
            return false;
        else return true;
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
    private HexSpriteType _spriteType;
    public HexSpriteType spriteType { get { return _spriteType; }
        private set {
            if (image != null)
                _spriteType = value;
                image.sprite = GetHexSprite(value);
            }
    }
    public Vector3 position { get { return image?.transform.position ?? Vector3.zero; } }
    public HexPosition hexPosition { get; private set; }
    private Image image;
    private float colorValue = 1;

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

    public Hex(HexPosition i_pos, Image i_img)
    {
        hexPosition = i_pos;
        image = i_img;
        colorcode = ColorCode.White;
        spriteType = HexSpriteType.Available;        
    }
    public Hex(HexPosition i_pos, Image i_img, ColorCode i_clr)
    {
        hexPosition = i_pos;
        image = i_img;
        colorcode = i_clr;
        spriteType = HexSpriteType.Normal;
        image.color = GetColorByColorcode(colorcode);
    }
    public Hex(HexPosition i_pos, Image i_img, ColorCode i_clr, HexSpriteType hst) 
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
}
