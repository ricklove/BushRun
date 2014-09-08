using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;

public class GuiView : MonoBehaviour
{
    public GUIStyle choiceStyle = GUI.skin.box;
    public GUIStyle selectedChoiceStyle = GUI.skin.box;

    public IGuiViewModel GuiViewModel { get; set; }

    private Dictionary<GuiSizeHash, int> _fontSizes;

    void Start()
    {
        _fontSizes = new Dictionary<GuiSizeHash, int>();
    }

    void OnGUI()
    {
        foreach (var item in GuiViewModel.GuiItems)
        {
            var fontSize = _fontSizes.ContainsKey(item.GuiSizeHash) ? _fontSizes[item.GuiSizeHash] : CalculateFontSize(item.GuiSizeHash);
            var style = GetStyle(item.StyleType);
            style.fontSize = fontSize;

            if (item.ClickCallback != null)
            {
                if (GUI.Button(new Rect(item.Left, item.Top, item.Width, item.Height), item.Text, style))
                {
                    item.ClickCallback();
                }
            }
            else
            {
                GUI.Box(new Rect(item.Left, item.Top, item.Width, item.Height), item.Text, style);
            }
        }
    }

    private int CalculateFontSize(GuiSizeHash item)
    {
        var width = item.Width;
        var height = item.Height;

        var style = GetStyle(item.StyleType);

        style.fontSize = (int)(height * 0.6f);

        // Reduce font size if needed
        var longestWord = item.Text.Split(' ').Where(w => w.Trim().Length > 0).OrderByDescending(w => w.Trim().Length).Select(w => w.Trim()).First();
        longestWord = "w" + longestWord + "w";
        var wContent = new GUIContent(longestWord);
        var content = new GUIContent(item.Text);

        var mSize = style.CalcSize(wContent);
        var mHeight = style.CalcHeight(content, width);

        while ((mSize.x > width * 0.85f)
            || (mHeight > height * 0.85f))
        {
            var diffRatio = mHeight / (height * 0.9f);
            var reduce = 1 / diffRatio;
            var halfReduce = (1 + reduce) / 2;
            var ratio = Math.Min(halfReduce, 0.8f);

            style.fontSize = (int)(style.fontSize * ratio);

            mSize = style.CalcSize(wContent);
            mHeight = style.CalcHeight(content, width);
        }

        _fontSizes[item] = style.fontSize;

        return _fontSizes[item];
    }

    private GUIStyle GetStyle(GuiStyleType styleType)
    {
        var style = choiceStyle;

        switch (styleType)
        {
            case GuiStyleType.Choice:
                style = choiceStyle;
                break;
            case GuiStyleType.Choice_Highlight:
                style = selectedChoiceStyle;
                break;
            default:
                break;
        }
        return style;
    }
}

public interface IGuiViewModel
{
    IList<GuiItem> GuiItems { get; }
}

public class GuiItem
{
    public string Text { get; set; }
    public GuiStyleType StyleType { get; set; }
    public float Left { get; set; }
    public float Top { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public Action ClickCallback { get; set; }

    public GuiSizeHash GuiSizeHash
    {
        get
        {
            return new GuiSizeHash() { Text = Text, StyleType = StyleType, Height = Height, Width = Width };
        }
    }
}

public struct GuiSizeHash
{
    public string Text { get; set; }
    public GuiStyleType StyleType { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }

    public override int GetHashCode()
    {
        return Text.GetHashCode()
            ^ StyleType.GetHashCode()
            ^ Width.GetHashCode()
            ^ Height.GetHashCode();
    }
}

public enum GuiStyleType
{
    Choice,
    Choice_Highlight
}