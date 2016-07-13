#Region "Microsoft.VisualBasic::7c363ca582b98a8166c9cbe8eccf4a2c, ..\Metagenome\Taxonomy.vb"

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

Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Perl

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

''' <summary>
''' Create taxonomic objects,
''' Return classes Or full text Of a taxonoDim Object,
''' Calculate consensus Of an array Of taxonomic objects.
''' </summary>
Public Class Taxonomy : Inherits ClassObject

    Shared ReadOnly ranks As String() = {"domain", "phylum", "class", "orderx", "family", "genus", "species", "strain"}

    ''' <summary>
    ''' Create a new taxonomy object
    ''' </summary>
    ''' <param name="line"></param>
    Sub New(line As String)
        Dim data As String() = line.Split(";"c)
        ' Remove trailing NAs And replace internal blanks With "Unassigned"
        Dim assigned As New Pointer

        For i As Integer = 0 To data.Length
            If data(i).IsBlank OrElse data(i) = "NA" Then
                data(i) = "Unassigned"
            End If
        Next

        domain = data(++assigned)
        phylum = data(++assigned)
        [class] = data(++assigned)
        order = data(++assigned)
        family = data(++assigned)
        genus = data(++assigned)
        species = data(++assigned)
        strain = data(++assigned)
    End Sub

    ''' <summary>
    ''' Return the domain Of an Object
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property domain As String

    ''' <summary>
    ''' Return the phylum Of an Object
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property phylum As String

    ''' <summary>
    ''' Return the Class Of an Object
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property [class] As String

    ''' <summary>
    ''' Return the order Of an Object
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property order As String

    ''' <summary>
    ''' Return the family Of an Object
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property family As String

    ''' <summary>
    ''' Return the genus Of an Object
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property genus As String

    ''' <summary>
    ''' Return the species Of an Object
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property species As String

    ''' <summary>
    ''' Return the strain Of an Object
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property strain As String

    Protected Overrides Function __toString() As String
        Return taxstring()
    End Function

    ''' <summary>
    ''' Return the object as a ";" delimited string
    ''' </summary>
    ''' <returns></returns>
    Public Function taxstring() As String
        Dim array As String() = {domain, phylum, [class], order, family, genus, species, strain}
        Return array.JoinBy(";")
    End Function

    ''' <summary>
    ''' {domain, phylum, [class], order, family, genus, species, strain}
    ''' </summary>
    ''' <param name="l"></param>
    ''' <returns></returns>
    Default Public Property DepthLevel(l As Integer) As String
        Get
            Select Case l
                Case 0 : Return domain
                Case 1 : Return phylum
                Case 2 : Return [class]
                Case 3 : Return order
                Case 4 : Return family
                Case 5 : Return genus
                Case 6 : Return species
                Case 7 : Return strain
                Case Else
                    Throw New ArgumentOutOfRangeException(l)
            End Select
        End Get
        Set(value As String)
            Select Case l
                Case 0 : _domain = value
                Case 1 : _phylum = value
                Case 2 : _class = value
                Case 3 : _order = value
                Case 4 : _family = value
                Case 5 : _genus = value
                Case 6 : _species = value
                Case 7 : _strain = value
                Case Else
                    Throw New ArgumentOutOfRangeException(l)
            End Select
        End Set
    End Property

    ''' <summary>
    ''' Return the depth of an object - last rank with valid taxonomy
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property depth As String
        Get
            Dim d As String = Nothing
            Call GetDepth(d)
            Return d
        End Get
    End Property

    Public Function GetDepth(Optional ByRef depth As String = "NA") As Integer
        Dim self As String() = {domain, phylum, [class], order, family, genus, species, strain}
        Dim d As Integer

        For i As Integer = 0 To self.Length - 1
            Dim lv As String = self(i)

            If (Not lv.IsBlank) AndAlso lv <> "NA" AndAlso lv <> "Unassigned" Then
                depth = ranks(i)
                d = i
            End If
        Next

        Return d
    End Function

    ''' <summary>
    ''' For an array of tax objects and a majority required, calculate a consensus taxonomy
    ''' Return the consensus tax Object, As well As stats On the agreement
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function consemsus(array As Taxonomy(), majority As Double)
        ' Correct For percentages 1-100
        If (majority <= 1) Then majority = majority * 100

        ' Set up variables To store the results
        Dim newTax As String() = {}  ' consensus taxon
        Dim rankCounts As Integer  ' number of different taxa for each rank
        Dim maxPcts As Double ' percentage of most popular taxon for each rank
        Dim naPcts As Double  ' percentage of each rank that has no taxonomy assigned
        Dim conVote As Integer = 0
        Dim taxCount As Integer = array.Length
        Dim minRankIndex As Integer = -1
        Dim minRank As String = "NA"

        ' Calculate the Consensus

        ' Flesh out the taxonomies so they all have indices To 7
        For Each t In array
            For i As Integer = 0 To 7

                ' If no value For that depth, add it
                If t.GetDepth - 1 < i Then t(i) = "NA"
            Next
        Next

        Dim done As Integer = 0

        ' For each taxonomic rank
        For i As Integer = 0 To 7

            ' Initializes hashes With the counts Of Each tax assignment
            Dim tallies As New Dictionary(Of String, Integer) ' For Each tax value -- how many objects have this taxonomy
            Dim rankCnt = 0 ' How many different taxa values are there For that rank
            Dim maxCnt = 0 ' what was the size Of the most common taxon
            Dim naCnt = 0 ' how many are unassigned 
            Dim topPct = 0 ' used To determine If we are done With the taxonomy Or Not

            ' Step through the taxonomies And count them
            For Each t As Taxonomy In array
                tallies(t(i)) += 1
            Next

            ' For Each unique tax assignment
            For Each k In tallies.Keys

                If k <> "NA" Then

                    rankCnt += 1
                    minRankIndex = i
                    If tallies(k) > maxCnt Then maxCnt = tallies(k)
                Else
                        naCnt = tallies(k)
                    End If

                Dim vote = (tallies(k) / taxCount) * 100
                If ((k <> "NA") AndAlso (vote > topPct)) Then topPct = vote
                vote = (100 * (tallies(k) / taxCount) + 0.5)
                If ((Not done) AndAlso (vote >= majority)) Then
                    Push(newTax, k)
                    If k <> "NA" Then conVote = vote

                End If
        Next
        Next
    End Function
End Class

