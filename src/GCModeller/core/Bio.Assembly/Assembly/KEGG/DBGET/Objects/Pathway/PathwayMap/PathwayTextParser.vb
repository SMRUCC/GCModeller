﻿Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers

Namespace Assembly.KEGG.DBGET.bGetObject

    Public Module PathwayTextParser

        Public Function ParsePathway(data As String) As Pathway
            Return ParsePathway(New WebForm(data))
        End Function

        <Extension>
        Public Function ParsePathway(form As WebForm) As Pathway
            Return New Pathway With {
                .compound = form.GetXmlTuples("COMPOUND").ToArray,
                .genes = form.GetXmlTuples("GENE").ToArray,
                .drugs = form.GetXmlTuples("DRUG").ToArray,
                .name = form!NAME,
                .description = form!DESCRIPTION,
                .EntryId = form!ENTRY.Split(" "c).First,
                .organism = form!ORGANISM,
                .references = form.References,
                .modules = form.GetXmlTuples("MODULE").ToArray,
                .[class] = form!CLASS,
                .related_pathways = form.GetXmlTuples("REL_PATHWAY").ToArray
            }
        End Function
    End Module
End Namespace