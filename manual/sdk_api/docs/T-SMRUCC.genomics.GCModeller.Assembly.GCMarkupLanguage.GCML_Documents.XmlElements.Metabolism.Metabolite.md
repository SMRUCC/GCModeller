---
title: Metabolite
---

# Metabolite
_namespace: [SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism](N-SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Metabolism.html)_






### Properties

#### Identifier
UniqueID.(本目标对象的唯一标识符)
#### NumOfFluxAssociated
与本代谢物相关的流对象的数目，计算规则：
 当处于不可逆反应的时候：处于产物边，计数为零，处于底物边，计数为1
 当处于可逆反应的时候：无论是处于产物边还是底物边，都被计数为0.5
