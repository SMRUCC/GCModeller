#Region "Microsoft.VisualBasic::2894bcbe84bb520ea69616a7c35e3711, core\Bio.Assembly\ComponentModel\Annotation\CatalogProfiles.vb"

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
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ComponentModel.Annotation

    Public Class CatalogProfiles

        Public Property catalogs As New Dictionary(Of String, CatalogProfile)

        Default Public ReadOnly Property GetCatalogValue(name As String) As CatalogProfile
            Get
                Return catalogs.TryGetValue(name)
            End Get
        End Property

        Sub New()
        End Sub

        Sub New(data As Dictionary(Of String, NamedValue(Of Integer)()))

        End Sub

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
