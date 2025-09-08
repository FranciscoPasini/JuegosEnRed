public interface IPlayerState
{
    void Enter(PlayerController player);   // qué pasa al entrar en este estado
    void Update(PlayerController player);  // qué pasa en cada frame (si aplica)
    void Exit(PlayerController player);    // cleanup al salir del estado
}
