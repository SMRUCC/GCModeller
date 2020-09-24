#Region "Microsoft.VisualBasic::088b08041df2d7d7e3b6a12264af9bd8, sub-system\FBA\FBA_DP\rFBA\ModelWriter.vb"

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

    ' Module ModelWriter
    ' 
    '     Function: __creates, __generate, __getEffectors, (+2 Overloads) CreateObject
    '     Class Regulation
    ' 
    '         Properties: Effectors, GeneId, Regulator
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports SMRUCC.genomics.Analysis.RNA_Seq
Imports SMRUCC.genomics.Analysis.RNA_Seq.dataExprMAT
Imports SMRUCC.genomics.ComponentModel.EquaionModel
Imports SMRUCC.genomics.Model.SBML
Imports SMRUCC.genomics.Model.SBML.Level2.Elements

Public Module ModelWriter

    Public Function CreateObject(SBML As Level2.XmlFile, MetabolismEnzymeLink As Mapping.EnzymeGeneMap()) As ModelReader.MetabolismFlux()
        Dim FluxObject As ModelReader.MetabolismFlux() = (From MetabolismFlux
                                                          In SBML.Model.listOfReactions
                                                          Select __generate(MetabolismFlux, MetabolismEnzymeLink)).ToArray
        Return FluxObject
    End Function

    Private Function __generate(MetabolismFlux As Reaction, MetabolismEnzymeLink As Mapping.EnzymeGeneMap()) As ModelReader.MetabolismFlux
        Dim Flux = New ModelReader.MetabolismFlux
        Flux.Equation = EquationBuilder.ToString(Function() (From item As speciesReference
                                                             In MetabolismFlux.Reactants
                                                             Select New KeyValuePair(Of Double, String)(item.stoichiometry, item.species)).ToArray,
            Function() (From item As speciesReference In MetabolismFlux.Products Select New KeyValuePair(Of Double, String)(item.stoichiometry, item.species)).ToArray, MetabolismFlux.reversible)
        Flux.Lower_Bound = MetabolismFlux.LowerBound
        Flux.Upper_Bound = MetabolismFlux.UpperBound
        Flux.UniqueId = MetabolismFlux.id
        Flux.CommonName = MetabolismFlux.name

        Dim Enzymes = (From item As Mapping.EnzymeGeneMap In MetabolismEnzymeLink.AsParallel
                       Where String.Equals(Flux.UniqueId, item.EnzymeRxn) Select item).ToArray
        If Not Enzymes.IsNullOrEmpty Then
            Dim item = Enzymes.First

            Flux.AssociatedEnzymeGenes = item.GeneId
            If Not String.IsNullOrEmpty(item.CommonName) Then Flux.CommonName = item.CommonName
        End If

        Return Flux
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Pcc"></param>
    ''' <param name="RPKMColumn"><paramref name="Pcc"></paramref>中的某一列的列标</param>
    ''' <param name="Regulations"></param>
    ''' <param name="EffectorMappings">从Regprecise到MetaCyc数据库对象的关系映射</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateObject(Pcc As IO.File,
                                 RPKMColumn As Integer,
                                 Regulations As Regulation(),
                                 EffectorMappings As Mapping.EffectorMap()) As ModelReader.GeneExpression()

        Dim AvgRPKM As Double = (From item In Pcc.Skip(1) Select Val(item(RPKMColumn))).Average
        Dim PccValues As ExprSamples() = GenesCOExpr.CalculatePccMatrix(MatrixAPI.ToSamples(Pcc, True))
        Dim pccIdList As String() = (From item In PccValues Select item.locusId).ToArray
        Dim GeneIds As String() = (From item In Regulations.AsParallel Select item.GeneId Distinct Order By GeneId Ascending).ToArray
        Dim LQuery = (From strId As String
                      In GeneIds
                      Select strId.__creates(
                          Pcc,
                          RPKMColumn,
                          Regulations,
                          AvgRPKM,
                          PccValues,
                          pccIdList,
                          EffectorMappings)).ToArray
        Return LQuery
    End Function

    <Extension>
    Private Function __creates(strId As String,
                               PCC As IO.File,
                               RPKMColumn As Integer,
                               Regulations As Regulation(),
                               AvgRPKM As Double,
                               PccValues As ExprSamples(),
                               pccIdList As String(),
                               EffectorMappings As Mapping.EffectorMap()) As ModelReader.GeneExpression

        Dim Expression As New ModelReader.GeneExpression With {
            .AccessionId = strId,
            .BasalExpression = 10
        }
        Dim Collection = PCC.FindAtColumn(strId, 0)

        If Collection.IsNullOrEmpty Then
            Expression.RPKM = AvgRPKM
            Call $"[WARNING] Could not found object ""{strId}"", RPKM value was set to average!".__DEBUG_ECHO
        Else
            Expression.RPKM = Val(Collection.First.Column(RPKMColumn))
        End If

        Dim Regulation = (From item As Regulation In Regulations.AsParallel
                          Where String.Equals(item.GeneId, strId)
                          Select item).ToArray
        Expression.Regulators = (From item As Regulation In Regulation
                                 Let rPcc As Double() = (From pccitem In PccValues
                                                         Let idx As Integer = Array.IndexOf(pccIdList, item.Regulator)
                                                         Where String.Equals(pccitem.locusId, strId) AndAlso idx > -1
                                                         Select pccitem(idx)).ToArray
                                 Let Effectors As String = item.__getEffectors(EffectorMappings)
                                 Let valuePcc As Double = If(rPcc.IsNullOrEmpty, 0, rPcc.First)
                                 Let strData As String = String.Format("{0}|{1}|{2}", item.Regulator, valuePcc, Effectors)
                                 Select strData).ToArray
        Return Expression
    End Function

    <Extension>
    Private Function __getEffectors(item As Regulation, EffectorMappings As Mapping.EffectorMap()) As String
        Dim sBuilder As StringBuilder = New StringBuilder(128)
        For Each sEffector In item.Effectors
            Dim f = (From map As Mapping.EffectorMap In EffectorMappings
                     Where String.Equals(sEffector, map.Effector)
                     Select map.MetaCycId).ToArray
            If Not f.IsNullOrEmpty Then
                Call sBuilder.Append(f.First & ", ")
            End If
        Next
        If sBuilder.Length > 0 Then
            Call sBuilder.Remove(sBuilder.Length - 2, 2)
            Return sBuilder.ToString
        Else
            Return ""
        End If
    End Function

    Public Class Regulation
        Public Property GeneId As String
        <Column("MatchedRegulator")> Public Property Regulator As String
        <CollectionAttribute("Effector")> Public Property Effectors As String()
    End Class
End Module
