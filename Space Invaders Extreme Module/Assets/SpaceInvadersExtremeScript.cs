using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class SpaceInvadersExtremeScript : MonoBehaviour {

    public KMNeedyModule Module;
    static int _moduleIdCounter = 1;
    int _moduleID = 0;
    public GameObject Display;
    public KMAudio Audio;
    public Sprite[] One;
    public Sprite[] Two;
    public Sprite[] ThreeA;
    public Sprite[] ThreeB;
    public Sprite[] FourA;
    public Sprite[] FourB;
    public Sprite[] FourC;
    public Sprite[] FiveA;
    public Sprite[] FiveB;
    public Sprite[] FiveC;
    public Sprite[] FiveD;
    public Sprite[] Screensaver;
    public SpriteRenderer Renderer;
    public KMSelectable[] Buttons;
    private bool NoAnswer;
    public TextMesh Text;
    private Sprite[] RandomArray;
    private int Current;
    private bool Active;
    private readonly string[] Names = { "STAGE 1", "STAGE 2", "STAGE 3A", "STAGE 3B", "STAGE 4A", "STAGE 4B", "STAGE 4C", "STAGE 5A", "STAGE 5B", "STAGE 5C", "STAGE 5D" };
    private Sprite[][] SpriteArrays = new Sprite[11][];
    private KMSelectable.OnInteractHandler ButtonPressed(int pos)
    {
        return delegate
        {
            Buttons[pos].AddInteractionPunch(0.5f);
            switch (pos)
            {
                case 0:
                    if (Active)
                    {
                        Audio.PlaySoundAtTransform("press", transform);
                        Current = (Current + 10) % 11;
                        Text.text = Names[Current];
                    }
                    break;

                case 1:
                    if (Active)
                    {
                        Audio.PlaySoundAtTransform("press", transform);
                        Current = (Current + 1) % 11;
                        Text.text = Names[Current];
                    }
                    break;

                default:
                    if (Active)
                    {
                        NoAnswer = false;
                        if (SpriteArrays[Current][0] == RandomArray[0])
                        {
                            Audio.PlaySoundAtTransform("disarm", transform);
                            Module.HandlePass();
                        }
                        else
                        {
                            Debug.LogFormat("[Space Invaders Extreme #{0}] {1} is not the displayed background. Strike!", _moduleID, Names[Current]);
                            Module.HandleStrike();
                            Module.HandlePass();
                        }
                    }
                    break;
            }
            return false;
        };
    }

    void Awake()
    {
        for (int i = 0; i < Buttons.Length; i++)
        {
            Buttons[i].OnInteract += ButtonPressed(i);
        }
        _moduleID = _moduleIdCounter++;
        Text.text = "";
        SpriteArrays = new Sprite[][]
    {
        One,
        Two,
        ThreeA,
        ThreeB,
        FourA,
        FourB,
        FourC,
        FiveA,
        FiveB,
        FiveC,
        FiveD,
        Screensaver
    };
        Module.OnActivate += delegate { StartCoroutine(Gif()); };
        Module.OnNeedyActivation += delegate { NoAnswer = true;  Active = true; Current = 0; Activate(); };
        Module.OnNeedyDeactivation += delegate { Active = false;  Deactivate(); };
    }

    void Update()
    {
        
    }

    private IEnumerator Gif()
    {
        RandomArray = SpriteArrays[11];
        while (true)
        {
            for (int i = 0; i < RandomArray.Length; i++)
            {
                Renderer.sprite = RandomArray[i];
                yield return new WaitForSeconds(0.03f);
            }
        }
    }

    void Activate()
    {
        RandomArray = SpriteArrays[Rnd.Range(0,11)];
        Text.text = Names[0];
    }

    void Deactivate()
    {
        RandomArray = SpriteArrays[11];
        Text.text = "";
        if (NoAnswer == true)
        {
            Module.HandleStrike();
            NoAnswer = true;
        }
    }

#pragma warning disable 414
    private string TwitchHelpMessage = "Use '!{0} 3B' to enter STAGE 3B into the module.";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant();
        string[] validcmds = { "1", "2", "3a", "3b", "4a", "4b", "4c", "5a", "5b", "5c", "5d" };
        if (!validcmds.Contains(command))
        {
            yield return "sendtochaterror Invalid command.";
            yield break;
        }
        yield return null;
        while (validcmds[Current] != command)
        {
            Buttons[1].OnInteract();
            yield return null;
        }
        Buttons[2].OnInteract();
    }
}
