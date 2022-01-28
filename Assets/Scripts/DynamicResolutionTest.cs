using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DynamicResolutionTest : MonoBehaviour
{
    public int RenderHeight = 500;
    public RenderTextureFormat PixelFormat;
    private RenderTexture _texture;
    private Camera _camera;
    private Camera _screenCamera;
    private float _currentAspect;
    private int _currentRenderHeight = 0;

    public void Start()
    {
        _camera = GetComponent<Camera>();
        _screenCamera = transform.GetChild(0).GetComponent<Camera>();
        
        RecalcRT();
        
    }

    private void RecalcRT()
    {
        var aspectRatio = (float)Screen.height / (float)Screen.width;
        //Debug.Log("HWere" + aspectRatio + " " + Screen.currentResolution.height);
        if (_currentAspect == aspectRatio && _currentRenderHeight == RenderHeight)
        {
            return;
        }

        
        if (_texture != null)
        {
            Destroy(_texture);
            _texture = null;
        }

        _currentAspect = aspectRatio;
        int width = (int)(RenderHeight / aspectRatio);
        
        _texture = new RenderTexture(width,RenderHeight,32,PixelFormat,0);
        _texture.filterMode = FilterMode.Point;
        _camera.targetTexture = _texture;
    }

    public void Update()
    {
        RecalcRT();
    }
    
    private void OnEnable()
    {
        RenderPipelineManager.endFrameRendering += RenderPipelineManagerOnendFrameRendering;
         
    }
    
    private void OnDisable()
    {
        RenderPipelineManager.endFrameRendering -= RenderPipelineManagerOnendFrameRendering;
    }
    
    private void RenderPipelineManagerOnendFrameRendering(ScriptableRenderContext arg1, Camera[] arg2)
    {
        if (arg2[0] == _screenCamera)
        {
            Graphics.Blit(_texture, (RenderTexture)null,Vector2.one,Vector2.zero);    
        }
        
    }


    /*
    public float ResolutionScale = 1.0f;

    float m_widthScale = 1.0f;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float oldWidthScale = m_widthScale;

        m_widthScale = ResolutionScale;

        if (m_widthScale != oldWidthScale)
        {
            ScalableBufferManager.ResizeBuffers(m_widthScale, m_widthScale);
        }
    }
*/
}