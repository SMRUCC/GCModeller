#Region "Microsoft.VisualBasic::a7ef2da3c24fc5453c6eab410568e0fa, core\Bio.Assembly\Assembly\KEGG\Medical\Drug.vb"

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

    '     Class Drug
    ' 
    '         Properties: Activity, Atoms, Bounds, Comments, DBLinks
    '                     Entry, Exact_Mass, Formula, Interaction, Metabolism
    '                     Mol_Weight, Names, Remarks, Source, Targets
    ' 
    '         Function: ToString
    ' 
    '     Class Bound
    ' 
    '         Properties: a, b, Edit, index, N
    ' 
    '         Function: ToString
    ' 
    '         Sub: Assign
    ' 
    '     Class Atom
    ' 
    '         Properties: Atom, Charge, Edit, Formula, index
    '                     M
    ' 
    '         Function: ToString
    ' 
    '         Sub: Assign
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace Assembly.KEGG.Medical

    Public Class Drug : Implements INamedValue, IKEGGRemarks

        Public Property Entry As String Implements INamedValue.Key
        Public Property Names As String()
        Public Property Formula As String
        Public Property Exact_Mass As Double
        Public Property Mol_Weight As Double
        Public Property Remarks As String() Implements IKEGGRemarks.Remarks
        Public Property Activity As String
        Public Property DBLinks As DBLink()
        Public Property Atoms As Atom()
        Public Property Bounds As Bound()
        Public Property Comments As String()
        Public Property Targets As String()
        Public Property Metabolism As NamedValue(Of String)()
        Public Property Interaction As NamedValue(Of String)()
        Public Property Source As String()

        Public ReadOnly Property CompoundID As String()
            Get
                If Remarks.IsNullOrEmpty Then
                    Return {}
                End If

                Dim table = Remarks _
                    .Select(Function(s)
                                Return s.GetTagValue(":", trim:=True)
                            End Function) _
                    .ToDictionary() _
                    .FlatTable

                If table.ContainsKey("Same as") Then
                    ' 可能会对应多个Compound
                    Return table("Same as") _
                        .Split _
                        .Select(AddressOf Trim) _
                        .Where(Function(id) id.First = "C"c) _
                        .ToArray
                Else
                    Return {}
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return GetJson
        End Function

    End Class

    Public Class Bound : Implements IAddress(Of Integer)

        Public Property index As Integer Implements IAddress(Of Integer).Address
        Public Property a As Integer
        Public Property b As Integer
        Public Property N As Integer
        Public Property Edit As String

        Public Sub Assign(address As Integer) Implements IAddress(Of Integer).Assign
            index = address
        End Sub

        Public Overrides Function ToString() As String
            Return $"[{index}] {a} <-{N}-> {b} {Edit}"
        End Function
    End Class

    Public Class Atom : Implements IAddress(Of Integer)

        Public Property index As Integer Implements IAddress(Of Integer).Address
        Public Property Formula As String
        Public Property Atom As String
        Public Property M As Double
        Public Property Charge As Double
        Public Property Edit As String

        Public Sub Assign(address As Integer) Implements IAddress(Of Integer).Assign
            index = address
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
