public interface IPlayerState
{
    void Enter(PlayerStateController player);   // qué pasa al entrar en este estado
    void Exit(PlayerStateController player);    // cleanup al salir del estado
}
