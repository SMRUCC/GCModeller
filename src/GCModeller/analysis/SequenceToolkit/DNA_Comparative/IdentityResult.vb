#Region "Microsoft.VisualBasic::95c48f88d78d551bdfe2c25a2251551c, GCModeller\analysis\SequenceToolkit\DNA_Comparative\IdentityResult.vb"

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


    ' Code Statistics:

    '   Total Lines: 129
    '    Code Lines: 91
    ' Comment Lines: 19
    '   Blank Lines: 19
    '     File Size: 4.84 KB


    ' Class IdentityResult
    ' 
    '     Properties: Identities, SeqId
    ' 
    '     Function: (+2 Overloads) SigmaMatrix, SimpleTag, ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.DeltaSimilarity1998
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' 核酸序列的一致性的计算结果
''' </summary>
Public Class IdentityResult : Implements INamedValue

    Public Property SeqId As String Implements INamedValue.Key
    <Meta>
    Public Property Identities As Dictionary(Of String, Double)

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Shared Function SimpleTag(fa As FastaSeq) As String
        Return fa.Title.Split.First
    End Function

    ''' <summary>
    ''' 直接使用整条序列来进行计算
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="round"></param>
    ''' <param name="simple"></param>
    ''' <returns></returns>
    Public Shared Iterator Function SigmaMatrix(source As FastaFile, Optional round% = -1, Optional simple As Boolean = True) As IEnumerable(Of IdentityResult)
        Dim nts As NucleicAcid() = source.Select(Function(seq) New NucleicAcid(seq)).ToArray
        Dim getTag As Func(Of NucleicAcid, String)

        If simple Then
            getTag = Function(x) x.UserTag.Split.First
        Else
            getTag = Function(x) x.UserTag
        End If

        Dim getValue As Func(Of Double, Double)

        If round <= 0 Then
            getValue = Function(r) r
        Else
            getValue = Function(r) Math.Round(r, round)
        End If

        For Each nt As NucleicAcid In nts
            Dim result = LinqAPI.MakeList(Of NamedValue(Of Double)) <=
 _
                From x As NucleicAcid
                In nts.AsParallel
                Where Not x Is nt  ' 由于是自己的全长序列与自己的全长序列进行比较，二者一致，故而距离为0，这里为了节省时间就不做计算了
                Let sigma As Double = DifferenceMeasurement.Sigma(nt, x)
                Select New NamedValue(Of Double) With {
                    .Name = getTag(x),
                    .Value = getValue(sigma * 1000)
                }

            ' 自己与自己相互进行比较肯定是0距离的
            ' 直接添加
            result += New NamedValue(Of Double) With {
                .Name = getTag(nt),
                .Value = 0R
            }

            Call nt.UserTag.__DEBUG_ECHO

            Yield New IdentityResult With {
                .Identities = result.ToDictionary(Function(x) x.Name, Function(x) x.Value),
                .SeqId = nt.UserTag
            }
        Next
    End Function

    ''' <summary>
    ''' 使用默认的``dnaA - gyrB``为外标尺进行计算
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="round%"></param>
    ''' <param name="simple"></param>
    ''' <returns></returns>
    Public Shared Iterator Function SigmaMatrix(source As IEnumerable(Of GBFF.File), Optional round% = -1, Optional simple As Boolean = True) As IEnumerable(Of IdentityResult)
        Dim data As GBFF.File() = source.ToArray
        Dim nts As NucleicAcid() = data.Select(Function(x) New NucleicAcid(x.Origin.ToFasta)).ToArray
        Dim getTag As Func(Of NucleicAcid, String)

        If simple Then
            getTag = Function(x) x.UserTag.Split.First
        Else
            getTag = Function(x) x.UserTag
        End If

        Dim getValue As Func(Of Double, Double)

        If round <= 0 Then
            getValue = Function(r) r
        Else
            getValue = Function(r) Math.Round(r, round)
        End If

        For Each genome As GBFF.File In data
            Dim rule As New NucleicAcid(genome.dnaA_gyrB)
            Dim result = LinqAPI.MakeList(Of NamedValue(Of Double)) <=
 _
              From x As NucleicAcid
              In nts.AsParallel
              Let sigma As Double = DifferenceMeasurement.Sigma(rule, x)
              Select New NamedValue(Of Double) With {
                  .Name = getTag(x),
                  .Value = getValue(sigma * 1000)
              }

            Call rule.UserTag.__DEBUG_ECHO

            Yield New IdentityResult With {
                .Identities = result _
                    .ToDictionary(Function(x) x.Name,
                                  Function(x) x.Value),
                .SeqId = rule.UserTag
            }
        Next
    End Function
End Class
