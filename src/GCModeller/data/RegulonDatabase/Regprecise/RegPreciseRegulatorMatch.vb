Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel.NucleotideModels

Namespace Regprecise

    Public Class RegPreciseRegulatorMatch

        ''' <summary>
        ''' The genome query.(待注释的目标基因组的蛋白编号)
        ''' </summary>
        ''' <returns></returns>
        Public Property Query As String

        ''' <summary>
        ''' The regprecise regulator name
        ''' </summary>
        ''' <returns></returns>
        Public Property Regulator As String
        Public Property Family As String
        Public Property Description As String

        ''' <summary>
        ''' 相似度
        ''' </summary>
        ''' <returns></returns>
        Public Property Identities As Double

        Public Property Regulog As String
        Public Property biological_process As String
        Public Property effector As String
        Public Property mode As String
        ''' <summary>
        ''' 注释来源的基因组名称
        ''' </summary>
        ''' <returns></returns>
        Public Property species As String

        ''' <summary>
        ''' 这个<see cref="Regulator"/>所调控的位点集合
        ''' </summary>
        ''' <returns></returns>
        Public Property RegulonSites As String()
    End Class

    Public Class MotifSiteMatch : Inherits Contig
        Implements INamedValue

        Public Property ID As String Implements IKeyedEntity(Of String).Key
        Public Property left As Integer
        Public Property right As Integer
        Public Property strand As String
        Public Property src As String()

        Public ReadOnly Property hits As Integer
            Get
                Return src.Length
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Protected Overrides Function __getMappingLoci() As NucleotideLocation
            Return New NucleotideLocation(left, right, strand)
        End Function
    End Class
End Namespace