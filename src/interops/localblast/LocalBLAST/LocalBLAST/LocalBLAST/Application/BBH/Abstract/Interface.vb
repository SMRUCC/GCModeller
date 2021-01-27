﻿#Region "Microsoft.VisualBasic::d7b0ca27ef0b614670d74db93e9be357, LocalBLAST\LocalBLAST\LocalBLAST\Application\BBH\Abstract\Interface.vb"

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

    '     Interface IBlastHit
    ' 
    '         Properties: hitName, queryName
    ' 
    '     Interface IQueryHits
    ' 
    '         Properties: identities
    ' 
    '     Module Extensions
    ' 
    '         Function: isEmpty
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.BLASTOutput

Namespace LocalBLAST.Application.BBH.Abstract

    Public Interface IBlastHit
        Property queryName As String
        Property hitName As String
    End Interface

    Public Interface IQueryHits : Inherits IBlastHit
        ReadOnly Property identities As Double
    End Interface

    <HideModuleName> Public Module Extensions

        ''' <summary>
        ''' 目标比对结果是否是空的?
        ''' 
        ''' 1. 对象是空值
        ''' 2. 空的<see cref="IQueryHits.queryName"/>
        ''' 3. 空的<see cref="IQueryHits.hitName"/>或者其等于<see cref="IBlastOutput.HITS_NOT_FOUND"/>
        ''' </summary>
        ''' <param name="hit"></param>
        ''' <returns></returns>
        <Extension> Public Function isEmpty(hit As IQueryHits) As Boolean
            If hit Is Nothing Then Return True
            If String.IsNullOrEmpty(hit.queryName) Then Return True
            If String.IsNullOrEmpty(hit.hitName) OrElse String.Equals(hit.hitName, IBlastOutput.HITS_NOT_FOUND) Then
                Return True
            End If

            Return False
        End Function
    End Module
End Namespace
