#Region "Microsoft.VisualBasic::d8e6989b21a654d5c90fb486f93bdfb1, Bio.Assembly\Metagenomics\OTUData.vb"

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

    '     Class OTUData
    ' 
    '         Properties: data, OTU, taxonomy
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

#If netcore5 = 0 Then
Imports System.Data.Linq.Mapping
#Else
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
#End If
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Metagenomics

    ''' <summary>
    ''' <see cref="OTUData.Data"/> that associated with <see cref="OTUData.OTU"/> tag
    ''' </summary>
    Public Class OTUData : Implements INamedValue
        Implements ITaxonomyLineage

        ''' <summary>
        ''' ``#OTU_num``
        ''' </summary>
        ''' <returns></returns>
        <Column(Name:="#OTU_num")>
        Public Property OTU As String Implements INamedValue.Key
        ''' <summary>
        ''' Usually this property is the BIOM format taxonomy information
        ''' </summary>
        ''' <returns></returns>
        Public Property taxonomy As String Implements ITaxonomyLineage.Taxonomy

        ''' <summary>
        ''' 每一个样本的OTU含量或者其他的结果数据
        ''' </summary>
        ''' <returns></returns>
        Public Property data As Dictionary(Of String, String)

        Sub New()
        End Sub

        ''' <summary>
        ''' Copy data
        ''' </summary>
        ''' <param name="data"></param>
        Sub New(data As OTUData)
            With Me
                .OTU = data.OTU
                .taxonomy = data.taxonomy
                .data = New Dictionary(Of String, String)(data.data)
            End With
        End Sub

        Public Overrides Function ToString() As String
            Return $"{taxonomy Or OTU.AsDefault}: {data.GetJson(indent:=True)}"
        End Function
    End Class
End Namespace
