public interface IPlayerState
{
    void Enter(PlayerStateController player);   // qué pasa al entrar en este estado
    void Update(PlayerStateController player);  // qué pasa en cada frame (si aplica)
    void Exit(PlayerStateController player);    // cleanup al salir del estado
}
