namespace CyberpixelOk.Interactions
{
    public interface IInteractable
    {
        string InteractionLabel { get; }

        bool CanInteract(InteractorContext context);

        void Interact(InteractorContext context);
    }
}
