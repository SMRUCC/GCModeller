#Region "Microsoft.VisualBasic::98dd192408632bea11b0a3ce17763ff8, engine\Model\Cellular\Vector\ProteinComposition.vb"

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

    '   Total Lines: 164
    '    Code Lines: 61 (37.20%)
    ' Comment Lines: 91 (55.49%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 12 (7.32%)
    '     File Size: 5.18 KB


    '     Class ProteinComposition
    ' 
    '         Properties: A, C, D, E, F
    '                     G, H, I, K, L
    '                     M, N, O, P, proteinID
    '                     Q, R, S, T, U
    '                     V, W, Y
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: FromRefSeq, GetEnumerator, IEnumerable_GetEnumerator, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Namespace Cellular.Vector

    ''' <summary>
    ''' the amino acid composition vector
    ''' </summary>
    Public Class ProteinComposition : Implements IEnumerable(Of NamedValue(Of Double)), INamedValue

        ''' <summary>
        ''' is the polypeptide unique reference id
        ''' </summary>
        ''' <returns></returns>
        Public Property proteinID As String Implements INamedValue.Key

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
        ''' * L-Pyrrolysine
        ''' </summary>
        ''' <returns></returns>
        Public Property O As Integer

        Friend Shared ReadOnly aa As PropertyInfo()

        Shared Sub New()
            aa = DataFramework.Schema(Of ProteinComposition)(PropertyAccess.Readable, True, True) _
                .Values _
                .Where(Function(p) p.Name.Length = 1) _
                .OrderBy(Function(p) p.Name) _
                .ToArray
        End Sub

        Public Shared Function Blank(protId As String) As ProteinComposition
            Return New ProteinComposition With {.proteinID = protId}
        End Function

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

        Public Overrides Function ToString() As String
            Return proteinID
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
End Namespace
