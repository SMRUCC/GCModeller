#Region "Microsoft.VisualBasic::884b449a80eeb15e762b737005b4fc3b, GCModeller\core\Bio.Assembly\ComponentModel\Annotation\Profiles\CatalogProfile.vb"

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

    '   Total Lines: 110
    '    Code Lines: 87
    ' Comment Lines: 3
    '   Blank Lines: 20
    '     File Size: 3.98 KB


    '     Class CatalogProfile
    ' 
    '         Properties: information, isEmpty, profile
    ' 
    '         Constructor: (+4 Overloads) Sub New
    '         Function: (+2 Overloads) Add, GenericEnumerator, GetEnumerator, OrderByValues, Take
    '                   ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ComponentModel.Annotation

    ''' <summary>
    ''' a wrapper of <see cref="Dictionary(Of String, Double)"/>
    ''' </summary>
    Public Class CatalogProfile : Implements Enumeration(Of NamedValue(Of Double))

        Public Property profile As New Dictionary(Of String, Double)
        Public Property information As New Dictionary(Of String, String)

        ''' <summary>
        ''' does the <see cref="profile"/> is empty?
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property isEmpty As Boolean
            Get
                Return profile.IsNullOrEmpty
            End Get
        End Property

        Sub New()
        End Sub

        Sub New(data As IEnumerable(Of NamedValue(Of Double)))
            For Each item In data
                profile(item.Name) = item.Value
                information(item.Name) = item.Description
            Next
        End Sub

        Sub New(copy As CatalogProfile)
            profile = New Dictionary(Of String, Double)(copy.profile)
            information = New Dictionary(Of String, String)(copy.information)
        End Sub

        Sub New(data As IEnumerable(Of NamedValue(Of Integer)))
            For Each item In data
                profile(item.Name) = item.Value
                information(item.Name) = item.Description
            Next
        End Sub

        Public Function Add(value As NamedValue(Of Double)) As CatalogProfile
            Call profile.Add(value.Name, value.Value)
            Call information.Add(value.Name, value.Description)

            Return Me
        End Function

        Public Function Add(name As String, value As Double) As CatalogProfile
            Call profile.Add(name, value)
            Return Me
        End Function

        ''' <summary>
        ''' just sort desc
        ''' </summary>
        ''' <returns></returns>
        Public Function OrderByValues() As CatalogProfile
            Return New CatalogProfile With {
                .information = New Dictionary(Of String, String)(information),
                .profile = profile _
                    .OrderByDescending(Function(a) a.Value) _
                    .ToDictionary
            }
        End Function

        ''' <summary>
        ''' sort desc and then take top N
        ''' </summary>
        ''' <param name="topN"></param>
        ''' <returns></returns>
        Public Function Take(topN As Integer) As CatalogProfile
            Return New CatalogProfile With {
                .information = New Dictionary(Of String, String)(information),
                .profile = profile _
                    .OrderByDescending(Function(a) a.Value) _
                    .Take(topN) _
                    .ToDictionary
            }
        End Function

        Public Overrides Function ToString() As String
            Return profile.Keys.GetJson
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of NamedValue(Of Double)) Implements Enumeration(Of NamedValue(Of Double)).GenericEnumerator
            For Each item In profile
                Yield New NamedValue(Of Double) With {
                    .Name = item.Key,
                    .Value = item.Value,
                    .Description = information.TryGetValue(item.Key)
                }
            Next
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of NamedValue(Of Double)).GetEnumerator
            Yield GenericEnumerator()
        End Function

        Public Overloads Shared Widening Operator CType(profile As NamedValue(Of Double)()) As CatalogProfile
            Return New CatalogProfile With {
                .profile = profile.ToDictionary.FlatTable,
                .information = profile.ToDictionary(Function(a) a.Name, Function(a) a.Description)
            }
        End Operator

        Public Shared Narrowing Operator CType(profile As CatalogProfile) As NamedValue(Of Double)()
            If profile Is Nothing Then
                Return {}
            Else
                Return profile.AsEnumerable.ToArray
            End If
        End Operator
    End Class

End Namespace
