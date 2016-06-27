Imports System.Text
Imports System.Xml.Serialization

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
                        .Parameters = New Microsoft.VisualBasic.ComponentModel.KeyValuePair() {
 _
                            New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "targetdb", .Value = "-i"},
                            New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "dbtype", .Value = "-p"}}
                    },
                    New Executable.Executable_BLAST With {
                        .Name = "blastn",
                        .AssemblyCommand = "blastall.exe -p blastn",
                        .Parameters = New Microsoft.VisualBasic.ComponentModel.KeyValuePair() {
 _
                            New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "query", .Value = "-i"},
                            New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "subject", .Value = "-d"},
                            New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "evalue", .Value = "-e"},
                            New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "output", .Value = "-o"}}
                    },
                    New Executable.Executable_BLAST With {
                        .Name = "blastp",
                        .AssemblyCommand = "blastall.exe -p blastp",
                        .Parameters = New Microsoft.VisualBasic.ComponentModel.KeyValuePair() {
 _
                            New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "query", .Value = "-i"},
                            New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "subject", .Value = "-d"},
                            New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "evalue", .Value = "-e"},
                            New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "output", .Value = "-o"}}}
                    },
                    .MolTypeProtein = "T",
                    .MoltypeNucleotide = "F"
        }

        Public Shared ReadOnly Property BLASTPlus As ProgramProfiles = New ProgramProfiles With {
            .Name = "BLAST+",
            .ExecutableCommands = New Executable() {
                New Executable.Executable_BuildDB With {
                    .Name = "builddb", .AssemblyCommand = "makeblastdb.exe",
                    .Parameters = New Microsoft.VisualBasic.ComponentModel.KeyValuePair() {
                        New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "targetdb", .Value = "-in"},
                        New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "dbtype", .Value = "-dbtype"}}},
                New Executable.Executable_BLAST With {
                    .Name = "blastn", .AssemblyCommand = "blastall -p blastn",
                    .Parameters = New Microsoft.VisualBasic.ComponentModel.KeyValuePair() {
                        New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "query", .Value = "-query"},
                        New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "subject", .Value = "-db"},
                        New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "evalue", .Value = "-evalue"},
                        New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "output", .Value = "-out"}}},
                New Executable.Executable_BLAST With {
                    .Name = "blastp", .AssemblyCommand = "blastp.exe",
                    .Parameters = New Microsoft.VisualBasic.ComponentModel.KeyValuePair() {
                        New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "query", .Value = "-query"},
                        New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "subject", .Value = "-db"},
                        New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "evalue", .Value = "-evalue"},
                        New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "output", .Value = "-out"}}}},
            .MolTypeProtein = "prot", .MoltypeNucleotide = "nucl"}

        Public Shared ReadOnly Property RpsBLAST As ProgramProfiles = New ProgramProfiles With {
            .Name = "rpsBLAST",
            .ExecutableCommands = New Executable() {
                New Executable.Executable_BuildDB With {
                    .Name = "builddb", .AssemblyCommand = "makeprofiledb.exe",
                    .Parameters = New Microsoft.VisualBasic.ComponentModel.KeyValuePair() {
                        New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "targetdb", .Value = "-in"},
                        New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "dbtype", .Value = "-dbtype"}}},
                New Executable.Executable_BLAST With {
                    .Name = "rpsblast", .AssemblyCommand = "rpsblast.exe",
                    .Parameters = New Microsoft.VisualBasic.ComponentModel.KeyValuePair() {
                        New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "query", .Value = "-query"},
                        New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "subject", .Value = "-db"},
                        New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "evalue", .Value = "-evalue"},
                        New Microsoft.VisualBasic.ComponentModel.KeyValuePair With {.Key = "output", .Value = "-out"}}}}}
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
        <XmlElement("Command")> Public Property Parameters As Microsoft.VisualBasic.ComponentModel.KeyValuePair()

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

            Public Function CreateCommand(Query As String, Subject As String, EValue As String, Output As String) As CommandLine.IORedirectFile
                Dim sBuilder As StringBuilder = New StringBuilder(512)
                Call sBuilder.Append(String.Format("{0} ""{1}"" ", GetValue("query"), Query))
                Call sBuilder.Append(String.Format("{0} ""{1}"" ", GetValue("subject"), Subject))
                Call sBuilder.Append(String.Format("{0} ""{1}"" ", GetValue("evalue"), EValue))
                Call sBuilder.Append(String.Format("{0} ""{1}""", GetValue("output"), Output))

                Return New CommandLine.IORedirectFile(MyBase.AssemblyCommand, sBuilder.ToString)
            End Function

            Public Overrides Function ToString() As String
                Return "Executable_BLAST ProgramProfile"
            End Function
        End Class

        Public Class Executable_BuildDB : Inherits Executable

            Public Function CreateCommand(TargetDb As String, DbType As String) As CommandLine.IORedirectFile
                Dim sBuilder As StringBuilder = New StringBuilder(512)
                Call sBuilder.Append(String.Format("{0} ""{1}"" ", GetValue("targetdb"), TargetDb))
                Call sBuilder.Append(String.Format("{0} ""{1}""", GetValue("dbtype"), DbType))

                Return New CommandLine.IORedirectFile(MyBase.AssemblyCommand, sBuilder.ToString)
            End Function
        End Class
    End Class

End Namespace