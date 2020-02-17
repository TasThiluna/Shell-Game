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
	private static readonly Vector3[] defaultCupPositions = new[] { new Vector3(-0.056f, 0.0347f, 0.0187428f), new Vector3(0, 0.0347f, 0.0187428f), new Vector3(0.056f, 0.0347f, 0.0187428f) };
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
		startingCup = rnd.Range(0, 3);
		pearl.SetParent(cups[startingCup], false);
		Debug.LogFormat("[Shell Game #{0}] The pearl is under the {1} cup.", moduleId, positionNames[startingCup]);
		pearl.SetParent(defaultPosition, true);
		for (int i = 0; i < 10; i++)
			rotations[i] = rnd.Range(0, 3);
		StartCoroutine(RiseCups());
	}

	IEnumerator RiseCups()
	{
		yield return new WaitForSeconds(3f);
		var elapsed = 0f;
		var duration = 1f;
		while (elapsed < duration)
		{
			for (int i = 0; i < cups.Length; i++)
				cups[i].localPosition = new Vector3(
					defaultCupPositions[i].x,
					Mathf.Lerp(defaultCupPositions[i].y, .0765f, elapsed / duration),
					defaultCupPositions[i].z
				);
			yield return null;
			elapsed += Time.deltaTime;
		}
		for (int i = 0; i < cups.Length; i++)
			cups[i].localPosition = new Vector3(defaultCupPositions[i].x, .0765f, defaultCupPositions[i].z);

		yield return new WaitForSeconds(2f);
		elapsed = 0f;
		while (elapsed < duration)
		{
			for (int i = 0; i < cups.Length; i++)
				cups[i].localPosition = new Vector3(
					defaultCupPositions[i].x,
					Mathf.Lerp(.0765f, defaultCupPositions[i].y, elapsed / duration),
					defaultCupPositions[i].z
				);

			yield return null;
			elapsed += Time.deltaTime;
		}
		for (int i = 0; i < cups.Length; i++)
			cups[i].localPosition = defaultCupPositions[i];
		pearl.SetParent(cups[startingCup], true);
		if (!hasRotated)
			StartCoroutine(RotateCups());
	}

	IEnumerator RotateCups()
	{
		var rotationNames = new string[3] { "LM", "LR", "MR" };
		for (int i = 0; i < 10; i++)
		{
			foreach (int ix in cupsToRotate[rotations[i]])
				cups[ix].SetParent(pivots[rotations[i]], true);
			var endRotation = Quaternion.Euler(0f, 180f, 0f);
			var elapsed = 0f;
			var duration = .5f;
			while (elapsed < duration)
			{
				pivots[rotations[i]].localRotation = Quaternion.Slerp(Quaternion.identity, endRotation, elapsed / duration);
				yield return null;
				elapsed += Time.deltaTime;
			}
			pivots[rotations[i]].localRotation = endRotation;
			foreach (int ix in cupsToRotate[rotations[i]])
				cups[ix].SetParent(defaultPosition, true);
			pivots[rotations[i]].localRotation = Quaternion.identity;

			var t = cups[cupsToRotate[rotations[i]][0]];
			cups[cupsToRotate[rotations[i]][0]] = cups[cupsToRotate[rotations[i]][1]];
			cups[cupsToRotate[rotations[i]][1]] = t;

			yield return new WaitForSeconds(.25f);
		}
		hasRotated = true;
		pearl.SetParent(defaultPosition, true);
		StartCoroutine(RiseCups());
	}
}
