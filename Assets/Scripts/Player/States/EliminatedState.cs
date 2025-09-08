public class EliminatedState : IPlayerState
{
    public void Enter(PlayerStateController player)
    {
        player.gameObject.SetActive(false);
    }

    public void Update(PlayerStateController player) { }

    public void Exit(PlayerStateController player) { }
}