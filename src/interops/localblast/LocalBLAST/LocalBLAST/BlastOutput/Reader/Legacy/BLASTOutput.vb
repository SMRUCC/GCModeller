#Region "Microsoft.VisualBasic::be9158dd9ae44bd421bc89c235256f57, ..\localblast\LocalBLAST\LocalBLAST\BlastOutput\Reader\Legacy\BLASTOutput.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Text.RegularExpressions
Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Extensions
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Linq

Namespace LocalBLAST.BLASTOutput.Legacy

    ''' <summary>
    ''' An analysis tool for local blast log file.(本地BLAST日志文件分析对象)
    ''' </summary>
    ''' <remarks>
    ''' Parser for the blast output format from legacy blast suite, which is before blast+ suite was released.
    ''' </remarks>
    Public Class BLASTOutput : Inherits IBlastOutput
        Implements System.IDisposable

        <XmlElement> Public Property Queries As Query()
        <XmlElement> Public Method As String

        Public Shared Function TryParse(LogFile As String) As BLASTOutput
            Dim Array As String() = Regex.Split(FileIO.FileSystem.ReadAllText(LogFile),
                                                Legacy.Query.BLAST_SECTION_HEADER,
                                                RegexOptions.Multiline)
            Dim Query = From Line As String In Array.Skip(1).AsParallel
                        Let q = Legacy.Query.TryParse(Line)
                        Where Not q.IsEmpty
                        Select q
                        Order By q.QueryName Ascending '
            Dim Blastlog As BLASTOutput = New BLASTOutput With {.FilePath = LogFile & ".xml"}
            Dim s As String = Array.Last

            Blastlog.Method = Mid(s.Match("Method: .+"), 9)
            Blastlog.Database = Mid(s.Match("Database: .+"), 11)
            Blastlog.Queries = Query.ToArray

            Return Blastlog
        End Function

        Public Overrides Function Grep(Query As TextGrepMethod, Hits As TextGrepMethod) As IBlastOutput
            Dim LQuery = (From idx
                          In Queries.SeqIterator
                          Select idx.value.GrepQuery(Query) + idx.value.GrepHits(Hits)).ToArray '
            Return Me
        End Function

        Public Overrides Function Save(Optional File As String = "", Optional Encoding As Encoding = Nothing) As Boolean
            If String.IsNullOrEmpty(File) Then
                File = Me.FilePath
            End If

#If DEBUG Then
            Try
                Call FileIO.FileSystem.WriteAllText(File, text:=Me.GetXml, append:=False)
            Catch ex As Exception
                Call FileIO.FileSystem.WriteAllText(My.Computer.FileSystem.SpecialDirectories.Temp & "/Error.log", ex.ToString, append:=False)
                Throw
            End Try
#Else
            Call FileIO.FileSystem.WriteAllText(File, Me.GetXml, append:=False)
#End If
            Return True
        End Function

        Public Shared Function Load(File As String) As BLASTOutput
            Dim LogFile = File.LoadXml(Of BLASTOutput)()
            LogFile.FilePath = File
            Return LogFile
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0}; ({1} queries)", FilePath, Queries.Count)
        End Function

#Region "IDisposable Support"
        ' IDisposable
        Protected Overrides Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    Call Save(FilePath)
                    ' TODO:  释放托管状态(托管对象)。
                End If

                ' TODO:  释放非托管资源(非托管对象)并重写下面的 Finalize()。
                ' TODO:  将大型字段设置为 null。
            End If
            Me.disposedValue = True
        End Sub

        ' TODO:  仅当上面的 Dispose( disposing As Boolean)具有释放非托管资源的代码时重写 Finalize()。
        'Protected Overrides Sub Finalize()
        '    ' 不要更改此代码。    请将清理代码放入上面的 Dispose( disposing As Boolean)中。
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub
#End Region

        Public Overrides Function ExportOverview() As LocalBLAST.BLASTOutput.Views.Overview
            Throw New NotImplementedException
        End Function

        Public Overrides Function ExportAllBestHist(Optional coverage As Double = 0.5, Optional identities_cutoff As Double = 0.15) As LocalBLAST.Application.BBH.BestHit()
            Throw New NotImplementedException
        End Function

        Public Overrides Function CheckIntegrity(QuerySource As SequenceModel.FASTA.FastaFile) As Boolean
            Throw New NotImplementedException
        End Function
    End Class
End Namespace
