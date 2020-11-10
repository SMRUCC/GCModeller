#Region "Microsoft.VisualBasic::76c16f5fbc8626e3480a82b7a55502ab, annotations\Proteomics\AnalysisCommon.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Module AnalysisCommon
    ' 
    '     Function: ApplyDEPFilter, DepFilter2, Ttest
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Vectorization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports RDotNET
Imports RDotNET.Extensions.VisualBasic.API
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports RServer = RDotNET.Extensions.VisualBasic.RSystem

Public Module AnalysisCommon

    <Extension>
    Public Function Ttest(data As Matrix, treatment As String(), control As String()) As DEP_iTraq()
        Dim treatmentData = data.Project(treatment)
        Dim controlData = data.Project(control)
        Dim result As New List(Of DEP_iTraq)
        Dim labels As String() = treatment.AsList + control

        For i As Integer = 0 To data.expression.Length - 1
            result += New DEP_iTraq With {
                .ID = data(i).geneID,
                .FCavg = treatmentData(i).experiments.Average / controlData(i).experiments.Average,
                .log2FC = Math.Log(.FCavg, 2),
                .pvalue = t.Test(treatmentData(i).experiments, controlData(i).experiments).Pvalue,
                .Properties = data(i).ToDataSet(labels).AsCharacter
            }
        Next

        Return result
    End Function

    <Extension>
    Public Function DepFilter2(proteins As IEnumerable(Of DEP_iTraq), level#, pvalue#, FDR_threshold#) As DEP_iTraq()
        With proteins.Shadows
            Dim test As BooleanVector
            Dim log2FC As Vector = !log2FC
            Dim p As Vector = !pvalue
            Dim FDR As Vector = p.FDR
            Dim n% = .Length

            ' vector shadows dynamics language feature
            With CObj(.ByRef)

                test = (Math.Log(level, 2) <= Vector.Abs(log2FC)) & (p <= pvalue)

                ' apply FDR selector if the threshold is less than 1
                .FDR = FDR

                If FDR_threshold < 1 Then
                    test = test & (FDR <= FDR_threshold)
                End If

                .isDEP = test

                With Which.IsTrue(test).Count
                    Call println("resulted %s DEPs from %s proteins!", .ByRef, n)
                End With
            End With

            Return .ByRef
        End With
    End Function

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

            ' vector shadows dynamics language feature
            With CObj(.ByRef)

                test = (Math.Log(level, 2) <= Vector.Abs(log2FC)) & (p <= pvalue)

                ' apply FDR selector if the threshold is less than 1
                .FDR = FDR

                If FDR_threshold < 1 Then
                    test = test & (FDR <= FDR_threshold)
                End If

                .isDEP = test

                With Which.IsTrue(test).Count
                    Call println("resulted %s DEPs from %s proteins!", .ByRef, n)
                End With
            End With

            Return .ByRef
        End With
    End Function
End Module
