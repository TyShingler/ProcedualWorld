using System;
using System.Collections;
using System.Collections.Generic;
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
    float sampleRate = 32.0f;

    //Part 2
    public ComputeShader lerpFieldGenerator;
    public RenderTexture lerpField_texture;
    public Material lerpField_material;

    //Part 3
    public ComputeShader caseNumberGenerator;
    public RenderTexture caseNumber_texture;
    public Material caseNumber_material;

    //Timing
    int shaderWaitTime = 20;
    float tempTime;
    int count = 0;
    
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
        caseNumberGenerator.SetTexture(caseNumberHandler, "Result", caseNumber_texture);

        float[] caseNumbers = new float[volumeSize * volumeSize * volumeSize];
        ComputeBuffer numberBuffer = new ComputeBuffer(caseNumbers.Length, sizeof(float));
        caseNumberGenerator.SetBuffer(caseNumberHandler, "Result_Floats", numberBuffer);

        SetOffsets(caseNumberGenerator);

        caseNumberGenerator.Dispatch(caseNumberHandler, volumeSize / 8, volumeSize / 8, volumeSize / 8);
        caseNumber_material.mainTexture = caseNumber_texture;
        
        numberBuffer.GetData(caseNumbers);
        numberBuffer.Dispose();
        //printFloatArray(caseNumbers, "CaseNumber");
    }

    private void GenerateLerpField()
    {
        int lerpFieldHandler = lerpFieldGenerator.FindKernel("LerpField");

        lerpField_texture = CreateTexture();

        lerpFieldGenerator.SetTexture(lerpFieldHandler, "Result", lerpField_texture);
        lerpFieldGenerator.SetTexture(lerpFieldHandler, "Input", sampler_texture);

        SetOffsets(lerpFieldGenerator);

        lerpFieldGenerator.Dispatch(lerpFieldHandler, volumeSize / 8, volumeSize / 8, volumeSize / 8);
        lerpField_material.mainTexture = lerpField_texture;
    }

    private void GenerateSamples()
    {
        int samplerHandler = sampler.FindKernel("Sampler");
        sampler_texture = CreateTexture();
        sampler.SetTexture(samplerHandler, "Result", sampler_texture);

        SetOffsets( sampler );

        sampler.Dispatch(samplerHandler, volumeSize / 8, volumeSize / 8, volumeSize / 8);
        sampler_material.mainTexture = sampler_texture;
    }

    private void SetOffsets(ComputeShader computeShader)
    {
        computeShader.SetInt("xOffset", xOffsets);
        computeShader.SetInt("yOffset", yOffsets);
        computeShader.SetInt("zOffset", zOffsets);
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
