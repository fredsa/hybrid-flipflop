﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class FlipFlip : MonoBehaviour {

	IEnumerator Start () {
		Debug.LogFormat ("Frame #{0}: XRSettings.supportedDevices=={{\"{1}}} // XR Devices supports on this device.", Time.frameCount, string.Join (", ", XRSettings.supportedDevices));

		int attempts = 0;
		int successes = 0;
		int failures = 0;

		while (true) {
			attempts++;
			Debug.LogFormat ("Frame #{0}: \n\n------------------------------", Time.frameCount);
			Debug.LogFormat ("Frame #{0}: Iteration # {1}", Time.frameCount, attempts);
			Debug.LogFormat ("Frame #{0}: XRSettings.loadedDeviceName==\"{1}\" // Currently loaded device name.", Time.frameCount, XRSettings.loadedDeviceName);
			Debug.LogFormat ("Frame #{0}: XRSettings.enabled=={1} // VR is {2}.", Time.frameCount, XRSettings.enabled, XRSettings.enabled ? "enabled" : "disabled");

			string previousDeviceName = XRSettings.loadedDeviceName;
			string desiredDeviceName = GetCanonicalDeviceName (GetNextSupportedDeviceName ());

			yield return SwitchTo (desiredDeviceName);

			string newDeviceName = XRSettings.loadedDeviceName;

			bool success = IsSameDevice (XRSettings.loadedDeviceName, desiredDeviceName) && XRSettings.enabled == IsVRDevice (desiredDeviceName);
			if (!success) {
				Debug.LogFormat ("***********************************************************************");
				Debug.LogFormat ("***********************************************************************");
				Debug.LogFormat ("***********************************************************************");
				Debug.LogFormat ("***********************************************************************");
			}
			Debug.LogFormat ("Frame #{0}: RESULT=={1} switching \"{2}\" => \"{3}\"", Time.frameCount, success ? "success" : "FAILURE", previousDeviceName, success ? newDeviceName : desiredDeviceName);
			if (!success) {
				Debug.LogFormat ("***********************************************************************");
				Debug.LogFormat ("***********************************************************************");
				Debug.LogFormat ("***********************************************************************");
				Debug.LogFormat ("***********************************************************************");
			}

			if (success) {
				successes++;
			} else {
				failures++;
			}
			float percentFail = (float) failures / attempts * 100;
			Debug.LogFormat ("Frame #{0}: TOTALS: percentFail={1:.0}%, attempts=={2}, successes=={3}, failures=={4}", Time.frameCount, percentFail, attempts, successes, failures);
		}
	}

	IEnumerator SwitchTo (string desiredDeviceName) {
		Debug.LogFormat ("Frame #{0}: XRSettings.LoadDeviceByName(\"{1}\"); // Load new device async.", Time.frameCount, desiredDeviceName);
		XRSettings.LoadDeviceByName (desiredDeviceName);

		Debug.LogFormat ("Frame #{0}: yield return null; // Wait one frame.", Time.frameCount);
		yield return null;

		Debug.LogFormat ("Frame #{0}: XRSettings.loadedDeviceName: \"{1}\" // Currently loaded device name.", Time.frameCount, XRSettings.loadedDeviceName);

		bool shouldEnable = IsVRDevice (desiredDeviceName);
		if (XRSettings.enabled != shouldEnable) {
			Debug.LogFormat ("Frame #{0}: XRSettings.enabled: {1} -> {2} // {3} VR.", Time.frameCount, XRSettings.enabled, shouldEnable, shouldEnable ? "Enable" : "Disable");
			XRSettings.enabled = shouldEnable;
		}
	}

	private bool IsVRDevice (string deviceName) {
		return GetCanonicalDeviceName (deviceName).Length > 0;
	}

	private string GetNextSupportedDeviceName () {
		foreach (string deviceName in XRSettings.supportedDevices) {
			if (!IsSameDevice (deviceName, XRSettings.loadedDeviceName)) {
				return deviceName;
			}
		}
		throw new Exception ("Frame #{0}: FIX ME: Unable to determine next XR device SDK to load.");
	}

	private bool IsSameDevice (string deviceName1, string deviceName2) {
		deviceName1 = GetCanonicalDeviceName (deviceName1);
		deviceName2 = GetCanonicalDeviceName (deviceName2);
		return string.Compare (deviceName1, XRSettings.loadedDeviceName, true) == 0;
	}

	private string GetCanonicalDeviceName (string deviceName) {
		deviceName = deviceName.ToLower ();
		return deviceName.Equals ("none") ? "" : deviceName;
	}
}