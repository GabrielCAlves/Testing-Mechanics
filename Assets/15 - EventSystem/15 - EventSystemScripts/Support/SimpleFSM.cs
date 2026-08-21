using UnityEngine;

public class SimpleFSM : MonoBehaviour
{
    [Header("References")]
    public Animator animator;

    [Header("Current State")]
    [SerializeField] private string currentState;

    // Constantes para os nomes dos estados (para evitar erros de digitação)
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
        public const string Swim = "Swim";
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

    // Método público para mudar de estado
    public void SetState(string newState)
    {
        if (string.IsNullOrEmpty(newState) || currentState == newState) // OR || ou123
            return;

        currentState = newState;
        animator.SetTrigger(newState);
    }

    // Métodos públicos para cada estado
    public void SetIdle() => SetState(States.Idle);
    public void SetAttack() => SetState(States.Attack);
    public void SetGuard() => SetState(States.Guard);
    public void SetParry() => SetState(States.Parry);
    public void SetTakeDamage() => SetState(States.TakeDamage);
    public void SetWalkForward() => SetState(States.WalkForward);
    public void SetWalkBackward() => SetState(States.WalkBackward);
    public void SetWalkToRight() => SetState(States.WalkToRight);
    public void SetWalkToLeft() => SetState(States.WalkToLeft);
    public void SetRun() => SetState(States.Run);
    public void SetJump() => SetState(States.Jump);
    public void SetFall() => SetState(States.Fall);
    public void SetDie() => SetState(States.Die);
    public void SetReload() => SetState(States.Reload);
    public void SetClimbUp() => SetState(States.ClimbUp);
    public void SetClimbDown() => SetState(States.ClimbDown);
    public void SetClimbToRight() => SetState(States.ClimbToRight);
    public void SetClimbToLeft() => SetState(States.ClimbToLeft);
    public void SetGoUpTheStairs() => SetState(States.GoUpTheStairs);
    public void SetGoDownTheStairs() => SetState(States.GoDownTheStairs);
    public void SetSquat() => SetState(States.Squat);
    public void SetPrepareToShoot() => SetState(States.PrepareToShoot);
    public void SetAim() => SetState(States.Aim);
    public void SetSingleShot() => SetState(States.SingleShot);
    public void SetBurstShot() => SetState(States.BurstShot);
    public void SetAutoShot() => SetState(States.AutoShot);
    public void SetWalkFrontShoot() => SetState(States.WalkFrontShoot);
    public void SetWalkLeftShoot() => SetState(States.WalkLeftShoot);
    public void SetWalkRightShoot() => SetState(States.WalkRightShoot);
    public void SetWalkBackShoot() => SetState(States.WalkBackShoot);
    public void SetRunWithGun() => SetState(States.RunWithGun);
    public void SetRunWithWeapon() => SetState(States.RunWithWeapon);
    public void SetHipnotized() => SetState(States.Hipnotized);
    public void SetDistracted() => SetState(States.Distracted);
    public void SetPoisoned() => SetState(States.Poisoned);
    public void SetBurned() => SetState(States.Burned);
    public void SetFreezed() => SetState(States.Freezed);
    public void SetFly() => SetState(States.Fly);
    public void SetSwim() => SetState(States.Swim);
    public void SetTeleport() => SetState(States.Teleport);

    // Estados adicionais
    public void SetCrouch() => SetState(States.Crouch);
    public void SetSlide() => SetState(States.Slide);
    public void SetRoll() => SetState(States.Roll);
    public void SetSprint() => SetState(States.Sprint);
    public void SetAimDownSight() => SetState(States.AimDownSight);
    public void SetMeleeAttack() => SetState(States.MeleeAttack);
    public void SetGrenadeThrow() => SetState(States.GrenadeThrow);
    public void SetHeal() => SetState(States.Heal);
    public void SetInteract() => SetState(States.Interact);
    public void SetStun() => SetState(States.Stun);

    public string GetCurrentState()
    {
        return currentState;
    }
}