#Region "Microsoft.VisualBasic::9e1332ac14a26615739a0f814464ac11, GCModeller\data\RegulonDatabase\Regprecise\RegpreciseBBH.vb"

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

    '   Total Lines: 169
    '    Code Lines: 117
    ' Comment Lines: 25
    '   Blank Lines: 27
    '     File Size: 6.46 KB


    '     Class RegpreciseBBH
    ' 
    '         Properties: effectors, family, geneName, HitName, pathway
    '                     QueryName, regulationMode, Tfbs
    ' 
    '         Function: (+2 Overloads) JoinTable
    ' 
    '     Class RegpreciseMPBBH
    ' 
    '         Properties: MPScore, PfamString, Similarity, SubjectPfamString
    ' 
    '         Function: GetLocusTag, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports SMRUCC.genomics.Data.Xfam.Pfam.ProteinDomainArchitecture.MPAlignment.BiDirectionalBesthit
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.BBH

Namespace Regprecise

    ''' <summary>
    ''' RegPrecise regulator matched by blastp sbh/bbh mapping
    ''' </summary>
    Public Class RegpreciseBBH : Inherits BiDirectionalBesthit
        Implements INamedValue
        Implements IRegulatorMatched

        ''' <summary>
        ''' 和Regprecise数据库之中的调控因子所比对上的目标菌株的基因组之中的蛋白质
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Column("locus_id")> Public Overrides Property QueryName As String Implements INamedValue.Key,
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

        Public Property geneName As String

        Public Property family As String Implements IRegulatorMatched.Family

        Public Property regulationMode As String
        Public Property pathway As String

        Public Property effectors As String()
        ''' <summary>
        ''' a list of known TFBS id that regulated by this regulator in regprecise database
        ''' </summary>
        ''' <returns></returns>
        Public Property Tfbs As String()

        Public Shared Iterator Function JoinTable(sbh As IEnumerable(Of BestHit), regulators As IEnumerable(Of RegulatorTable)) As IEnumerable(Of RegpreciseBBH)
            Dim TF As Dictionary(Of String, RegulatorTable) = regulators _
                .GroupBy(Function(r) r.locus_tag) _
                .ToDictionary(Function(r) r.Key,
                              Function(g)
                                  Return g.First
                              End Function)

            For Each hit As BestHit In sbh
                If hit.HitName = HITS_NOT_FOUND Then
                    Continue For
                End If

                Dim reg As RegulatorTable = TF.TryGetValue(hit.HitName.Split(":"c).Last)

                If reg Is Nothing Then
                    Call $"no regulator information was found for '{hit.HitName}'".Warning
                    Continue For
                End If

                Yield New RegpreciseBBH With {
                    .description = hit.description,
                    .HitName = hit.HitName,
                    .effectors = reg.effector,
                    .family = reg.family,
                    .forward = hit.score,
                    .length = hit.query_length.ToString,
                    .level = Levels.SBH,
                    .pathway = reg.pathway,
                    .positive = hit.positive,
                    .QueryName = hit.QueryName,
                    .regulationMode = reg.regulationMode,
                    .reverse = .forward,
                    .term = reg.regulog,
                    .geneName = reg.geneName
                }
            Next
        End Function

        Public Shared Iterator Function JoinTable(bbh As IEnumerable(Of BiDirectionalBesthit), regulators As IEnumerable(Of RegulatorTable)) As IEnumerable(Of RegpreciseBBH)
            Dim TF As Dictionary(Of String, RegulatorTable) = regulators _
                .GroupBy(Function(r) r.locus_tag) _
                .ToDictionary(Function(r) r.Key,
                              Function(g)
                                  Return g.First
                              End Function)

            For Each hit As BiDirectionalBesthit In bbh
                If hit.HitName = HITS_NOT_FOUND Then
                    Continue For
                End If

                Dim reg As RegulatorTable = TF.TryGetValue(hit.HitName.Split(":"c).Last)

                If reg Is Nothing Then
                    Call $"no regulator information was found for '{hit.HitName}'".Warning
                    Continue For
                End If

                Yield New RegpreciseBBH With {
                    .description = hit.description,
                    .HitName = hit.HitName,
                    .effectors = reg.effector,
                    .family = reg.family,
                    .forward = hit.forward,
                    .length = hit.length,
                    .level = hit.level,
                    .pathway = reg.pathway,
                    .positive = hit.positive,
                    .QueryName = hit.QueryName,
                    .regulationMode = reg.regulationMode,
                    .reverse = hit.reverse,
                    .term = reg.regulog,
                    .geneName = reg.geneName
                }
            Next
        End Function

    End Class

    ''' <summary>
    ''' Bidirectional best hit regulator with the regprecise database.(调控因子与Regprecise数据库的双向最佳比对结果)
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    Public Class RegpreciseMPBBH : Inherits RegpreciseBBH

        ' Implements IMPAlignmentResult
        Implements INamedValue
        Implements IRegulatorMatched

#Region "Public Property"

        <Column("Pfam-String")> Public Property PfamString As String
        <Column("subject.pfam-string")> Public Property SubjectPfamString As String

        Public Property Similarity As Double 'Implements IMPAlignmentResult.Similarity
        Public Property MPScore As Double 'Implements IMPAlignmentResult.MPScore

#End Region

        Public Function GetLocusTag() As String
            Return Me.HitName.Split(CChar(":")).Last
        End Function

        Public Overrides Function ToString() As String
            Return String.Format("{0}({1})", QueryName, HitName)
        End Function
    End Class
End Namespace
