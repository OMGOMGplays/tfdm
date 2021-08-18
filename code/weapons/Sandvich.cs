using Sandbox;
using System;

[Library("tfdm_sandvich", Title = "Sandvich")]

partial class Sandvich : BaseDmWeapon 
{
    public override string ViewModelPath => "hvwpns/hvywpnswpns/sandvich.vmdl";
    public override float PrimaryRate => 0.3f;

    public override void Spawn() 
    {
        base.Spawn();

        SetModel("hvwpns/hvywpnspwns/sandvich.vmdl");
        AmmoClip = 1;
    }

    public override void SimulateAnimator(PawnAnimator anim) 
    {
        anim.SetParam("holdtype", 2);
    }
}