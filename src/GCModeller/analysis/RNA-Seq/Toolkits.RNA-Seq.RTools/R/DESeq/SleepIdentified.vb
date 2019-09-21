#Region "Microsoft.VisualBasic::e1b445fd17adf2da4ba625b97951cf6c, analysis\RNA-Seq\Toolkits.RNA-Seq.RTools\R\DESeq\SleepIdentified.vb"

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

    '     Module SleepIdentified
    ' 
    '         Function: IdentifyChanges
    ' 
    '         Sub: __analysis
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Math

Namespace DESeq2

    ''' <summary>
    ''' 鉴别出底表达量和休眠的基因
    ''' </summary>
    ''' 
    <Package("Expression.Stat")>
    Public Module SleepIdentified

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="DESEq"></param>
        ''' <param name="samples"></param>
        ''' <param name="diff">产生差异表达的最小的阈值，请注意，这个和明显差异的含义是不一样的</param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Stat.Identification")>
        Public Function IdentifyChanges(DESEq As IEnumerable(Of ResultData),
                                        samples As IEnumerable(Of SampleTable),
                                        Optional diff As Double = 0.5,
                                        Optional levels As Integer = 1000) As ExprStats()

            Dim stats As Dictionary(Of String, ExprStats) =
                DESEq.ToDictionary(Function(x) x.locus_tag,
                                   Function(x) New ExprStats With {
                                        .locus = x.locus_tag,
                                        .log2FoldChange = x.log2FoldChange,
                                        .Samples = New Dictionary(Of String, String)
                                   })

            For Each sample As SampleTable In samples
                Dim Name As String = sample.sampleName
                Dim lvMaps = DESEq.GenerateMapping(Function(x) x.dataExpr0(Name), levels)    ' 首先生成mappings
                Call __analysis(stats, lvMaps, diff, sample:=Name)
            Next

            For Each gene As ExprStats In stats.Values
                Dim max As Integer = gene.GetMaxLevel
                For Each sample As SampleTable In samples
                    If max = 0 Then
                        Call gene.Samples.Add(ExprStats.LEVEL2 & sample.sampleName, "0")
                    Else
                        Dim l As Integer = gene.GetLevel(sample.sampleName)
                        Dim p As Double = l / max
                        Call gene.Samples.Add(ExprStats.LEVEL2 & sample.sampleName, CStr(p))
                    End If
                Next
            Next

            Return stats.Values.ToArray
        End Function

        Private Sub __analysis(ByRef stats As Dictionary(Of String, ExprStats), lvMaps As Dictionary(Of String, Integer), diff As Double, sample As String)
            For Each x As ExprStats In stats.Values
                Dim level As Integer = lvMaps(x.locus)
                Dim s As String

                If level < 5 Then  ' 在这里区分低表达还是不表达
                    If x.log2FoldChange >= diff Then
                        s = "-"  ' 发生了变化，说明基因原先是不表达的
                    Else
                        s = "+?"  ' 处于低表达水平，但是即使低表达水平，也能够行使正常的生物学功能
                    End If
                Else
                    s = "+"
                End If

                Call x.Samples.Add(ExprStats.lEVEL & sample, CStr(level))
                Call x.Samples.Add(ExprStats.STATs & sample, s)
            Next
        End Sub
    End Module
End Namespace
