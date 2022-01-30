using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class PlayerHealth : MonoBehaviour
{
    public int HitsToKnockout = 3;
    public float RegenTime;

    private int _recentHits;
    private float _regenTimer;

    private float _knockedOutTimestamp;
    private bool _knockedOut;
    public bool KnockedOut => _knockedOut;
    public float TimeAlive => Time.time - _knockedOutTimestamp;

    public float RespawnTime = 7f;

    public void TakeDamage()
    {
        if (_knockedOut)
        {
            return;
        }
        
        _recentHits++;
        if (_recentHits >= HitsToKnockout)
        {
            KnockOut();
            return;
        }

        {
            var vignetteVal = _recentHits / (float)HitsToKnockout;
            if (GlobalVolume.GetProfile().TryGet(out Vignette vignette))
            {
                vignette.intensity.overrideState = true;
                vignette.intensity.value = vignetteVal;
            }
        }

        _regenTimer = RegenTime;
    }

    private void KnockOut()
    {
        _knockedOut = true;
        _knockedOutTimestamp = Time.time;
            
        if (GlobalVolume.GetProfile().TryGet(out Exposure exposure))
        {
            exposure.fixedExposure.value = 15f;
        }
            
        SpookerManager.Instance.KillAllSpookers();
        BigManManager.Instance.SleepAndReset();

        StartCoroutine(RespawnRoutine());
    }

    private void Update()
    {
        if (_knockedOut)
        {
            return;
        }
        
        if (_regenTimer > 0)
        {
            _regenTimer -= Time.deltaTime;
            if (_regenTimer <= 0)
            {
                _recentHits = 0;
                if(GlobalVolume.GetProfile().TryGet(out Vignette vignette))
                {
                    vignette.intensity.overrideState = true;
                    vignette.intensity.value = 0f;
                }

                if (GlobalVolume.GetProfile().TryGet(out Exposure exposure))
                {
                    exposure.fixedExposure.value = -1.503142f;
                }
            }
        }
    }

    private IEnumerator RespawnRoutine()
    {
        var timer = 0f;
        while (timer < RespawnTime)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        
        DoRespawn();
    }

    private void DoRespawn()
    {
        // respawns spookers
        SpookerManager.Instance.OnPlayerRespawn();
        
        var profile = GlobalVolume.GetProfile();
        if (profile.TryGet(out Vignette vignette))
        {
            vignette.intensity.overrideState = true;
            vignette.intensity.value = 0f;
        }

        if (profile.TryGet(out Exposure exposure))
        {
            exposure.fixedExposure.value = -1.503142f;
        }
        
        

        _recentHits = 0;
        _regenTimer = 0;
        _knockedOut = false;
    }
}
