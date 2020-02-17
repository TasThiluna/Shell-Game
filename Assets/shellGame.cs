using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using rnd = UnityEngine.Random;

public class shellGame : MonoBehaviour
{
    public new KMAudio audio;
    public KMBombInfo bomb;
	public KMBombModule module;

	public Transform[] cups;
	public Transform[] pivots;
	public Transform pearl;
	public Transform defaultPosition;

	private int startingCup;
	private int[] rotations = new int[10];

	private static readonly int[][] cupsToRotate = new int[3][] {
		new int[2] { 0, 1 },
		new int[2] { 0, 2 },
		new int[2] { 1, 2 }
	};
	private static readonly string[] positionNames = new string[3] { "left", "middle", "right" };
	private bool hasRotated;

    private static int moduleIdCounter = 1;
    private int moduleId;
    private bool moduleSolved;

    void Awake()
    {
    	moduleId = moduleIdCounter++;
    }

    void Start()
    {
		startingCup = rnd.Range(0,3);
		pearl.SetParent(cups[startingCup], false);
		Debug.LogFormat("[Shell Game #{0}] The pearl is under the {1} cup.", moduleId, positionNames[startingCup]);
		pearl.SetParent(defaultPosition, true);
		for (int i = 0; i < 10; i++)
			rotations[i] = rnd.Range(0,3);
		foreach (Transform cup in cups)
			StartCoroutine(RiseCup(cup));
    }

	IEnumerator RiseCup(Transform cup)
	{
		yield return new WaitForSeconds(3f);
		var elapsed = 0f;
		var duration = 1f;
		var startPosition = cup.localPosition;
		while (elapsed < duration)
		{
			cup.localPosition = new Vector3(
				startPosition.x,
				Mathf.Lerp(startPosition.y, .0765f, elapsed / duration),
				startPosition.z
			);
			yield return null;
			elapsed += Time.deltaTime;
		}
		yield return new WaitForSeconds(2f);
		var endPosition = cup.localPosition;
		elapsed = 0f;
		while (elapsed < duration)
		{
			cup.localPosition = new Vector3(
				startPosition.x,
				Mathf.Lerp(endPosition.y, startPosition.y, elapsed / duration),
				startPosition.z
			);
			yield return null;
			elapsed += Time.deltaTime;
		}
		cup.localPosition = startPosition;
		pearl.SetParent(cups[startingCup], true);
		if (Array.IndexOf(cups, cup) == 0 && !hasRotated)
			StartCoroutine(RotateCups());
	}

	IEnumerator RotateCups()
	{
		var rotationNames = new string[3] { "LM", "LR", "MR" };
		for (int i = 0; i < 10; i++)
		{
			foreach (int ix in cupsToRotate[rotations[i]])
			{
				cups[ix].SetParent(pivots[rotations[i]], true);
				Debug.LogFormat("[Shell Game #{0}] {1} {2}", moduleId, rotationNames[rotations[i]], ix);
			}
			var endRotation = Quaternion.Euler(0f, 180f, 0f);
			var elapsed = 0f;
			var duration = .5f;
			while (elapsed < duration)
			{
				pivots[rotations[i]].localRotation = Quaternion.Slerp(Quaternion.identity, endRotation, elapsed / duration);
				yield return new WaitForSeconds(.5f);
				elapsed += Time.deltaTime;
			}
			pivots[rotations[i]].localRotation = endRotation;
			foreach (int ix in cupsToRotate[rotations[i]])
				cups[ix].SetParent(defaultPosition, true);
			pivots[rotations[i]].localRotation = Quaternion.identity;
			yield return new WaitForSeconds(.25f);
		}
		hasRotated = true;
		pearl.SetParent(defaultPosition, true);
		foreach (Transform cup in cups)
			StartCoroutine(RiseCup(cup));
	}
}
