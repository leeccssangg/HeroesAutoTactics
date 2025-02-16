using System.Collections.Generic;
using R3;
using TW.UGUI.MVPPattern;
using TW.UGUI.Core.Sheets;
using UnityEngine;
using UnityEngine.UI;
using TW.UGUI.Core.Views;
using TW.UGUI.Core.Screens;
using System;
using Pextension;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Linq;
using Sirenix.Utilities;
using DG.Tweening;

public class SheetPlayerInfoAvatar : Sheet, ISetupAble
{
    public static class Events
    {

    }

    public override UniTask Initialize(Memory<object> args)
    {

        return UniTask.CompletedTask;
    }
    public void Setup()
    {

    }
}
