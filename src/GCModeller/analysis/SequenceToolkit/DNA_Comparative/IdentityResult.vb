#Region "Microsoft.VisualBasic::3dab9c194999514edac8e12fb137b172, ..\GCModeller\analysis\SequenceToolkit\DNA_Comparative\IdentityResult.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' 核酸序列的一致性的计算结果
''' </summary>
Public Class IdentityResult

    Public Property SeqId As String

    <Meta>
    Public Property Identities As Dictionary(Of String, Double)

    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function

    Public Shared Function SimpleTag(fa As FastaToken) As String
        Return fa.Title.Split.First
    End Function

    Public Shared Iterator Function SigmaMatrix(source As FastaFile, Optional round As Integer = -1, Optional simple As Boolean = True) As IEnumerable(Of IdentityResult)
        Dim nts As NucleicAcid() =
            source.ToArray(Function(x) New NucleicAcid(x), Parallel:=True)
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
            Dim result As List(Of NamedValue(Of Double)) =
                LinqAPI.MakeList(Of NamedValue(Of Double)) <=
                    From x As NucleicAcid
                    In nts.AsParallel
                    Where Not x Is nt
                    Let sigma As Double = DifferenceMeasurement.Sigma(nt, x)
                    Select New NamedValue(Of Double) With {
                        .Name = getTag(x),
                        .x = getValue(sigma * 1000)
                    }
            result += New NamedValue(Of Double) With {
                .Name = getTag(nt),
                .x = 0R
            }

            Call nt.UserTag.__DEBUG_ECHO

            Yield New IdentityResult With {
                .Identities = result.ToDictionary(Function(x) x.Name, Function(x) x.x),
                .SeqId = nt.UserTag
            }
        Next
    End Function
End Class
