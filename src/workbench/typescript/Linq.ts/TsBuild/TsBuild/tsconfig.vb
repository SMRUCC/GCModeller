#Region "Microsoft.VisualBasic::97d0c0a339e78f9602f692e35093f283, typescript\Linq.ts\TsBuild\TsBuild\tsconfig.vb"

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

    ' Class tsconfig
    ' 
    '     Properties: compileOnSave, compilerOptions, exclude, extends, files
    '                 include
    ' 
    '     Function: ToString
    ' 
    ' Class compilerOptions
    ' 
    '     Properties: [lib], [module], allowSyntheticDefaultImports, declaration, noImplicitAny
    '                 outDir, outFile, preserveConstEnums, removeComments, sourceMap
    '                 target
    ' 
    ' Enum ModuleTypes
    ' 
    '     AMD, CommonJS, ES2015, ES6, ESNext
    '     None, System, UMD
    ' 
    '  
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' The presence of a tsconfig.json file in a directory indicates that the directory is the 
''' root of a TypeScript project. The tsconfig.json file specifies the root files and the 
''' compiler options required to compile the project. A project is compiled in one of the 
''' following ways:
'''
''' #### Using tsconfig.json
''' 
''' 1. By invoking tsc With no input files, In which Case the compiler searches For the 
'''    tsconfig.json file starting In the current directory And continuing up the parent 
'''    directory chain.
''' 2. By invoking tsc With no input files And a --project (Or just -p) command line Option 
'''    that specifies the path Of a directory containing a tsconfig.json file, Or a path To 
'''    a valid .json file containing the configurations.
'''    
''' When input files are specified on the command line, tsconfig.json files are ignored.
''' </summary>
Public Class tsconfig

    Public Property compilerOptions As compilerOptions
    Public Property compileOnSave As Boolean

    Public Property files As String()
    Public Property exclude As String()
    Public Property include As String()
    Public Property extends As String

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function ToString() As String
        Return Me.GetJson
    End Function
End Class

Public Class compilerOptions
    Public Property [module] As ModuleTypes
    Public Property target As String
    Public Property [lib] As String()
    Public Property sourceMap As Boolean
    Public Property allowSyntheticDefaultImports As Boolean
    Public Property outDir As String
    Public Property outFile As String
    Public Property declaration As Boolean
    Public Property noImplicitAny As Boolean
    Public Property removeComments As Boolean
    Public Property preserveConstEnums As Boolean
End Class

''' <summary>
''' + Only "AMD" and "System" can be used in conjunction with --outFile.
''' + "ES6" and "ES2015" values may be used when targeting "ES5" or lower.
''' </summary>
Public Enum ModuleTypes
    None
    CommonJS
    AMD
    System
    UMD
    ES6
    ES2015
    ESNext
End Enum
