using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AvatarAnimator : MonoBehaviour
{
    public SpriteAnimation idle;
    public SpriteAnimation attackRight;
    public SpriteAnimation attackLeft;
    public SpriteAnimation jump;
    public SpriteAnimation hit;

    private SpriteAnimator animator;
    private PlayerManager player;

    private void Awake()
    {
        animator = GetComponent<SpriteAnimator>();
        player = GetComponent<PlayerManager>();
    }

    private void Update()
    {
        SpriteAnimation animation;

        switch (player.currentAction)
        {
            case PlayerManager.CurrentAction.Jump:
                animation = jump;
                break;
            case PlayerManager.CurrentAction.AttackLeft:
                animation = attackLeft;
                break;
            case PlayerManager.CurrentAction.AttackRight:
                animation = attackRight;
                break;
            case PlayerManager.CurrentAction.Stunned:
                animation = hit;
                break;
            default:
                animation = idle;
                break;
        }

        animator.Animation = animation;
    }
}