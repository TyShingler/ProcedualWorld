using UnityEngine;

//[ExecuteInEditMode]
public class Sampler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RunShader();
    }
    
    //Part 1
    public ComputeShader sampler;
    public RenderTexture sampler_texture;
    public Material sampler_material;
    int volumeSize = 16;
    //Not currently used float sampleRate = 32.0f;

    //Part 2
    public ComputeShader lerpFieldGenerator;
    public RenderTexture lerpField_texture;
    public Material lerpField_material;

    //Part 3
    public ComputeShader caseNumberGenerator;
    public RenderTexture caseNumber_texture;
    public Material caseNumber_material;

    //Timing
    //Not currently used int shaderWaitTime = 20;
    //Not currently used float tempTime;
    //Not currently used int count = 0;
    
    public int xOffsets = 0;
    public int yOffsets = 0;
    public int zOffsets = 0;

    private void RunShader()
    {
        //Part 1: Sampling the space.
        GenerateSamples();
        //Part 2: Generating the Lerp values in the X, Y, and Z directions.
        GenerateLerpField();
        //Part 3: Calculate the case number.
        GenerateCaseNumbers();
    }

    private void printFloatArray(float[] samples, string start)
    {
        string str = start;
        foreach (float f in samples)
        {
            str += " (" + f + ")";
        }
        Debug.Log(str);
    }

    private void GenerateCaseNumbers()
    {
        int caseNumberHandler = caseNumberGenerator.FindKernel("CaseNumber");

        caseNumber_texture = CreateTexture();

        caseNumberGenerator.SetTexture(caseNumberHandler, "CaseNumber_Texture", caseNumber_texture);

        float[] caseNumbers = new float[volumeSize * volumeSize * volumeSize];
        ComputeBuffer lerpBuffer = new ComputeBuffer(caseNumbers.Length, sizeof(float));
        caseNumberGenerator.SetBuffer(caseNumberHandler, "CaseNumber_Floats", lerpBuffer);

        SetupShaderOffsets(caseNumberGenerator);

        caseNumberGenerator.Dispatch(caseNumberHandler, volumeSize / 8, volumeSize / 8, volumeSize / 8);
        caseNumber_material.mainTexture = caseNumber_texture;


        lerpBuffer.GetData(caseNumbers);
        lerpBuffer.Dispose();
    }

    struct LerpField
    {
        public float[] lerpValues;
        public ComputeBuffer lerpBuffer;
    }

    private void GenerateLerpField()
    {
        int lerpFieldHandler = lerpFieldGenerator.FindKernel("LerpField");

        lerpField_texture = CreateTexture();
        lerpFieldGenerator.SetTexture(lerpFieldHandler, "LerpField_Texture", lerpField_texture);

        LerpField XField = SetLerpFieldArrayForDirection(lerpFieldHandler, "LerpFloats_X");
        LerpField YField = SetLerpFieldArrayForDirection(lerpFieldHandler, "LerpFloats_Y");
        LerpField ZField = SetLerpFieldArrayForDirection(lerpFieldHandler, "LerpFloats_Z");

        SetupShaderOffsets(lerpFieldGenerator);

        lerpFieldGenerator.Dispatch(lerpFieldHandler, volumeSize / 8, volumeSize / 8, volumeSize / 8);
        lerpField_material.mainTexture = lerpField_texture;

        GetFieldDataFromComputeShader(XField);
        GetFieldDataFromComputeShader(YField);
        GetFieldDataFromComputeShader(ZField);
    }

    // Returns the array, but it is also stored in the lerpField passed in.
    private static float[] GetFieldDataFromComputeShader(LerpField lerpField)
    {
        lerpField.lerpBuffer.GetData(lerpField.lerpValues);
        lerpField.lerpBuffer.Dispose();
        return lerpField.lerpValues;
    }

    private LerpField SetLerpFieldArrayForDirection(int lerpFieldHandler, string arrayName)
    {
        LerpField result = new LerpField();
        result.lerpValues = new float[volumeSize * volumeSize * volumeSize];
        result.lerpBuffer = new ComputeBuffer(result.lerpValues.Length, sizeof(float));
        lerpFieldGenerator.SetBuffer(lerpFieldHandler, arrayName, result.lerpBuffer);
        return result;
    }

    private void GenerateSamples()
    {
        int samplerHandler = sampler.FindKernel("Sampler");
        sampler_texture = CreateTexture();
        sampler.SetTexture(samplerHandler, "Result", sampler_texture);

        SetupShaderOffsets(sampler);

        sampler.Dispatch(samplerHandler, volumeSize / 8, volumeSize / 8, volumeSize / 8);
        sampler_material.mainTexture = sampler_texture;

    }

    private void SetupShaderOffsets(ComputeShader shader)
    {
        shader.SetInt("xOffset", xOffsets);
        shader.SetInt("yOffset", yOffsets);
        shader.SetInt("zOffset", zOffsets);
    }

    private RenderTexture CreateTexture()
    {
        RenderTexture new_texture = new RenderTexture(volumeSize * 4, volumeSize * 4, 24);
        new_texture.enableRandomWrite = true;
        new_texture.Create();
        return new_texture;
    }

    // Update is called once per frame
    void Update()
    {
        RunShader();
    }

}
