Imports Microsoft.VisualBasic.DocumentFormat.Csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.DocumentFormat.Csv.Extensions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.AnalysisTools.ProteinTools.Sanger.Pfam
Imports SMRUCC.genomics.AnalysisTools.ProteinTools.Sanger.Pfam.ProteinDomainArchitecture.MPAlignment.BiDirectionalBesthit
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports SMRUCC.genomics.NCBI.Extensions
Imports SMRUCC.genomics.NCBI.Extensions.LocalBLAST.Application.BBH

Namespace Regprecise

    Public Class RegpreciseBBH : Inherits BiDirectionalBesthit

        Implements sIdEnumerable
        Implements IRegulatorMatched

        ''' <summary>
        ''' 和Regprecise数据库之中的调控因子所比对上的目标菌株的基因组之中的蛋白质
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column("LocusId")> Public Overrides Property QueryName As String Implements sIdEnumerable.Identifier,
            IRegulatorMatched.locusId
            Get
                Return MyBase.QueryName
            End Get
            Set(value As String)
                MyBase.QueryName = value
            End Set
        End Property

        ''' <summary>
        ''' Regprecise_regulator
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column("Regprecise_Matched")> Public Overrides Property HitName As String Implements IRegulatorMatched.Address
            Get
                Return MyBase.HitName
            End Get
            Set(value As String)
                MyBase.HitName = value
            End Set
        End Property

        <Column("Family.Regprecise")> Public Property Family As String Implements IRegulatorMatched.Family

        Public Property RegulationEffects As String
        Public Property RegprecisePhenotypeAssociation As String

        <Collection("Possible.Effectors")> Public Property Effectors As String()
        <Collection("Possible.Regprecise_TfbsId")> Public Property RegpreciseTfbsIds As String()
    End Class
    ''' <summary>
    ''' Bidirectional best hit regulator with the regprecise database.(调控因子与Regprecise数据库的双向最佳比对结果)
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    Public Class RegpreciseMPBBH : Inherits RegpreciseBBH

        Implements IMPAlignmentResult
        Implements sIdEnumerable
        Implements IRegulatorMatched

#Region "Public Property"

        <Column("Pfam-String")> Public Property PfamString As String
        <Column("subject.pfam-string")> Public Property SubjectPfamString As String

        Public Property Similarity As Double Implements IMPAlignmentResult.Similarity
        Public Property MPScore As Double Implements IMPAlignmentResult.MPScore

#End Region

        Public Function GetLocusTag() As String
            Return Me.HitName.Split(CChar(":")).Last
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0}({1})", QueryName, HitName)
        End Function
    End Class
End Namespace