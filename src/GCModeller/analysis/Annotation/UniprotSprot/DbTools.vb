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