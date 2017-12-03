#Region "Microsoft.VisualBasic::7ecbe89e31b9a096d942a914ea1fabcd, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\Medical\DrugGroup.vb"

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

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Assembly.KEGG.Medical

    Public Class DrugGroup : Implements INamedValue

        Public Property Entry As String Implements INamedValue.Key
        Public Property Names As String()
        Public Property Members As String()
        Public Property Remarks As String()
        Public Property Comments As String
        Public Property Targets As String()
        Public Property [Class] As NamedCollection(Of String)()
        Public Property Metabolism As NamedValue(Of String)()
        Public Property Interaction As NamedValue(Of String)()

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
