Imports System.ComponentModel

''' <summary>
''' 
''' </summary>
''' <remarks>
''' flowchart LR
'''   subgraph A [反映型测量模型]
'''     LV[潜变量 ξ] --“导致”--> MV1[指标 X1]
'''     LV[潜变量 ξ] --“导致”--> MV2[指标 X2]
'''     LV[潜变量 ξ] --“导致”--> MV3[指标 X3]
'''   end
''' 
'''   subgraph B [形成型测量模型]
'''     MV4[指标 X1] --“构成”--> LV2[潜变量 ξ]
'''     MV5[指标 X2] --“构成”--> LV2[潜变量 ξ]
'''     MV6[指标 X3] --“构成”--> LV2[潜变量 ξ]
'''   end
''' </remarks>
Public Enum MeasurementModels
    ''' <summary>
    ''' *Reflective measurement model*（反映型测量模型）
    ''' </summary>
    <Description("A:Reflective")> A
    ''' <summary>
    ''' *Formative measurement model*（形成型测量模型）
    ''' </summary>
    <Description("B:Formative")> B
End Enum