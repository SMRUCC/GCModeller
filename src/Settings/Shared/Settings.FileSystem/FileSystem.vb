#Region "Microsoft.VisualBasic::1f363922baccdc82916c5ae39e48cb96, Shared\Settings.FileSystem\FileSystem.vb"

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

'     Module FileSystem
' 
' 
'     Module FileSystem
' 
'         Properties: CDD, COGs, Correlations, GO, InterproXml
'                     IsNullOrEmpty, KEGGFamilies, MotifLDM, RegpreciseBBH, RegPreciseRegulatorFasta
'                     RegpreciseRoot, Regulations, RepositoryRoot
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: GetLocalblast, (+2 Overloads) GetMotifLDM, GetPfamDb, GetR_HOME, GetRegpreciseBBH
'                   GetRegpreciseRoot, GetRegulations, GetRepositoryRoot, IsRepositoryNullOrEmpty, TryGetSource
' 
' 
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.FileIO.Path
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace GCModeller.FileSystem

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>由于可能会修改参数然后在调用的这种情况出现，所以这里的数据可能需要实时更新，所以不再使用属性的简写形式了</remarks>
    ''' 
#If ENABLE_API_EXPORT Then
    <Package("GCModeller.Repository.FileSystem", Publisher:="amethyst.asuka@gcmodeller.org")>
    Module FileSystem
#Else
    Module FileSystem
#End If

        ''' <summary>
        ''' 这个是为了加载数据而构建的，故而假若数据源不存在的话就会返回备用的
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="alt"></param>
        ''' <returns></returns>
        Public Function TryGetSource(source As String, alt As Func(Of String)) As String
            If Not source.DirectoryExists Then
                Return alt()
            Else
                Return source
            End If
        End Function

        Sub New()
            If Not Settings.Session.Initialized Then
                Call Settings.Session.Initialize()
            End If
        End Sub

        Const RepositoryNotInitialized$ = "The repository root directory path variable in the settings file is not initialized yet, using command ""Settings set RepositoryRoot <DIR>"" for initialize the repository location."
        Const RepositoryHandleInvalid$ = "The configured repository path ""{0}"" is not exists on your file system!"

        ''' <summary>
        ''' The root directory for stores the GCModeller database such as fasta sequence for annotation.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property RepositoryRoot As String
            Get
                Dim DIR$ = Settings.Session.SettingsFile.RepositoryRoot

                If DIR.StringEmpty Then
                    Call RepositoryNotInitialized.Warning
                ElseIf Not DIR.DirectoryExists Then
                    Call String.Format(RepositoryHandleInvalid, DIR).Warning
                End If

                Return DIR$
            End Get
        End Property

        ''' <summary>
        ''' &lt;RepositoryRoot>/Regprecise/
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property RegpreciseRoot As String
            Get
                Return RepositoryRoot & "/Regprecise/"
            End Get
        End Property

        ''' <summary>
        ''' &lt;RegpreciseRoot>/MEME/MAST_LDM/
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property MotifLDM As String
            Get
                Return RegpreciseRoot & "/MEME/MAST_LDM/"
            End Get
        End Property

        Public ReadOnly Property RegpreciseBBH As String
            Get
                Return RegpreciseRoot & "/MEME/bbh/"
            End Get
        End Property

        ''' <summary>
        ''' regulations.xml文件在GCModeller数据库之中的位置
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Regulations As String
            Get
                Return RegpreciseRoot & "/MEME/regulations.xml"
            End Get
        End Property

        Public ReadOnly Property RegPreciseRegulatorFasta As String
            Get
                Return RegpreciseRoot & "/Fasta/regulators/"
            End Get
        End Property

        ''' <summary>
        ''' 配置文件之中是否包含有GCModeller数据库的位置的路径参数
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsNullOrEmpty As Boolean
            Get
                Return String.IsNullOrEmpty(RepositoryRoot)
            End Get
        End Property

        Public ReadOnly Property Correlations As String
            Get
                Return FileSystem.RepositoryRoot & "/Correlations/"
            End Get
        End Property

        Public ReadOnly Property InterproXml As String
            Get
                Return FileSystem.RepositoryRoot & "/Interpro/interpro.xml"
            End Get
        End Property

        ''' <summary>
        ''' Regprecise数据库之中的调控因子蛋白质的摘要Dump信息
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property KEGGFamilies As String
            Get
                Return RegpreciseRoot & "/MEME/prot-regulators_KEGG.csv"
            End Get
        End Property

        ''' <summary>
        ''' NCBI CDD数据库的文件夹位置
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property CDD As String
            Get
                Return RepositoryRoot & "/CDD/"
            End Get
        End Property

        ''' <summary>
        ''' GO数据库的文件夹位置
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property GO As String
            Get
                Return RepositoryRoot & "/GO/"
            End Get
        End Property

        ''' <summary>
        ''' COG数据库文件夹
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property COGs As String
            Get
                Return RepositoryRoot & "/COGs/"
            End Get
        End Property

#Region "API Functions"

        ''' <summary>
        ''' The root directory for stores the GCModeller database such as fasta sequence for annotation.
        ''' </summary>
        ''' <returns></returns>
        ''' 
        Public Function GetRepositoryRoot() As String
            Return Settings.Session.SettingsFile.RepositoryRoot
        End Function

        <ExportAPI("RegpreciseRoot")>
        Public Function GetRegpreciseRoot() As String
            Return RepositoryRoot & "/Regprecise/"
        End Function

        ''' <summary>
        ''' &lt;RegpreciseRoot>/MEME/MAST_LDM/
        ''' </summary>
        ''' <returns></returns>
        <ExportAPI("MotifLDM")>
        Public Function GetMotifLDM() As String
            Return RegpreciseRoot & "/MEME/MAST_LDM/"
        End Function

        ''' <summary>
        ''' &lt;RegpreciseRoot>/MEME/MAST_LDM/
        ''' </summary>
        ''' <returns></returns>
        <ExportAPI("MotifLDM")>
        Public Function GetMotifLDM(Name As String) As String
            Return FileSystem.GetMotifLDM() & "/" & Name & ".Xml"
        End Function

        <ExportAPI("RegpreciseBBH")>
        Public Function GetRegpreciseBBH() As String
            Return RegpreciseRoot & "/MEME/bbh/"
        End Function

        ''' <summary>
        ''' regulations.xml文件在GCModeller数据库之中的位置
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Regulations.DB")>
        Public Function GetRegulations() As String
            Return RegpreciseRoot & "/MEME/regulations.xml"
        End Function

        ''' <summary>
        ''' Do you configured the repository root directory path parameter in the settings file?
        ''' 
        ''' 配置文件之中是否包含有GCModeller数据库的位置的路径参数
        ''' </summary>
        ''' <returns></returns>
        ''' 
        Public Function IsRepositoryNullOrEmpty() As Boolean
            Return String.IsNullOrEmpty(RepositoryRoot)
        End Function

        ''' <summary>
        ''' 默认返回NCBI CDD数据库之中的Pfam数据库
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        <ExportAPI("Get.PfamDb")>
        Public Function GetPfamDb(Optional name As String = "") As String
            Dim prefix As String = FileSystem.CDD & "/Pfam.fasta"
            Dim misc As String = FileSystem.CDD & $"/Misc/{name}.fasta"

            If String.IsNullOrEmpty(name) AndAlso prefix.FileExists Then
                Return prefix
            ElseIf name.FileExists Then
                Return name
            ElseIf misc.FileExists Then
                Return misc
            End If

            Dim PfamA As String = FileSystem.RepositoryRoot & "/Pfam/Pfam-A.fasta"

            If PfamA.FileExists Then
                Return PfamA
            Else
                Dim exMsg As String =
                    "Unable retrive the protein domain database file source!" & vbCrLf &
                   $"    Name    =>  '{name}'" & vbCrLf &
                   $"     CDD    =>  '{FileSystem.CDD}' " & vbCrLf &
                   $"  Pfam-A    =>  '{PfamA}'" & vbCrLf &
                   $" Repository =>  '{FileSystem.RepositoryRoot}'"
                Call exMsg.__DEBUG_ECHO
                Call App.LogException(New Exception(exMsg))

                Return ""
            End If
        End Function

#If Not DISABLE_BUG_UNKNOWN Then

        ''' <summary>
        ''' This function will search for the blast+ bin directory automatically based on the 
        ''' registry, config data and system directories.
        ''' (会自动搜索注册表，配置文件和文件系统之上的目录，实在找不到会返回空字符串并且记录下错误)
        ''' </summary>
        ''' <returns></returns>
        ''' 
        <ExportAPI("GetLocalBlast")>
        Public Function GetLocalblast() As String
            Dim blast As String = Settings.Session.SettingsFile.BlastBin

            If blast.DirectoryExists Then
                Return blast
            End If

            Dim lstPath As String() = Environment.GetEnvironmentVariable("PATH").Split(";"c)

            blast = lstPath _
                .Where(Function(path$) InStr(path, "blast", CompareMethod.Text) > 0) _
                .FirstOrDefault

            If blast.DirectoryExists Then
                Return blast
            End If

            Dim getlstPath = ProgramPathSearchTool.SearchDirectory("blast").ToArray

            If getlstPath.IsNullOrEmpty Then
NO_DIR:
                Dim exMsg As String =
                    "Unable retrive the blast program directory path!" & vbCrLf &
                   $"    list of environment path:" & vbCrLf & lstPath.JoinBy(vbCrLf & "         ")
                Call exMsg.__DEBUG_ECHO
                Call App.LogException(New Exception(exMsg))

                Return ""
            End If

            For Each path As String In getlstPath
                If Not FileIO.FileSystem.GetFiles(path, FileIO.SearchOption.SearchAllSubDirectories, "blastp.exe").IsNullOrEmpty Then
                    Return path & "/bin/"
                End If
            Next

            GoTo NO_DIR
        End Function

        <ExportAPI("Get.R_HOME")>
        Public Function GetR_HOME() As String
            Dim R_HOME As String = Settings.Session.SettingsFile.R_HOME

            If R_HOME.DirectoryExists Then
                Return R_HOME
            End If

            Dim Directories = ProgramPathSearchTool.SearchDirectory("R").ToArray

            If Directories.IsNullOrEmpty Then
                Return ""
            End If

            For Each R As String In (From s As String In Directories Select s Order By s.Length Descending)
                If FileIO.FileSystem.GetFiles(R, FileIO.SearchOption.SearchTopLevelOnly, "R*.exe").Count > 1 Then
                    Return R
                End If

                Dim EXEList = ProgramPathSearchTool.SearchProgram(R, "R")

                If EXEList.Count > 1 Then
                    R = RepositoryFileSystem.GetMostAppreancePath(EXEList)
                    Return R
                End If
            Next

            Return ""
        End Function
#End If
#End Region

    End Module
End Namespace
