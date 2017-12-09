using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaderOverride : MonoBehaviour
{
    public Shader shader;
    public Color color;

    public Gradient healthColor;
    float health = 0.0f;
    Camera _camera;

    Vector2 cameraSkew = new Vector2(-0.01f, -0.01f);


    public GameObject _follow;
    void Start()
    {
        _camera = this.GetComponent<Camera>();
        _camera.SetReplacementShader(shader, null);
        //Shader.SetGlobalColor("uColor", color);

        
        SetObliqueness(cameraSkew);
    }

    void SetObliqueness(Vector2 skew)
    {
        Matrix4x4 mat = _camera.projectionMatrix;
        mat[0, 2] = skew.x;
        mat[1, 2] = skew.y;
        _camera.projectionMatrix = mat;
    }

    void SetObliqueness(float horizObl, float vertObl)
    {
        Matrix4x4 mat = _camera.projectionMatrix;
        mat[0, 2] = horizObl;
        mat[1, 2] = vertObl;
        _camera.projectionMatrix = mat;
    }

    public void setHealth(float amt)
    {
        health = Mathf.Clamp01(amt);
    }

    public float getHealth()
    {
        return health;
    }

    public void addHealth(float amt)
    {
        health = Mathf.Clamp01(health+amt);
    }

    // Update is called once per frame
    void Update ()
    {
        if (_follow != null)
            _camera.transform.position = _follow.transform.position;


        //health = Mathf.Repeat(Time.time * 0.1f, 1.0f);
        Shader.SetGlobalColor("uColor", healthColor.Evaluate(health));

        _camera.Render();        
    }
}
