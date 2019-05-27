public class CeaseFireAction : CustomAction
{
    public FireWeaponAction WeaponAction;
    public CustomAction OnComplete;

    public override void Initiate()
    {
        base.Initiate();
        WeaponAction.CeaseFire();
        if (OnComplete != null)
        {
            OnComplete.Initiate();
        }
    }
}
