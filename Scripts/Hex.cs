using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ColorCode : byte { White, Red, Green, Blue }
public enum HexSpriteType : byte { Normal, Fixed, Available, Frame }
public struct HexPosition
{
    public byte ringIndex, ringPosition;
    public static readonly HexPosition zer0 = new HexPosition(0, 0);

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
            int x = ringIndex + 1;
            int a = ringPosition / x;
            if (ringPosition % x == 0) return 0;
            else return (byte)(ringPosition - a * x);
        }
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
}
