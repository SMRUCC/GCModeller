Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Language

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
''' Return classes Or full text Of a taxonomy Object,
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
    ''' Return the depth of an object - last rank with valid taxonomy
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property depth As String
        Get
            Dim self As String() = {domain, phylum, [class], order, family, genus, species, strain}
            Dim d As String = "NA"

            For i As Integer = 0 To self.Length - 1
                Dim lv As String = self(i)

                If (Not lv.IsBlank) AndAlso lv <> "NA" AndAlso lv <> "Unassigned" Then
                    d = ranks(i)
                End If
            Next

            Return d
        End Get
    End Property

    ''' <summary>
    ''' For an array of tax objects and a majority required, calculate a consensus taxonomy
    ''' Return the consensus tax Object, As well As stats On the agreement
    ''' </summary>
    ''' <returns></returns>
    Public Function consemsus()

    End Function
End Class
