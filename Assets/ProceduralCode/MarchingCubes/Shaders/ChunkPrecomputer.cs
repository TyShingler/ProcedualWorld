using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ChunkPrecomputer
{
    public int xOffsets = 0;
    public int yOffsets = 0;
    public int zOffsets = 0;

    public ComputeShader samplerShader;
    public RenderTexture sampler_texture;
    public Material sampler_material;

    public ComputeShader caseShader;
    public RenderTexture caseNumber_texture;
    public Material caseNumber_material;

    public ChunkPrecomputer(ComputeShader samplerShaderIN, ComputeShader caseNumberShader)
    {
        samplerShader = samplerShaderIN;
        caseShader = caseNumberShader;
        sampler_material = (Material)AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/LerpFieldM.mat");
        caseNumber_material = (Material)AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/CaseNumberM.mat");
    }

    public PrecomputedChunkCollection PrecomputeChunk()
    {
        ///---Shader setup--- 
        //--Kernals
        int sampleHandler = samplerShader.FindKernel("PrecomputeChunkSampler");
        int caseShaderHandler = caseShader.FindKernel("PrecomputeChunkCase");
        //--Textures
        //-Lerp
        sampler_texture = CreateTexture(18, 18 * 18);
        samplerShader.SetTexture(sampleHandler, "DebugImage", sampler_texture);

        //-Case
        caseNumber_texture = CreateTexture(16, 16 * 16);
        caseShader.SetTexture(caseShaderHandler, "CaseNumber_Texture", caseNumber_texture);

        //--Float Arrays
        //-Lerp
        BufferPair samples = SetupComputeBuffer(sampleHandler, samplerShader, "SampleField", 18*18*18);
        
        //-Case
        BufferPair caseNumbers = SetupComputeBuffer(caseShaderHandler, caseShader, "CaseNumber_Floats", 16 * 16 * 16);

        //--Setting Offsets
        SetupShaderOffsets(samplerShader);
        SetupShaderOffsets(caseShader);

        //--Dispatch
        samplerShader.Dispatch(sampleHandler, 18 / 6, 18 / 6, 18 / 6);
        caseShader.Dispatch(caseShaderHandler, 16 / 8, 16 / 8, 16 / 8);

        //--Returning Textures
        sampler_material.mainTexture = sampler_texture;
        caseNumber_material.mainTexture = caseNumber_texture;

        //--Fetch data for method return
        FetchArrayData(samples);
        FetchArrayData(caseNumbers);
        PrecomputedChunkCollection collection = new PrecomputedChunkCollection();
        collection.samples = samples.array;
        collection.caseNumbers = caseNumbers.array;

        return collection;
    }

    public struct PrecomputedChunkCollection
    {
        public float[] samples;
        public float[] caseNumbers;
    }

    private static float[] FetchArrayData(BufferPair pair)
    {
        pair.buffer.GetData(pair.array);
        pair.buffer.Dispose();
        return pair.array;
    }

    private RenderTexture CreateTexture(int width, int height )
    {
        RenderTexture new_texture = new RenderTexture(width, height, 24);
        new_texture.enableRandomWrite = true;
        new_texture.Create();
        return new_texture;
    }

    struct BufferPair
    {
        public float[] array;
        public ComputeBuffer buffer;
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
}
