﻿#Region "Microsoft.VisualBasic::4db41bad8a478c9d555814700ec942fa, Bio.Assembly\Assembly\NCBI\WebServiceHandler\ComponentModel.vb"

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

    '     Class I_QueryEntry
    ' 
    '         Properties: Title, URL
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Language

Namespace Assembly.NCBI.Entrez.ComponentModels

    ''' <summary>
    ''' 用于表示获取查询结果的一个入口点
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class I_QueryEntry : Inherits BaseClass
        Implements IKeyValuePairObject(Of String, String)

        Public Property Title As String Implements IKeyValuePairObject(Of String, String).Value
        Public Property URL As String Implements IKeyValuePairObject(Of String, String).Key

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class
End Namespace
