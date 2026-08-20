using UnityEngine;

public class EventDrivenFSM : MonoBehaviour
{
    [Header("References")]
    public Animator animator;

    [Header("State Events")]
    public GameEvent idleEvent;
    public GameEvent attackEvent;
    public GameEvent guardEvent;
    public GameEvent parryEvent;
    public GameEvent takeDamageEvent;
    public GameEvent walkForwardEvent;
    public GameEvent walkBackwardEvent;
    public GameEvent walkToRightEvent;
    public GameEvent walkToLeftEvent;
    public GameEvent runEvent;
    public GameEvent jumpEvent;
    public GameEvent fallEvent;
    public GameEvent dieEvent;
    public GameEvent reloadEvent;
    public GameEvent climbUpEvent;
    public GameEvent climbDownEvent;
    public GameEvent climbToRightEvent;
    public GameEvent climbToLeftEvent;
    public GameEvent goUpTheStairsEvent;
    public GameEvent goDownTheStairsEvent;
    public GameEvent squatEvent;
    public GameEvent prepareToShootEvent;
    public GameEvent aimEvent;
    public GameEvent singleShotEvent;
    public GameEvent burstShotEvent;
    public GameEvent autoShotEvent;
    public GameEvent walkFrontShootEvent;
    public GameEvent walkLeftShootEvent;
    public GameEvent walkRightShootEvent;
    public GameEvent walkBackShootEvent;
    public GameEvent runWithGunEvent;
    public GameEvent runWithWeaponEvent;
    public GameEvent hipnotizedEvent;
    public GameEvent distractedEvent;
    public GameEvent poisonedEvent;
    public GameEvent burnedEvent;
    public GameEvent freezedEvent;
    public GameEvent flyEvent;
    public GameEvent teleportEvent;

    // 10 eventos adicionais
    public GameEvent crouchEvent;
    public GameEvent slideEvent;
    public GameEvent rollEvent;
    public GameEvent sprintEvent;
    public GameEvent aimDownSightEvent;
    public GameEvent meleeAttackEvent;
    public GameEvent grenadeThrowEvent;
    public GameEvent healEvent;
    public GameEvent interactEvent;
    public GameEvent stunEvent;

    [Header("Current State")]
    [SerializeField] private string currentState;

    // Constantes para os nomes dos estados
    public static class States
    {
        public const string Idle = "Idle";
        public const string Attack = "Attack";
        public const string Guard = "Guard";
        public const string Parry = "Parry";
        public const string TakeDamage = "TakeDamage";
        public const string WalkForward = "WalkForward";
        public const string WalkBackward = "WalkBackward";
        public const string WalkToRight = "WalkToRight";
        public const string WalkToLeft = "WalkToLeft";
        public const string Run = "Run";
        public const string Jump = "Jump";
        public const string Fall = "Fall";
        public const string Die = "Die";
        public const string Reload = "Reload";
        public const string ClimbUp = "ClimbUp";
        public const string ClimbDown = "ClimbDown";
        public const string ClimbToRight = "ClimbToRight";
        public const string ClimbToLeft = "ClimbToLeft";
        public const string GoUpTheStairs = "GoUpTheStairs";
        public const string GoDownTheStairs = "GoDownTheStairs";
        public const string Squat = "Squat";
        public const string PrepareToShoot = "PrepareToShoot";
        public const string Aim = "Aim";
        public const string SingleShot = "SingleShot";
        public const string BurstShot = "BurstShot";
        public const string AutoShot = "AutoShot";
        public const string WalkFrontShoot = "WalkFrontShoot";
        public const string WalkLeftShoot = "WalkLeftShoot";
        public const string WalkRightShoot = "WalkRightShoot";
        public const string WalkBackShoot = "WalkBackShoot";
        public const string RunWithGun = "RunWithGun";
        public const string RunWithWeapon = "RunWithWeapon";
        public const string Hipnotized = "Hipnotized";
        public const string Distracted = "Distracted";
        public const string Poisoned = "Poisoned";
        public const string Burned = "Burned";
        public const string Freezed = "Freezed";
        public const string Fly = "Fly";
        public const string Teleport = "Teleport";

        // 10 estados adicionais
        public const string Crouch = "Crouch";
        public const string Slide = "Slide";
        public const string Roll = "Roll";
        public const string Sprint = "Sprint";
        public const string AimDownSight = "AimDownSight";
        public const string MeleeAttack = "MeleeAttack";
        public const string GrenadeThrow = "GrenadeThrow";
        public const string Heal = "Heal";
        public const string Interact = "Interact";
        public const string Stun = "Stun";
    }

    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        SetState(States.Idle);
    }

    // Método privado para mudar de estado via GameEvent
    private void SetState(string newState, GameEvent stateEvent = null)
    {
        if (string.IsNullOrEmpty(newState))
            return;

        currentState = newState;
        animator.SetTrigger(newState);

        // Se um evento foi fornecido, dispara-o
        if (stateEvent != null)
            stateEvent.Raise();
    }

    // Métodos públicos que podem ser chamados por outros scripts
    public void SetIdle() => SetState(States.Idle, idleEvent);
    public void SetAttack() => SetState(States.Attack, attackEvent);
    public void SetGuard() => SetState(States.Guard, guardEvent);
    public void SetParry() => SetState(States.Parry, parryEvent);
    public void SetTakeDamage() => SetState(States.TakeDamage, takeDamageEvent);
    public void SetWalkForward() => SetState(States.WalkForward, walkForwardEvent);
    public void SetWalkBackward() => SetState(States.WalkBackward, walkBackwardEvent);
    public void SetWalkToRight() => SetState(States.WalkToRight, walkToRightEvent);
    public void SetWalkToLeft() => SetState(States.WalkToLeft, walkToLeftEvent);
    public void SetRun() => SetState(States.Run, runEvent);
    public void SetJump() => SetState(States.Jump, jumpEvent);
    public void SetFall() => SetState(States.Fall, fallEvent);
    public void SetDie() => SetState(States.Die, dieEvent);
    public void SetReload() => SetState(States.Reload, reloadEvent);
    public void SetClimbUp() => SetState(States.ClimbUp, climbUpEvent);
    public void SetClimbDown() => SetState(States.ClimbDown, climbDownEvent);
    public void SetClimbToRight() => SetState(States.ClimbToRight, climbToRightEvent);
    public void SetClimbToLeft() => SetState(States.ClimbToLeft, climbToLeftEvent);
    public void SetGoUpTheStairs() => SetState(States.GoUpTheStairs, goUpTheStairsEvent);
    public void SetGoDownTheStairs() => SetState(States.GoDownTheStairs, goDownTheStairsEvent);
    public void SetSquat() => SetState(States.Squat, squatEvent);
    public void SetPrepareToShoot() => SetState(States.PrepareToShoot, prepareToShootEvent);
    public void SetAim() => SetState(States.Aim, aimEvent);
    public void SetSingleShot() => SetState(States.SingleShot, singleShotEvent);
    public void SetBurstShot() => SetState(States.BurstShot, burstShotEvent);
    public void SetAutoShot() => SetState(States.AutoShot, autoShotEvent);
    public void SetWalkFrontShoot() => SetState(States.WalkFrontShoot, walkFrontShootEvent);
    public void SetWalkLeftShoot() => SetState(States.WalkLeftShoot, walkLeftShootEvent);
    public void SetWalkRightShoot() => SetState(States.WalkRightShoot, walkRightShootEvent);
    public void SetWalkBackShoot() => SetState(States.WalkBackShoot, walkBackShootEvent);
    public void SetRunWithGun() => SetState(States.RunWithGun, runWithGunEvent);
    public void SetRunWithWeapon() => SetState(States.RunWithWeapon, runWithWeaponEvent);
    public void SetHipnotized() => SetState(States.Hipnotized, hipnotizedEvent);
    public void SetDistracted() => SetState(States.Distracted, distractedEvent);
    public void SetPoisoned() => SetState(States.Poisoned, poisonedEvent);
    public void SetBurned() => SetState(States.Burned, burnedEvent);
    public void SetFreezed() => SetState(States.Freezed, freezedEvent);
    public void SetFly() => SetState(States.Fly, flyEvent);
    public void SetTeleport() => SetState(States.Teleport, teleportEvent);

    // Estados adicionais
    public void SetCrouch() => SetState(States.Crouch, crouchEvent);
    public void SetSlide() => SetState(States.Slide, slideEvent);
    public void SetRoll() => SetState(States.Roll, rollEvent);
    public void SetSprint() => SetState(States.Sprint, sprintEvent);
    public void SetAimDownSight() => SetState(States.AimDownSight, aimDownSightEvent);
    public void SetMeleeAttack() => SetState(States.MeleeAttack, meleeAttackEvent);
    public void SetGrenadeThrow() => SetState(States.GrenadeThrow, grenadeThrowEvent);
    public void SetHeal() => SetState(States.Heal, healEvent);
    public void SetInteract() => SetState(States.Interact, interactEvent);
    public void SetStun() => SetState(States.Stun, stunEvent);

    public string GetCurrentState()
    {
        return currentState;
    }
}