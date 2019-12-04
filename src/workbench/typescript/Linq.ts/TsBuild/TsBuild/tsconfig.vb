#Region "Microsoft.VisualBasic::e913bf91b3ac7ebf9d6b692a4c39a9e8, KEGG_canvas\typescript\Linq\TsBuild\TsBuild\tsconfig.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



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