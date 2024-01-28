using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugHome : DebugPage
{
    public override string Header
    {
        get
        {
            return "Home";
        }
    }

    protected override void RunItems(DebugMenu caller)
    {
        if (Button("Restart"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        foreach (PlayerManager i in UnityEngine.Object.FindObjectsOfType<PlayerManager>())
        {
            Rigidbody2D rigidbody = i.GetComponent<Rigidbody2D>();

            ReadOnly("Player " + i.name);
            ReadOnly("* HP: " + i.health);
            ReadOnly("* Velocity: " + rigidbody.velocity);
            ReadOnly("* Ang Vel: " + rigidbody.angularVelocity);
            ReadOnly("* Gravity: " + rigidbody.gravityScale);

            if (Button("Damage"))
            {
                i.DealDamage(25);
            }

            Separator();
        }
    }
}