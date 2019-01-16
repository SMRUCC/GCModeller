Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.LinearAlgebra

''' <summary>
''' 当T检验无法正常工作的时候，使用这个模块进行P值的计算
''' 仅适用于LabelFree蛋白组数据分析
''' 
''' > https://www.nature.com/articles/nbt.1511
''' </summary>
Public Module SignificanceAB

    ''' <summary>
    ''' 函数返回FoldChange向量所对应的P值向量
    ''' </summary>
    ''' <param name="ratio">某一个比对组别的FoldChange值向量</param>
    ''' <returns></returns>
    ''' 
    <Extension>
    Public Function Significance(ratio As Vector) As Vector
        ratio = Vector.Log(ratio, base:=2)
        Dim orders = ratio
    End Function
End Module
