Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports LANS.SystemsBiology.GCModeller.ModellingEngine.Assembly.DocumentFormat
Imports LANS.SystemsBiology.Assembly
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic
Imports LANS.SystemsBiology.Assembly.DOOR

Namespace FileStream

    ''' <summary>
    ''' 一个转录单元对象是有多个具有调控作用的motif以及多个编码产物的编码区所构成的
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TranscriptUnit : Implements IReadOnlyId

        ''' <summary>
        ''' 通常为属性<see cref="LANS.SystemsBiology.Assembly.Door.GeneBrief.OperonID"></see>的这个编号值
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column("Operon-Id")> Public Property OperonId As String

        ''' <summary>
        ''' 请使用这个属性来唯一标记当前的对象
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TU_GUID As String Implements IReadOnlyId.Identity
            Get
                Return String.Format("{0}..{1}", OperonId, PromoterGene)
            End Get
        End Property

        <CollectionAttribute("Motifs")> Public Property Motifs As String()
        ''' <summary>
        ''' <see cref="TranscriptUnit.OperonId"></see>的这个操纵子对象之中的结构基因，请注意，在初始化基因表达调控网络的结构的时候请务必要展开这个属性，否则结构基因的调控过程会被遗漏掉了的
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <CollectionAttribute("Operon-Genes")> Public Property OperonGenes As String()

        Public Property RegulatorK As Double
        Public Property PromoterGene As String

        ''' <summary>
        ''' Regulator对第一个基因的Pcc值
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TFPcc As Double

        ''' <summary>
        ''' Pcc值指的是<see cref="TranscriptUnit.Motifs"></see>对<see cref="TranscriptUnit.OperonGenes"></see>中的基因对象的调控作用的大小
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <CollectionAttribute("Pcc-values", "; ")> Public Property PccValues As Double()

        ''' <summary>
        ''' The basal expression level of this <see cref="TranscriptUnit"></see> object
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column("Basal-value")> Public Property BasalValue As Double
        Public Overrides Function ToString() As String
            Return String.Format("{0} --> {1}", String.Join("; ", Motifs), OperonId)
        End Function

        Public Shared Function CreateObject(Regulation As TranscriptRegulation(), Door As OperonView) As List(Of TranscriptUnit)
            Dim LQuery = (From item In Regulation Select CreateObject(item)).ToList
            Dim Regulations = (From item In LQuery Select item.OperonId Distinct).ToArray
            Dim NullRegulations = (From item In CreateObject(Door) Where Array.IndexOf(Regulation, item.OperonId) = -1 Select item).ToArray
            Call LQuery.AddRange(NullRegulations)
            Return LQuery
        End Function

        Private Shared Function CreateObject(Door As OperonView) As List(Of TranscriptUnit)
            Dim LQuery = (From Operon As SystemsBiology.Assembly.DOOR.Operon
                          In Door.Operons
                          Let tu = New TranscriptUnit With {
                              .OperonGenes = Operon.lstLocus,
                              .OperonId = "#" & Operon.Key,
                              .PromoterGene = Operon.InitialX.Synonym
                          }
                          Select tu).ToList
            Return LQuery
        End Function

        Public Function Clone() As TranscriptUnit
            Dim ClonedObject As TranscriptUnit = New TranscriptUnit With {
                .BasalValue = Me.BasalValue,
                .Motifs = Me.Motifs,
                .OperonGenes = Me.OperonGenes,
                .OperonId = Me.OperonId,
                .PccValues = Me.PccValues,
                .TFPcc = Me.TFPcc,
                .PromoterGene = Me.PromoterGene,
                .RegulatorK = Me.RegulatorK
            }
            Return ClonedObject
        End Function

        Private Shared Function CreateObject(Regulation As TranscriptRegulation) As TranscriptUnit
            Dim TranscriptUnit As TranscriptUnit = New TranscriptUnit
            TranscriptUnit.BasalValue = 5
            'TranscriptUnit.Effectors = Regulation.Effectors
            TranscriptUnit.OperonGenes = Regulation.OperonGeneIds
            TranscriptUnit.OperonId = String.Format("#{0}", Regulation.DoorId)
            TranscriptUnit.PccValues = Regulation.PccArray
            'TranscriptUnit.Motifs = Regulation.TF
            TranscriptUnit.TFPcc = Regulation.TFPcc
            TranscriptUnit.RegulatorK = 1.2
            TranscriptUnit.PromoterGene = Regulation.OperonPromoter

            Return TranscriptUnit
        End Function
    End Class

End Namespace