﻿#Region "Microsoft.VisualBasic::75fed9939a89163f4d0355bed75c86f6, Microsoft.VisualBasic.Core\src\ApplicationServices\VBDev\NetCore5\target.vb"

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

    '     Class target
    ' 
    '         Properties: dependencies, LibraryFile, runtime, runtimeTargets
    ' 
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Namespace ApplicationServices.Development.NetCore5

    Public Class target

        Public Property dependencies As Dictionary(Of String, String)
        Public Property runtime As Dictionary(Of String, runtime)
        Public Property runtimeTargets As Dictionary(Of String, runtimeTarget)

        Public ReadOnly Property LibraryFile As String
            Get
                If runtime.IsNullOrEmpty Then
                    Return Nothing
                Else
                    Return runtime.Keys _
                        .Where(Function(fileName)
                                   ' skip of the system dll files
                                   Return (Not fileName.Contains("/")) AndAlso fileName.ExtensionSuffix("dll")
                               End Function) _
                        .FirstOrDefault
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return LibraryFile
        End Function

    End Class

End Namespace
