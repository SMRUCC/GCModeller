#Region "Microsoft.VisualBasic::638dc37dbb9ec834f2db9aa03011365a, engine\Model\Models\Genotype.vb"

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

    ' Structure Genotype
    ' 
    '     Function: GetEnumerator, IEnumerable_GetEnumerator, ToString
    ' 
    ' Class RNAComposition
    ' 
    '     Properties: A, C, G, geneID, U
    ' 
    '     Function: FromNtSequence, GetEnumerator, IEnumerable_GetEnumerator, ToString
    ' 
    ' Class ProteinComposition
    ' 
    '     Properties: A, C, D, E, F
    '                 G, H, I, K, L
    '                 M, N, O, P, proteinID
    '                 Q, R, S, T, U
    '                 V, W, Y
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: FromRefSeq, GetEnumerator, IEnumerable_GetEnumerator
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

''' <summary>
''' 目标细胞模型的基因组模型
''' </summary>
Public Structure Genotype : Implements IEnumerable(Of CentralDogma)

    ''' <summary>
    ''' 假设基因组之中的基因型定义信息全部都是由中心法则来构成的
    ''' </summary>
    ''' <remarks>
    ''' 请注意，当前的模块之中所定义的计算模型和GCMarkup之类的数据模型在看待基因组的构成上面的角度是有一些差异的：
    ''' 
    ''' 例如，对于复制子的描述上面，GCMarkup数据模型之中是更加倾向于将复制子分开进行描述的，这样子
    ''' 会更加的方便人类进行模型文件的阅读
    ''' 而在本计算模型之中，因为计算模型是计算机程序所阅读的，并且在实际的生命活动之中，染色体和质粒是在同一个环境之中
    ''' 工作的，所以在计算模型之中，没有太多的复制子的概念，而是将他们都合并到当前的这个中心法则列表对象之中，作为一个
    ''' 整体来进行看待
    ''' </remarks>
    Dim centralDogmas As CentralDogma()

    Dim RNAMatrix As RNAComposition()
    Dim ProteinMatrix As ProteinComposition()

    Public Overrides Function ToString() As String
        Return $"{centralDogmas.Length} genes"
    End Function

    Public Iterator Function GetEnumerator() As IEnumerator(Of CentralDogma) Implements IEnumerable(Of CentralDogma).GetEnumerator
        For Each cd As CentralDogma In centralDogmas
            Yield cd
        Next
    End Function

    Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Yield GetEnumerator()
    End Function
End Structure

Public Class RNAComposition : Implements IEnumerable(Of NamedValue(Of Double))

    Public Property geneID As String
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Property A As Integer
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Property U As Integer
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Property G As Integer
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <returns></returns>
    Public Property C As Integer

    Public Overrides Function ToString() As String
        Return geneID
    End Function

    ''' <summary>
    ''' 因为这个是RNA序列，所以其构成应该是其基因模板的互补
    ''' </summary>
    ''' <param name="nt">DNA模板序列</param>
    ''' <returns></returns>
    Public Shared Function FromNtSequence(nt As String, geneID As String) As RNAComposition
        Dim RNA As String = NucleicAcid.Complement(nt)
        Dim composition As Dictionary(Of String, Integer) = RNA _
            .GroupBy(Function(c) c) _
            .ToDictionary(Function(c)
                              Return c.Key.ToString
                          End Function,
                          Function(c) c.Count)

        Return New RNAComposition With {
            .geneID = geneID,
            .A = composition.TryGetValue("A"),
            .C = composition.TryGetValue("C"),
            .G = composition.TryGetValue("G"),
            .U = composition.TryGetValue("T")
        }
    End Function

    Public Iterator Function GetEnumerator() As IEnumerator(Of NamedValue(Of Double)) Implements IEnumerable(Of NamedValue(Of Double)).GetEnumerator
        Yield New NamedValue(Of Double)("A", A)
        Yield New NamedValue(Of Double)("U", U)
        Yield New NamedValue(Of Double)("G", G)
        Yield New NamedValue(Of Double)("C", C)
    End Function

    Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Yield GetEnumerator()
    End Function
End Class

Public Class ProteinComposition : Implements IEnumerable(Of NamedValue(Of Double))

    Public Property proteinID As String

    ''' <summary>
    ''' L-Alanine
    ''' </summary>
    ''' <returns></returns>
    Public Property A As Integer
    ''' <summary>
    ''' L-Arginine
    ''' </summary>
    ''' <returns></returns>
    Public Property R As Integer
    ''' <summary>
    ''' L-Asparagine
    ''' </summary>
    ''' <returns></returns>
    Public Property N As Integer
    ''' <summary>
    ''' L-Aspartic acid
    ''' </summary>
    ''' <returns></returns>
    Public Property D As Integer
    ''' <summary>
    ''' L-Cysteine
    ''' </summary>
    ''' <returns></returns>
    Public Property C As Integer
    ''' <summary>
    ''' L-Glutamic acid
    ''' </summary>
    ''' <returns></returns>
    Public Property E As Integer
    ''' <summary>
    ''' L-Glutamine
    ''' </summary>
    ''' <returns></returns>
    Public Property Q As Integer
    ''' <summary>
    ''' Glycine
    ''' </summary>
    ''' <returns></returns>
    Public Property G As Integer
    ''' <summary>
    ''' L-Histidine
    ''' </summary>
    ''' <returns></returns>
    Public Property H As Integer
    ''' <summary>
    ''' L-Isoleucine
    ''' </summary>
    ''' <returns></returns>
    Public Property I As Integer
    ''' <summary>
    ''' L-Leucine
    ''' </summary>
    ''' <returns></returns>
    Public Property L As Integer
    ''' <summary>
    ''' L-Lysine
    ''' </summary>
    ''' <returns></returns>
    Public Property K As Integer
    ''' <summary>
    ''' L-Methionine
    ''' </summary>
    ''' <returns></returns>
    Public Property M As Integer
    ''' <summary>
    ''' L-Phenylalanine
    ''' </summary>
    ''' <returns></returns>
    Public Property F As Integer
    ''' <summary>
    ''' L-Proline
    ''' </summary>
    ''' <returns></returns>
    Public Property P As Integer
    ''' <summary>
    ''' L-Serine
    ''' </summary>
    ''' <returns></returns>
    Public Property S As Integer
    ''' <summary>
    ''' L-Threonine
    ''' </summary>
    ''' <returns></returns>
    Public Property T As Integer
    ''' <summary>
    ''' L-Tryptophan
    ''' </summary>
    ''' <returns></returns>
    Public Property W As Integer
    ''' <summary>
    ''' L-Tyrosine
    ''' </summary>
    ''' <returns></returns>
    Public Property Y As Integer
    ''' <summary>
    ''' L-Valine
    ''' </summary>
    ''' <returns></returns>
    Public Property V As Integer
    ''' <summary>
    ''' L-Selenocysteine
    ''' </summary>
    ''' <returns></returns>
    Public Property U As Integer
    ''' <summary>
    ''' L-Pyrrolysine
    ''' </summary>
    ''' <returns></returns>
    Public Property O As Integer

    Shared ReadOnly aa As PropertyInfo()

    Shared Sub New()
        aa = DataFramework.Schema(Of ProteinComposition)(PropertyAccess.Readable, True, True) _
            .Values _
            .Where(Function(p) p.Name.Length = 1) _
            .ToArray
    End Sub

    Public Shared Function FromRefSeq(sequence As String, proteinID As String) As ProteinComposition
        Dim protein As New ProteinComposition With {.proteinID = proteinID}
        Dim composition = sequence _
            .GroupBy(Function(a) a) _
            .ToDictionary(Function(a) CStr(a.Key),
                          Function(a)
                              Return a.Count
                          End Function)

        For Each aa As PropertyInfo In ProteinComposition.aa
            Call aa.SetValue(protein, composition.TryGetValue(aa.Name))
        Next

        Return protein
    End Function

    Public Iterator Function GetEnumerator() As IEnumerator(Of NamedValue(Of Double)) Implements IEnumerable(Of NamedValue(Of Double)).GetEnumerator
        For Each aminoAcid As PropertyInfo In aa
            Yield New NamedValue(Of Double)(aminoAcid.Name, aminoAcid.GetValue(Me))
        Next
    End Function

    Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Yield GetEnumerator()
    End Function
End Class
