﻿using HoloToolkit;

using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// HandsManager keeps track of when a hand is detected.
/// </summary>
public class HandsManager : Singleton<HandsManager>
{
    public UnityEvent GrabEvent;

    [Tooltip("Audio clip to play when Finger Pressed.")]
    public AudioClip FingerPressedSound;
    private AudioSource audioSource;

    /// <summary>
    /// Tracks the hand detected state.
    /// </summary>
    public bool HandDetected
    {
        get;
        private set;
    }

    // Keeps track of the GameObject that the hand is interacting with.
    public GameObject FocusedGameObject { get; private set; }

    void Awake()
    {
        EnableAudioHapticFeedback();

        UnityEngine.XR.WSA.Input.InteractionManager.InteractionSourceDetectedLegacy += InteractionManager_SourceDetected;
        UnityEngine.XR.WSA.Input.InteractionManager.InteractionSourceLostLegacy += InteractionManager_SourceLost;

        /* TODO: DEVELOPER CODE ALONG 2.a */

        // 2.a: Register for SourceManager.SourcePressed event.
        UnityEngine.XR.WSA.Input.InteractionManager.InteractionSourcePressedLegacy += InteractionManager_SourcePressed;

        // 2.a: Register for SourceManager.SourceReleased event.
        UnityEngine.XR.WSA.Input.InteractionManager.InteractionSourceReleasedLegacy += InteractionManager_SourceReleased;

        // 2.a: Initialize FocusedGameObject as null.
        FocusedGameObject = null;
    }

    private void EnableAudioHapticFeedback()
    {
        // If this hologram has an audio clip, add an AudioSource with this clip.
        if (FingerPressedSound != null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.clip = FingerPressedSound;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1;
            audioSource.dopplerLevel = 0;
        }
    }

    private void InteractionManager_SourceDetected(UnityEngine.XR.WSA.Input.InteractionSourceState hand)
    {
        HandDetected = true;
    }

    private void InteractionManager_SourceLost(UnityEngine.XR.WSA.Input.InteractionSourceState hand)
    {
        HandDetected = false;

        // 2.a: Reset FocusedGameObject.
        ResetFocusedGameObject();
    }

    private void InteractionManager_SourcePressed(UnityEngine.XR.WSA.Input.InteractionSourceState hand)
    {


        if (InteractibleManager.Instance.FocusedGameObject != null)
        {

           // print("PINCHED---------------------");
            GrabEvent.Invoke();


            // Play a select sound if we have an audio source and are not targeting an asset with a select sound.
            if (audioSource != null && !audioSource.isPlaying &&
                (InteractibleManager.Instance.FocusedGameObject.GetComponent<Interactible>() != null &&
                InteractibleManager.Instance.FocusedGameObject.GetComponent<Interactible>().TargetFeedbackSound == null))
            {
                audioSource.Play();
            }

            // 2.a: Cache InteractibleManager's FocusedGameObject in FocusedGameObject.
            FocusedGameObject = InteractibleManager.Instance.FocusedGameObject;
        }
    }

    private void InteractionManager_SourceReleased(UnityEngine.XR.WSA.Input.InteractionSourceState hand)
    {
        // 2.a: Reset FocusedGameObject.
        ResetFocusedGameObject();
    }

    private void ResetFocusedGameObject()
    {
        // 2.a: Set FocusedGameObject to be null.
        FocusedGameObject = null;

        // 2.a: On GestureManager call ResetGestureRecognizers
        // to complete any currently active gestures.
        GestureManager.Instance.ResetGestureRecognizers();
    }

    void OnDestroy()
    {
        UnityEngine.XR.WSA.Input.InteractionManager.InteractionSourceDetectedLegacy -= InteractionManager_SourceDetected;
        UnityEngine.XR.WSA.Input.InteractionManager.InteractionSourceLostLegacy -= InteractionManager_SourceLost;

        // 2.a: Unregister the SourceManager.SourceReleased event.
        UnityEngine.XR.WSA.Input.InteractionManager.InteractionSourceReleasedLegacy -= InteractionManager_SourceReleased;

        // 2.a: Unregister for SourceManager.SourcePressed event.
        UnityEngine.XR.WSA.Input.InteractionManager.InteractionSourcePressedLegacy -= InteractionManager_SourcePressed;
    }
}