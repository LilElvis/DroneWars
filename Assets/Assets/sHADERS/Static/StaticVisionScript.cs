using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//[ImageEffectAllowedInSceneView]
//[ExecuteInEditMode]
public class StaticVisionScript : MonoBehaviour
{
	public Shader nightVisionShader;
    public Shader sonarShader;
    public Shader filmGrainShader;
    public Shader waveShader;
    public Shader[] ChromaticAberrationShader;

    private Material[] ChromaticAberrationMaterial;

    private Material nightVisionMaterial;
    private Material sonarMaterial;
    private Material filmGrainMaterial;
    private Material waveMaterial;

	private Camera CameraSettings;


    private bool[] lightStatus;
    private float[] lightBrightness;
	
	[Space(10)]
	public FilmGrain filmGrain;
    private float scrollAmount;
    private float scrollSpeed;
    private float filmGrainGlitchAmount = 0.0f;
    
    private Vector4 jitterParam;
    public Vector2 jitterAmount = new Vector2(8.0f, 8.0f);    
    [Range(0.0f, 1.0f)]
    public float jitterBias = 1.0f;

    public bool filmGrainJitterActive =      true;
    public bool filmGrainFaceActive =        true;
    public bool filmGrainScrollingActive =   true;
    public bool filmGrainMultActive =        true;
    public bool filmGrainMovieActive =       false;
    

   

    public CameraModeFloat barrelDistortAmount;
    public CameraModeFloat barrelDistortZoomAmount;
    public float filmGrainBarrel;
    public float filmGrainBarrelZoom;
    
	
	[Space(10)]
    public Color SonarColor = new Color(0.5f, 0.5f, 1.0f);
    [Range(0.0f, 1.0f)]
    private float SonarDiffusePass = 0.0f;
    private float SonarTimeMult = 0.25f;
    private float SonarMult = 0.005f;
	
	[Space(10)]
    public MovieTexture Movie;
    private float MovieAmount = 1.0f;
    private float filmGrainFaceAmount = 0.0f;

    public enum HackerVisionMode { Normal = 0, Night = 1, Sonar = 2, Last = 3};
    public HackerVisionMode Vision = HackerVisionMode.Normal;


	

	
	[Header("Camera Wave Amount")]
    public Vector2 WaveCount = new Vector2(40.0f, 40.0f);
    public Vector2 WaveIntensity = new Vector2(0.01f, 0.01f);
    public Vector2 WaveTimeMult = new Vector2(1.0f, 1.0f);
	[Space(5)]

    [Tooltip("Determines concentration of aberration closer to the center of the screen")]
    [Range(0.1f, 3.0f)]
    public float CA_Dispersal = 1.0f;

    [Tooltip("Determines amount of Chromatic Aberration")]
    [Range(0.0f, 0.2f)]
    public float CA_Offset = 0.025f;

    RenderTexture temp;

    float timeNudge;

    Matrix4x4 ProjBiasMatrix = new Matrix4x4();

    float timeSinceOffset = 0.0f;
    float timeOffsetLength = 0.1f;
    static float timeOffsetLengthMin = 0.1f;
    static float timeOffsetLengthMax = 0.1f * 3.0f;
    float timeOffsetRange = 0.05f;
    Vector2 timeOffsetValue;

    public float timeSinceSwap = 1.0f;

    [System.Serializable]
    public struct CameraModeFloat
    {
		public float normal;
		public float night;
		public float thermal;
		public float sonar;
	}

	[System.Serializable]
    public struct CameraMaterials
    {
		public Material normal;
		public Material camera;
		public Material thermal;
		public Material sonar;
	}

	[System.Serializable]
    public struct FilmGrain
    {
		public Texture ScrollingTexture;
		public Texture MultTexture;
		public Texture GlitchTexture;
		public Texture FaceTexture;

		[Range(0.0f, 1.0f)]
		public float nightVisionAmount;

		[Range(0.0f, 1.0f)]
		public float normalAmount;

	}

	private int defaultCameraLayersActive;

    // Awake is called when the script instance is being loaded
    public void Awake()
    {
        nightVisionMaterial = new Material(nightVisionShader);
        sonarMaterial = new Material(sonarShader);
        filmGrainMaterial = new Material(filmGrainShader);
        waveMaterial = new Material(waveShader);
        ChromaticAberrationMaterial = new Material[ChromaticAberrationShader.Length];
        for(int i = 0; i < ChromaticAberrationMaterial.Length; ++i)
        {
            ChromaticAberrationMaterial[i] = new Material(ChromaticAberrationShader[i]);
        }

        barrelDistortAmount.normal = 1.00f;
        barrelDistortAmount.night = 1.15f;
        barrelDistortAmount.thermal = 1.50f;
        barrelDistortAmount.sonar = 1.00f;

        barrelDistortZoomAmount.normal = 1.00f;
        barrelDistortZoomAmount.night = 1.00f;
        barrelDistortZoomAmount.thermal = 1.00f;
        barrelDistortZoomAmount.sonar = 1.00f;

        filmGrainBarrel = barrelDistortAmount.normal;
        filmGrainBarrelZoom = barrelDistortZoomAmount.normal;
        
        //Movie.loop = true;

        timeSinceOffset = 10.0f;

        CameraSettings = GetComponent<Camera>();
		defaultCameraLayersActive = CameraSettings.cullingMask;

        scrollSpeed = Random.Range(0.8f, 1.2f);
        scrollAmount = Random.Range(0.0f, 1000.0f);
        temp = new RenderTexture(Screen.width / 2, Screen.height / 2, 0);
        timeNudge = Random.Range(0.0f, 1000.0f);

        filmGrainMaterial.SetTexture("uScrollingTexture", filmGrain.ScrollingTexture);
        filmGrainMaterial.SetTexture("uMultTexture", filmGrain.MultTexture);
        filmGrainMaterial.SetTexture("uScrollingGlitchTexture", filmGrain.GlitchTexture);
        filmGrainMaterial.SetTexture("uMovie", Movie);
        filmGrainMaterial.SetFloat("uMovieAmount", MovieAmount);
        filmGrainMaterial.SetTexture("uSpookyFaceTexture", filmGrain.FaceTexture);
        filmGrainMaterial.SetFloat("uSpookyAmount", 0.0f);
        //Movie.Play();

        ProjBiasMatrix.SetRow(0, new Vector4(2.0f, 0.0f, 0.0f, -1.0f));
        ProjBiasMatrix.SetRow(1, new Vector4(0.0f, 2.0f, 0.0f, -1.0f));
        ProjBiasMatrix.SetRow(2, new Vector4(0.0f, 0.0f, 2.0f, -1.0f));
        ProjBiasMatrix.SetRow(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

        Shader.SetGlobalFloat("_EmissionVisionMult", 0.0f);
    }

    void Start()
    {
        

        //all_my_damn_lights.FindAll(s => s.Equals("Light")); //= GetComponent<Light>();

        //Movie.loop = true;

        timeSinceOffset = 10.0f;

        CameraSettings = GetComponent<Camera>();
        

        scrollSpeed = Random.Range(0.8f, 1.2f);
        scrollAmount = Random.Range(0.0f, 1000.0f);
        temp = new RenderTexture(Screen.width, Screen.height, 0);
        timeNudge = Random.Range(0.0f, 1000.0f);

        filmGrainMaterial.SetTexture("uScrollingTexture", filmGrain.ScrollingTexture);
        filmGrainMaterial.SetTexture("uMultTexture", filmGrain.MultTexture);
        filmGrainMaterial.SetTexture("uScrollingGlitchTexture", filmGrain.GlitchTexture);
        //filmGrainMaterial.SetTexture("uMovie", Movie);
        filmGrainMaterial.SetFloat("uMovieAmount", MovieAmount);
        filmGrainMaterial.SetTexture("uSpookyFaceTexture", filmGrain.FaceTexture);
        filmGrainMaterial.SetFloat("uSpookyAmount", 0.0f);
        //Movie.Play();

        ProjBiasMatrix.SetRow(0, new Vector4(2.0f, 0.0f, 0.0f, -1.0f));
        ProjBiasMatrix.SetRow(1, new Vector4(0.0f, 2.0f, 0.0f, -1.0f));
        ProjBiasMatrix.SetRow(2, new Vector4(0.0f, 0.0f, 2.0f, -1.0f));
        ProjBiasMatrix.SetRow(3, new Vector4(0.0f, 0.0f, 0.0f,  1.0f));

        Shader.SetGlobalFloat("_EmissionVisionMult", 0.0f);
    }

    public void Update()
    {
        // Film Grain transition between vision modes
        if (timeSinceOffset > timeOffsetLength * 5.0f && Time.timeSinceLevelLoad > 5.0f)
        {
            float RandomOffsetChance = Random.Range(0.0f, 1000.0f);
            if (RandomOffsetChance > 996.0f)
            {
                timeSinceOffset = 0.0f;
                timeOffsetValue = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
                timeOffsetLength = Random.Range(timeOffsetLengthMin, timeOffsetLengthMax);
                //
                //Movie.Play();
                //MovieAmount = 1.0f;
                filmGrainMaterial.SetFloat("uMovieAmount", MovieAmount);

                if (Random.Range(0.0f, 1.0f) > 0.95f)
                {
                    filmGrainFaceAmount = 1.0f;
                }
            }
        }

        timeSinceOffset += Time.deltaTime;
        timeSinceSwap += Time.deltaTime;
    }

    const float timeToWaitTillSearch = 10.0f;
    float timeSinceLastSearch = timeToWaitTillSearch;

    public void OnPreRender()
    {

        if (Vision == HackerVisionMode.Normal)
        {
            Shader.SetGlobalFloat("_EmissionVisionMult", 0.0f);
        }
        else
        {
            Shader.SetGlobalFloat("_EmissionVisionMult", 10.0f);
        }
    }

    // OnRenderImage is called after all rendering is complete to render image
    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        {
            Material CA_Active;

            if (CA_Offset < 0.55f / 16)
            {
                CA_Active = ChromaticAberrationMaterial[0];
            }
            else if (CA_Offset < 0.55f / 8)
            {
                CA_Active = ChromaticAberrationMaterial[1];
            }
            else if (CA_Offset < 0.55f / 4)
            {
                CA_Active = ChromaticAberrationMaterial[2];
            }
            else
            {
                CA_Active = ChromaticAberrationMaterial[3];
            }

            CA_Active = ChromaticAberrationMaterial[2];

            CA_Active.SetFloat("_Dispersal", CA_Dispersal);
            CA_Active.SetFloat("_Offset", CA_Offset);

            Graphics.Blit(source, temp, CA_Active);
            Graphics.Blit(temp, source);
        }

        #region Shader variance for the film grain
        float jitterBiasMult = 0.0f;
        //if (filmGrainJitterActive && Vision == HackerVisionMode.Thermal)
        //{ Shader.EnableKeyword("SAT_GRAIN_JITTER"); jitterBiasMult = 1.0f; }
        //else
        //    Shader.DisableKeyword("SAT_GRAIN_JITTER");
        if(filmGrainFaceActive)
            Shader.EnableKeyword("SAT_GRAIN_FACE");
        else
            Shader.DisableKeyword("SAT_GRAIN_FACE");
        if(filmGrainScrollingActive)
            Shader.EnableKeyword("SAT_GRAIN_SCROLLING");
        else
            Shader.DisableKeyword("SAT_GRAIN_SCROLLING");
        if(filmGrainMultActive)
            Shader.EnableKeyword("SAT_GRAIN_MULT");
        else
            Shader.DisableKeyword("SAT_GRAIN_MULT");
        if(filmGrainMovieActive)
            Shader.EnableKeyword("SAT_GRAIN_MOVIE");
        else
            Shader.DisableKeyword("SAT_GRAIN_MOVIE");
        
        #endregion


        float filmGrainAmountTotal = 0.0f;

        if (timeSinceOffset < timeOffsetLength && Vision != HackerVisionMode.Sonar)
        {
            filmGrainMaterial.SetFloat("uScrollingGlitchAmount", 1.0f);
            filmGrainAmountTotal += 0.1f;


            filmGrainFaceAmount -= Time.deltaTime * 2.0f;
            filmGrainFaceAmount = Mathf.Max(0.0f, filmGrainFaceAmount);
            //filmGrainFaceAmount
            //filmGrainMaterial.SetFloat("uSpookyAmount", Mathf.Pow(Mathf.InverseLerp(timeOffsetLength, 0.0f, timeSinceOffset), 1.0f));
            filmGrainMaterial.SetFloat("uSpookyAmount", filmGrainFaceAmount);

            filmGrainMaterial.SetVector("uOffsetAmount", new Vector2(
                timeOffsetValue.x + Random.Range(-timeOffsetRange, timeOffsetRange),
                timeOffsetValue.y + Random.Range(-timeOffsetRange, timeOffsetRange)));
        }
        else
        {
            //Movie.Stop();
            //MovieAmount = 0.0f;
            filmGrainMaterial.SetFloat("uMovieAmount", MovieAmount);
            filmGrainMaterial.SetFloat("uSpookyAmount", 0.0f);

            filmGrainMaterial.SetFloat("uScrollingGlitchAmount", 0.0f);
            filmGrainMaterial.SetVector("uOffsetAmount", new Vector2(0.0f, 0.0f));
        }

        filmGrainMaterial.SetVector("uScrollAmount", new Vector2(timeNudge + Time.time * scrollSpeed, timeNudge + Time.time * scrollSpeed));

        filmGrainAmountTotal += Mathf.Clamp(filmGrain.normalAmount + Mathf.InverseLerp(0.3f, 0.1f, timeSinceSwap), 0.0f, 1.0f);
        float RandomNum = Random.Range(0.0f, 1.0f);
        filmGrainMaterial.SetFloat("RandomNumber", RandomNum);
        filmGrainMaterial.SetFloat("uAmount", filmGrainAmountTotal);
                        
        filmGrainMaterial.SetFloat("uDistortion", filmGrainBarrel);
        filmGrainMaterial.SetFloat("uZoom", 1.0f/filmGrainBarrelZoom);

        jitterParam.x = Random.Range(0.0f, 1.0f);
        jitterParam.y = Random.Range(0.0f, 1.0f);
        jitterParam.z = jitterAmount.x * jitterBias * jitterBiasMult;
        jitterParam.w = jitterAmount.y * jitterBias * jitterBiasMult;
        filmGrainMaterial.SetVector("jitterParam", jitterParam);

        waveMaterial.SetVector("uWaveCount", WaveCount);
        waveMaterial.SetVector("uWaveIntensity", WaveIntensity);
        waveMaterial.SetVector("uTime", WaveTimeMult * Time.time);

        //agentMaterials.camera.SetColor("_EmissionColor", new Color(0.0f, 0.0f, 0.0f));
        //phantomMaterials.camera.SetColor("_EmissionColor", new Color(0.0f, 0.0f, 0.0f));
        //Shader.SetGlobalFloat(emissionShader.GetInstanceID(), 1.0f); 
        //_EmissionMult
        //emissionShader.
        
        if (Vision == HackerVisionMode.Normal)
        {

            Graphics.Blit(source, temp, waveMaterial);
            Graphics.Blit(temp, destination, filmGrainMaterial);
        }
        else if (Vision == HackerVisionMode.Night)
        {
            //float RandomNum = Random.Range(0.0f, 1.0f);
            nightVisionMaterial.SetFloat("RandomNumber", RandomNum);
            nightVisionMaterial.SetFloat("uAmount", 0.0f);
            nightVisionMaterial.SetFloat("uLightMult", 1.2f);


            filmGrainMaterial.SetFloat("uAmount", Mathf.Max(filmGrain.nightVisionAmount, filmGrainAmountTotal));

            Graphics.Blit(source, temp, nightVisionMaterial);
            Graphics.Blit(temp, destination, filmGrainMaterial);
        }
        else if (Vision == HackerVisionMode.Sonar)
        {
            sonarMaterial.SetVector("uColorAdd", new Vector4(SonarColor.r, SonarColor.g, SonarColor.b, SonarTimeMult * Time.time));
            sonarMaterial.SetVector("uParameter", new Vector4(SonarMult, SonarDiffusePass, 0.0f, 0.0f));

            Matrix4x4 inverseMatrix = Matrix4x4.Inverse(CameraSettings.projectionMatrix) * ProjBiasMatrix;
            sonarMaterial.SetMatrix("uProjBiasMatrixInverse", inverseMatrix);

            filmGrainMaterial.SetFloat("uAmount", filmGrainAmountTotal - filmGrain.normalAmount);

            Graphics.Blit(source, temp, sonarMaterial);
            Graphics.Blit(temp, destination, filmGrainMaterial);
        }
                
    }
}
