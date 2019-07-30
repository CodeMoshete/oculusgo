public class CeaseFireAction : CustomAction
{
    public FireWeaponBaseAction WeaponAction;
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
