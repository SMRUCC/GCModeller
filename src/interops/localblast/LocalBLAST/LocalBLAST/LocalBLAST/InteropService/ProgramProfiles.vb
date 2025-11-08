#Region "Microsoft.VisualBasic::b9c370c613a3ceac14e4bf551112fb48, localblast\LocalBLAST\LocalBLAST\LocalBLAST\InteropService\ProgramProfiles.vb"

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

    '   Total Lines: 183
    '    Code Lines: 145 (79.23%)
    ' Comment Lines: 14 (7.65%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 24 (13.11%)
    '     File Size: 9.48 KB


    '     Class ProgramProfiles
    ' 
    '         Properties: BLASTPlus, ExecutableCommands, LocalBLAST, MoltypeNucleotide, MolTypeProtein
    '                     Name, RpsBLAST
    ' 
    '         Function: GetCommand, Load, ToString
    ' 
    '     Class Executable
    ' 
    '         Properties: AssemblyCommand, Name, Parameters
    ' 
    '         Function: GetValue, ToString
    '         Class Executable_BLAST
    ' 
    '             Function: CreateCommand, ToString
    ' 
    '         Class Executable_BuildDB
    ' 
    '             Function: CreateCommand
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace LocalBLAST.InteropService

    Public NotInheritable Class ProgramProfiles

#Region "Profiles Value"

        Public Shared ReadOnly Property LocalBLAST As ProgramProfiles =
            New ProgramProfiles With {
                .Name = "LocalBLAST",
                .ExecutableCommands = New Executable() {
 _
                    New Executable.Executable_BuildDB With {
                        .Name = "builddb",
                        .AssemblyCommand = "formatdb.exe",
                        .Parameters = New KeyValuePair() {
 _
                            New KeyValuePair With {.Key = "targetdb", .Value = "-i"},
                            New KeyValuePair With {.Key = "dbtype", .Value = "-p"}}
                    },
                    New Executable.Executable_BLAST With {
                        .Name = "blastn",
                        .AssemblyCommand = "blastall.exe -p blastn",
                        .Parameters = New KeyValuePair() {
 _
                            New KeyValuePair With {.Key = "query", .Value = "-i"},
                            New KeyValuePair With {.Key = "subject", .Value = "-d"},
                            New KeyValuePair With {.Key = "evalue", .Value = "-e"},
                            New KeyValuePair With {.Key = "output", .Value = "-o"}}
                    },
                    New Executable.Executable_BLAST With {
                        .Name = "blastp",
                        .AssemblyCommand = "blastall.exe -p blastp",
                        .Parameters = New KeyValuePair() {
 _
                            New KeyValuePair With {.Key = "query", .Value = "-i"},
                            New KeyValuePair With {.Key = "subject", .Value = "-d"},
                            New KeyValuePair With {.Key = "evalue", .Value = "-e"},
                            New KeyValuePair With {.Key = "output", .Value = "-o"}}}
                    },
                    .MolTypeProtein = "T",
                    .MoltypeNucleotide = "F"
        }

        Public Shared ReadOnly Property BLASTPlus As ProgramProfiles = New ProgramProfiles With {
            .Name = "BLAST+",
            .ExecutableCommands = New Executable() {
                New Executable.Executable_BuildDB With {
                    .Name = "builddb", .AssemblyCommand = "makeblastdb.exe",
                    .Parameters = New KeyValuePair() {
                        New KeyValuePair With {.Key = "targetdb", .Value = "-in"},
                        New KeyValuePair With {.Key = "dbtype", .Value = "-dbtype"}}},
                New Executable.Executable_BLAST With {
                    .Name = "blastn", .AssemblyCommand = "blastall -p blastn",
                    .Parameters = New KeyValuePair() {
                        New KeyValuePair With {.Key = "query", .Value = "-query"},
                        New KeyValuePair With {.Key = "subject", .Value = "-db"},
                        New KeyValuePair With {.Key = "evalue", .Value = "-evalue"},
                        New KeyValuePair With {.Key = "output", .Value = "-out"}}},
                New Executable.Executable_BLAST With {
                    .Name = "blastp", .AssemblyCommand = "blastp.exe",
                    .Parameters = New KeyValuePair() {
                        New KeyValuePair With {.Key = "query", .Value = "-query"},
                        New KeyValuePair With {.Key = "subject", .Value = "-db"},
                        New KeyValuePair With {.Key = "evalue", .Value = "-evalue"},
                        New KeyValuePair With {.Key = "output", .Value = "-out"}}}},
            .MolTypeProtein = "prot", .MoltypeNucleotide = "nucl"}

        Public Shared ReadOnly Property RpsBLAST As ProgramProfiles = New ProgramProfiles With {
            .Name = "rpsBLAST",
            .ExecutableCommands = New Executable() {
                New Executable.Executable_BuildDB With {
                    .Name = "builddb", .AssemblyCommand = "makeprofiledb.exe",
                    .Parameters = New KeyValuePair() {
                        New KeyValuePair With {.Key = "targetdb", .Value = "-in"},
                        New KeyValuePair With {.Key = "dbtype", .Value = "-dbtype"}}},
                New Executable.Executable_BLAST With {
                    .Name = "rpsblast", .AssemblyCommand = "rpsblast.exe",
                    .Parameters = New KeyValuePair() {
                        New KeyValuePair With {.Key = "query", .Value = "-query"},
                        New KeyValuePair With {.Key = "subject", .Value = "-db"},
                        New KeyValuePair With {.Key = "evalue", .Value = "-evalue"},
                        New KeyValuePair With {.Key = "output", .Value = "-out"}}}}}
#End Region

        <XmlAttribute> Public Property Name As String
        <XmlAttribute> Public Property MolTypeProtein As String
        <XmlAttribute> Public Property MoltypeNucleotide As String
        <XmlElement>
        Public Property ExecutableCommands As Executable()

        Protected Friend Shared ReadOnly DefaultProfiles As ProgramProfiles() =
            New NCBI.Extensions.LocalBLAST.InteropService.ProgramProfiles() {
 _
                NCBI.Extensions.LocalBLAST.InteropService.ProgramProfiles.BLASTPlus,
                NCBI.Extensions.LocalBLAST.InteropService.ProgramProfiles.LocalBLAST,
                NCBI.Extensions.LocalBLAST.InteropService.ProgramProfiles.RpsBLAST
        }

        Public Function GetCommand(Name As String) As Executable
            Dim exeInst As Executable = (From exec As Executable
                                         In ExecutableCommands
                                         Where String.Equals(exec.Name, Name, StringComparison.OrdinalIgnoreCase)
                                         Select exec).FirstOrDefault
            Return exeInst
        End Function

        Public Overrides Function ToString() As String
            Return Name
        End Function

        Public Shared Function Load(FilePath As String) As ProgramProfiles
            Return FilePath.LoadXml(Of ProgramProfiles)()
        End Function
    End Class

    Public MustInherit Class Executable

        <XmlAttribute> Public Property Name As String
        <XmlAttribute> Public Property AssemblyCommand As String

        ''' <summary>
        ''' The collection of switches information in the target assembly command.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlElement("Command")> Public Property Parameters As KeyValuePair()

        ''' <summary>
        ''' Get an item value in the <see cref="Executable.Parameters"></see> property, if the query key is not exists in the 
        ''' collection then throw an key not found exception.(获取<see cref="Executable.Parameters"></see>属性中的一个对象的值，
        ''' 如果目标对象不存在于集合之中，则会抛出一个错误)
        ''' </summary>
        ''' <param name="key"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetValue(key As String) As String
            Dim LQuery = (From item In Parameters Where String.Equals(item.Key, key) Select item.Value).ToArray
            If LQuery.IsNullOrEmpty Then
                Throw New KeyNotFoundException(String.Format("Target key name ""{0}"" is not found in the assembly commandline profiles!", key))
            Else
                Return LQuery.First
            End If
        End Function

        Public Overrides Function ToString() As String
            Return AssemblyCommand
        End Function

        Public Class Executable_BLAST : Inherits Executable

            Public Function CreateCommand(Query As String, Subject As String, EValue As String, Output As String) As CommandLine.IORedirect
                Dim sBuilder As StringBuilder = New StringBuilder(512)
                Call sBuilder.Append(String.Format("{0} ""{1}"" ", GetValue("query"), Query))
                Call sBuilder.Append(String.Format("{0} ""{1}"" ", GetValue("subject"), Subject))
                Call sBuilder.Append(String.Format("{0} ""{1}"" ", GetValue("evalue"), EValue))
                Call sBuilder.Append(String.Format("{0} ""{1}""", GetValue("output"), Output))

                Return New CommandLine.IORedirect(MyBase.AssemblyCommand, sBuilder.ToString, IOredirect:=False, hide:=False)
            End Function

            Public Overrides Function ToString() As String
                Return "Executable_BLAST ProgramProfile"
            End Function
        End Class

        Public Class Executable_BuildDB : Inherits Executable

            Public Function CreateCommand(TargetDb As String, DbType As String) As CommandLine.IORedirect
                Dim sBuilder As StringBuilder = New StringBuilder(512)
                Call sBuilder.Append(String.Format("{0} ""{1}"" ", GetValue("targetdb"), TargetDb))
                Call sBuilder.Append(String.Format("{0} ""{1}""", GetValue("dbtype"), DbType))

                Return New CommandLine.IORedirect(MyBase.AssemblyCommand, sBuilder.ToString, IOredirect:=False, hide:=False)
            End Function
        End Class
    End Class

End Namespace
