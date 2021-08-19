using Sandbox;
using System;

[Library("tfdm_sandvich", Title = "Sandvich")]

partial class Sandvich : BaseDmWeapon 
{
    public override string ViewModelPath => "";
    public override float PrimaryRate => 0.3f;

    public override void Spawn() 
    {
        base.Spawn();

        SetModel("models/hvwpns/hvywpnspwns/sandvich.vmdl");
        AmmoClip = 1;
    }

    public override void SimulateAnimator(PawnAnimator anim) 
    {
        anim.SetParam("holdtype", 2);
    }
}