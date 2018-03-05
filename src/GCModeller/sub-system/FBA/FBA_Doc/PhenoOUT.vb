#Region "Microsoft.VisualBasic::b22c4bfd5d2a46be51d46df2de43491b, sub-system\FBA\FBA_Doc\PhenoOUT.vb"

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

    '     Class PhenoOUT
    ' 
    '         Properties: Properties
    ' 
    '     Interface IPhenoOUT
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection

Namespace Models.rFBA

    ''' <summary>
    ''' 基本类型之中的flux是野生型的数据
    ''' </summary>
    Public Class PhenoOUT : Inherits FBA_OUTPUT.TabularOUT
        Implements IDynamicMeta(Of Double)
        Implements IPhenoOUT

        Dim _props As Dictionary(Of String, Double)

        <Meta(GetType(Double))>
        Public Property Properties As Dictionary(Of String, Double) Implements IDynamicMeta(Of Double).Properties
            Get
                If _props Is Nothing Then
                    _props = New Dictionary(Of String, Double)
                End If
                Return _props
            End Get
            Set(value As Dictionary(Of String, Double))
                _props = value
            End Set
        End Property
    End Class

    Public Interface IPhenoOUT : Inherits INamedValue, IDynamicMeta(Of Double)
    End Interface
End Namespace
