#Region "Microsoft.VisualBasic::fe421e3244f5dc47b02ad104d3d0dd6d, core\Bio.Assembly\ComponentModel\Annotation\CatalogProfiles.vb"

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

    '     Class CatalogProfile
    ' 
    '         Properties: profile
    ' 
    '         Function: (+2 Overloads) Add, GenericEnumerator, GetEnumerator, ToString
    ' 
    '     Class CatalogProfiles
    ' 
    '         Properties: catalogs
    ' 
    '         Function: GetProfiles, haveCategory, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ComponentModel.Annotation

    Public Class CatalogProfile : Implements Enumeration(Of NamedValue(Of Double))

        Public Property profile As New Dictionary(Of String, Double)

        Public Function Add(value As NamedValue(Of Double)) As CatalogProfile
            Call profile.Add(value.Name, value.Value)
            Return Me
        End Function

        Public Function Add(name As String, value As Double)
            Call profile.Add(name, value)
            Return Me
        End Function

        Public Overrides Function ToString() As String
            Return profile.Keys.GetJson
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of NamedValue(Of Double)) Implements Enumeration(Of NamedValue(Of Double)).GenericEnumerator
            For Each item In profile
                Yield New NamedValue(Of Double) With {
                    .Name = item.Key,
                    .Value = item.Value
                }
            Next
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of NamedValue(Of Double)).GetEnumerator
            Yield GenericEnumerator()
        End Function

        Public Overloads Shared Widening Operator CType(profile As NamedValue(Of Double)()) As CatalogProfile
            Return New CatalogProfile With {
                .profile = profile.ToDictionary.FlatTable
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

    Public Class CatalogProfiles

        Public Property catalogs As New Dictionary(Of String, CatalogProfile)

        Default Public ReadOnly Property GetCatalogValue(name As String) As CatalogProfile
            Get
                Return catalogs.TryGetValue(name)
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function haveCategory(name As String) As Boolean
            Return catalogs.ContainsKey(name)
        End Function

        Public Iterator Function GetProfiles() As IEnumerable(Of NamedValue(Of CatalogProfile))
            For Each catalog In catalogs
                Yield New NamedValue(Of CatalogProfile) With {
                    .Name = catalog.Key,
                    .Value = catalog.Value
                }
            Next
        End Function

        Public Overrides Function ToString() As String
            Return catalogs.Keys.GetJson
        End Function

    End Class
End Namespace
