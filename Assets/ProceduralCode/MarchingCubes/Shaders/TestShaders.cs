using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestShaders : MonoBehaviour
{
    public ComputeShader testShader;
    public RenderTexture test_texture;
    public Material test_material;

    public ComputeShader sampleShader;
    public RenderTexture sample_texture;
    public Material sample_material;
    //public ComputeShader

    public ComputeShader lerpShader;
    public RenderTexture lerpField_texture;
    public Material lerpField_material;

    public int xOffsets = 0;
    public int yOffsets = 0;
    public int zOffsets = 0;

    // Start is called before the first frame update
    void Start()
    {
        TestShader();   
    }

    private void TestShader()
    {
        float[] samples = GetSamples();

        float[] output = Test(samples);
        compareFloatArrays(samples, output);

        GetLerpFields(samples);
    }

    public void compareFloatArrays(float[] a, float[] b)
    {
        string str = "Test: ";
        for (int i = 0; i < a.Length; i++)
        {
            str += "(" + (a[i] == b[i] ? 1 : 0) + ") ";
        }
        Debug.Log(str);
    }

    private float[] Test(float[] arr)
    {
        int testHandler = testShader.FindKernel("Test");

        test_texture = CreateTexture(18, 18 * 18);
        testShader.SetTexture(testHandler, "DebugImage", test_texture);

        ComputeBuffer dataIN = new ComputeBuffer(arr.Length, sizeof(float));
        dataIN.SetData(arr);
        testShader.SetBuffer(testHandler, "FloatsIN", dataIN);

        BufferPair pairOUT = SetupComputeBuffer(testHandler, testShader, "FloatsOUT", 18 * 18 * 18);

        SetupShaderOffsets(testShader);

        testShader.Dispatch(testHandler, 18 / 6, 18 / 6, 18 / 6);

        //test_material.mainTexture = sample_texture;

        return FetchArrayData(pairOUT);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private static float[] FetchArrayData(BufferPair pair)
    {
        pair.buffer.GetData(pair.array);
        pair.buffer.Dispose();
        return pair.array;
    }

    struct BufferPair
    {
        public float[] array;
        public ComputeBuffer buffer;
    }

    private RenderTexture CreateTexture(int width, int height)
    {
        RenderTexture new_texture = new RenderTexture(width, height, 24);
        new_texture.enableRandomWrite = true;
        new_texture.Create();
        return new_texture;
    }

    private BufferPair SetupComputeBuffer(int shaderHandler, ComputeShader shader, string arrayName, int arraySize)
    {
        BufferPair result = new BufferPair();
        result.array = new float[arraySize];
        result.buffer = new ComputeBuffer(result.array.Length, sizeof(float));
        shader.SetBuffer(shaderHandler, arrayName, result.buffer);
        return result;
    }

    private void SetupShaderOffsets(ComputeShader shader)
    {
        shader.SetInt("xOffset", xOffsets);
        shader.SetInt("yOffset", yOffsets);
        shader.SetInt("zOffset", zOffsets);
    }

    public float[] GetSamples()
    {
        int samplerHandler = sampleShader.FindKernel("PrecomputeChunkSampler");

        sample_texture = CreateTexture(18, 18 * 18);
        sampleShader.SetTexture(samplerHandler, "DebugImage", sample_texture);

        BufferPair pair = SetupComputeBuffer(samplerHandler, sampleShader, "SampleField", 18 * 18 * 18);

        SetupShaderOffsets(sampleShader);

        sampleShader.Dispatch(samplerHandler, 18 / 6, 18 / 6, 18 / 6);

        sample_material.mainTexture = sample_texture;

        return FetchArrayData(pair);

    }

    BufferPair XField;
    BufferPair YField;
    BufferPair ZField;

    public void GetLerpFields(float[] samples)
    {
        int lerpShaderHandler = lerpShader.FindKernel("PrecomputeChunkLerp");

        lerpField_texture = CreateTexture(18, 18 * 18);
        lerpShader.SetTexture(lerpShaderHandler, "LerpField_Texture", lerpField_texture);

        ComputeBuffer dataIN = new ComputeBuffer(samples.Length, sizeof(float));
        dataIN.SetData(samples);
        lerpShader.SetBuffer(lerpShaderHandler, "SamplesIn", dataIN);

        XField = SetupComputeBuffer(lerpShaderHandler, lerpShader, "LerpFloats_X", 18 * 18 * 18);
        YField = SetupComputeBuffer(lerpShaderHandler, lerpShader, "LerpFloats_Y", 18 * 18 * 18);
        ZField = SetupComputeBuffer(lerpShaderHandler, lerpShader, "LerpFloats_Z", 18 * 18 * 18);

        SetupShaderOffsets(lerpShader);

        lerpShader.Dispatch(lerpShaderHandler, 18 / 6, 18 / 6, 18 / 6);
        lerpField_material.mainTexture = lerpField_texture;
    }

}
