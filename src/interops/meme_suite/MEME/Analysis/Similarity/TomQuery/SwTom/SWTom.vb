#Region "Microsoft.VisualBasic::29b64cac4368bf245876485048aa8427, meme_suite\MEME\Analysis\Similarity\TomQuery\SwTom\SWTom.vb"

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

'     Class SWAlignment
' 
'         Constructor: (+1 Overloads) Sub New
' 
'     Module SWTom
' 
'         Function: __alignHSP, __alignInvoke, __parts, (+3 Overloads) Compare, (+2 Overloads) CompareBest
'         Class __similarity
' 
'             Constructor: (+1 Overloads) Sub New
'             Function: Similarity
' 
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis
Imports SMRUCC.genomics.Analysis.SequenceTools

Namespace Analysis.Similarity.TOMQuery

    Public Class SWAlignment : Inherits GSW(Of MotifScans.ResidueSite)

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="query"></param>
        ''' <param name="subject"></param>
        ''' <param name="equals">越相似得分应该越高</param>
        Sub New(query As MotifScans.AnnotationModel,
                subject As MotifScans.AnnotationModel,
                equals As ISimilarity(Of MotifScans.ResidueSite))
            Call MyBase.New(query.PWM, subject.PWM, symbolProvider(equals))
        End Sub

        Private Shared Function symbolProvider(equals As ISimilarity(Of MotifScans.ResidueSite)) As GenericSymbol(Of MotifScans.ResidueSite)
            Return New GenericSymbol(Of MotifScans.ResidueSite)(
                equals:=Function(x, y) equals(x, y) >= 0.85,
                similarity:=Function(x, y) equals(x, y),
                toChar:=AddressOf TomTOm.ToChar,
                empty:=Function() Nothing
            )
        End Function
    End Class

    <Package("TOMQuery.Smith-Waterman", Category:=APICategories.ResearchTools)>
    Public Module SWTom

        Private Class __similarity

            ReadOnly __compares As TomTOm.ColumnCompare
            ReadOnly __offsets As Double = 0.6

            Sub New(compare As TomTOm.ColumnCompare, offset As Double)
                __compares = compare
                __offsets = offset
            End Sub

            Public Function Similarity(x As MotifScans.ResidueSite, y As MotifScans.ResidueSite) As Integer
                Dim value As Double = __compares(x, y)
                value = 10 * (value - __offsets)
                Return CInt(value)
            End Function
        End Class

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="query"></param>
        ''' <param name="subject"></param>
        ''' <param name="method"></param>
        ''' <param name="cutoff">0% - 100%</param>
        ''' <param name="minW">
        ''' The  minimal width of the motif hsp fragment size, default is 6 residue, this is recommended by meme program.
        ''' </param>
        ''' <returns></returns>
        <ExportAPI("Compare")>
        Public Function Compare(query As MotifScans.AnnotationModel,
                                subject As MotifScans.AnnotationModel,
                                Optional method As String = "pcc",
                                Optional cutoff As Double = 0.75,
                                Optional minW As Integer = 4,
                                Optional tomThreshold As Double = 0.75,
                                Optional bitsLevel As Double = 1.5) As Output
            Dim param As New Parameters With {
                .Method = method,
                .MinW = minW,
                .SWThreshold = cutoff,
                .TOMThreshold = tomThreshold,
                .BitsLevel = bitsLevel
            }
            Return Compare(query, subject, param)
        End Function

        <ExportAPI("Compare")>
        Public Function Compare(query As MotifScans.AnnotationModel,
                                subject As MotifScans.AnnotationModel,
                                params As Parameters) As Output
            Dim similarity As ISimilarity(Of MotifScans.ResidueSite) =
                AddressOf New __similarity(TomTOm.GetMethod(params.Method), params.SWOffset).Similarity
            Return Compare(query, subject, similarity, params)
        End Function

        <ExportAPI("Compare")>
        Public Function Compare(query As MotifScans.AnnotationModel,
                                subject As MotifScans.AnnotationModel,
                                method As ISimilarity(Of MotifScans.ResidueSite),
                                params As Parameters) As Output
            Dim sw As New SWAlignment(query, subject, method)
            Dim out As SequenceTools.Output = SequenceTools.Output.CreateObject(sw, params.SWThreshold, params.MinW)
            Dim output As New Output With {
                .Query = query,
                .Subject = subject,
                .HSP = __alignHSP(query, subject, out, params),
                .Directions = out.Directions,
                .DP = out.DP,
                .Parameters = params,
                .QueryMotif = query.Motif,
                .SubjectMotif = subject.Motif
            }
            Return output
        End Function

        Private Function __alignHSP(query As MotifScans.AnnotationModel,
                                    subject As MotifScans.AnnotationModel,
                                    sw As SequenceTools.Output,
                                    param As Parameters) As SW_HSP()
            Dim method = TomTOm.GetMethod(param.Method)
            Dim alignment = (From out As SW_HSP
                             In sw.HSP.Select(Function(x) x.__alignInvoke(query, subject, method, param))
                             Where Not out Is Nothing
                             Select out).ToArray
            Return alignment
        End Function

        <Extension> Private Function __alignInvoke(hsp As HSP,
                                                   query As MotifScans.AnnotationModel,
                                                   subject As MotifScans.AnnotationModel,
                                                   method As TomTOm.ColumnCompare,
                                                   param As Parameters) As SW_HSP
            Dim queryp As MotifScans.AnnotationModel = __parts(query, hsp.fromA, hsp.toA)
            Dim subjectp As MotifScans.AnnotationModel = __parts(subject, hsp.fromB, hsp.toB)
            Dim out As DistResult = TomTOm.Compare(
                queryp,
                subjectp,
                method,
                param)

            If out Is Nothing OrElse out.MatchSimilarity < param.TOMThreshold Then
                Return Nothing
            End If

            Dim align As New SW_HSP With {
                .Query = queryp,
                .Subject = subjectp,
                .Alignment = out,
                .ToS = hsp.toB,
                .FromQ = hsp.fromA,
                .FromS = hsp.fromB,
                .Score = hsp.score,
                .ToQ = hsp.toA
            }
            Return align
        End Function

        Private Function __parts(LDM As MotifScans.AnnotationModel,
                                 start As Integer,
                                 ends As Integer) As MotifScans.AnnotationModel
            Dim array As MotifScans.ResidueSite() = LDM.PWM.Skip(start).Take(ends - start).ToArray
            Return New MotifScans.AnnotationModel With {
                .PWM = array,
                .Uid = LDM.Uid
            }
        End Function

        <ExportAPI("Compare.Best")>
        Public Function CompareBest(query As MotifScans.AnnotationModel,
                                    method As ISimilarity(Of MotifScans.ResidueSite),
                                    param As Parameters) As Output()
            Dim LQuery = (From x In TomTOm.Motifs Select Compare(query, x.Value, method, param)).ToArray
            Call Console.Write(".")
            Return LQuery
        End Function

        <ExportAPI("Compare.Best")>
        Public Function CompareBest(memeText As String, param As Parameters) As Output()
            Dim similarity As ISimilarity(Of MotifScans.ResidueSite) =
                AddressOf New __similarity(TomTOm.GetMethod(param.Method), param.SWOffset).Similarity
            Dim query = MotifScans.AnnotationModel.LoadDocument(memeText)
            Dim LQuery = (From x As MotifScans.AnnotationModel
                          In query.AsParallel
                          Select CompareBest(x, similarity, param)).ToArray
            Dim resultSet = (From x In LQuery.Unlist Where x.Match Select x).ToArray
            Return resultSet
        End Function
    End Module
End Namespace
