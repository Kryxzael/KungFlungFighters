using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

[RequireComponent(typeof(SpriteAnimator))]
public class Mimicker : MonoBehaviour
{
	private const float KEYFRAME_INTERVAL_SECONDS = 0.05f;

	/* *** */

	public SpriteAnimator Target;
	private SpriteAnimation _lastKnownPlayerAnimation;

	/* *** */

	public Queue<PositionSnapshot> KeyFrames = new Queue<PositionSnapshot>();
	public float Delay = 1f;

	private void OnEnable()
	{
		StartCoroutine(PerformMimic());
	}

	private IEnumerator PerformMimic()
	{
		var animator = GetComponent<SpriteAnimator>();
		var flippable = GetComponent<Flippable>();

		/*
		 * Begin pulse
		 */

		InvokeRepeating(nameof(CreateKeyFrame), time: 0f, repeatRate: KEYFRAME_INTERVAL_SECONDS);

		/*
		 * Loop
		 */
		while (true)
		{
			var keyframe = FindPointOnTimeline(DateTime.Now.AddSeconds(-Delay));


			if (keyframe == null)
			{
				yield return new WaitForFixedUpdate();
				continue;
			}

			var (position, rotation, snapshot) = keyframe.Value;

			transform.position = position;
			transform.rotation = rotation;
			animator.Animation = snapshot.Animation;

			yield return new WaitForFixedUpdate();
		}
	}

	/// <summary>
	/// Gets the position and snapshot at the provided real-time
	/// </summary>
	/// <param name="time"></param>
	/// <returns></returns>
	private (Vector2 position, Quaternion rotation, PositionSnapshot snapshot)? FindPointOnTimeline(DateTime time)
	{
		if (!KeyFrames.Any())
			return null;

		PositionSnapshot last = default;
		foreach (var i in KeyFrames)
		{
			if (i.Time > time)
			{
				if (last.Time == default)
					return null;

				float t;

				t = Mathf.InverseLerp(
					a: (float)(last.Time - DateTime.Today).TotalSeconds, 
					b: (float)(i.Time - DateTime.Today).TotalSeconds, 
					value: (float)(time - DateTime.Today).TotalSeconds
				);

				return (Vector2.Lerp(last.Position, i.Position, t), Quaternion.Lerp(last.Rotation, i.Rotation, t), last);
			}

			last = i;
		}

		return (last.Position, last.Rotation, last);
	}

	private void Update()
	{
		//Create keyframe if the player's sprite changes
		if (Target && Target.Animation != _lastKnownPlayerAnimation)
		{
			CreateKeyFrame();
            _lastKnownPlayerAnimation = Target.Animation;
		}
			

	}

	/// <summary>
	/// Creates a new keyframe of the player
	/// </summary>
	private void CreateKeyFrame()
	{
		if (Target)
			KeyFrames.Enqueue(PositionSnapshot.FromObjects(Target));

		if (KeyFrames.Count > 500)
			KeyFrames.Dequeue();
	}
}