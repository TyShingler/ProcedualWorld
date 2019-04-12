using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUSampler
{
    int volumeSize;

    //Part LerpField
    private ComputeShader lerpFieldGenerator;
    private RenderTexture lerpField_texture;
    //private Material lerpField_material;

    //Part CaseNumber
    private ComputeShader caseNumberGenerator;
    private RenderTexture caseNumber_texture;
    //private Material caseNumber_material;

    private int xOffsets = 0;
    private int yOffsets = 0;
    private int zOffsets = 0;


    public GPUSampler(ComputeShader lerpFieldShader, ComputeShader caseNumberShader)
    {
        volumeSize = 16;
        lerpFieldGenerator = lerpFieldShader;
        caseNumberGenerator = caseNumberShader;
    }

    

    public float[] GenerateCaseNumbers()
    {
        int caseNumberHandler = caseNumberGenerator.FindKernel("CaseNumber");

        caseNumber_texture = CreateTexture();

        caseNumberGenerator.SetTexture(caseNumberHandler, "CaseNumber_Texture", caseNumber_texture);

        float[] caseNumbers = new float[volumeSize * volumeSize * volumeSize];
        ComputeBuffer lerpBuffer = new ComputeBuffer(caseNumbers.Length, sizeof(float));
        caseNumberGenerator.SetBuffer(caseNumberHandler, "CaseNumber_Floats", lerpBuffer);

        SetupShaderOffsets(caseNumberGenerator);

        caseNumberGenerator.Dispatch(caseNumberHandler, volumeSize / 8, volumeSize / 8, volumeSize / 8);
        //caseNumber_material.mainTexture = caseNumber_texture;


        lerpBuffer.GetData(caseNumbers);
        lerpBuffer.Dispose();
        return caseNumbers;
    }

    public FieldCollection GenerateLerpField()
    {
        int lerpFieldHandler = lerpFieldGenerator.FindKernel("LerpField");

        lerpField_texture = CreateTexture();
        lerpFieldGenerator.SetTexture(lerpFieldHandler, "LerpField_Texture", lerpField_texture);

        LerpField XField = SetLerpFieldArrayForDirection(lerpFieldHandler, "LerpFloats_X");
        LerpField YField = SetLerpFieldArrayForDirection(lerpFieldHandler, "LerpFloats_Y");
        LerpField ZField = SetLerpFieldArrayForDirection(lerpFieldHandler, "LerpFloats_Z");

        SetupShaderOffsets(lerpFieldGenerator);

        lerpFieldGenerator.Dispatch(lerpFieldHandler, volumeSize / 8, volumeSize / 8, volumeSize / 8);
        //lerpField_material.mainTexture = lerpField_texture;

        GetFieldDataFromComputeShader(XField);
        GetFieldDataFromComputeShader(YField);
        GetFieldDataFromComputeShader(ZField);

        FieldCollection collection = new FieldCollection();
        collection.X = XField;
        collection.Y = YField;
        collection.Z = ZField;
        return collection;
    }

    

    public struct FieldCollection
    {
        public LerpField X;
        public LerpField Y;
        public LerpField Z;
    }

    public struct LerpField
    {
        public float[] lerpValues;
        public ComputeBuffer lerpBuffer;
    }

    private void SetupShaderOffsets(ComputeShader shader)
    {
        shader.SetInt("xOffset", xOffsets);
        shader.SetInt("yOffset", yOffsets);
        shader.SetInt("zOffset", zOffsets);
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
        //Need to add one b/c the 17th layers are needed to make meshes.
        result.lerpValues = new float[(volumeSize+1) * (volumeSize+1)  * (volumeSize+1)];
        result.lerpBuffer = new ComputeBuffer(result.lerpValues.Length, sizeof(float));
        lerpFieldGenerator.SetBuffer(lerpFieldHandler, arrayName, result.lerpBuffer);
        return result;
    }

    private RenderTexture CreateTexture()
    {
        RenderTexture new_texture = new RenderTexture(volumeSize * 4, volumeSize * 4, 24);
        new_texture.enableRandomWrite = true;
        new_texture.Create();
        return new_texture;
    }
}

