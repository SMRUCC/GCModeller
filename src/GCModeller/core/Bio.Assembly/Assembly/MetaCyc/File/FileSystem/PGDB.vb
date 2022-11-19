#Region "Microsoft.VisualBasic::bda5fbeca583cced26384c4db46677a6, GCModeller\core\Bio.Assembly\Assembly\MetaCyc\File\FileSystem\PGDB.vb"

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

    '   Total Lines: 325
    '    Code Lines: 205
    ' Comment Lines: 75
    '   Blank Lines: 45
    '     File Size: 21.91 KB


    '     Class PGDB
    ' 
    '         Properties: BindRxns, Compounds, DataDIR, DNABindingSites, Enzrxns
    '                     FASTAFiles, Genes, Pathways, Promoters, ProteinFeature
    '                     Proteins, ProtLigandCplxes, Reactions, Regulations, Species
    '                     Terminators, TransUnits, WholeGenome
    ' 
    '         Function: Load, ParallelReadingMetaCyc, PreLoad, ReflectionLoadMetaCyc, ToString
    ' 
    '         Sub: Add, Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Object
Imports SMRUCC.genomics.Assembly.MetaCyc.File.FileSystem.FastaObjects
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA.FastaFile

Namespace Assembly.MetaCyc.File.FileSystem

    ''' <summary>
    ''' The MetaCyc database file reader object.
    ''' (MetaCyc数据库文件的读取对象)
    ''' </summary>
    ''' <remarks>DataFiles</remarks>
    Public Class PGDB

        ''' <summary>
        ''' Binding reactions.(调控因子与DNA分子的结合反应)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property BindRxns As MetaCyc.File.DataFiles.BindRxns

        ''' <summary>
        ''' All of the chemical compounds and biomolecule that defines in this table.
        ''' (细胞中的所有的化学物质和生物分子组成)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Compounds As MetaCyc.File.DataFiles.Compounds

        ''' <summary>
        ''' Regulation rule of the gene expression and enzyme activity.(对基因表达过程的调控以及酶活力的调节)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Regulations As MetaCyc.File.DataFiles.Regulations

        Public Property Reactions As MetaCyc.File.DataFiles.Reactions

        Public Property Pathways As MetaCyc.File.DataFiles.Pathways

        ''' <summary>
        ''' (蛋白质分子和其他的组分结合所形成的蛋白质复合物)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ProtLigandCplxes As MetaCyc.File.DataFiles.ProtLigandCplxes

        Public Property ProteinFeature As MetaCyc.File.DataFiles.ProteinFeatures

        Public Property Genes As MetaCyc.File.DataFiles.Genes

        Public Property TransUnits As MetaCyc.File.DataFiles.TransUnits

        Public Property DNABindingSites As MetaCyc.File.DataFiles.DNABindSites

        Public Property Promoters As MetaCyc.File.DataFiles.Promoters

        Public Property Terminators As MetaCyc.File.DataFiles.Terminators

        Public Property Enzrxns As MetaCyc.File.DataFiles.Enzrxns

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <remarks>
        ''' 在本处的蛋白质指的是具备有生物学活性的多肽链单体蛋白，而蛋白质复合物则指的是多个多肽链单体蛋白的
        ''' 聚合物以及其与小分子化合物所形成的复合物
        ''' </remarks>
        Public Property Proteins As MetaCyc.File.DataFiles.Proteins

        Public Property Species As MetaCyc.File.DataFiles.Species = New MetaCyc.File.DataFiles.Species

        ''' <summary>
        ''' Adding methods for general object add to a specific data table in the metacyc database.
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend Shared ReadOnly AddingMethods As Dictionary(Of Tables, Action(Of PGDB, Slots.Object)) =
            New Dictionary(Of Tables, Action(Of PGDB, Slots.Object)) From {
 _
                {Tables.bindrxns, Sub(MetaCyc As PGDB, e As Slots.Object) MetaCyc.BindRxns.Add(e)},
                {Tables.compounds, Sub(MetaCyc As PGDB, e As Slots.Object) MetaCyc.Compounds.Add(e)},
                {Tables.dnabindsites, Sub(MetaCyc As PGDB, e As Slots.Object) MetaCyc.DNABindingSites.Add(e)},
                {Tables.enzrxns, Sub(MetaCyc As PGDB, e As Slots.Object) MetaCyc.Enzrxns.Add(e)},
                {Tables.genes, Sub(MetaCyc As PGDB, e As Slots.Object) MetaCyc.Genes.Add(e)},
                {Tables.pathways, Sub(MetaCyc As PGDB, e As Slots.Object) MetaCyc.Pathways.Add(e)},
                {Tables.promoters, Sub(MetaCyc As PGDB, e As Slots.Object) MetaCyc.Promoters.Add(e)},
                {Tables.proteinfeatures, Sub(MetaCyc As PGDB, e As Slots.Object) MetaCyc.ProteinFeature.Add(e)},
                {Tables.proteins, Sub(MetaCyc As PGDB, e As Slots.Object) MetaCyc.Proteins.Add(e)},
                {Tables.protligandcplxes, Sub(MetaCyc As PGDB, e As Slots.Object) MetaCyc.ProtLigandCplxes.Add(e)},
                {Tables.reactions, Sub(MetaCyc As PGDB, e As Slots.Object) MetaCyc.Reactions.Add(e)},
                {Tables.regulation, Sub(MetaCyc As PGDB, e As Slots.Object) MetaCyc.Regulations.Add(e)},
                {Tables.terminators, Sub(MetaCyc As PGDB, e As Slots.Object) MetaCyc.Terminators.Add(e)},
                {Tables.transunits, Sub(MetaCyc As PGDB, e As Slots.Object) MetaCyc.TransUnits.Add(e)}
        }

        Protected Friend _DIR As String

        Public Property FASTAFiles As FastaCollection

        Public ReadOnly Property DataDIR As String
            Get
                Return _DIR
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return _DIR
        End Function

        ''' <summary>
        ''' Add the target element object into a specific table in a MetaCyc database object.
        ''' (将目标元素对象添加进入一个MetaCyc数据库中的一张指定的数据表之中)
        ''' </summary>
        ''' <param name="MetaCyc"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Public Shared Sub Add(MetaCyc As PGDB, e As DataFiles.Slots.Object)
            Dim method = PGDB.AddingMethods(e.Table)
            Call method(MetaCyc, e)
        End Sub

        ''' <summary>
        ''' Get the whole genome sequence data for this species bacteria.
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Property WholeGenome As FASTA.FastaSeq
            Get
                Dim Specie = Species.First
                Dim File As String = String.Format("{0}/{1}-{2}.fsa", _DIR, Specie.Identifier, Specie.Genome)
                Return FASTA.FastaSeq.Load(File)  '加载全基因组序列的FASTA文件
            End Get
        End Property

        ''' <summary>
        ''' 保存数据库，假若SavedDir参数不为空的话，则保存至SavedDir所指示的文件夹之内
        ''' </summary>
        ''' <param name="EXPORT">可选参数，目标MetaCyc数据库所要保存的文件夹位置</param>
        ''' <remarks></remarks>
        Public Sub Save(Optional EXPORT As String = "")
            If String.IsNullOrEmpty(EXPORT) Then
                EXPORT = _DIR
            End If

            On Error Resume Next '好邪恶

            Call FileIO.FileSystem.CreateDirectory(EXPORT)
            If Not String.Equals(FileIO.FileSystem.GetDirectoryInfo(_DIR).FullName, FileIO.FileSystem.GetDirectoryInfo(EXPORT).FullName) Then
                Call FileIO.FileSystem.CopyDirectory(_DIR, EXPORT)
                _DIR = EXPORT
            End If

            Using Loader = MetaCyc.File.FileSystem.DatabaseLoadder.CreateInstance(Me)
                Call Loader.GetBindRxns.Save(EXPORT & "/bindrxns.dat")
                Call Loader.GetCompounds.Save(EXPORT & "/compounds.dat")
                Call Loader.GetDNABindingSites.Save(EXPORT & "/dnabindsites.dat")
                Call Loader.GetEnzrxns.Save(EXPORT & "/enzrxns.dat")
                Call Loader.GetGenes.Save(EXPORT & "/genes.dat")
                Call Loader.GetPathways.Save(EXPORT & "/pathways.dat")
                Call Loader.GetPromoters.Save(EXPORT & "/promoters.dat")
                Call Loader.GetProteins.Save(EXPORT & "/proteins.dat")
                Call Loader.GetProteinFeature.Save(EXPORT & "/protein-features.dat")
                Call Loader.GetProtLigandCplx.Save(EXPORT & "/protligandcplxes.dat")
                Call Loader.GetReactions.Save(EXPORT & "/reactions.dat")
                Call Loader.GetRegulations.Save(EXPORT & "/regulation.dat")
                Call Loader.GetTerminators.Save(EXPORT & "/terminators.dat")
                Call Loader.GetTransUnits.Save(EXPORT & "/transunits.dat")

                Call Me.Species.Save(EXPORT & "/species.dat")

                If Me.Species.NumOfTokens = 1 Then
                    Dim Specie = Species.First

                    Call Me.WholeGenome.SaveTo(String.Format("{0}/{1}-{2}.fsa", EXPORT, Specie.Identifier, Specie.Genome))
                    Call MetaCyc.File.FileSystem.FastaObjects.GeneObject.Save(Me.FASTAFiles.DNAseq, EXPORT & "/dnaseq.fsa")
                    Call MetaCyc.File.FileSystem.FastaObjects.Proteins.Save(Me.FASTAFiles.protseq, EXPORT & "/protseq.fsa")
                End If
            End Using
        End Sub

        ''' <summary>
        ''' (使用反射机制进行数据库数据的加载操作，根据条件编译来选择为并行加载还是串行加载)
        ''' </summary>
        ''' <param name="DIR"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function Load(DIR As String) As PGDB
#If PARALLEL Then
            Return MetaCyc.File.FileSystem.PGDB.ParallelReadingMetaCyc(Dir)
#Else
            Return MetaCyc.File.FileSystem.PGDB.ReflectionLoadMetaCyc(DIR)
#End If
        End Function

        ''' <summary>
        ''' 预加载MetaCyc数据库之中的部分数据
        ''' </summary>
        ''' <param name="DIR">目标MetaCyc数据库的Data文件夹</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function PreLoad(DIR As String, Optional Explicit As Boolean = True) As PGDB
            Dim MetaCyc As PGDB = New PGDB With {
                ._DIR = DIR,
                .FASTAFiles = New FastaCollection
            }
            MetaCyc.Species = DataFiles.Reflection.FileStream.Read(Of Slots.Specie, Species)(DIR & "/species.dat", New Species)

            If MetaCyc.Species.NumOfTokens = 1 Then
                Dim Specie = MetaCyc.Species.First

                MetaCyc.FASTAFiles.ProteinSourceFile = DIR & "/protseq.fsa"
                MetaCyc.FASTAFiles.DNASourceFilePath = DIR & "/dnaseq.fsa"
                MetaCyc.FASTAFiles.DNAseq = FastaCollection.LoadGeneObjects(MetaCyc.FASTAFiles.DNASourceFilePath, Explicit)
                MetaCyc.FASTAFiles.protseq = FastaCollection.LoadProteins(MetaCyc.FASTAFiles.ProteinSourceFile, Explicit)

                Dim OSPath As String = String.Format("{0}/{1}-{2}.fsa", DIR, Specie.Identifier, Specie.Genome)
                If FileIO.FileSystem.FileExists(OSPath) Then
                    MetaCyc.FASTAFiles.Origin = FASTA.FastaSeq.Load(OSPath)
                    MetaCyc.FASTAFiles.OriginSourceFile = OSPath
                End If
            End If

            Return MetaCyc
        End Function

        ''' <summary>
        ''' (使用反射机制进行数据库数据的加载操作)
        ''' </summary>
        ''' <param name="Dir"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ReflectionLoadMetaCyc(Dir As String) As PGDB
            Dim PGDB As PGDB = New PGDB With {
                ._DIR = Dir
            }

            Call MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.BindReaction, MetaCyc.File.DataFiles.BindRxns)(Dir & "/bindrxns.dat", PGDB.BindRxns)
            Call MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.Compound, MetaCyc.File.DataFiles.Compounds)(Dir & "/compounds.dat", PGDB.Compounds)
            Call MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.DNABindSite, MetaCyc.File.DataFiles.DNABindSites)(Dir & "/dnabindsites.dat", PGDB.DNABindingSites)
            Call MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.Enzrxn, MetaCyc.File.DataFiles.Enzrxns)(Dir & "/enzrxns.dat", PGDB.Enzrxns)
            Call MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.Gene, MetaCyc.File.DataFiles.Genes)(Dir & "/genes.dat", PGDB.Genes)
            Call MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.Pathway, MetaCyc.File.DataFiles.Pathways)(Dir & "/pathways.dat", PGDB.Pathways)
            Call MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.Promoter, MetaCyc.File.DataFiles.Promoters)(Dir & "/promoters.dat", PGDB.Promoters)
            Call MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.Protein, MetaCyc.File.DataFiles.Proteins)(Dir & "/proteins.dat", PGDB.Proteins)
            Call MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.ProteinFeature, MetaCyc.File.DataFiles.ProteinFeatures)(Dir & "/protein-features.dat", PGDB.ProteinFeature)
            Call MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.ProtLigandCplxe, MetaCyc.File.DataFiles.ProtLigandCplxes)(Dir & "/protligandcplxes.dat", PGDB.ProtLigandCplxes)
            Call MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.Reaction, MetaCyc.File.DataFiles.Reactions)(Dir & "/reactions.dat", PGDB.Reactions)
            Call MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.Regulation, MetaCyc.File.DataFiles.Regulations)(Dir & "/regulation.dat", PGDB.Regulations)
            Call MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.Terminator, MetaCyc.File.DataFiles.Terminators)(Dir & "/terminators.dat", PGDB.Terminators)
            Call MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.TransUnit, MetaCyc.File.DataFiles.TransUnits)(Dir & "/transunits.dat", PGDB.TransUnits)

            Return PGDB
        End Function

        Public Shared Function ParallelReadingMetaCyc(Dir As String) As PGDB
            Dim PGDB As PGDB = New PGDB With {._DIR = Dir}

            Dim ReadingBindReaction As MetaCyc.File.DataFiles.Reflection.FileStream.ReadingThread(Of MetaCyc.File.DataFiles.Slots.BindReaction, MetaCyc.File.DataFiles.BindRxns) =
                AddressOf MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.BindReaction, MetaCyc.File.DataFiles.BindRxns)
            Dim ReadingCompounds As MetaCyc.File.DataFiles.Reflection.FileStream.ReadingThread(Of MetaCyc.File.DataFiles.Slots.Compound, MetaCyc.File.DataFiles.Compounds) =
                AddressOf MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.Compound, MetaCyc.File.DataFiles.Compounds)
            Dim ReadingDNABinding As MetaCyc.File.DataFiles.Reflection.FileStream.ReadingThread(Of MetaCyc.File.DataFiles.Slots.DNABindSite, MetaCyc.File.DataFiles.DNABindSites) =
                AddressOf MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.DNABindSite, MetaCyc.File.DataFiles.DNABindSites)
            Dim ReadingEnzreaction As MetaCyc.File.DataFiles.Reflection.FileStream.ReadingThread(Of MetaCyc.File.DataFiles.Slots.Enzrxn, MetaCyc.File.DataFiles.Enzrxns) =
                AddressOf MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.Enzrxn, MetaCyc.File.DataFiles.Enzrxns)
            Dim ReadingGenomes As MetaCyc.File.DataFiles.Reflection.FileStream.ReadingThread(Of MetaCyc.File.DataFiles.Slots.Gene, MetaCyc.File.DataFiles.Genes) =
                AddressOf MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.Gene, MetaCyc.File.DataFiles.Genes)
            Dim ReadingPathways As MetaCyc.File.DataFiles.Reflection.FileStream.ReadingThread(Of MetaCyc.File.DataFiles.Slots.Pathway, MetaCyc.File.DataFiles.Pathways) =
                AddressOf MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.Pathway, MetaCyc.File.DataFiles.Pathways)
            Dim ReadingPromoters As MetaCyc.File.DataFiles.Reflection.FileStream.ReadingThread(Of MetaCyc.File.DataFiles.Slots.Promoter, MetaCyc.File.DataFiles.Promoters) =
                AddressOf MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.Promoter, MetaCyc.File.DataFiles.Promoters)
            Dim ReadingProteins As MetaCyc.File.DataFiles.Reflection.FileStream.ReadingThread(Of MetaCyc.File.DataFiles.Slots.Protein, MetaCyc.File.DataFiles.Proteins) =
                AddressOf MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.Protein, MetaCyc.File.DataFiles.Proteins)
            Dim ReadingProtFeature As MetaCyc.File.DataFiles.Reflection.FileStream.ReadingThread(Of MetaCyc.File.DataFiles.Slots.ProteinFeature, MetaCyc.File.DataFiles.ProteinFeatures) =
                AddressOf MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.ProteinFeature, MetaCyc.File.DataFiles.ProteinFeatures)
            Dim ReadingProtCplx As MetaCyc.File.DataFiles.Reflection.FileStream.ReadingThread(Of MetaCyc.File.DataFiles.Slots.ProtLigandCplxe, MetaCyc.File.DataFiles.ProtLigandCplxes) =
                AddressOf MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.ProtLigandCplxe, MetaCyc.File.DataFiles.ProtLigandCplxes)
            Dim ReadingReactions As MetaCyc.File.DataFiles.Reflection.FileStream.ReadingThread(Of MetaCyc.File.DataFiles.Slots.Reaction, MetaCyc.File.DataFiles.Reactions) =
                AddressOf MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.Reaction, MetaCyc.File.DataFiles.Reactions)
            Dim ReadingRegulation As MetaCyc.File.DataFiles.Reflection.FileStream.ReadingThread(Of MetaCyc.File.DataFiles.Slots.Regulation, MetaCyc.File.DataFiles.Regulations) =
                AddressOf MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.Regulation, MetaCyc.File.DataFiles.Regulations)
            Dim ReadingTerminator As MetaCyc.File.DataFiles.Reflection.FileStream.ReadingThread(Of MetaCyc.File.DataFiles.Slots.Terminator, MetaCyc.File.DataFiles.Terminators) =
                AddressOf MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.Terminator, MetaCyc.File.DataFiles.Terminators)
            Dim ReadingTransUnits As MetaCyc.File.DataFiles.Reflection.FileStream.ReadingThread(Of MetaCyc.File.DataFiles.Slots.TransUnit, MetaCyc.File.DataFiles.TransUnits) =
                AddressOf MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of MetaCyc.File.DataFiles.Slots.TransUnit, MetaCyc.File.DataFiles.TransUnits)

            Dim ABindRxns As System.IAsyncResult = ReadingBindReaction.BeginInvoke(Dir & "/bindrxns.dat", PGDB.BindRxns, Nothing, Nothing)
            Dim ACompounds As System.IAsyncResult = ReadingCompounds.BeginInvoke(Dir & "/compounds.dat", PGDB.Compounds, Nothing, Nothing)
            Dim ADNABindSites As System.IAsyncResult = ReadingDNABinding.BeginInvoke(Dir & "/dnabindsites.dat", PGDB.DNABindingSites, Nothing, Nothing)
            Dim AEnzrxns As System.IAsyncResult = ReadingEnzreaction.BeginInvoke(Dir & "/enzrxns.dat", PGDB.Enzrxns, Nothing, Nothing)
            Dim AGenes As System.IAsyncResult = ReadingGenomes.BeginInvoke(Dir & "/genes.dat", PGDB.Genes, Nothing, Nothing)
            Dim APathways As System.IAsyncResult = ReadingPathways.BeginInvoke(Dir & "/pathways.dat", PGDB.Pathways, Nothing, Nothing)
            Dim APromoters As System.IAsyncResult = ReadingPromoters.BeginInvoke(Dir & "/promoters.dat", PGDB.Promoters, Nothing, Nothing)
            Dim AProteins As System.IAsyncResult = ReadingProteins.BeginInvoke(Dir & "/proteins.dat", PGDB._Proteins, Nothing, Nothing)
            Dim AProteinFeatures As System.IAsyncResult = ReadingProtFeature.BeginInvoke(Dir & "/protein-features.dat", PGDB.ProteinFeature, Nothing, Nothing)
            Dim AProtLigandCplxes As System.IAsyncResult = ReadingProtCplx.BeginInvoke(Dir & "/protligandcplxes.dat", PGDB.ProtLigandCplxes, Nothing, Nothing)
            Dim AReactions As System.IAsyncResult = ReadingReactions.BeginInvoke(Dir & "/reactions.dat", PGDB.Reactions, Nothing, Nothing)
            Dim ARegulations As System.IAsyncResult = ReadingRegulation.BeginInvoke(Dir & "/regulation.dat", PGDB.Regulations, Nothing, Nothing)
            Dim ATerminators As System.IAsyncResult = ReadingTerminator.BeginInvoke(Dir & "/terminators.dat", PGDB.Terminators, Nothing, Nothing)
            Dim ATransUnits As System.IAsyncResult = ReadingTransUnits.BeginInvoke(Dir & "/transunits.dat", PGDB.TransUnits, Nothing, Nothing)

            PGDB.BindRxns = ReadingBindReaction.EndInvoke(PGDB.BindRxns, ABindRxns)
            PGDB.Compounds = ReadingCompounds.EndInvoke(PGDB.Compounds, ACompounds)
            PGDB.DNABindingSites = ReadingDNABinding.EndInvoke(PGDB.DNABindingSites, ADNABindSites)
            PGDB.Enzrxns = ReadingEnzreaction.EndInvoke(PGDB.Enzrxns, AEnzrxns)
            PGDB.Genes = ReadingGenomes.EndInvoke(PGDB.Genes, AGenes)
            PGDB.Pathways = ReadingPathways.EndInvoke(PGDB.Pathways, APathways)
            PGDB.Promoters = ReadingPromoters.EndInvoke(PGDB.Promoters, APromoters)
            PGDB.ProtLigandCplxes = ReadingProtCplx.EndInvoke(PGDB.ProtLigandCplxes, AProtLigandCplxes)
            PGDB.ProteinFeature = ReadingProtFeature.EndInvoke(PGDB.ProteinFeature, AProteinFeatures)
            PGDB.Proteins = ReadingProteins.EndInvoke(PGDB.Proteins, AProteins)
            PGDB.Reactions = ReadingReactions.EndInvoke(PGDB.Reactions, AReactions)
            PGDB.Regulations = ReadingRegulation.EndInvoke(PGDB.Regulations, ARegulations)
            PGDB.Terminators = ReadingTerminator.EndInvoke(PGDB.Terminators, ATerminators)
            PGDB.TransUnits = ReadingTransUnits.EndInvoke(PGDB.TransUnits, ATransUnits)

            Return PGDB
        End Function
    End Class
End Namespace
