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
    Public Property [module] As String
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