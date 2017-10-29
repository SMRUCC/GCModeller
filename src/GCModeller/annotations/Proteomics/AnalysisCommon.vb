Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.SyntaxAPI.Vectors
Imports RDotNET
Imports RDotNET.Extensions.VisualBasic.API
Imports RServer = RDotNET.Extensions.VisualBasic.RSystem

Module AnalysisCommon

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="proteins"></param>
    ''' <param name="level#">未经过log2转化的FC等级阈值</param>
    ''' <param name="pvalue#"></param>
    ''' <param name="FDR_threshold#">1表示不进行FDR校验</param>
    ''' <returns></returns>
    <Extension>
    Public Function ApplyDEPFilter(proteins As IEnumerable(Of DEP_iTraq), level#, pvalue#, FDR_threshold#) As DEP_iTraq()

        ' enable vector programming language feature
        With proteins.VectorShadows

            Dim n% = DirectCast(!Me, VectorShadows(Of DEP_iTraq)).Length
            Dim test As BooleanVector
            Dim log2FC As Vector = DirectCast(.log2FC, VectorShadows(Of Double))
            Dim p As Vector = DirectCast(.pvalue, VectorShadows(Of Double))
            Dim FDR As Vector

            ' obtain the memory pointer to the R server memory
            Dim var$ = stats.padjust(p, n:=p.Length)

            SyncLock RServer.R
                With RServer.R

                    ' read the Rserver memory from the pointer and 
                    ' then convert the symbol to a numeric vector
                    FDR = .Evaluate(var) _
                          .AsNumeric _
                          .ToArray
                End With
            End SyncLock

            .FDR = FDR

            test = (Math.Log(level, 2) <= Vector.Abs(log2FC)) & (p <= pvalue)

            If FDR_threshold < 1 Then
                test = test & (FDR <= FDR_threshold)
            End If

            .isDEP = test

            With Which.IsTrue(test).Count
                Call println("resulted %s DEPs from %s proteins!", .ref, n)
            End With

            Return DirectCast(!Me, VectorShadows(Of DEP_iTraq))
        End With
    End Function
End Module
