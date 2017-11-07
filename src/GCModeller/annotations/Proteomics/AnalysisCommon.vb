Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
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
        With proteins.Shadows

            Dim test As BooleanVector
            Dim log2FC As Vector = !log2FC
            Dim p As Vector = !pvalue
            Dim FDR As Vector
            Dim n% = .Length

            ' obtain the memory pointer to the R server memory
            Dim var$ = stats.padjust(p, n:=p.Length)

            SyncLock RServer.R
                With RServer.R

                    ' read the Rserver memory from the memory pointer and 
                    ' then convert the symbol to a numeric vector
                    FDR = .Evaluate(var) _
                          .AsNumeric _
                          .ToArray
                End With
            End SyncLock

            With CObj(.ref)

                test = (Math.Log(level, 2) <= Vector.Abs(log2FC)) & (p <= pvalue)

                ' apply FDR selector if the threshold is less than 1
                .FDR = FDR

                If FDR_threshold < 1 Then
                    test = test & (FDR <= FDR_threshold)
                End If

                .isDEP = test

                With Which.IsTrue(test).Count
                    Call println("resulted %s DEPs from %s proteins!", .ref, n)
                End With
            End With

            Return .ref
        End With
    End Function
End Module
