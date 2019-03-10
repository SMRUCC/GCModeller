﻿#Region "Microsoft.VisualBasic::b180b5fa37082d9eac7291201e18df38, RDotNET.Extensions.VisualBasic\ScriptBuilder\RPackage\dynamicTreeCut\dynamicTreeCut.vb"

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

    '     Module Func
    ' 
    '         Function: printFlush
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Linq
Imports RDotNET.Extensions.VisualBasic.SymbolBuilder

Namespace SymbolBuilder.packages.dynamicTreeCut

    Public Module Func

        ''' <summary>
        ''' Passes all its arguments unchaged to the standard print function; after the execution of print it flushes the console, if possible.
        ''' </summary>
        ''' <param name="args">Arguments to be passed to the standard print function.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Passes all its arguments unchaged to the standard print function; after the execution of print it flushes the console, if possible.
        ''' </remarks>
        Public Function printFlush(ParamArray args As Object()) As String
            Dim x As String() = args.Select(Function(obj) Scripting.ToString(obj)).ToArray
            Dim xs As String = String.Join(", ", x)
            Return $"printFlush({xs})"
        End Function
    End Module
End Namespace
