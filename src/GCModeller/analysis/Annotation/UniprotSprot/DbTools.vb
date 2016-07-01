#Region "Microsoft.VisualBasic::99d6b442e999fccf1d08db24632091bb, ..\GCModeller\analysis\Annotation\UniprotSprot\DbTools.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
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

Imports SMRUCC.genomics.BioAssemblyExtensions
Imports System.Data.SQLite.Linq.DataMapping.Interface.Reflector
Imports Microsoft.VisualBasic
Imports System.Data.SQLite.Linq.DataMapping.Interface

Namespace UniprotSprot

    Public Class DbTools : Inherits AnnotationTool

        Sub New(LocalBlast As SMRUCC.genomics.NCBI.Extensions.LocalBLAST.InteropService.InteropService)
            Call MyBase.New(LocalBlast, DB:=Settings.Session.Initialize.RepositoryRoot & "/Uniprot/")
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="UniprotSprot"></param>
        ''' <param name="InstalledRoot">数据库将要安装的路径</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function InstallDatabase(UniprotSprot As String, InstalledRoot As String) As SMRUCC.genomics.GCModeller.Workbench.DatabaseServices.Model_Repository.Uniprot()
            Dim UniprotData = (From Fasta In SMRUCC.genomics.Assembly.Uniprot.UniprotFasta.LoadFasta(UniprotSprot) Select Fasta Group By Fasta.OrgnsmSpName Into Group).ToArray
            Dim ChunkBuffer As New List(Of SMRUCC.genomics.GCModeller.Workbench.DatabaseServices.Model_Repository.Uniprot)

            Call FileIO.FileSystem.CreateDirectory(InstalledRoot)

            Dim Engine = SMRUCC.genomics.GCModeller.Workbench.DatabaseServices.Model_Repository.RepositoryEngine.SQLiteEngine
            Dim TableSchema = New TableSchema(type:=GetType(SMRUCC.genomics.GCModeller.Workbench.DatabaseServices.Model_Repository.Uniprot))

            If Engine.ExistsTable(TableSchema.TableName) Then
                Call Engine.DeleteTable(TableSchema.TableName)
            End If
            Call Engine.CreateTableFor(Of SMRUCC.genomics.GCModeller.Workbench.DatabaseServices.Model_Repository.Uniprot)()

            Dim LQuery = (From GroupedItem In UniprotData.AsParallel
                          Let Path As String = Function() As String
                                                   If Not String.IsNullOrEmpty(GroupedItem.OrgnsmSpName) Then
                                                       Return InstalledRoot & "/" & GroupedItem.OrgnsmSpName.First & "/" & GroupedItem.OrgnsmSpName.NormalizePathString() & ".fasta"
                                                   Else
                                                       Return InstalledRoot & "/UnknownSpecies.fasta"
                                                   End If
                                               End Function()
                          Let GroupedUniprot = GroupedItem.Group.ToArray
                          Let FastaData As SMRUCC.genomics.SequenceModel.FASTA.FastaToken() = (From item As SMRUCC.genomics.Assembly.Uniprot.UniprotFasta
                                                                                      In GroupedUniprot
                                                                                                   Select New SMRUCC.genomics.SequenceModel.FASTA.FastaToken With
                                                                                                          {
                                                                                                              .Attributes = New String() {item.UniprotID},
                                                                                                              .SequenceData = item.SequenceData
                                                                                                          }).ToArray Select GroupedUniprot, Path, FastaData).ToArray

            For Each Source In LQuery
                Call CType(Source.FastaData, SMRUCC.genomics.SequenceModel.FASTA.FastaFile).Save(Source.Path)
                Dim Entries = (From item As SMRUCC.genomics.Assembly.Uniprot.UniprotFasta
                               In Source.GroupedUniprot
                               Select SMRUCC.genomics.GCModeller.Workbench.DatabaseServices.Model_Repository.Uniprot.CreateObject(item, SecurityString.GetFileHashString(Source.Path))).ToArray
                Call ChunkBuffer.AddRange(Entries)

                For Each Entry As SMRUCC.genomics.GCModeller.Workbench.DatabaseServices.Model_Repository.Uniprot In Entries
                    Call Engine.Insert(TableSchema, Entry)
                Next
                Call Console.WriteLine("[DEBUG] {0} data done!", Source.Path.ToFileURL)
            Next

            Call Console.WriteLine("[DEBUG] {0} entry was writers into the repository database.", ChunkBuffer.Count)

            Return ChunkBuffer.ToArray
        End Function

        Protected Overrides Function GetAnnotationSourceMeta() As SMRUCC.genomics.GCModeller.Workbench.DatabaseServices.Model_Repository.DbFileSystemObject.ProteinDescriptionModel()
            Throw New NotImplementedException
        End Function
    End Class
End Namespace
