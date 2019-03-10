﻿#Region "Microsoft.VisualBasic::704b9dc670f441b1982d7ab2c5bd5f5d, analysis\Metagenome\Metagenome\gast\Taxonomy.vb"

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

    '     Class Taxonomy
    ' 
    '         Properties: depth, strain
    ' 
    '         Constructor: (+5 Overloads) Sub New
    '         Function: __data, consensus, GetDepth, GetTree, TaxonomyString
    '                   ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Perl
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Metagenomics

'package Taxonomy;

'#########################################
'#
'# scriptname: Taxonomy.pm
'#
'# Author: Susan Huse, shuse@mbl.edu
'#
'# Date: May 2008
'#
'# Copyright (C) 2008 Marine Biological Laborotory, Woods Hole, MA
'# 
'# This program Is free software; you can redistribute it And/Or
'# modify it under the terms Of the GNU General Public License
'# As published by the Free Software Foundation; either version 2
'# Of the License, Or (at your Option) any later version.
'# 
'# This program Is distributed In the hope that it will be useful,
'# but WITHOUT ANY WARRANTY; without even the implied warranty Of
'# MERCHANTABILITY Or FITNESS For A PARTICULAR PURPOSE.  See the
'# GNU General Public License For more details.
'# 
'# For a copy Of the GNU General Public License, write To the Free Software
'# Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
'# Or visit http://www.gnu.org/copyleft/gpl.html
'#
'# Keywords : remove the space before the colon And list keywords separated by a space
'# 
'########################################

'use strict;
'use warnings;

'our $VERSION = '1.0';

Namespace gast

    ''' <summary>
    ''' Create taxonomic objects,
    ''' Return classes Or full text Of a taxonoDim Object,
    ''' Calculate consensus Of an array Of taxonomic objects.
    ''' </summary>
    Public Class Taxonomy : Inherits Metagenomics.Taxonomy

        Friend Shared ReadOnly ranks$() = {"domain", "phylum", "class", "orderx", "family", "genus", "species", "strain"}

        ''' <summary>
        ''' Create a new taxonomy object
        ''' </summary>
        ''' <param name="line"></param>
        Sub New(line As String)
            Call Me.New(__data(line))
        End Sub

        ''' <summary>
        ''' Makes the property value copy
        ''' </summary>
        ''' <param name="taxonomy"></param>
        Sub New(taxonomy As Metagenomics.Taxonomy)
            kingdom = taxonomy.kingdom
            phylum = taxonomy.phylum
            [class] = taxonomy.class
            order = taxonomy.order
            family = taxonomy.family
            genus = taxonomy.genus
            species = taxonomy.species
            scientificName = taxonomy.scientificName
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="line"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' $newString =~ s/;$//;
        ''' 这个语法应该是正则表达式替换匹配字符串为空白字符串
        ''' </remarks>
        Private Shared Function __data(line As String) As String()
            If line Is Nothing Then
                line = "Unassigned"
            End If

            Dim data As String() = line.Split(";"c)

            ' Remove trailing NAs And replace internal blanks With "Unassigned"
            For i As Integer = 0 To data.Length - 1
                If data(i).StringEmpty OrElse data(i) = "NA" Then
                    data(i) = "Unassigned"
                End If
            Next

            Return data
        End Function

        Sub New(taxonomy As Dictionary(Of String, String))
            Call MyBase.New(taxonomy)
        End Sub

        Sub New(data As String())
            Dim assigned As VBInteger = 0

            If data.Length < 8 Then
                ReDim Preserve data(8)
            End If

            kingdom = data(++assigned)
            phylum = data(++assigned)
            [class] = data(++assigned)
            order = data(++assigned)
            family = data(++assigned)
            genus = data(++assigned)
            species = data(++assigned)
            strain = data(++assigned)
        End Sub

        Protected Sub New()
        End Sub

        ''' <summary>
        ''' Return the strain Of an Object
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property strain As String

        ''' <summary>
        ''' Return the object as a ";" delimited string
        ''' </summary>
        ''' <returns></returns>
        Public Function TaxonomyString() As String
            Dim array$() = {kingdom, phylum, [class], order, family, genus, species, strain}
            Return array.JoinBy(";")
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return TaxonomyString()
        End Function

        Public Function GetTree(rankLevel As Integer) As String
            Dim ls As New List(Of String)

            For i As Integer = 0 To rankLevel
                ls += Me(i)
            Next

            Return String.Join(";", ls.ToArray)
        End Function

        ''' <summary>
        ''' ``{domain, phylum, [class], order, family, genus, species, strain}``
        ''' </summary>
        ''' <param name="level"></param>
        ''' <returns></returns>
        Default Public Property RankValue(level As Integer) As String
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Select Case level
                    Case 0 : Return kingdom
                    Case 1 : Return phylum
                    Case 2 : Return [class]
                    Case 3 : Return order
                    Case 4 : Return family
                    Case 5 : Return genus
                    Case 6 : Return species
                    Case 7 : Return strain
                    Case Else
                        Throw New ArgumentOutOfRangeException(level)
                End Select
            End Get
            Set(value As String)
                Select Case level
                    Case 0 : kingdom = value
                    Case 1 : phylum = value
                    Case 2 : [class] = value
                    Case 3 : order = value
                    Case 4 : family = value
                    Case 5 : genus = value
                    Case 6 : species = value
                    Case 7 : _strain = value
                    Case Else
                        Throw New ArgumentOutOfRangeException(level)
                End Select
            End Set
        End Property

        ''' <summary>
        ''' Return the depth of an object - last rank with valid taxonomy
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property depth As TaxonomyRanks
            Get
                Dim d As TaxonomyRanks = TaxonomyRanks.NA
                Call GetDepth(d)
                Return d
            End Get
        End Property

        Shared ReadOnly RanksInteger As TaxonomyRanks() = {
            TaxonomyRanks.Kingdom,
            TaxonomyRanks.Phylum,
            TaxonomyRanks.Class,
            TaxonomyRanks.Order,
            TaxonomyRanks.Family,
            TaxonomyRanks.Genus,
            TaxonomyRanks.Species,
            TaxonomyRanks.Strain
        }

        Public Function GetDepth(Optional ByRef depth As TaxonomyRanks = TaxonomyRanks.NA) As Integer
            Dim d As Integer = -1

            For i As Integer = 0 To RanksInteger.Length - 1
                Dim rank = RankValue(i)

                If (Not rank.StringEmpty) AndAlso rank <> "NA" AndAlso rank <> "Unassigned" Then
                    depth = CType(TaxonomyRanks.Kingdom + i, TaxonomyRanks)
                    d = i
                Else
                    Exit For
                End If
            Next

            Return d
        End Function

        ''' <summary>
        ''' For an array of tax objects and a majority required, calculate a consensus taxonomy
        ''' Return the consensus tax Object, As well As stats On the agreement
        ''' </summary>
        ''' <param name="majority">
        ''' 这个参数值必须要大于50%才能够正常的工作
        ''' </param>
        ''' <returns></returns>
        Public Shared Function consensus(array As Taxonomy(), majority#) As Taxonomy()
            ' Set up variables To store the results
            Dim newTax As String() = {}      ' consensus taxon
            Dim rankCounts As Integer() = {} ' number of different taxa for each rank
            Dim maxPcts As Double() = {}     ' percentage of most popular taxon for each rank
            Dim naPcts As Double() = {}      ' percentage of each rank that has no taxonomy assigned
            Dim conVote As Integer = 0
            Dim taxCount As Integer = array.Length
            Dim minRankIndex As Integer = -1
            Dim minRank As String = "NA"

            ' Correct For percentages 1-100
            If (majority <= 1) Then
                majority = majority * 100
            End If

            ' Calculate the Consensus

            ' Flesh out the taxonomies so they all have indices To 7
            For Each t As Taxonomy In array
                For i As Integer = 0 To 7

                    ' If no value For that depth, add it
                    If t.GetDepth < i Then
                        t(i) = "NA"
                    End If
                Next
            Next

            Dim done As Boolean = False

            ' For each taxonomic rank
            For i As Integer = 0 To 7

                ' Initializes hashes With the counts Of Each tax assignment
                Dim tallies As New Dictionary(Of String, Integer) ' For Each tax value -- how many objects have this taxonomy
                Dim rankCnt = 0                                   ' How many different taxa values are there For that rank
                Dim maxCnt = 0                                    ' what was the size Of the most common taxon
                Dim naCnt = 0                                     ' how many are unassigned 
                Dim topPct = 0                                    ' used To determine If we are done With the taxonomy Or Not

                ' Step through the taxonomies And count them
                For Each t As Taxonomy In array
                    If Not tallies.ContainsKey(t(i)) Then
                        Call tallies.Add(t(i), 0)
                    End If
                    tallies(t(i)) += 1
                Next

                ' For Each unique tax assignment
                For Each k In tallies.Keys

                    If k <> "NA" Then
                        rankCnt += 1
                        minRankIndex = i

                        If tallies(k) > maxCnt Then
                            maxCnt = tallies(k)
                        End If
                    Else
                        naCnt = tallies(k)
                    End If

                    Dim vote = (tallies(k) / taxCount) * 100

                    If ((k <> "NA") AndAlso (vote > topPct)) Then
                        topPct = vote
                    End If

                    ' vote = (100 * (tallies(k) / taxCount) + 0.5)
                    If ((Not done) AndAlso (vote >= majority)) Then
                        Call Push(newTax, k)

                        If k <> "NA" Then
                            conVote = vote
                        End If
                    End If
                Next

                If topPct < majority Then
                    done = True
                End If

                ' If ($#newTax < $i) {push (@newTax, "NA");}
                Call Push(rankCounts, rankCnt)

                If (taxCount > 0) Then
                    Push(maxPcts, (100 * (maxCnt / taxCount) + 0.5))
                    Push(naPcts, (100 * (naCnt / taxCount) + 0.5))
                End If
            Next

            Dim taxReturn As Taxonomy() = {}

            If newTax.Length = 0 Then
                ' If no consensus at all, Call it Unknown
                Push(taxReturn, New Taxonomy("Unknown"))
            Else
                ' taxonomy Object For consensus
                Push(taxReturn, New Taxonomy(newTax))
            End If

            ' If (! $taxReturn[0]) {$taxReturn[0] = "NA";}
            If taxReturn(0) Is Nothing Then
                ' # 20081126 - empty tax should be 'Unknown'
                taxReturn(0) = New Taxonomy("Unknown")
            End If

            ' if ($taxReturn[-1] eq "Unassigned") {pop @taxReturn;} 
            ' If resolvedThen To an Unassigned rank, remove it.
            If taxReturn.Last.TaxonomyString = "Unassigned" Then
                ' If resolved to an Unassigned rank, remove it. -1表示最后一个元素
                Pop(taxReturn)
            End If

            ' winning majority
            taxReturn.Set(1, New Taxonomy(conVote + 0.5))

            If (minRankIndex >= 0) Then
                minRank = ranks(minRankIndex)
            End If

            taxReturn.Set(2, New Taxonomy(minRank))                                           ' lowest rank With valid assignment
            taxReturn.Set(3, New Taxonomy(rankCounts.Select(Function(x) x.ToString).ToArray)) ' number Of different taxa at Each rank
            taxReturn.Set(4, New Taxonomy(maxPcts.Select(Function(x) x.ToString).ToArray))    ' percentage Of the most popular taxon (!= "NA")
            taxReturn.Set(5, New Taxonomy(naPcts.Select(Function(x) x.ToString).ToArray))     ' percentage that are unassigned ("NA")

            Return taxReturn
        End Function
    End Class
End Namespace
