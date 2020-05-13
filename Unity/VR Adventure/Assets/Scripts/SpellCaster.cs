using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


public enum Spell { None, Fireball, ForcePush }

public class SpellCaster : MonoBehaviour {
    public HandPresence LeftController;
    public HandPresence RightController;


    private bool _isPressingRight = false;
    private bool _isPressingLeft = false;
    private bool castingSpell = false;

    public GameObject FireballPrefab;
    private CharacterController _characterController = null;
    private Fireball _castFireball;
    private Spell _currentSpell = Spell.None;


    private float _forcePushPower = 100.0f;
    private float _forcePushChargeupTime = 1.0f;
    private float _forcePushCharge = 0.0f;

    private Vector3 _pointBetween {
        get {
            return LeftController.Hand.IndexFinger.position + (RightController.Hand.IndexFinger.position - LeftController.Hand.IndexFinger.position) * .5f;
        }
    }


    void Start () {
        _characterController = GetComponent<CharacterController>();
    }

    void Update () {
        CheckForSpellcast();
        if (castingSpell) {
            switch (_currentSpell) {

                case Spell.Fireball:
                    _castFireball.transform.position = _pointBetween;
                    float distance = Vector3.Distance(RightController.Hand.IndexFinger.position, LeftController.Hand.IndexFinger.position);
                    _castFireball.Size = distance - .01f;

                    break;
                case Spell.ForcePush:
                    ChargeForcePush();
                    break;
                default:
                    break;
            }
        }

        if (_forcePushCharge > 0 && _currentSpell != Spell.ForcePush) {
            DechargeForcePush();
        }
    }


    private void CheckForSpellcast () {
        if (!castingSpell) {
            // Fireball - Pinch with hands, draw hands out to form fireball
            if (RightController.CurrentGesture == HandGesture.Pinch && LeftController.CurrentGesture == HandGesture.Pinch &&
                Vector3.Distance(RightController.Hand.IndexFinger.position, LeftController.Hand.IndexFinger.position) < .1f) {
                castingSpell = true;
                _currentSpell = Spell.Fireball;
                CastFireball();
            }

            // Force Push - One, or both, hands flat, some button down, apply movement velocity
            else if (RightController.Hand?.SkinnedMeshRenderer?.material != null) {
                if (RightController.CurrentGesture == HandGesture.Open) {
                    if (RightController.SecondaryButtonPressed) {
                        castingSpell = true;
                        _currentSpell = Spell.ForcePush;
                    }
                }
            }

            // Shield - Both hands flat, some button down, draw a square to form shield
            if (RightController.CurrentGesture == HandGesture.Open && LeftController.CurrentGesture == HandGesture.Open) {

            }
        }

        else if (castingSpell) {
            if (_currentSpell == Spell.Fireball && (RightController.CurrentGesture != HandGesture.Pinch || LeftController.CurrentGesture != HandGesture.Pinch)) {
                castingSpell = false;
                _currentSpell = Spell.None;
                _castFireball.Create();
                _castFireball = null;
            }

            if (_currentSpell == Spell.ForcePush && (!RightController.SecondaryButtonPressed || RightController.CurrentGesture != HandGesture.Open)) {
                if (RightController.Velocity.magnitude > 1.0f) {
                    LayerMask mask = LayerMask.GetMask("TriggerCollider");
                    Collider [ ] collisions = Physics.OverlapSphere(transform.position, 5.0f, mask);
                    if (collisions.Length > 0) {
                        for (int i = 0; i < collisions.Length; i++) {
                            float angle = Vector3.Angle(-RightController.transform.right, (collisions [ i ].transform.position - RightController.transform.position).normalized);
                            Debug.Log("Angle = " + angle.ToString());

                            if (angle < 90) {
                                Fireball fireball = collisions [ i ].GetComponent<Fireball>();
                                if (fireball != null) {
                                    if (fireball.Active) {
                                        fireball.ApplyForce(RightController.Velocity * _forcePushPower * _forcePushCharge);
                                        //Get hit
                                    }
                                }
                            }
                        }
                    }
                }

                _currentSpell = Spell.None;
                castingSpell = false;
            }
        }
    }

    private void ChargeForcePush () {
        if (_forcePushCharge < _forcePushChargeupTime) {
            Mathf.Clamp(_forcePushCharge += Time.deltaTime, 0, _forcePushChargeupTime);
            float percentageCharged = _forcePushCharge / _forcePushChargeupTime;
            RightController.Hand.SkinnedMeshRenderer.material.SetFloat("Vector1_654486F", percentageCharged);
        }
    }

    private void DechargeForcePush () {
        Mathf.Clamp(_forcePushCharge -= Time.deltaTime, 0, _forcePushChargeupTime);
        float percentageCharged = _forcePushCharge / _forcePushChargeupTime;
        RightController.Hand.SkinnedMeshRenderer.material.SetFloat("Vector1_654486F", percentageCharged);
    }

    private void CastFireball () {
        GameObject instans = Instantiate(FireballPrefab);
        instans.transform.position = _pointBetween;
        _castFireball = instans.GetComponent<Fireball>();
    }
}
