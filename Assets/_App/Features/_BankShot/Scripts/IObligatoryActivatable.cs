namespace DigitalLove.Game.BankShot
{
    public interface IObligatoryActivatable
    {
        bool IsObligatory { get; }
        bool ActivatedThisThrow { get; }

        void Configure(bool isObligatory);
        void ResetForThrow();
        void RegisterActivation();
    }
}
