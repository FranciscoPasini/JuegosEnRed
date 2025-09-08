public class EliminatedState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        player.gameObject.SetActive(false);
    }

    public void Update(PlayerController player) { }

    public void Exit(PlayerController player) { }
}