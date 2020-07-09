#Region "Microsoft.VisualBasic::c91c4e9330182cc1142607202ae25c8e, Bio.Assembly\Assembly\MetaCyc\File\FileSystem\DatabaseLoadder.vb"

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

    '     Class DatabaseLoadder
    ' 
    '         Properties: Database, SBMLMetabolismModel
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: (+2 Overloads) CreateInstance, GetBindRxns, GetCompounds, GetDNABindingSites, GetEnzrxns
    '                   GetGenes, GetPathways, GetPromoters, GetProteinFeature, GetProteins
    '                   GetProtLigandCplx, GetReactions, GetRegulations, GetTerminators, GetTransUnits
    '                   ToString
    ' 
    '         Sub: (+2 Overloads) Dispose, Save
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic

Namespace Assembly.MetaCyc.File.FileSystem

    ''' <summary>
    ''' 当对MetaCyc数据库进行延时加载的时候，则需要使用到本对象进行数据的读取操作
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DatabaseLoadder : Inherits ComponentModel.TabularLazyLoader
        Implements IDisposable

        ''' <summary>
        ''' MetaCyc database directory.(MetaCyc数据库文件夹)
        ''' </summary>
        ''' <remarks></remarks>
        Public Property Database As PGDB

        Public ReadOnly Property SBMLMetabolismModel As String
            Get
                Return String.Format("{0}/metabolic-reactions.sbml", Database._DIR)
            End Get
        End Property

        Public Sub New(MetaCyc As PGDB)
            Call MyBase.New(MetaCyc.DataDIR, {"*.dat"})
            Me.Database = MetaCyc
        End Sub

        Public Shared Widening Operator CType(MetaCycDIR As String) As DatabaseLoadder
            Return DatabaseLoadder.CreateInstance(MetaCycDIR)
        End Operator

        ''' <summary>
        ''' Returns the compounds table in the target metacyc database.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetCompounds() As MetaCyc.File.DataFiles.Compounds
            If Database.Compounds Is Nothing Then
                Dim Path As String = Database._DIR & "/compounds.dat"
                Call Console.WriteLine("MetaCyc.Reflection.FileStream.Read()::Loading table " & vbCrLf & """{0}""", Path)

                Database.Compounds = New MetaCyc.File.DataFiles.Compounds
                Call Assembly.MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of
                     MetaCyc.File.DataFiles.Slots.Compound,
                     MetaCyc.File.DataFiles.Compounds)(Path, Database.Compounds)
                Database.Compounds.Values = (From cp In Database.Compounds.Values Select cp.Trim).AsList
            End If
            Return Database.Compounds
        End Function

        ''' <summary>
        ''' Returns the BindRxns table in the target metacyc database.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetBindRxns() As MetaCyc.File.DataFiles.BindRxns
            If Database.BindRxns Is Nothing Then
                Dim Path As String = Database._DIR & "/bindrxns.dat"
                Call Console.WriteLine("MetaCyc.Reflection.FileStream.Read()::Loading table " & vbCrLf & """{0}""", Path)

                Database.BindRxns = New MetaCyc.File.DataFiles.BindRxns
                Call Assembly.MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of
                     MetaCyc.File.DataFiles.Slots.BindReaction,
                     MetaCyc.File.DataFiles.BindRxns)(Path, Database.BindRxns)
            End If
            Return Database.BindRxns
        End Function

        ''' <summary>
        ''' Returns the Regulations table in the target metacyc database.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetRegulations() As MetaCyc.File.DataFiles.Regulations
            If Database.Regulations Is Nothing Then
                Dim Path As String = Database._DIR & "/regulation.dat"
                Call Console.WriteLine("MetaCyc.Reflection.FileStream.Read()::Loading table " & vbCrLf & """{0}""", Path)

                Database.Regulations = New MetaCyc.File.DataFiles.Regulations
                Call Assembly.MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of
                     MetaCyc.File.DataFiles.Slots.Regulation,
                     MetaCyc.File.DataFiles.Regulations)(Path, Database.Regulations)
            End If
            Return Database.Regulations
        End Function

        ''' <summary>
        ''' Returns the Reactions table in the target metacyc database.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetReactions() As MetaCyc.File.DataFiles.Reactions
            If Database.Reactions Is Nothing Then
                Dim Path As String = Database._DIR & "/reactions.dat"
                Call Console.WriteLine("MetaCyc.Reflection.FileStream.Read()::Loading table " & vbCrLf & """{0}""", Path)

                Database.Reactions = New MetaCyc.File.DataFiles.Reactions
                Call Assembly.MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of
                     MetaCyc.File.DataFiles.Slots.Reaction,
                     MetaCyc.File.DataFiles.Reactions)(Path, Database.Reactions)
                Database.Reactions.Values = (From item In Database.Reactions.Values Select DataFiles.Reactions.Trim(item)).AsList
            End If
            Return Database.Reactions
        End Function

        ''' <summary>
        ''' Returns the Pathways table in the target metacyc database.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetPathways() As MetaCyc.File.DataFiles.Pathways
            If Database.Pathways Is Nothing Then
                Dim Path As String = Database._DIR & "/pathways.dat"
                Call Console.WriteLine("MetaCyc.Reflection.FileStream.Read()::Loading table " & vbCrLf & """{0}""", Path)

                Database.Pathways = New MetaCyc.File.DataFiles.Pathways
                Call Assembly.MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of
                     MetaCyc.File.DataFiles.Slots.Pathway,
                     MetaCyc.File.DataFiles.Pathways)(Path, Database.Pathways)
            End If
            Return Database.Pathways
        End Function

        ''' <summary>
        ''' Returns the ProtLigandCplx table in the target metacyc database.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetProtLigandCplx() As MetaCyc.File.DataFiles.ProtLigandCplxes
            If Database.ProtLigandCplxes Is Nothing Then
                Dim Path As String = Database._DIR & "/protligandcplxes.dat"
                Call Console.WriteLine("MetaCyc.Reflection.FileStream.Read()::Loading table " & vbCrLf & """{0}""", Path)

                Database.ProtLigandCplxes = New MetaCyc.File.DataFiles.ProtLigandCplxes
                Call Assembly.MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of
                     MetaCyc.File.DataFiles.Slots.ProtLigandCplxe,
                     MetaCyc.File.DataFiles.ProtLigandCplxes)(Path, Database.ProtLigandCplxes)
            End If
            Return Database.ProtLigandCplxes
        End Function

        ''' <summary>
        ''' Returns the ProteinFeature table in the target metacyc database.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetProteinFeature() As MetaCyc.File.DataFiles.ProteinFeatures
            If Database.ProteinFeature Is Nothing Then
                Dim Path As String = Database._DIR & "/protein-features.dat"
                Call Console.WriteLine("MetaCyc.Reflection.FileStream.Read()::Loading table " & vbCrLf & """{0}""", Path)

                Database.ProteinFeature = New MetaCyc.File.DataFiles.ProteinFeatures
                Call Assembly.MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of
                     MetaCyc.File.DataFiles.Slots.ProteinFeature,
                     MetaCyc.File.DataFiles.ProteinFeatures)(Path, Database.ProteinFeature)
            End If
            Return Database.ProteinFeature
        End Function

        ''' <summary>
        ''' Returns the Genes table in the target metacyc database.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetGenes() As MetaCyc.File.DataFiles.Genes
            If Database.Genes Is Nothing Then
                Dim Path As String = Database._DIR & "/genes.dat"
                Call Console.WriteLine("MetaCyc.Reflection.FileStream.Read()::Loading table " & vbCrLf & """{0}""", Path)

                Database.Genes = New MetaCyc.File.DataFiles.Genes
                Call Assembly.MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of
                     MetaCyc.File.DataFiles.Slots.Gene,
                     MetaCyc.File.DataFiles.Genes)(Path, Database.Genes)
            End If
            Return Database.Genes
        End Function

        ''' <summary>
        ''' Returns the TransUnits table in the target metacyc database.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTransUnits() As MetaCyc.File.DataFiles.TransUnits
            If Database.TransUnits Is Nothing Then
                Dim Path As String = Database._DIR & "/transunits.dat"
                Call Console.WriteLine("MetaCyc.Reflection.FileStream.Read()::Loading table " & vbCrLf & """{0}""", Path)

                Database.TransUnits = New MetaCyc.File.DataFiles.TransUnits
                Call Assembly.MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of
                     MetaCyc.File.DataFiles.Slots.TransUnit,
                     MetaCyc.File.DataFiles.TransUnits)(Path, Database.TransUnits)
            End If
            Return Database.TransUnits
        End Function

        ''' <summary>
        ''' Returns the DNABindingSites table in the target metacyc database.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDNABindingSites() As MetaCyc.File.DataFiles.DNABindSites
            If Database.DNABindingSites Is Nothing Then
                Dim Path As String = Database._DIR & "/dnabindsites.dat"
                Call Console.WriteLine("MetaCyc.Reflection.FileStream.Read()::Loading table " & vbCrLf & """{0}""", Path)

                Database.DNABindingSites = New MetaCyc.File.DataFiles.DNABindSites
                Call Assembly.MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of
                     MetaCyc.File.DataFiles.Slots.DNABindSite,
                     MetaCyc.File.DataFiles.DNABindSites)(Path, Database.DNABindingSites)
            End If
            Return Database.DNABindingSites
        End Function

        ''' <summary>
        ''' Returns the Promoters table in the target metacyc database.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetPromoters() As MetaCyc.File.DataFiles.Promoters
            If Database.Promoters Is Nothing Then
                Dim Path As String = Database._DIR & "/promoters.dat"
                Call Console.WriteLine("MetaCyc.Reflection.FileStream.Read()::Loading table " & vbCrLf & """{0}""", Path)

                Database.Promoters = New MetaCyc.File.DataFiles.Promoters
                Call Assembly.MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of
                     MetaCyc.File.DataFiles.Slots.Promoter,
                     MetaCyc.File.DataFiles.Promoters)(Path, Database.Promoters)
            End If
            Return Database.Promoters
        End Function

        ''' <summary>
        ''' Returns the Terminators table in the target metacyc database.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetTerminators() As MetaCyc.File.DataFiles.Terminators
            If Database.Terminators Is Nothing Then
                Dim Path As String = Database._DIR & "/terminators.dat"
                Call Console.WriteLine("MetaCyc.Reflection.FileStream.Read()::Loading table " & vbCrLf & """{0}""", Path)

                Database.Terminators = New MetaCyc.File.DataFiles.Terminators
                Call Assembly.MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of
                     MetaCyc.File.DataFiles.Slots.Terminator,
                     MetaCyc.File.DataFiles.Terminators)(Path, Database.Terminators)
            End If
            Return Database.Terminators
        End Function

        ''' <summary>
        ''' Returns the Enzrxns table in the target metacyc database.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetEnzrxns() As MetaCyc.File.DataFiles.Enzrxns
            If Database.Enzrxns Is Nothing Then
                Dim Path As String = Database._DIR & "/enzrxns.dat"
                Call Console.WriteLine("MetaCyc.Reflection.FileStream.Read()::Loading table " & vbCrLf & """{0}""", Path)

                Database.Enzrxns = New MetaCyc.File.DataFiles.Enzrxns
                Call Assembly.MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of
                     MetaCyc.File.DataFiles.Slots.Enzrxn,
                     MetaCyc.File.DataFiles.Enzrxns)(Path, Database.Enzrxns)
            End If
            Return Database.Enzrxns
        End Function

        ''' <summary>
        ''' Returns the Proteins table in the target metacyc database.
        ''' </summary>
        ''' <remarks>
        ''' 在本处的蛋白质指的是具备有生物学活性的多肽链单体蛋白，而蛋白质复合物则指的是多个多肽链单体蛋白的
        ''' 聚合物以及其与小分子化合物所形成的复合物
        ''' </remarks>
        Public Function GetProteins() As MetaCyc.File.DataFiles.Proteins
            If Database.Proteins Is Nothing Then
                Dim Path As String = Database._DIR & "/proteins.dat"
                Call Console.WriteLine("MetaCyc.Reflection.FileStream.Read()::Loading table " & vbCrLf & """{0}""", Path)

                Database.Proteins = New MetaCyc.File.DataFiles.Proteins
                Call Assembly.MetaCyc.File.DataFiles.Reflection.FileStream.Read(Of
                     MetaCyc.File.DataFiles.Slots.Protein,
                     MetaCyc.File.DataFiles.Proteins)(Path, Database.Proteins)
            End If
            Return Database.Proteins
        End Function

        Public Overrides Function ToString() As String
            Return "[METACYC_LOADDER] " & Database.ToString
        End Function

        ''' <summary>
        ''' Preload the target metacyc database.(MetaCyc数据库预加载)
        ''' </summary>
        ''' <param name="MetaCycDir"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateInstance(MetaCycDir As String, Optional Explicit As Boolean = True) As DatabaseLoadder
            Dim MetaCyc As MetaCyc.File.FileSystem.PGDB = Assembly.MetaCyc.File.FileSystem.PGDB.PreLoad(MetaCycDir, Explicit)
            Return New MetaCyc.File.FileSystem.DatabaseLoadder(MetaCyc)
        End Function

        Public Shared Function CreateInstance(MetaCyc As MetaCyc.File.FileSystem.PGDB) As DatabaseLoadder
            Return New MetaCyc.File.FileSystem.DatabaseLoadder(MetaCyc)
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' 检测冗余的调用

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
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

        ' Visual Basic 添加此代码是为了正确实现可处置模式。
        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。    请将清理代码放入上面的 Dispose (disposing As Boolean)中。
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

        Public Sub Save(Optional EXPORT As String = "")
            Call Database.Save(EXPORT)
        End Sub
    End Class
End Namespace
