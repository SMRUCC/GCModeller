﻿#Region "Microsoft.VisualBasic::1db3519fe8d4eae9d103765c300cf18b, sub-system\PLAS.NET\SSystem\Compiler.vb"

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


    ' Code Statistics:

    '   Total Lines: 26
    '    Code Lines: 17 (65.38%)
    ' Comment Lines: 4 (15.38%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 5 (19.23%)
    '     File Size: 975 B


    ' Module Compiler
    ' 
    ' 
    '     Delegate Function
    ' 
    '         Properties: Compilers
    ' 
    '         Function: SBMLCompiler, ScriptCompiler
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.Analysis.SSystem.Script

Public Module Compiler

    Public Delegate Function ICompiler(input As String, out As String, autoFix As Boolean) As Boolean

    ''' <summary>
    ''' 格式名称的大小写不敏感
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Compilers As IReadOnlyDictionary(Of String, ICompiler) =
        New HashDictionary(Of ICompiler) From {
 _
            {"script", AddressOf ScriptCompiler},
            {"sbml", AddressOf SBMLCompiler}
    }

    Public Function ScriptCompiler(input As String, out As String, autoFix As Boolean) As Boolean
        Return Script.ScriptCompiler.Compile(input, autoFix).Save(out)
    End Function

    Public Function SBMLCompiler(input As String, out As String, autoFix As Boolean) As Boolean
        Return SBML.Compile(input, autoFix).Save(out)
    End Function
End Module
