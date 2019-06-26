# ClipboardMate
A simple application that can track your clipboard, quickly copy with keybinds, and cycle through clipboards.

## Scripts
Script files run from Documents/CopyPaster/

A script must contain a `Script_Start` function with one string array parameter.

An example script:
```cs
using System;
using System.Drawing;
using System.Windows;

public class CopyImage
{
    public void Script_Start(string[] args)
    {
        Clipboard.SetData(DataFormats.Bitmap, new Bitmap(args[0]));
    }
}
```
