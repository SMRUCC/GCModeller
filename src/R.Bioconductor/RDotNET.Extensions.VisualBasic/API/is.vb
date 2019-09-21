#Region "Microsoft.VisualBasic::3c163e4098b540dfd05c4d0bdf6468a6, RDotNET.Extensions.VisualBasic\API\is.vb"

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

    '     Module stats
    ' 
    '         Function: ts
    ' 
    '     Module base
    ' 
    '         Function: vector
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

Namespace API.is

    ' 2017-6-28
    '
    ' 在这里是使用命名空间[as]来实现as.XXX的R函数名称语法的
    ' 函数在as命名空间下被分散在不同的module之中表示来自于不同的R API的命名空间
    ' [is]命名空间之中的设计也是如此
    ' 所以不需要专门修改这个命名空间下的module为class了

    Public Module stats

        ''' <summary>
        ''' as.ts and is.ts coerce an object to a time-series and test whether an object is a time series.
        ''' </summary>
        ''' <param name="x">an arbitrary R object.</param>
        ''' <returns>
        ''' ``is.ts`` tests if an object is a time series. It is generic: you can write methods to handle 
        ''' specific classes of objects, see InternalMethods.
        ''' </returns>
        Public Function ts(x As String) As Boolean
            Return $"is.ts({x})".__call.AsBoolean
        End Function
    End Module

    Public Module base

        ''' <summary>
        ''' is.vector returns TRUE if x is a vector of the specified mode having no attributes other than names. 
        ''' It returns FALSE otherwise.
        ''' </summary>
        ''' <param name="x$"></param>
        ''' <param name="mode$"></param>
        ''' <returns></returns>
        Public Function vector(x$, Optional mode$ = "any") As String
            Dim var$ = RDotNetGC.Allocate

            SyncLock R
                With R
                    .call = $"{var} <- is.vector({x}, mode = {Rstring(mode)})"
                End With
            End SyncLock

            Return var
        End Function
    End Module
End Namespace
